using ABP.Core.Application.Dtos;
using ABP.Core.Application.Interfaces;
using ABP.Core.Domain.Entities;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Core.Application.Services
{
    public class CashierService : ICashierService
    {
        private readonly IGenericRepository<SavingsAccount> _accountRepo;
        private readonly IGenericRepository<Transaction> _transactionRepo;
        private readonly IGenericRepository<CreditCard> _creditCardRepo;
        private readonly IGenericRepository<Loan> _loanRepo;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public CashierService(
            IGenericRepository<SavingsAccount> accountRepo,
            IGenericRepository<Transaction> transactionRepo,
            IGenericRepository<CreditCard> creditCardRepo,
            IGenericRepository<Loan> loanRepo,
            IMapper mapper,
            IEmailService emailService)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _creditCardRepo = creditCardRepo;
            _loanRepo = loanRepo;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<int> DepositAsync(DepositDto depositDto)
        {
            var account = await _accountRepo.FirstOrDefaultAsync(a => a.AccountNumber == depositDto.DestinationAccountNumber && a.IsActive);
            if (account == null) throw new InvalidOperationException("Destino de cuenta no válido o inactivo.");
            if (depositDto.Amount <= 0) throw new ArgumentException("Monto debe ser mayor que cero.");

            account.Balance += depositDto.Amount;
            await _accountRepo.UpdateAsync(account.Id, account);

            var transaction = new Transaction
            {
                SavingAccountId = account.Id,
                TransactionDate = DateTime.UtcNow,
                Amount = depositDto.Amount,
                Type = TransactionType.Credit,
                Origin = "DEPÓSITO",
                Beneficiary = account.OwnerId,
                UserId = "Cajero", // Assuming current user context is resolved elsewhere
                Status = TransactionStatus.Completed
            };
            await _transactionRepo.AddAsync(transaction);

            // Notify account owner via email
            await _emailService.SendAsync(account.OwnerId, "Depósito recibido", $"Se ha depositado {depositDto.Amount:C} en su cuenta {account.AccountNumber}.");

            return transaction.Id;
        }

        public async Task<int> WithdrawAsync(WithdrawalDto withdrawalDto)
        {
            var account = await _accountRepo.FirstOrDefaultAsync(a => a.AccountNumber == withdrawalDto.SourceAccountNumber && a.IsActive);
            if (account == null) throw new InvalidOperationException("Cuenta origen no válida o inactiva.");
            if (withdrawalDto.Amount <= 0) throw new ArgumentException("Monto debe ser mayor que cero.");
            if (account.Balance < withdrawalDto.Amount) throw new InvalidOperationException("Fondos insuficientes.");

            account.Balance -= withdrawalDto.Amount;
            await _accountRepo.UpdateAsync(account.Id, account);

            var transaction = new Transaction
            {
                SavingAccountId = account.Id,
                TransactionDate = DateTime.UtcNow,
                Amount = withdrawalDto.Amount,
                Type = TransactionType.Debit,
                Origin = "RETIRO",
                Beneficiary = account.OwnerId,
                UserId = "Cajero",
                Status = TransactionStatus.Completed
            };
            await _transactionRepo.AddAsync(transaction);
            return transaction.Id;
        }

        public async Task<int> PayCreditCardAsync(string creditCardNumber, decimal amount)
        {
            var card = await _creditCardRepo.FirstOrDefaultAsync(c => c.CardNumber == creditCardNumber && c.IsActive);
            if (card == null) throw new InvalidOperationException("Tarjeta no válida o inactiva.");
            if (amount <= 0) throw new ArgumentException("Monto debe ser mayor que cero.");
            if (card.Balance < amount) throw new InvalidOperationException("Saldo insuficiente en la tarjeta.");

            card.Balance -= amount;
            await _creditCardRepo.UpdateAsync(card.Id, card);

            var transaction = new Transaction
            {
                SavingAccountId = 0,
                TransactionDate = DateTime.UtcNow,
                Amount = amount,
                Type = TransactionType.Debit,
                Origin = "PAGO TARJETA",
                Beneficiary = card.OwnerId,
                UserId = "Cajero",
                Status = TransactionStatus.Completed
            };
            await _transactionRepo.AddAsync(transaction);
            return transaction.Id;
        }

        public async Task<int> PayLoanAsync(string loanNumber, decimal amount)
        {
            var loan = await _loanRepo.FirstOrDefaultAsync(l => l.LoanNumber == loanNumber && l.IsActive);
            if (loan == _default) throw new InvalidOperationException("Préstamo no válido o inactivo.");
            if (amount <= 0) throw new ArgumentException("Monto debe ser mayor que cero.");
            if (loan.OutstandingBalance < amount) throw new InvalidOperationException("Monto supera el saldo pendiente del préstamo.");

            loan.OutstandingBalance -= amount;
            await _loanRepo.UpdateAsync(loan.Id, loan);

            var transaction = new Transaction
            {
                SavingAccountId = 0,
                TransactionDate = DateTime.UtcNow,
                Amount = amount,
                Type = TransactionType.Debit,
                Origin = "PAGO PRÉSTAMO",
                Beneficiary = loan.BorrowerId,
                UserId = "Cajero",
                Status = TransactionStatus.Completed
            };
            await _transactionRepo.AddAsync(transaction);
            return transaction.Id;
        }

        public async Task<int> TransferToThirdPartyAsync(string destinationAccountNumber, decimal amount)
        {
            var account = await _accountRepo.FirstOrDefaultAsync(a => a.AccountNumber == destinationAccountNumber && a.IsActive);
            if (account == null) throw new InvalidOperationException("Cuenta de tercero no válida.");
            if (amount <= 0) throw new ArgumentException("Monto debe ser mayor que cero.");

            // Assuming the cashier's own working account is a special system account
            var systemAccount = await _accountRepo.FirstOrDefaultAsync(a => a.IsSystem && a.IsActive);
            if (systemAccount == null) throw new InvalidOperationException("Cuenta del cajero no configurada.");
            if (systemAccount.Balance < amount) throw new InvalidOperationException("Fondos insuficientes en la cuenta del cajero.");

            systemAccount.Balance -= amount;
            account.Balance += amount;
            await _accountRepo.UpdateAsync(systemAccount.Id, systemAccount);
            await _accountRepo.UpdateAsync(account.Id, account);

            var transaction = new Transaction
            {
                SavingAccountId = systemAccount.Id,
                TransactionDate = DateTime.UtcNow,
                Amount = amount,
                Type = TransactionType.Debit,
                Origin = "TRANSFERENCIA TERCEROS",
                Beneficiary = account.OwnerId,
                UserId = "Cajero",
                Status = TransactionStatus.Completed
            };
            await _transactionRepo.AddAsync(transaction);
            return transaction.Id;
        }

        public async Task<DashboardDto> GetDashboardAsync(string cashierUserId)
        {
            var today = DateTime.UtcNow.Date;
            var transactions = await _transactionRepo.GetAllListAsync();
            var todays = transactions.Where(t => t.TransactionDate.Date == today && t.UserId == cashierUserId);

            var dto = new DashboardDto
            {
                TransactionsToday = todays.Count(),
                DepositsToday = todays.Count(t => t.Type == TransactionType.Credit && t.Origin == "DEPÓSITO"),
                WithdrawalsToday = todays.Count(t => t.Type == TransactionType.Debit && t.Origin == "RETIRO"),
                PaymentsToday = todays.Count(t => t.Origin == "PAGO TARJETA" || t.Origin == "PAGO PRÉSTAMO")
            };
            return dto;
        }

        public async Task<IReadOnlyList<TransactionDto>> GetTransactionHistoryAsync(string cashierUserId, int page = 1, int pageSize = 20)
        {
            var query = _transactionRepo.GetAll()
                .Where(t => t.UserId == cashierUserId)
                .OrderByDescending(t => t.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var list = await query.ToListAsync();
            return _mapper.Map<IReadOnlyList<TransactionDto>>(list);
        }
    }
}
