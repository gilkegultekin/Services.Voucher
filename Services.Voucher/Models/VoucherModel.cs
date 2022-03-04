using System;

namespace Services.Voucher.Models
{
    public class VoucherModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string ProductCodes { get; set; }
    }
}
