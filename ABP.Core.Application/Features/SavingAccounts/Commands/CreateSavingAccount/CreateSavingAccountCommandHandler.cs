using ABP.Core.Application.Dtos.SavingAccounts;
using ABP.Core.Domain.Common.Enums;
using ABP.Core.Domain.Entities;
using ABP.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;


namespace ABP.Core.Application.Features.SavingAccounts.Commands.CreateSavingAccount
{
    public class CreateSavingAccountCommandHandler
         : IRequestHandler<CreateSavingAccountCommand, SavingAccountDto>
    {
        private readonly ISavingAccountRepository _savingAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public CreateSavingAccountCommandHandler(
            ISavingAccountRepository savingAccountRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _savingAccountRepository = savingAccountRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<SavingAccountDto> Handle(CreateSavingAccountCommand request, CancellationToken cancellationToken)
        {
            // Regla: solo una cuenta principal activa por cliente
            if (request.AccountType == SavingAccountType.Main)
            {
                var existingPrincipal = await _savingAccountRepository.GetPrincipalAccountByClientIdAsync(request.ClientId);
                if (existingPrincipal != null)
                    throw new InvalidOperationException("El cliente ya tiene una cuenta principal activa.");
            }

            var accountNumber = await GenerateUniqueAccountNumberAsync();

            var account = new SavingAccount
            {
                Id = 0,
                AccountNumber = accountNumber,
                ClientId = request.ClientId,
                Balance = request.InitialBalance,
                AccountType = request.AccountType,
                Status = SavingAccountStatus.Active
            };

            var createdAccount = await _savingAccountRepository.AddAsync(account);

            // Registrar el monto inicial como crédito, si aplica
            if (request.InitialBalance > 0 && createdAccount != null)
            {
                var transaction = new Transaction
                {
                    Id = 0,
                    SavingAccountId = createdAccount.Id,
                    TransactionDate = DateTime.UtcNow,
                    Amount = request.InitialBalance,
                    Type = TransactionType.Credit,
                    Beneficiary = string.Empty,
                    Origin = "Depósito inicial",
                    Status = TransactionStatus.Approved,
                    ResponsibleUserId = null
                };
                await _transactionRepository.AddAsync(transaction);
            }

            return _mapper.Map<SavingAccountDto>(createdAccount);
        }

        private async Task<string> GenerateUniqueAccountNumberAsync()
        {
            string accountNumber;
            bool exists;
            var random = new Random();
            do
            {
                accountNumber = random.Next(100000000, 999999999).ToString();
                exists = await _savingAccountRepository.ExistsAccountNumberAsync(accountNumber);
            } while (exists);

            return accountNumber;
        }
    }
}
