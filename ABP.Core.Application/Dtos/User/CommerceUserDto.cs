using ABP.Core.Domain.Common.Enums;

namespace ABP.Core.Application.Dtos.User
{
    public class CommerceUserDto : UserDto
    {
        public int CommerceId { get; set; }
        public string CommerceName { get; set; } = string.Empty;
        public string Role => Roles?.FirstOrDefault() ?? UserRoles.Commerce.ToString();
        public string Identification => DNI ?? string.Empty;
    }
}
