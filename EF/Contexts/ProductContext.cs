using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EF.Models;

namespace EF.Contexts
{
    public class ProductContext : DbContext
    {
        private const string LocalDb = "Server=(localdb)\\mssqllocaldb;Database=ecatlocaldev;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static string ContextName { get { return "ProductContext"; } }

        public ProductContext() : base(LocalDb) { }

        public ProductContext(string connectionString = LocalDb) : base(connectionString)
        {

        }

        public DbSet<Product> Products { get; set; }


    }
}
