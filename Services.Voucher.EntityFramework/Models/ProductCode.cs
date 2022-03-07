using System.Collections.Generic;

namespace Services.Voucher.EntityFramework.Models
{
    public class ProductCode
    {
        public string Id { get; set; }

        public ICollection<VoucherProductCode> VoucherProductCodes { get; set; }
    }
}
