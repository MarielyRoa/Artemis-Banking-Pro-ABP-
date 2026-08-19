using ABP.Core.Domain.Entities;
using ABP.Infrastructure.Persistence.Contexts;
using ABP.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ABP.Integration.Tests.Persistence.Repositories
{
    public class BeneficiaryRepositoryTest
    {
        private readonly DbContextOptions<ArtemisBankingAppContext> _dbContextOptions;

        public BeneficiaryRepositoryTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ArtemisBankingAppContext>()
                .UseInMemoryDatabase(databaseName: $"ArtemisBankingAppTestDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_Beneficiary_To_Database()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);
            var beneficiary = new Beneficiary
            {
                Id = 0,
                ClientId = "0",
                BeneficiaryName = "Test",
                BeneficiaryLastName = "Beneficiary",
                BeneficiaryAccountNumber = "0",
            };

            //Act
            var result = await beneficiaryRepository.AddAsync(beneficiary);

            //Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
            var beneficiaries = await context.Beneficiaries.ToListAsync();
            beneficiaries.Should().ContainSingle();
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_Null()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);

            //Act
            Func<Task> act = async () => await beneficiaryRepository.AddAsync(null!);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'entity')");
        }

        [Fact]
        public async Task GetById_Should_Return_Beneficiary_When_Exists()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);
            var beneficiary = new Beneficiary
            {
                Id = 0,
                ClientId = "0",
                BeneficiaryName = "Test",
                BeneficiaryLastName = "Beneficiary",
                BeneficiaryAccountNumber = "0",
            };

            beneficiary = await beneficiaryRepository.AddAsync(beneficiary);

            //Act
            var result = await beneficiaryRepository.GetByIdAsync(beneficiary?.Id ?? 0);

            //Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(beneficiary?.Id);
            result.BeneficiaryName.Should().Be(beneficiary?.BeneficiaryName);
            result.BeneficiaryLastName.Should().Be(beneficiary?.BeneficiaryLastName);
            result.BeneficiaryAccountNumber.Should().Be(beneficiary?.BeneficiaryAccountNumber);
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NoExists()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);

            //Act
            var result = await beneficiaryRepository.GetByIdAsync(999);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_Beneficiary_InDatabase()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);
            var beneficiary = new Beneficiary
            {
                Id = 0,
                ClientId = "0",
                BeneficiaryName = "Test",
                BeneficiaryLastName = "Beneficiary",
                BeneficiaryAccountNumber = "0",
            };
            beneficiary = await beneficiaryRepository.AddAsync(beneficiary);
            beneficiary!.BeneficiaryName = "Update beneficiary";

            //Act
            var updated = await beneficiaryRepository.UpdateAsync(beneficiary!.Id, beneficiary!);

            //Assert
            updated.Should().NotBeNull();
            updated!.BeneficiaryName.Should().Be("Update beneficiary");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_Beneficiary_Not_Found()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);
            var fakeBeneficiary = new Beneficiary
            {
                Id = 0,
                ClientId = "0",
                BeneficiaryName = "Test",
                BeneficiaryLastName = "Beneficiary",
                BeneficiaryAccountNumber = "0",
            };

            //Act
            var updated = await beneficiaryRepository.UpdateAsync(fakeBeneficiary!.Id, fakeBeneficiary!);

            //Assert
            updated.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Existing_Beneficiary()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiary = new Beneficiary
            {
                Id = 0,
                ClientId = "0",
                BeneficiaryName = "Test",
                BeneficiaryLastName = "Beneficiary",
                BeneficiaryAccountNumber = "0",
            };
            var repository = new BeneficiaryRepository(context);
            beneficiary = await repository.AddAsync(beneficiary);

            //Act
            await repository.DeleteAsync(beneficiary!.Id);
            var entity = await repository.GetByIdAsync(beneficiary.Id);

            //Assert
            entity.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Remove_When_Id_Not_Found()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var repository = new BeneficiaryRepository(context);

            //Act
            Func<Task> act = async () => await repository.DeleteAsync(999);

            //Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllListAsync_Should_Return_All_Beneficiaries()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            context.Beneficiaries.AddRange(
                new Beneficiary { Id = 0, BeneficiaryName = "Test" },
                new Beneficiary { Id = 0, BeneficiaryName = "Beneficiaries" });
            await context.SaveChangesAsync();
            var repository = new BeneficiaryRepository(context);

            //Act
            var result = await repository.GetAllListAsync();

            //Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllListAsync_Should_Return_Empty_When_No_Beneficiaries()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var repository = new BeneficiaryRepository(context);

            //Act
            var result = await repository.GetAllListAsync();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllQuery_Should_Return_All_Queryable_Beneficiaries()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            context.Beneficiaries.Add(new Beneficiary { Id = 1, BeneficiaryName = "Test", BeneficiaryAccountNumber = "" });
            await context.SaveChangesAsync();
            var repository = new BeneficiaryRepository(context);

            //Act
            var query = repository.GetAllQuery();
            var result = await query.ToListAsync();

            //Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllByClient_Should_Return_Beneficiaries_For_Client()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);

            var clientId = "0";

            var beneficiary = new Beneficiary
            {
                Id = 0,
                ClientId = clientId
            };

            beneficiary = await beneficiaryRepository.AddAsync(beneficiary);

            //Act
            var result = await beneficiaryRepository.GetAllByClientIdAsync(clientId);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().Contain(b => b.ClientId == clientId);
        }

        [Fact]
        public async Task GetAllByClient_Should_Return_Null_When_NoExists()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);

            //Act
            var result = await beneficiaryRepository.GetAllByClientIdAsync("999");

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByAccountAndClientIdAsync_Should_Return_Beneficiaries_For_Account_And_Client()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);

            var account = "123";
            var clientId = "0";

            var beneficiary = new Beneficiary
            {
                Id = 0,
                BeneficiaryAccountNumber = account,
                ClientId = clientId
            };

            beneficiary = await beneficiaryRepository.AddAsync(beneficiary);

            //Act
            var result = await beneficiaryRepository.GetByAccountAndClientIdAsync(account, clientId);

            //Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByAccountAndClientIdAsync_Should_Return_Null_When_NoExists()
        {
            //Arange
            using var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);

            //Act
            var result = await beneficiaryRepository.GetByAccountAndClientIdAsync("999", "235");

            //Assert
            result.Should().BeNull();
        }
    }
}
