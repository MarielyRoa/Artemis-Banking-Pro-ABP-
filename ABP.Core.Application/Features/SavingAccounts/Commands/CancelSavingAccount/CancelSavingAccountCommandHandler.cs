using ABP.Core.Domain.Common.Enums;
using ABP.Core.Domain.Interfaces;
using ABP.Core.Domain.Entities;
using MediatR;

namespace ABP.Core.Application.Features.SavingAccounts.Commands.CancelSavingAccount
{
    public class CancelSavingAccountCommandHandler : IRequestHandler<CancelSavingAccountCommand, bool>
    {
        private readonly ISavingAccountRepository _savingAccountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public CancelSavingAccountCommandHandler(
            ISavingAccountRepository savingAccountRepository,
            ITransactionRepository transactionRepository)
        {
            _savingAccountRepository = savingAccountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<bool> Handle(CancelSavingAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _savingAccountRepository.GetByIdAsync(request.SavingAccountId);
            if (account == null)
                throw new InvalidOperationException("La cuenta no existe.");

            // Solo se pueden cancelar cuentas secundarias
            if (account.AccountType != SavingAccountType.Secondary)
                throw new InvalidOperationException("Solo se pueden cancelar cuentas secundarias.");

            if (account.Status != SavingAccountStatus.Active)
                throw new InvalidOperationException("La cuenta ya está cancelada.");

            // Si tiene balance, se transfiere completo a la cuenta principal, con registro cruzado
            if (account.Balance > 0)
            {
                var principalAccount = await _savingAccountRepository.GetPrincipalAccountByClientIdAsync(account.ClientId);
                if (principalAccount == null)
                    throw new InvalidOperationException("El cliente no tiene una cuenta principal activa para recibir el balance.");

                var amountToTransfer = account.Balance;

                // Débito en la cuenta secundaria que se cancela
                await _transactionRepository.AddAsync(new Transaction
                {
                    Id = 0,
                    SavingAccountId = account.Id,
                    TransactionDate = DateTime.UtcNow,
                    Amount = amountToTransfer,
                    Type = TransactionType.Debit,
                    Beneficiary = string.Empty,
                    Origin = "Cancelación de cuenta - transferencia a principal",
                    Status = TransactionStatus.Approved,
                    ResponsibleUserId = null
                });

                // Crédito en la cuenta principal que recibe
                await _transactionRepository.AddAsync(new Transaction
                {
                    Id = 0,
                    SavingAccountId = principalAccount.Id,
                    TransactionDate = DateTime.UtcNow,
                    Amount = amountToTransfer,
                    Type = TransactionType.Credit,
                    Beneficiary = string.Empty,
                    Origin = $"Balance recibido por cancelación de cuenta {account.AccountNumber}",
                    Status = TransactionStatus.Approved,
                    ResponsibleUserId = null
                });

                principalAccount.Balance += amountToTransfer;
                await _savingAccountRepository.UpdateAsync(principalAccount.Id, principalAccount);

                account.Balance = 0;
            }

            account.Status = SavingAccountStatus.Cancelled;
            await _savingAccountRepository.UpdateAsync(account.Id, account);

            return true;
        }
    }
}
