using AutoMapper;
using Services.Voucher.Domain.Models;

namespace Services.Voucher.Persistence.MongoDB.MapperProfiles
{
    public class VoucherMongoDbProfile : Profile
    {
        public VoucherMongoDbProfile()
        {
            CreateMap<Models.Voucher, VoucherModel>()
                .ForMember(d => d.ProductCodes, opt => opt.MapFrom(s => s.ProductCodes.Split(',', System.StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}
