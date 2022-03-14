using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Services.Voucher.Persistence.MongoDB.Models
{
    public class Voucher
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string ProductCodes { get; set; }
    }
}
