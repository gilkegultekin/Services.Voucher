using System;
using System.Collections.Generic;

namespace Services.Voucher.Domain.Models
{
    public class VoucherModel
    {
        protected VoucherModel()
        {

        }

        public VoucherModel(Guid id, string name, double price, string[] productCodes)
        {
            Id = id;
            Name = name;
            Price = price;
            ProductCodes = productCodes;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public double Price { get; private set; }

        public string[] ProductCodes { get; private set; }
    }
}
