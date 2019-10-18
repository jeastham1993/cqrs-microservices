using System;

namespace Jeasthamdev.Cqrs.Contracts
{
    public interface NewOrderAdded
    {
        int OrderId { get; set; }
    }
}
