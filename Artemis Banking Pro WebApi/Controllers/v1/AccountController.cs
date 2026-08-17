using ABP.Core.Application.Dtos.User;
using ABP.Core.Application.Interfaces;
using ABP.Core.Domain.Common.Enums;
using ABP.Infrastructure.Identity.Entities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Artemis_Banking_Pro_WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountServiceWebApi _accountService;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(IAccountServiceWebApi accountService, UserManager<AppUser> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
        }

        [HttpPost("account/login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JwtResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Los datos enviados no son válidos.");
                }

                var response = await _accountService.Login(loginDto);

                if (response.HasError)
                {
                    return Unauthorized(response.Errors);
                }

                return Ok(new JwtResponseDto
                {
                    Token = response.AccessToken,
                    User = response.UserName,
                    Roles = response.Roles ?? [],
                    Expiration = response.Expiration,
                    HasError = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("account/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _accountService.ConfirmAccountAsync(request.UserId, request.Token);

            if (response.HasError)
            {
                return BadRequest(response);
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("account/get-reset-token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetToken([FromBody] ForgotPasswordApiRequestDto apiRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = new ForgotPasswordRequestDto
            {
                UserName = apiRequest.UserName
            };

            var response = await _accountService.ForgotPasswordAsync(request);

            if (response.HasError)
            {
                return BadRequest(response);
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("account/reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _accountService.ResetPasswordAsync(request);

            if (response.HasError)
            {
                return BadRequest(response);
            }

            return NoContent();
        }
    }
}
