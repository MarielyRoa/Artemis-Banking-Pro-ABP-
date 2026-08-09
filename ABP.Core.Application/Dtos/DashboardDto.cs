namespace ABP.Core.Application.Dtos
{
    public class DashboardDto
    {
        public int TransactionsToday { get; set; }
        public int DepositsToday { get; set; }
        public int WithdrawalsToday { get; set; }
        public int PaymentsToday { get; set; }
    }
}
