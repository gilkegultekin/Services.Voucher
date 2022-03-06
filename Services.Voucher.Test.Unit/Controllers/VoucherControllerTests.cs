using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Services.Voucher.Application.Dto;
using Services.Voucher.Application.Repository;
using Services.Voucher.Controllers;
using Services.Voucher.Domain.Models;
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
        private readonly IVoucherRepository _repository;
        private readonly IMapper _mapper;

        public VoucherControllerTests()
        {
            _repository = Substitute.For<IVoucherRepository>();
            var config = new MapperConfiguration(opts =>
            {
                // Add your mapper profile configs or mappings here
                opts.CreateMap<VoucherModel, VoucherDto>();
            });

            _mapper = config.CreateMapper(); // Use this mapper to instantiate your class
            _controller = new VoucherController(_repository, _mapper);
        }

        [Fact]
        public async Task Get_ShouldReturnAllVouchers_WhenNoCountIsProvided()
        {
            // Arrange
            var vouchers = new List<VoucherModel>();
            for (var i = 0; i < 1000; i++)
            {
                vouchers.Add(new VoucherModel
                {
                    Id = new Guid()
                });
            }
            _repository.GetVouchers().Returns(vouchers);
            

            // Act
            var result = await _controller.Get();
            var resultCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Equal(vouchers.Count(), resultCollection.Count());
        }

        [Fact]
        public async Task Get_ShouldReturnTheRequestedAmountOfVouchers_WhenACountIsProvided()
        {
            // Arrange
            int count = 5;
            var vouchers = new List<VoucherModel>();
            for (var i = 0; i < 1000; i++)
            {
                vouchers.Add(new VoucherModel
                {
                    Id = new Guid()
                });
            }
            _repository.GetVouchers().Returns(vouchers);

            // Act
            var result = await _controller.Get(count);
            var resultCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Equal(count, resultCollection.Count());
        }

        [Fact]
        public void GetVoucherById_StateUnderTest_ExpectedBehavior()
        {
            // TODO
        }

        [Fact]
        public async Task GetVouchersByName_ShouldReturnAllVouchersWithTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            var vouchers = new List<VoucherModel>();
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "A" });
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "A" });
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "B" });
            _repository.GetVouchersByName("A").Returns(vouchers);
            //TODO: Fix this unit test. Will probably have to use in memory db instead of mocking the repo direcly.

            // Act
            var result = await _controller.GetVouchersByName("A");
            var resultCollection = ParseActionResultAsOk<IEnumerable<VoucherDto>>(result.Result);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            Assert.Equal(2, resultCollection.Count());
            Assert.Equal(resultCollection.First().Id, vouchers.ElementAt(0).Id);
            Assert.Equal(resultCollection.ElementAt(1).Id, vouchers.ElementAt(1).Id);
        }

        [Fact]
        public void GetVouchersByNameSearch_ShouldReturnAllVouchersThatContainTheGivenSearchString_WhenVoucherExists()
        {
            // Arrange
            var vouchers = new List<VoucherModel>();
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "ABC"  });
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "ABCD" });
            vouchers.Add(new VoucherModel { Id = new Guid(), Name = "ACD" });
            _repository.GetVouchers().Returns(vouchers);

            // Act
            var result = _controller.GetVouchersByNameSearch("BC");

            // Assert
            Assert.Equal(result, new List<VoucherModel>() { vouchers.ElementAt(0), vouchers.ElementAt(1) });
        }

        [Fact]
        public void GetCheapestVoucherByProductCode_StateUnderTest_ExpectedBehavior()
        {
            // TODO
        }

        // TODO: This is not all the tests that we would like to see + the above tests can be made much smarter.
    }
}
