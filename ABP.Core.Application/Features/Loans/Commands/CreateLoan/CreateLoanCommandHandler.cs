using ABP.Core.Application.Dtos.Loans;
using ABP.Core.Application.Helpers;
using ABP.Core.Domain.Common.Enums;
using ABP.Core.Domain.Entities;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;


namespace ABP.Core.Application.Features.Loans.Commands.CreateLoan
{
    public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, LoanDto>
    {
        // TODO: confirmar con el equipo/profesor el criterio real de "alto riesgo".
        // Provisional: promedio de deuda pendiente en préstamos anteriores > este monto.
        private const decimal HighRiskAverageDebtThreshold = 100000m;

        private readonly ILoanRepository _loanRepository;
        private readonly ISavingAccountRepository _savingAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public CreateLoanCommandHandler(
            ILoanRepository loanRepository,
            ISavingAccountRepository savingAccountRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _loanRepository = loanRepository;
            _savingAccountRepository = savingAccountRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<LoanDto> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
        {
            var clientLoans = await _loanRepository.GetAllByClientIdAsync(request.ClientId);

            // Regla: cliente activo sin préstamo activo
            if (clientLoans.Any(l => l.Status == LoanStatus.Active))
                throw new InvalidOperationException("El cliente ya tiene un préstamo activo.");

            // Regla: cliente de alto riesgo según deuda promedio (provisional)
            var averageDebt = clientLoans.Count > 0 ? clientLoans.Average(l => l.AmountPending) : 0;
            if (averageDebt > HighRiskAverageDebtThreshold)
                throw new InvalidOperationException("Cliente de alto riesgo: no se puede asignar el préstamo.");

            // Debe existir cuenta principal activa para poder desembolsar
            var principalAccount = await _savingAccountRepository.GetPrincipalAccountByClientIdAsync(request.ClientId);
            if (principalAccount == null)
                throw new InvalidOperationException("El cliente no tiene una cuenta principal activa.");

            var loanNumber = await GenerateUniqueLoanNumberAsync();

            var installments = LoanAmortizationCalculator.GenerateAmortizationSchedule(
                request.Amount, request.AnnualInterestRate, request.TermInMonths, DateTime.UtcNow);

            var loan = new Loan
            {
                Id = 0,
                LoanNumber = loanNumber,
                ClientId = request.ClientId,
                AmountApproved = request.Amount,
                AmountPending = request.Amount,
                AnnualInterestRate = request.AnnualInterestRate,
                TermInMonths = request.TermInMonths,
                Status = LoanStatus.Active,
                AssignedByUserId = request.AssignedByUserId,
                TotalInstallments = request.TermInMonths,
                PaidInstallments = 0,
                ClientPaymentStatus = "Al día",
                LoanInstallments = installments
            };

            var createdLoan = await _loanRepository.AddAsync(loan);

            // Desembolso a la cuenta principal, registrado como crédito
            principalAccount.Balance += request.Amount;
            await _savingAccountRepository.UpdateAsync(principalAccount.Id, principalAccount);

            await _transactionRepository.AddAsync(new Transaction
            {
                Id = 0,
                SavingAccountId = principalAccount.Id,
                TransactionDate = DateTime.UtcNow,
                Amount = request.Amount,
                Type = TransactionType.Credit,
                Beneficiary = string.Empty,
                Origin = $"Desembolso préstamo {loanNumber}",
                Status = TransactionStatus.Approved,
                ResponsibleUserId = request.AssignedByUserId
            });

            return _mapper.Map<LoanDto>(createdLoan);
        }

        private async Task<string> GenerateUniqueLoanNumberAsync()
        {
            string loanNumber;
            bool exists;
            var random = new Random();
            do
            {
                loanNumber = random.Next(100000000, 999999999).ToString();
                exists = await _loanRepository.ExistsLoanNumberAsync(loanNumber);
            } while (exists);

            return loanNumber;
        }
    }
}
