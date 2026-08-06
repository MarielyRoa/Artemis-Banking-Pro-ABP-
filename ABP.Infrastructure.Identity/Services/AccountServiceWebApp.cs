using ABP.Core.Application.Dtos.User;
using ABP.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ABP.Infrastructure.Identity.Services
{
    public class AccountServiceWebApp
    {
        private readonly UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;

        public AccountServiceWebApp(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
        {
            LoginResponseDto responseDto = new()
            {
                Id = "",
                FirstName = "",
                LastName = "",
                Username = "",
                Email = "",
                HasError = false,
                Errors = []
            };

            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if(user == null)
            {
                responseDto.HasError = true;
                responseDto.Errors.Add($"No existe ninguna cuenta con {loginDto.Username}");

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
                responseDto.Errors.Add($"La cuenta no está activada.");

                return responseDto;
            }

            var result = await _signInManager.PasswordSignInAsync(loginDto.Username ?? "", loginDto.Password, false, true);

            if (!result.Succeeded)
            {
                responseDto.HasError = true;
                responseDto.Errors.Add($"La cuenta está bloqueada temporalmente por múltiples intentos fallidos.");
                return responseDto;
            }

            var roleList = await _userManager.GetRolesAsync(user);

            responseDto.Id = user.Id;
            responseDto.Email = user.Email ?? "";
            responseDto.Username = user.UserName ?? "";
            responseDto.FirstName = user.Name ?? "";
            responseDto.LastName = user.LastName ?? "";
            responseDto.IsVerified = user.EmailConfirmed;
            responseDto.Roles = roleList.ToList();

            return responseDto;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<RegisterResponseDto> RegisterUser(SaveUserDto saveDto)
        {
            RegisterResponseDto responseDto = new()
            {
                HasError = false,
                Errors = []
            };

            var userWithSameUsername = await _userManager.FindByNameAsync(saveDto.Username);
            if (userWithSameUsername != null)
            {
                responseDto.HasError = true;
                responseDto.Errors.Add($"El nombre de usuario ya existe");

                return responseDto;
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(saveDto.Email);
            if (userWithSameEmail != null)
            {
                responseDto.HasError = true;
                responseDto.Errors.Add($"Ya existe un usuario con este email.");

                return responseDto;
            }

            AppUser user = new AppUser()
            {
                Name = saveDto.FirstName ?? "",
                LastName = saveDto.LastName ?? "",
                Email = saveDto.Email,
                ProfileImage = saveDto.ProfileImage,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, saveDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, saveDto.Password);
            }
            else
            {
                responseDto.HasError = true;
                responseDto.Errors.Add($"Ocurrio un error al registrar el usuario");
            }

            return responseDto;
        }
    }
}
