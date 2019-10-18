using System.Threading.Tasks;
using Jeasthamdev.Cqrs.Contracts;
using Jeasthamdev.Cqrs.Read.Domain.Common;
using Jeasthamdev.Cqrs.Read.Infrastructure;
using MassTransit;
using Microsoft.Data.SqlClient;

namespace Jeasthamdev.Cqrs.Read.Domain.Events
{
    public class NewOrderConsumer : IConsumer<NewOrderAdded>
    {
        private readonly OrderRepository _orderRepo;

        public NewOrderConsumer(OrderRepository orderRepo)
        {
            this._orderRepo = orderRepo;
        }

        public async Task Consume(ConsumeContext<NewOrderAdded> context)
        {
            await this._orderRepo.AddSpecificOrderToCache(context.Message.OrderId);    
        }
    }
}