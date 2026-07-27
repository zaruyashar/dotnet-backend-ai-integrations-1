using Microsoft.EntityFrameworkCore;
using NetCoreAI.Project1_Api101.Entities;

namespace NetCoreAI.Project1_Api101.Context
{
    public class ApiContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = localhost; Initial Catalog = ApiAiDb; Integrated Security = true; TrustServerCertificate = true");
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
