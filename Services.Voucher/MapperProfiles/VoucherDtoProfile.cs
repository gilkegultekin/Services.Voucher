using AutoMapper;
using Services.Voucher.Application.Dto;
using Services.Voucher.Domain.Models;
using System.Linq;

namespace Services.Voucher.MapperProfiles
{
    public class VoucherDtoProfile : Profile
    {
        public VoucherDtoProfile()
        {
            CreateMap<VoucherModel, VoucherDto>()
                .ForMember(d => d.ProductCodes, opt => opt.MapFrom(s => string.Join(',', s.ProductCodes)));
        }
    }
}
