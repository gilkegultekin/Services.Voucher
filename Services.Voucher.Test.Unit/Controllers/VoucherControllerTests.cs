using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Services.Voucher.Application.Dto;
using Services.Voucher.Controllers;
using Services.Voucher.Core.Exceptions;
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
    //Since the test classes in this assembly share the fixture instance (and therefore the in-memory DB), they shouldn't run in parallel.
    //This attribute ensures that they run sequentially by placing them in the same test collection.
    [Collection("Our Test Collection #1")]
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
        public async void GetVoucherById_ShouldReturn404_WhenVoucherDoesNotExist()
        {
            //Arrange
            Guid id = Guid.NewGuid();

            // Act
            var result = await _controller.GetVoucherById(id);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
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
        public async Task GetVouchersByName_ShouldReturnEmptyCollection_WhenVoucherWithNameDoesNotExist()
        {
            // Arrange
            string name = "ABC";

            // Act
            var result = await _controller.GetVouchersByName(name);
            var dtoCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Empty(dtoCollection);
        }

        [Fact]
        public async Task GetVouchersByNameSearch_ShouldReturnAllVouchersThatContainTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            int count = 20;
            string search = "BX1CY";
            await Fixture.InsertVouchersWithName($"A{search}Z", 5);
            await Fixture.InsertVouchersWithName($"{search}4", 5);
            await Fixture.InsertVouchersWithName($"3{search}", 5);
            await Fixture.InsertVouchersWithName($"{search}", 5);

            // Act
            var result = await _controller.GetVouchersByNameSearch(search);
            var dtoCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Equal(count, dtoCollection.Count());
            Assert.True(dtoCollection.All(x => x.Name.Contains(search)));
        }

        [Fact]
        public async Task GetVouchersByNameSearch_ShouldReturnEmptyCollection_WhenVoucherContainingNameDoesNotExist()
        {
            // Arrange
            string search = "RNDM";

            // Act
            var result = await _controller.GetVouchersByNameSearch(search);
            var dtoCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Empty(dtoCollection);
        }

        [Fact]
        public async void GetCheapestVoucherByProductCode_ShouldReturnCheapestVoucher_WhenProductCodeExists()
        {
            // Arrange
            string productCode = "Gbbrsh";
            var cheapestVoucherId = await Fixture.InsertVoucherWithProductCodeAndPrice(productCode, 5);
            await Fixture.InsertVouchersWithProductCode(productCode, 10, 20, 10);

            // Act
            var result = await _controller.GetCheapestVoucherByProductCode(productCode);
            var dto = ParseActionResultAsOk<VoucherDto>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.True(dto != null);
            Assert.True(dto.Id == cheapestVoucherId);
        }

        [Fact]
        public async void GetCheapestVoucherByProductCode_ShouldThrowCustomServiceException_WhenProductCodeDoesNotExist()
        {
            // Arrange
            string productCode = "MrGbbrsh";
            Func<Task<ActionResult<VoucherDto>>> funcToExecute = async () => await _controller.GetCheapestVoucherByProductCode(productCode);

            // Assert
            await funcToExecute.Should().ThrowExactlyAsync<CustomServiceException>();
        }

        // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
    }
}
