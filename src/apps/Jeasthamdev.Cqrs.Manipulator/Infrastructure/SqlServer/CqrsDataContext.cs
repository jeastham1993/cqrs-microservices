using System.Collections.Generic;
using Jeasthamdev.Cqrs.Manipulator.Domain.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Jeasthamdev.Cqrs.Manipulator.Infrastructure
{
    public class CqrsDataContext : DbContext
    {
        public CqrsDataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Category> Category { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options){
            
        }
    }
}