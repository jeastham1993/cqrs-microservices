using System;

namespace Jeasthamdev.Cqrs.Read.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public int LineCount { get; set; }

        public decimal TotalValue { get; set; }
    }
}