using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class UserResponseDto
    {
        public string? Message { get; set; }
        public bool HasErrors { get; set; }
        public required List<string> Errors { get; set; } = [];
    }
}
