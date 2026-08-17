using ABP.Core.Application.Dtos.User;
using ABP.Core.Application.Interfaces;
using ABP.Core.Domain.Common.Enums;
using ABP.Core.Domain.Settings;
using ABP.Infrastructure.Identity.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ABP.Infrastructure.Identity.Services
{
    public class AccountServiceWebApi : BaseAccountService, IAccountServiceWebApi
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public AccountServiceWebApi(UserManager<AppUser> userManager, IEmailService emailService, IMapper mapper, IOptions<JwtSettings> jwtSettings) 
            : base(userManager, emailService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponseApiDto> Login(LoginDto loginDto)
        {
            LoginResponseApiDto responseDto = new()
            {
                Name = "",
                LastName = "",
                UserName = "",
                HasError = false,
                Errors = []
            };

            var user = await _userManager.FindByEmailAsync(loginDto.UserName) ?? await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
            {
                responseDto.HasError = true;
                responseDto.Errors.Add($"No existe ninguna cuenta con {loginDto.UserName}");
                return responseDto;
            }

            if (!user.EmailConfirmed)
            {
                responseDto.HasError = true;
                responseDto.Errors.Add($"La cuenta no está activada. Confirme su correo electrónico.");
                return responseDto;
            }

            if (!user.IsActive)
            {
                responseDto.HasError = true;
                responseDto.Errors.Add($"La cuenta está inactiva.");
                return responseDto;
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if(!result)
            {
                responseDto.HasError= true;
                responseDto.Errors.Add($"Credenciales inválidas para el usuario {loginDto.UserName}");
                return responseDto;
            }

            var roleList = await _userManager.GetRolesAsync(user);

            if (!roleList.Contains(UserRoles.Admin.ToString()) && !roleList.Contains(UserRoles.Commerce.ToString()))
            {
                responseDto.HasError = true;
                responseDto.Errors.Add("No tiene permisos para acceder a la API web.");
                return responseDto;
            }

            JwtSecurityToken jwtToken = await GenerateJwtToken(user);

            responseDto.Name = user.Name;
            responseDto.LastName = user.LastName;
            responseDto.UserName = user.UserName ?? "";
            responseDto.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            responseDto.Roles = roleList.ToList();
            responseDto.Expiration = jwtToken.ValidTo;

            return responseDto;
        }

        public async Task<PagedResponse<UserDto>> GetUsersAsync(UserQueryParameters queryParams)
        {
            int page = queryParams.Page < 1 ? 1 : queryParams.Page;
            int limit = queryParams.Limit > 100 ? 100 : (queryParams.Limit < 1 ? 20 : queryParams.Limit);

            var commerceUsers = await _userManager.GetUsersInRoleAsync(UserRoles.Commerce.ToString());
            var commerceUsersId = commerceUsers.Select(u => u.Id).ToList();

            var query = _userManager.Users.Where(u => !commerceUsersId.Contains(u.Id));

            if (!string.IsNullOrWhiteSpace(queryParams.Rol))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(queryParams.Rol);
                var usersInRoleIds = usersInRole.Select(u => u.Id).ToList();

                query = query.Where(u => usersInRoleIds.Contains(u.Id));
            }

            int totalRecords = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var userDto = _mapper.Map<List<UserDto>>(users);

            for (int i = 0; i < users.Count; i++)
            {
                var roles = await _userManager.GetRolesAsync(users[i]);
                userDto[i].Roles = roles.ToList();
            }

            return new PagedResponse<UserDto>(userDto, totalRecords, page, limit);
        }

        public async Task<PagedResponse<CommerceUserDto>> GetCommerceUsersAsync(CommerceQueryParameters queryParams)
        {
            int page = queryParams.Page < 1 ? 1 : queryParams.Page;
            int limit = queryParams.PageSize > 20 ? 20 : (queryParams.PageSize < 1 ? 20 : queryParams.PageSize);

            var commerceUsers = await _userManager.GetUsersInRoleAsync(UserRoles.Commerce.ToString());
            var commerceUsersId = commerceUsers.Select(u => u.Id).ToList();

            var query = _userManager.Users.Where(u => commerceUsersId.Contains(u.Id));

            int totalRecords = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var userDto = _mapper.Map<List<CommerceUserDto>>(users);

            for (int i = 0; i < users.Count; i++)
            {
                var roles = await _userManager.GetRolesAsync(users[i]);
                userDto[i].Roles = roles.ToList();
            }

            return new PagedResponse<CommerceUserDto>(userDto, totalRecords, page, limit);
        }

        public override async Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto requestDto)
        {
            return await base.ResetPasswordAsync(requestDto);
        }

        public override async Task<UserResponseDto> ConfirmAccountAsync(string userId, string token)
        {
            return await base.ConfirmAccountAsync(userId, token);
        }

        #region Private Methods

        private async Task<JwtSecurityToken> GenerateJwtToken(AppUser user)
        {
            var roleList = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roleList)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var issuedAt = DateTime.UtcNow;
            var expiration = issuedAt.AddMinutes(_jwtSettings.DurationInMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }.Union(userClaims)
             .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: issuedAt,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        #endregion
    }
}
