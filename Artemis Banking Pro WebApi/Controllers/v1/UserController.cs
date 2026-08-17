using ABP.Core.Application.Dtos.User;
using ABP.Core.Application.Interfaces;
using ABP.Core.Domain.Common.Enums;
using ABP.Infrastructure.Identity.Entities;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artemis_Banking_Pro_WebApi.Controllers.v1
{
    [Authorize(Roles = "Admin,Commerce")]
    [ApiVersion("1.0")]
    public class UserController : BaseApiController
    {
        private readonly IAccountServiceWebApi _accountService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IAccountServiceWebApi accountService, UserManager<AppUser> userManager, IMapper mapper)
        {
            _accountService = accountService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get([FromQuery] UserQueryParameters queryParams)
        {
            try
            {
                if (queryParams.Page <= 0)
                {
                    return BadRequest("El parámetro page debe ser mayor que cero.");
                }

                if (queryParams.Limit <= 0 || queryParams.Limit > 100)
                {
                    return BadRequest("El parámetro Limit debe estar entre 1 y 100.");
                }

                var response = await _accountService.GetUsersAsync(queryParams);

                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet("/users/commerce")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCommerceUsers([FromQuery] CommerceQueryParameters queryParams)
        {
            try
            {
                if (queryParams.Page <= 0)
                {
                    return BadRequest("El parámetro page debe ser mayor que cero.");
                }

                if (queryParams.PageSize <= 0 || queryParams.PageSize > 20)
                {
                    return BadRequest("El parámetro pageSize debe estar entre 1 y 20.");
                }

                var response = await _accountService.GetCommerceUsersAsync(queryParams);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] SaveUserDto saveDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.RegisterUser(saveDto, origin: null, isApi: true);

            if (result.HasError)
            {
                return BadRequest();
            }

            return StatusCode(StatusCodes.Status201Created, new
            {
                id = result.Id,
                userName = result.UserName,
                email = result.Email,
                role = saveDto.Role,
                isActive = false
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("users/commerce/{commerceId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterCommerce([FromRoute] int commerceId, [FromBody] SaveUserDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (registerDto.Password != registerDto.ConfirmPassword)
                {
                    return BadRequest(new { Error = "Password y la confirmacion del password no coinciden " });
                }

                registerDto.Role = UserRoles.Commerce.ToString();
                registerDto.IsActive = true;

                var result = await _accountService.RegisterUser(registerDto, null, true);

                if (result.HasError)
                {
                    return BadRequest(result.Errors);
                }

                var user = await _userManager.FindByIdAsync(result.Id);
                if (user != null)
                {
                    user.EmailConfirmed = true;
                    user.IsActive = true;
                    await _userManager.UpdateAsync(user);
                }

                return StatusCode(StatusCodes.Status201Created, new { Message = "Usuario comercio creado correctamente.", Id = result.Id });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
