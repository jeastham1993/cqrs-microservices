using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jeasthamdev.Cqrs.Read.Domain.Models;
using Jeasthamdev.Cqrs.Read.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jeasthamdev.Cqrs.Read.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly OrderRepository _orderRepo;

        public OrderController(ILogger<OrderController> logger
            ,OrderRepository orderRepo)
        {
            _logger = logger;
            this._orderRepo = orderRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            var orders = await this._orderRepo.GetAndStoreInCacheIfEmpty();

            return orders;
        }
    }
}
