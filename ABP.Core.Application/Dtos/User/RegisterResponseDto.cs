using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class RegisterResponseDto
    {
        public bool HasError { get; set; }
        public required List<string> Errors { get; set; }
    }
}
