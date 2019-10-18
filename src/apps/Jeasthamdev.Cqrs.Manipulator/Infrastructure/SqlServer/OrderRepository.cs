using System.Threading.Tasks;
using Jeasthamdev.Cqrs.Manipulator.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Jeasthamdev.Cqrs.Manipulator.Infrastructure
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CqrsDataContext _context;
        public OrderRepository(CqrsDataContext context)
        {
            this._context = context;
        }

        public async Task<Order> CreateAsync(Order toCreate)
        {
            if (await this._context.Orders.AnyAsync(p => p.OrderNumber.Equals(toCreate.OrderNumber, System.StringComparison.OrdinalIgnoreCase)) == false)
            {
                await this._context.Orders.AddAsync(toCreate);
                await this._context.SaveChangesAsync();
            }

            return toCreate;
        }
    }
}