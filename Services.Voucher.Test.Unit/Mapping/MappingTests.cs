using AutoMapper;
using Services.Voucher.EntityFramework.MapperProfiles;
using Services.Voucher.MapperProfiles;
using Xunit;

namespace Services.Voucher.Test.Unit.Mapping
{
    public class MappingTests
    {
        [Fact]
        public void AutoMapper_FromDatabaseObject_ToDomainModel_ConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<VoucherEntityProfile>());
            config.AssertConfigurationIsValid();
        }

        [Fact]
        public void AutoMapper_FromDomainModel_ToDto_ConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<VoucherDtoProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}
