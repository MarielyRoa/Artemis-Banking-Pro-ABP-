using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class ForgotPasswordRequestDto
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Origin { get; set; }
    }
}
