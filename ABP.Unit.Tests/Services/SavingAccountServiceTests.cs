using ABP.Core.Application.Dtos.Beneficiaries;
using ABP.Core.Application.Dtos.CreditCards;
using ABP.Core.Application.Dtos.SavingAccounts;
using ABP.Core.Application.Mappings.DtosAndViewModels;
using ABP.Core.Application.Mappings.EntitiesAndDtos;
using ABP.Core.Application.Services;
using ABP.Core.Domain.Entities;
using ABP.Infrastructure.Persistence.Contexts;
using ABP.Infrastructure.Persistence.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ABP.Unit.Tests.Services
{
    public class SavingAccountServiceTests
    {
        private readonly DbContextOptions<ArtemisBankingAppContext> _dbContextOptions;
        private readonly IMapper _mapper;

        public SavingAccountServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ArtemisBankingAppContext>()
                .UseInMemoryDatabase(databaseName: $"ArtemisBankingAppTestDb_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SavingAccountMappingProfile>();

            }, NullLoggerFactory.Instance);

            _mapper = config.CreateMapper();
        }

        private SavingAccountService CreateService()
        {
            var context = new ArtemisBankingAppContext(_dbContextOptions);
            var savingAccountRepository = new SavingAccountRepository(context);
            return new SavingAccountService(savingAccountRepository, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Added_Dto()
        {
            //Arrange
            var service = CreateService();
            var savingAccountDto = new SavingAccountDto
            {
                Id = 0,
                AccountNumber = "123",
                ClientId = "0"
            };

            //Act
            var result = await service.AddAsync(savingAccountDto);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.AccountNumber.Should().Be(savingAccountDto.AccountNumber);
            result.ClientId.Should().Be(savingAccountDto.ClientId);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Null_On_Exception()
        {
            //Arrange
            var service = CreateService();
            SavingAccountDto dto = null!;

            //Act
            var result = await service.AddAsync(dto);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Entity_When_Exists()
        {
            //Arrange
            var service = CreateService();
            var added = await service.AddAsync(new SavingAccountDto { Id = 0, AccountNumber="456" });
            added!.AccountNumber = "456";

            //Act
            var updated = await service.UpdateAsync(added, added.Id);

            //Assert
            updated.Should().NotBeNull();
            updated!.AccountNumber.Should().Be("456");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_No_Exists()
        {
            //Arrange
            var service = CreateService();
            var dto = new SavingAccountDto { Id = 999, AccountNumber = "678" };

            //Act
            var updated = await service.UpdateAsync(dto, dto.Id);

            //Assert
            updated.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Deleted()
        {
            //Arrange
            var service = CreateService();
            var dto = await service.AddAsync(new SavingAccountDto { Id = 0, AccountNumber = "897" });

            //Act
            var deleted = await service.DeleteAsync(dto!.Id);

            //Assert
            deleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Exception()
        {
            //Arrange
            var service = CreateService();

            //Act
            var deleted = await service.DeleteAsync(999);

            //Assert
            deleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Dto_When_Exists()
        {
            //Arrange
            var service = CreateService();
            var dto = await service.AddAsync(new SavingAccountDto { Id = 0, AccountNumber = "6754" });

            //Act
            var found = await service.GetByIdAsync(dto!.Id);

            //Assert
            found.Should().NotBeNull();
            found!.AccountNumber.Should().Be("6754");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_NotFound()
        {
            //Arrange
            var service = CreateService();

            //Act
            var found = await service.GetByIdAsync(999);

            //Assert
            found.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_List_Of_Dtos()
        {
            //Arrange
            var service = CreateService();
            await service.AddAsync(new SavingAccountDto { Id = 0, AccountNumber = "1" });
            await service.AddAsync(new SavingAccountDto { Id = 0, AccountNumber = "2" });

            //Act
            var result = await service.GetAllAsync();

            //Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Empty_When_Exception()
        {
            //Arrange
            var service = CreateService();

            //Act
            var result = await service.GetAllAsync();

            //Assert
            result.Should().BeEmpty();
        }
    }
}
