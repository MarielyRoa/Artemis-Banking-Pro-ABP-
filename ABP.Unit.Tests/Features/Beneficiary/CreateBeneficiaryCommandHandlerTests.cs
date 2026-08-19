using ABP.Core.Application.Features.Beneficiaries.Commands.CreateBeneficiary;
using ABP.Core.Application.Interfaces;
using ABP.Infrastructure.Persistence.Contexts;
using ABP.Infrastructure.Persistence.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ABP.Unit.Tests.Features.Beneficiary
{
    public class CreateBeneficiaryCommandHandlerTests
    {
        private readonly DbContextOptions<ArtemisBankingAppContext> _dbContextOptions;
        public CreateBeneficiaryCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ArtemisBankingAppContext>()
                .UseInMemoryDatabase(databaseName: $"ArtemisBankingAppTestDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnBeneficiaryId_WhenCreationSuccessful()
        {
            //Arrange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);

            var savingAccountRepository = new SavingAccountRepository(context);

            var accountServiceMock = new Mock<IBaseAccountService>();

            var handler = new CreateBeneficiaryCommandHandler(beneficiaryRepository, savingAccountRepository, accountServiceMock.Object);

            var command = new CreateBeneficiaryCommand
            {
               BeneficiaryAccountNumber = "89",
               ClientId = "2"
            };

            //Act
            int result = await handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeGreaterThan(0);
            var createdEntity = await context.Beneficiaries.FindAsync(result);
            createdEntity.Should().NotBeNull();
            createdEntity!.BeneficiaryAccountNumber.Should().Be(command.BeneficiaryAccountNumber);
            createdEntity!.ClientId.Should().Be(command.ClientId);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenRepositoryReturnsNull()
        {
            //Arrange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);

            var savingAccountRepository = new SavingAccountRepository(context);

            Mock<IBaseAccountService> accountServiceMock = new();

            CreateBeneficiaryCommandHandler handler = new(beneficiaryRepository, savingAccountRepository, accountServiceMock.Object);

            var command = new CreateBeneficiaryCommand
            {
                BeneficiaryAccountNumber = "89",
                ClientId = "2"
            };

            accountServiceMock.Setup(service => service.("89", "2"))
                  .ReturnsAsync(true);

            //Act
            int result = await handler.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeGreaterThan(0);
            var createdEntity = await context.Beneficiaries.FindAsync(result);
            createdEntity.Should().NotBeNull();
            createdEntity!.BeneficiaryAccountNumber.Should().Be(command.BeneficiaryAccountNumber);
            createdEntity!.ClientId.Should().Be(command.ClientId);
        }
    }
}
