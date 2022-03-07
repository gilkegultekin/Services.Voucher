using AutoMapper;
using Services.Voucher.Domain.Models;
using System.Linq;

namespace Services.Voucher.EntityFramework.MapperProfiles
{
    public class VoucherEntityProfile : Profile
    {
        public VoucherEntityProfile()
        {
            CreateMap<Models.Voucher, VoucherModel>()
                .ForMember(d => d.ProductCodes, opt => opt.MapFrom(s => s.VoucherProductCodes.Select(vpc => vpc.ProductCodeId).ToArray()));
        }
    }
}
