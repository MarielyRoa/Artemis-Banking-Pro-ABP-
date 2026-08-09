using System.Collections.Generic;

namespace ABP.Core.Application.Dtos.Cashier
{
    public class OperationHistoryDto
    {
        public IReadOnlyList<OperationDto> Operations { get; set; } = new List<OperationDto>();
    }
}
