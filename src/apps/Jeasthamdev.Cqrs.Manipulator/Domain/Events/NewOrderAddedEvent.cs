using Jeasthamdev.Cqrs.Contracts;

namespace Jeasthamdev.Cqrs.Manipulator.Domain.Events
{
    public class NewOrderAddedEvent : NewOrderAdded
    {
        public int OrderId { get; set; }
    }
}