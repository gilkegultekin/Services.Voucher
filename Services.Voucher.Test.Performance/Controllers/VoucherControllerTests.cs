using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Services.Voucher.Controllers;
using Services.Voucher.EntityFramework.Repository;
using Services.Voucher.Test.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [Theory]
        [MemberData(nameof(GetReturningAllData))]
        public async Task Get_ShouldBePerformant(int iterationCount, int maxElapsedMilliseconds)
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(maxElapsedMilliseconds)).Token;

            var task = Task.Run(async () =>
            {
                for (var i = 0; i < iterationCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await _controller.Get();
                }
            }, cancellationToken);

            await task;
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetReturningSubsetData))]
        public async Task Get_ShouldBePerformantWhenReturningASubset(int iterationCount, int rowCount, int maxElapsedMilliseconds)
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(maxElapsedMilliseconds)).Token;

            var task = Task.Run(async () =>
            {
                for (var i = 0; i < iterationCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await _controller.Get(rowCount);
                }
            }, cancellationToken);
            
            await task;
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetCheapestVoucherByProductCodeData))]
        public async Task GetCheapestVoucherByProductCode_ShouldBePerformant(int iterationCount, int maxElapsedMilliseconds)
        {
            //Arrange
            string productCode = "P007DX";
            await Fixture.InsertVouchersWithProductCode(productCode, 100, 200);

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(maxElapsedMilliseconds)).Token;

            var task = Task.Run(async () =>
            {
                for (var i = 0; i < iterationCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await _controller.GetCheapestVoucherByProductCode(productCode);
                }
            }, cancellationToken);

            await task;
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(DefaultData))]
        public async Task GetVoucherById_ShouldBePerformant(int iterationCount, int maxElapsedMilliseconds)
        {
            //Arrange
            var context = Fixture.GetNewContext(); //using a new context, so that the id's don't get tracked in the controller's context.
            var idList = await context.Vouchers.Select(v => v.Id).Take(iterationCount).ToArrayAsync();

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(maxElapsedMilliseconds)).Token;

            var task = Task.Run(async () =>
            {
                for (var i = 0; i < iterationCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await _controller.GetVoucherById(idList[i]);
                }
            }, cancellationToken);

            await task;
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(DefaultData))]
        public async Task GetVouchersByName_ShouldBePerformant(int iterationCount, int maxElapsedMilliseconds)
        {
            //Arrange
            int listCount = 10;
            var context = Fixture.GetNewContext();
            var names = await context.Vouchers.Select(v => v.Name).Take(listCount).ToArrayAsync();

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(maxElapsedMilliseconds)).Token;

            var task = Task.Run(async () =>
            {
                for (var i = 0; i < iterationCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await _controller.GetVouchersByName(names[i % listCount]);
                }
            }, cancellationToken);

            await task;
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(DefaultData))]
        public async Task GetVouchersByNameSearch_ShouldBePerformant(int iterationCount, int maxElapsedMilliseconds)
        {
            //Arrange
            int listCount = 10;
            var context = Fixture.GetNewContext();
            var names = await context.Vouchers.Select(v => v.Name).Take(listCount).ToArrayAsync();

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(maxElapsedMilliseconds)).Token;

            var task = Task.Run(async () =>
            {
                for (var i = 0; i < iterationCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await _controller.GetVouchersByNameSearch(Fixture.GetRandomSubstring(names[i % listCount]));
                }
            }, cancellationToken);

            await task;
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        public static IEnumerable<object[]> GetReturningSubsetData => new List<object[]>
        {
           new object[] { 10, 1, 1000 },
           new object[] { 10, 1000, 1000 },
           new object[] { 10, 10000, 1000 },
           new object[] { 100, 1, 3000 },
           new object[] { 100, 1000, 3000 },
           new object[] { 100, 10000, 6000 },
           new object[] { 1000, 1, 25000 },
           new object[] { 1000, 1000, 25000 },
           new object[] { 1000, 10000, 25000 },
           new object[] { 100000, 1000, 5000 }, //Original execution parameters
        };

        public static IEnumerable<object[]> GetReturningAllData => new List<object[]>
        {
           new object[] { 10, 1000 },
           new object[] { 100, 6000 },
           new object[] { 1000, 15000 },//Original execution parameters
           new object[] { 10000, 25000 },
        };

        public static IEnumerable<object[]> GetCheapestVoucherByProductCodeData => new List<object[]>
        {
           new object[] { 10, 1000 },
           new object[] { 100, 6000 },
           new object[] { 100, 15000 },//Original execution parameters
           new object[] { 1000, 15000 },
           new object[] { 10000, 25000 },
        };

        public static IEnumerable<object[]> DefaultData => new List<object[]>
        {
           new object[] { 10, 1000 },
           new object[] { 100, 6000 },
           new object[] { 1000, 15000 },
           new object[] { 10000, 25000 },
        };

        // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
    }
}
