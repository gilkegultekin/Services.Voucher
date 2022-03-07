using System;

namespace Services.Voucher.Application.Dto
{
    public class VoucherDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string ProductCodes { get; set; }
    }
}
