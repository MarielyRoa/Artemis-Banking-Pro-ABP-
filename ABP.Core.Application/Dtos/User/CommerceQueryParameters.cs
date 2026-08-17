using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class CommerceQueryParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "El parámetro page debe ser mayor que cero.")]
        public int Page { get; set; } = 1;

        [Range(1, 20, ErrorMessage = "El valor máximo permitido para pageSize debe ser 20.")]
        public int PageSize { get; set; } = 20;
    }
}
