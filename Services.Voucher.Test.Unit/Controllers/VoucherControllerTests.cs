using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Services.Voucher.Application.Dto;
using Services.Voucher.Controllers;
using Services.Voucher.EntityFramework.Contexts;
using Services.Voucher.EntityFramework.Repository;
using Services.Voucher.Test.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Services.Voucher.Test.Unit.Controllers
{
    public class VoucherControllerTests : TestBase
    {
        private readonly VoucherController _controller;
        private readonly VoucherContext _voucherContext;

        public VoucherControllerTests(EntityFrameworkFixture fixture) : base(fixture)
        {
            _voucherContext = Fixture.GetNewContext();
            var repository = new VoucherRepository(_voucherContext, Mapper, Substitute.For<ILogger<VoucherRepository>>());
            _controller = new VoucherController(repository, Mapper);
        }

        [Fact]
        public async Task Get_ShouldReturnAllVouchers_WhenNoCountIsProvided()
        {
            // Act
            var result = await _controller.Get();
            var dtoCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Equal(_voucherContext.Vouchers.Count(), dtoCollection.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task Get_ShouldReturnTheRequestedAmountOfVouchers_WhenACountIsProvided(int count)
        {
            // Act
            var result = await _controller.Get(count);
            var dtoCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Equal(count, dtoCollection.Count());
        }

        [Fact]
        public async void GetVoucherById_ShouldReturnVoucher_WhenVoucherExists()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            await Fixture.InsertVoucherWithId(id);

            // Act
            var result = await _controller.GetVoucherById(id);
            var dto = ParseActionResultAsOk<VoucherDto>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.True(dto != null);
            Assert.True(dto.Id == id);
        }

        [Fact]
        public async Task GetVouchersByName_ShouldReturnAllVouchersWithTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            string name = "A";
            int count = 10;
            await Fixture.InsertVouchersWithName(name, count);

            // Act
            var result = await _controller.GetVouchersByName(name);
            var dtoCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Equal(count, dtoCollection.Count());
            Assert.True(dtoCollection.All(x => x.Name == name));
        }

        [Fact]
        public async Task GetVouchersByNameSearch_ShouldReturnAllVouchersThatContainTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            int count = 20;
            string search = "BX1CY";
            await Fixture.InsertVouchersWithName("ABX1CYZ", 5);
            await Fixture.InsertVouchersWithName("BX1CY4", 5);
            await Fixture.InsertVouchersWithName("3BX1CY", 5);
            await Fixture.InsertVouchersWithName("BX1CY", 5);

            // Act
            var result = await _controller.GetVouchersByNameSearch(search);
            var dtoCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Equal(count, dtoCollection.Count());
            Assert.True(dtoCollection.All(x => x.Name.Contains(search)));
        }

        [Fact]
        public async void GetCheapestVoucherByProductCode_ShouldReturnCheapestVoucher_WhenProductCodeExists()
        {
            // Arrange
            string productCode = "Gbbrsh";
            var cheapestVoucherId = await Fixture.InsertVoucherWithProductCodeAndPrice(productCode, 5);
            await Fixture.InsertVouchersWithProductCode(productCode, 10, 20, 10, false);

            // Act
            var result = await _controller.GetCheapestVoucherByProductCode(productCode);
            var dto = ParseActionResultAsOk<VoucherDto>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.True(dto != null);
            Assert.True(dto.Id == cheapestVoucherId);
        }

        // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
    }
}
