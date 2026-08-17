using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class UserQueryParameters
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Rol { get; set; }
    }
}
