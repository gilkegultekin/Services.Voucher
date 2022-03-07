using Microsoft.Extensions.Logging;
using NSubstitute;
using Services.Voucher.Controllers;
using Services.Voucher.EntityFramework.Repository;
using Services.Voucher.Test.Core;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Services.Voucher.Test.Performance.Controllers
{
    public class VoucherControllerTests : TestBase
    {
        private readonly VoucherController _controller;

        public VoucherControllerTests(EntityFrameworkFixture fixture) : base(fixture)
        {
            var dbContext = Fixture.GetNewContext();
            var repository = new VoucherRepository(dbContext, Mapper, Substitute.For<ILogger<VoucherRepository>>());
            _controller = new VoucherController(repository, Mapper);
        }

        [Fact]
        public async Task Get_ShouldBePerformant()
        {
            var startTime = DateTime.Now;

            for (var i = 0; i < 500; i++)
            {
                await _controller.Get();
            }

            var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            Assert.True(elapsed < 15000);
        }

        [Fact]
        public async Task Get_ShouldBePerformantWhenReturningASubset()
        {
            var startTime = DateTime.Now;

            for (var i = 0; i < 500; i++)
            {
                await _controller.Get(1000);
            }

            var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            Assert.True(elapsed < 15000);
        }

        [Fact]
        public async Task GetCheapestVoucherByProductCode_ShouldBePerformant()
        {
            string productCode = "P007DX";
            await Fixture.InsertVouchersWithProductCode(productCode, 100, 200);

            var startTime = DateTime.Now;

            for (var i = 0; i < 1000; i++)
            {
                await _controller.GetCheapestVoucherByProductCode(productCode);
            }

            var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            Assert.True(elapsed < 15000);
        }

        // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
    }
}
