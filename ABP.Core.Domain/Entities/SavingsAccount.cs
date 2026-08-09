using System;

namespace ABP.Core.Domain.Entities
{
    public class SavingsAccount
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public string OwnerId { get; set; } = string.Empty;
    }
}
