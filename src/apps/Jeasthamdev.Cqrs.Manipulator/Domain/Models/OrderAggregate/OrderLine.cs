using System;
using Newtonsoft.Json;

namespace Jeasthamdev.Cqrs.Manipulator.Domain.Models
{
    public class OrderLine
    {
        [JsonProperty]
        public int Id { get; private set; }

        [JsonProperty]
        public string Product { get; private set; }

        [JsonProperty]
        public decimal Value { get; set; }

        [JsonProperty]
        public int? CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category? Category { get; set; }

        [JsonProperty]
        public virtual int OrderId { get; private set; }

        [JsonIgnore]
        public virtual Order Order { get; set; }
    }
}