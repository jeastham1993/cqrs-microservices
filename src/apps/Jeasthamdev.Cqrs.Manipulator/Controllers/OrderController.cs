using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jeasthamdev.Cqrs.Contracts;
using Jeasthamdev.Cqrs.Manipulator.Domain.Events;
using Jeasthamdev.Cqrs.Manipulator.Domain.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jeasthamdev.Cqrs.Manipulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderController(ILogger<OrderController> logger
            ,IOrderRepository orderRepo
            ,IPublishEndpoint publishEndpoint)
        {
            this._orderRepo = orderRepo;
            this._publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<Order> Create([FromBody] Order orderToCreate)
        {
            if (orderToCreate is null)
            {
                throw new ArgumentNullException(nameof(orderToCreate));
            }

            orderToCreate = await this._orderRepo.CreateAsync(orderToCreate);

            await this._publishEndpoint.Publish(new NewOrderAddedEvent{
                OrderId = orderToCreate.Id
            });
            
            return orderToCreate;
        }
    }
}
