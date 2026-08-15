using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class ConfirmAccountRequestDto
    {
        [Required(ErrorMessage = "El ID de usuario es requerido")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "El token de confirmación es requerido")]
        public required string Token { get; set; }
    }
}
