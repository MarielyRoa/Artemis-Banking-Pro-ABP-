using System;
using System.Threading.Tasks;
using ABP.Core.Application.Dtos;
using ABP.Core.Application.Dtos.Cashier;
using ABP.Core.Application.Interfaces;
using ABP.Core.Application.Services;
using ABP.Core.Domain.Entities;
using Moq;
using AutoMapper;
using Xunit;

namespace ABP.Unit.Tests.Services
{
    public class CashierServiceTests
    {
        private readonly Mock<IGenericRepository<SavingsAccount>> _accountRepoMock = new();
        private readonly Mock<IGenericRepository<Transaction>> _transactionRepoMock = new();
        private readonly Mock<IGenericRepository<CreditCard>> _creditCardRepoMock = new();
        private readonly Mock<IGenericRepository<Loan>> _loanRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();

        private ICashierService Service => new CashierService(
            _accountRepoMock.Object,
            _transactionRepoMock.Object,
            _creditCardRepoMock.Object,
            _loanRepoMock.Object,
            _mapperMock.Object,
            _emailServiceMock.Object);

        [Fact]
        public async Task DepositAsync_ShouldCreateTransaction_AndReturnId()
        {
            // Arrange
            var account = new SavingsAccount { Id = 1, AccountNumber = "123", Balance = 100, OwnerId = "user1", IsActive = true };
            _accountRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Func<SavingsAccount, bool>>()))
                .ReturnsAsync(account);
            _transactionRepoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync((Transaction t) => { t.Id = 99; return t; });

            var dto = new DepositDto { DestinationAccountNumber = "123", Amount = 50 };

            // Act
            var result = await Service.DepositAsync(dto);

            // Assert
            Assert.Equal(99, result);
            _accountRepoMock.Verify(r => r.UpdateAsync(account.Id, It.Is<SavingsAccount>(a => a.Balance == 150)), Times.Once);
            _emailServiceMock.Verify(e => e.SendAsync(account.OwnerId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task WithdrawAsync_ShouldThrow_WhenInsufficientFunds()
        {
            var account = new SavingsAccount { Id = 2, AccountNumber = "456", Balance = 30, OwnerId = "user2", IsActive = true };
            _accountRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Func<SavingsAccount, bool>>()))
                .ReturnsAsync(account);
            var dto = new WithdrawalDto { SourceAccountNumber = "456", Amount = 50 };
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await Service.WithdrawAsync(dto));
        }

        // Additional tests for PayCreditCardAsync, PayLoanAsync, TransferToThirdPartyAsync, GetDashboardAsync could be added similarly.
    }
}
