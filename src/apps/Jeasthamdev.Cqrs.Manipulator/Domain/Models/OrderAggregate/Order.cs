using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Jeasthamdev.Cqrs.Manipulator.Domain.Models
{
    public class Order
    {
        [JsonProperty(PropertyName = "orderLines")]
        private List<OrderLine> _orderLines; 

        [JsonProperty]
        public int Id { get; private set; }

        [JsonProperty]
        public string OrderNumber { get; private set; }

        [JsonProperty]
        public DateTime OrderDate { get; private set; }

        [JsonIgnore]
        public IReadOnlyCollection<OrderLine> OrderLines => _orderLines;
    }
}