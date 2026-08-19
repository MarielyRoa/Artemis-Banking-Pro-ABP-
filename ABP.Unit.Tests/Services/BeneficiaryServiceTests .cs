using ABP.Core.Application.Dtos.Beneficiaries;
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
    public class BeneficiaryServiceTests
    {
        private readonly DbContextOptions<ArtemisBankingAppContext> _dbContextOptions;
        private readonly IMapper _mapper;

        public BeneficiaryServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ArtemisBankingAppContext>()
                .UseInMemoryDatabase(databaseName: $"ArtemisBankingAppTestDb_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BeneficiaryMappingProfile>();

            }, NullLoggerFactory.Instance);

            _mapper = config.CreateMapper();
        }

        private BeneficiaryService CreateService()
        {
            var context = new ArtemisBankingAppContext(_dbContextOptions);
            var beneficiaryRepository = new BeneficiaryRepository(context);
            return new BeneficiaryService(beneficiaryRepository, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Added_Dto()
        {
            //Arrange
            var service = CreateService();
            var beneficiaryDto = new BeneficiaryDto
            {
                Id = 0,
                BeneficiaryName = "Test",
                BeneficiaryLastName = "Beneficiary"
            };

            //Act
            var result = await service.AddAsync(beneficiaryDto);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.BeneficiaryName.Should().Be(beneficiaryDto.BeneficiaryName);
            result.BeneficiaryLastName.Should().Be(beneficiaryDto.BeneficiaryLastName);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Null_On_Exception()
        {
            //Arrange
            var service = CreateService();
            BeneficiaryDto dto = null!;

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
            var added = await service.AddAsync(new BeneficiaryDto { Id = 0, BeneficiaryName="Equity" });
            added!.BeneficiaryName = "Updated Equity";

            //Act
            var updated = await service.UpdateAsync(added, added.Id);

            //Assert
            updated.Should().NotBeNull();
            updated!.BeneficiaryName.Should().Be("Updated Equity");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_No_Exists()
        {
            //Arrange
            var service = CreateService();
            var dto = new BeneficiaryDto { Id = 999, BeneficiaryName = "Ghost" };

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
            var dto = await service.AddAsync(new BeneficiaryDto { Id = 0, BeneficiaryName = "Temporary" });

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
            var dto = await service.AddAsync(new BeneficiaryDto { Id = 0, BeneficiaryName = "Bond" });

            //Act
            var found = await service.GetByIdAsync(dto!.Id);

            //Assert
            found.Should().NotBeNull();
            found!.BeneficiaryName.Should().Be("Bond");
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
            await service.AddAsync(new BeneficiaryDto { Id = 0, BeneficiaryName = "A" });
            await service.AddAsync(new BeneficiaryDto { Id = 0, BeneficiaryName = "B" });

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
