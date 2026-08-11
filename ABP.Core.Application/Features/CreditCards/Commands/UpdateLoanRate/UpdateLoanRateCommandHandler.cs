using ABP.Core.Application.Helpers;
using ABP.Core.Domain.Common.Enums;
using ABP.Core.Domain.Interfaces;
using MediatR;


namespace ABP.Core.Application.Features.CreditCards.Commands.UpdateLoanRate
{
    public class UpdateLoanRateCommandHandler : IRequestHandler<UpdateLoanRateCommand, bool>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanInstallmentRepository _loanInstallmentRepository;

        public UpdateLoanRateCommandHandler(
            ILoanRepository loanRepository,
            ILoanInstallmentRepository loanInstallmentRepository)
        {
            _loanRepository = loanRepository;
            _loanInstallmentRepository = loanInstallmentRepository;
        }

        public async Task<bool> Handle(UpdateLoanRateCommand request, CancellationToken cancellationToken)
        {
            var loan = await _loanRepository.GetByIdAsync(request.LoanId);
            if (loan == null)
                throw new InvalidOperationException("El préstamo no existe.");

            if (loan.Status != LoanStatus.Active)
                throw new InvalidOperationException("Solo se puede editar la tasa de un préstamo activo.");

            // Separar cuotas ya pagadas (no se tocan) de las pendientes (se recalculan)
            var allInstallments = loan.LoanInstallments.OrderBy(i => i.InstallmentNumber).ToList();
            var paidInstallments = allInstallments.Where(i => i.PaymentStatus == PaymentStatus.Paid).ToList();
            var pendingInstallments = allInstallments.Where(i => i.PaymentStatus != PaymentStatus.Paid).ToList();

            if (!pendingInstallments.Any())
                throw new InvalidOperationException("No hay cuotas pendientes para recalcular.");

            // El capital pendiente real es el balance de la última cuota pagada (o el monto total si no hay ninguna pagada)
            decimal remainingPrincipal = paidInstallments.Any()
                ? paidInstallments.Last().PendingAmount
                : loan.AmountApproved;

            int remainingTerm = pendingInstallments.Count;
            var newSchedule = LoanAmortizationCalculator.GenerateAmortizationSchedule(
                remainingPrincipal, request.NewAnnualInterestRate, remainingTerm, DateTime.UtcNow);

            // Reasigna las cuotas futuras con los nuevos valores, conservando su número e ID original
            for (int i = 0; i < pendingInstallments.Count; i++)
            {
                var oldInstallment = pendingInstallments[i];
                var newValues = newSchedule[i];

                oldInstallment.InstallmentAmount = newValues.InstallmentAmount;
                oldInstallment.InterestAmount = newValues.InterestAmount;
                oldInstallment.CapitalAmount = newValues.CapitalAmount;
                oldInstallment.PendingAmount = newValues.PendingAmount;
                oldInstallment.DueDate = newValues.DueDate;

                await _loanInstallmentRepository.UpdateAsync(oldInstallment.Id, oldInstallment);
            }

            loan.AnnualInterestRate = request.NewAnnualInterestRate;
            loan.AmountPending = remainingPrincipal;
            await _loanRepository.UpdateAsync(loan.Id, loan);

            return true;
        }
    }
}
