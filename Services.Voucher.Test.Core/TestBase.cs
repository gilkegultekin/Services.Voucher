using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Voucher.Application.Dto;
using Services.Voucher.Domain.Models;
using System.Linq;
using Xunit;

namespace Services.Voucher.Test.Core
{
    public class TestBase : IClassFixture<EntityFrameworkFixture>
    {
        protected readonly EntityFrameworkFixture Fixture;
        protected readonly IMapper Mapper;

        public TestBase(EntityFrameworkFixture fixture)
        {
            Fixture = fixture;

            var config = new MapperConfiguration(opts =>
            {
                //Add your mapper profile configs or mappings here
                opts.CreateMap<EntityFramework.Models.Voucher, VoucherModel>()
                .ForMember(d => d.ProductCodes, opt => opt.MapFrom(s => s.VoucherProductCodes.Select(vpc => vpc.ProductCodeId).ToArray()));
                opts.CreateMap<VoucherModel, VoucherDto>()
                .ForMember(d => d.ProductCodes, opt => opt.MapFrom(s => string.Join(',', s.ProductCodes)));
            });

            Mapper = config.CreateMapper();
        }

        protected virtual TResult ParseActionResultAsOk<TResult>(ActionResult<TResult> actionResult)
        {
            var okResult = actionResult.Result as OkObjectResult;
            return (TResult)okResult.Value;
        }
    }
}
