using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Services.Voucher.Controllers;
using Services.Voucher.EntityFramework.Contexts;
using Services.Voucher.EntityFramework.Repository;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Services.Voucher.Test.Performance.Controllers
{
    public class VoucherControllerTests
    {
        private readonly VoucherController _controller;

        public VoucherControllerTests()
        {
            var config = new MapperConfiguration(opts =>
            {
                // Add your mapper profile configs or mappings here
            });

            var mapper = config.CreateMapper(); // Use this mapper to instantiate your class
            var options = new DbContextOptionsBuilder<VoucherContext>()
            .UseInMemoryDatabase(databaseName: "PerformanceTestDB")
            .Options;
            var context = new VoucherContext(options);
            var repository = new VoucherRepository(context, mapper);
            _controller = new VoucherController(repository, mapper);
        }

        [Fact]
        public async Task Get_ShouldBePerformant()
        {
            var startTime = DateTime.Now;

            for (var i = 0; i < 1000; i++)
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

            for (var i = 0; i < 100000; i++)
            {
                await _controller.Get(1000);
            }

            var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            Assert.True(elapsed < 5000);
        }

        [Fact]
        public void GetCheapestVoucherByProductCode_ShouldBePerformant()
        {
            var startTime = DateTime.Now;

            for (var i = 0; i < 100; i++)
            {
                _controller.GetCheapestVoucherByProductCode("P007D");
            }

            var elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            Assert.True(elapsed < 15000);
        }

        // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
    }
}
