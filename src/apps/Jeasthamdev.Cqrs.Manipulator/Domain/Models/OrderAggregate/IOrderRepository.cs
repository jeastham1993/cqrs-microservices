using System.Threading.Tasks;

namespace Jeasthamdev.Cqrs.Manipulator.Domain.Models
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order toCreate);
    }
}