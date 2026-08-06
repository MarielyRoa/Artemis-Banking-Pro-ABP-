namespace ABP.Core.Application.Dtos.User
{
    public class SaveUserDto
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
        public bool? IsActive { get; set; }
        public List<string>? Roles { get; set; }
        public bool IsVerified { get; set; }
    }
}
