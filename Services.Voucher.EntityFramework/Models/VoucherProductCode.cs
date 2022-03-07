using System;

namespace Services.Voucher.EntityFramework.Models
{
    public class VoucherProductCode
    {
        public Guid Id { get; set; }

        public Guid VoucherId { get; set; }

        public Voucher Voucher { get; set; }

        public string ProductCodeId { get; set; }

        public ProductCode ProductCode { get; set; }
    }
}
