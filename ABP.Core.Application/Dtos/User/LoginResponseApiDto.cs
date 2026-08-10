using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class LoginResponseApiDto
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public List<string>? Roles { get; set; }
        public bool HasError { get; set; }
        public required List<string> Errors { get; set; }
        public string? AccessToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
