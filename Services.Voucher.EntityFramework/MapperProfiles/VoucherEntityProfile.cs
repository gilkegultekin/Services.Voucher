using AutoMapper;
using Services.Voucher.Domain.Models;

namespace Services.Voucher.EntityFramework.MapperProfiles
{
    public class VoucherEntityProfile : Profile
    {
        public VoucherEntityProfile()
        {
            CreateMap<Models.Voucher, VoucherModel>();
        }
    }
}
