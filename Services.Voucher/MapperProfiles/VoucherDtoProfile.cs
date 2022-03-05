using AutoMapper;
using Services.Voucher.Application.Dto;
using Services.Voucher.Domain.Models;

namespace Services.Voucher.MapperProfiles
{
    public class VoucherDtoProfile : Profile
    {
        public VoucherDtoProfile()
        {
            CreateMap<VoucherModel, VoucherDto>();
        }
    }
}
