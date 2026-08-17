using System.ComponentModel.DataAnnotations;

namespace ABP.Core.Application.Dtos.User
{
    public class ForgotPasswordApiRequestDto : ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "El correo electrónico o nombre de usuario es requerido")]
        public required string EmailOrUserName { get; set; } = null!;
    }
}
