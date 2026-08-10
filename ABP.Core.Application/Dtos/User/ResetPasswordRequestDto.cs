using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class ResetPasswordRequestDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string Token { get; set; }
    }
}
