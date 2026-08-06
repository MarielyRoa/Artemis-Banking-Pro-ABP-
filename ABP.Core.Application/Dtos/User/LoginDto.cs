using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class LoginDto
    {
        public required string Username { get; set; } = null!;
        public required string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
