using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Services.Voucher.Core.Exceptions;
using Services.Voucher.Domain.Models;
using Services.Voucher.EntityFramework.Contexts;
using Services.Voucher.EntityFramework.Repository;
using Services.Voucher.Test.Core;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Services.Voucher.Test.Unit.Repository
{
    //Since the test classes in this assembly share the fixture instance (and therefore the in-memory DB), they shouldn't run in parallel.
    //This attribute ensures that they run sequentially by placing them in the same test collection.
    [Collection("Our Test Collection #1")]
    public class VoucherRepositoryTests : TestBase
    {
        private readonly VoucherContext _voucherContext;
        private readonly VoucherRepository _voucherRepository;

        public VoucherRepositoryTests(EntityFrameworkFixture fixture) : base(fixture)
        {
            _voucherContext = Fixture.GetNewContext();
            _voucherRepository = new VoucherRepository(_voucherContext, Mapper, Substitute.For<ILogger<VoucherRepository>>());
        }

        [Fact]
        public async Task GetVouchers_ShouldReturnAllVouchers_WhenNoCountIsProvided()
        {
            //Arrange
            int expectedCount = await _voucherContext.Vouchers.CountAsync();

            // Act
            var result = await _voucherRepository.GetVouchers(take: 0, skip: 0);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(expectedCount);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task GetVouchers_ShouldReturnTheRequestedAmountOfVouchers_WhenACountIsProvided(int take)
        {
            // Act
            var result = await _voucherRepository.GetVouchers(take: take, skip: 0);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(take);
        }

        [Fact]
        public async void GetVoucherById_ShouldReturnVoucher_WhenVoucherExists()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            await Fixture.InsertVoucherWithId(id);

            // Act
            var voucher = await _voucherRepository.GetVoucherById(id);

            // Assert
            voucher.Should().NotBeNull();
            voucher.Id.Should().Be(id);
        }

        [Fact]
        public async void GetVoucherById_ShouldReturnNull_WhenVoucherDoesNotExist()
        {
            //Arrange
            Guid id = Guid.NewGuid();

            // Act
            var voucher = await _voucherRepository.GetVoucherById(id);

            // Assert
            voucher.Should().BeNull();
        }

        [Fact]
        public async Task GetVouchersByName_ShouldReturnAllVouchersWithTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            string name = "Z";
            int count = 10;
            await Fixture.InsertVouchersWithName(name, count);

            // Act
            var result = await _voucherRepository.GetVouchersByName(name);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(count);
            result.Should().AllSatisfy(v => v.Name.Should().Be(name));
        }

        [Fact]
        public async Task GetVouchersByName_ShouldReturnEmptyCollection_WhenVoucherWithNameDoesNotExist()
        {
            // Arrange
            string name = "XYZ";

            // Act
            var result = await _voucherRepository.GetVouchersByName(name);

            // Assert
            result.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task SearchVouchersByName_ShouldReturnAllVouchersThatContainTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            int count = 20;
            string search = "FGHBN";
            await Fixture.InsertVouchersWithName($"A{search}Z", 5);
            await Fixture.InsertVouchersWithName($"{search}4", 5);
            await Fixture.InsertVouchersWithName($"3{search}", 5);
            await Fixture.InsertVouchersWithName(search, 5);

            // Act
            var result = await _voucherRepository.SearchVouchersByName(search);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(count);
            result.Should().AllSatisfy(v => v.Name.Should().Contain(search));
        }

        [Fact]
        public async Task GetVouchersByNameSearch_ShouldReturnEmptyCollection_WhenVoucherContainingNameDoesNotExist()
        {
            // Arrange
            string search = "RNDMXYZ";

            // Act
            var result = await _voucherRepository.SearchVouchersByName(search);

            // Assert
            result.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async void GetCheapestVoucherByProductCode_ShouldReturnCheapestVoucher_WhenProductCodeExists()
        {
            // Arrange
            string productCode = "Gbbrsh123";
            double cheapestPrice = 5;
            var cheapestVoucherId = await Fixture.InsertVoucherWithProductCodeAndPrice(productCode, cheapestPrice);
            await Fixture.InsertVouchersWithProductCode(productCode, 10, 20, 10);

            // Act
            var result = await _voucherRepository.GetCheapestVoucherByProductCode(productCode);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cheapestVoucherId);
            result.ProductCodes.Should().Contain(productCode);
            result.Price.Should().Be(cheapestPrice);
        }

        [Fact]
        public async void GetCheapestVoucherByProductCode_ShouldThrowCustomServiceException_WhenProductCodeDoesNotExist()
        {
            // Arrange
            string productCode = "MrGbbrsh123";
            Func<Task<VoucherModel>> funcToExecute = async () => await _voucherRepository.GetCheapestVoucherByProductCode(productCode);

            // Assert
            await funcToExecute.Should().ThrowExactlyAsync<CustomServiceException>();
        }
    }
}