using System;

namespace Services.Voucher.Application.Dto
{
    public class VoucherDto
    {
        public VoucherDto(Guid id, string name, double price, string productCodes)
        {
            Id = id;
            Name = name;
            Price = price;
            ProductCodes = productCodes;
        }

        public Guid Id { get; }

        public string Name { get; }

        public double Price { get; }

        public string ProductCodes { get; }
    }
}
