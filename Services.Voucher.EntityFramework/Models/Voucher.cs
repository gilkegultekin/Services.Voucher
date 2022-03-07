using System;
using System.Collections.Generic;

namespace Services.Voucher.EntityFramework.Models
{
    public class Voucher
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public ICollection<VoucherProductCode> VoucherProductCodes { get; set; }
    }
}
