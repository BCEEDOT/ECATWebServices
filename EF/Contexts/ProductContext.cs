using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ECATDataLib.Models;

namespace ECATDataLib.Contexts
{
    public class ProductContext : DbContext
    {
        //private const string LocalDb = "Server=(localdb)\\mssqllocaldb;Database=ecatlocaldev;Trusted_Connection=True;MultipleActiveResultSets=true";

        private static string dbConn;

        //public static string ContextName { get { return "ProductContext"; } }

        public ProductContext() : base("ecat") { }

        public ProductContext(string connectionString) : base(connectionString)
        {
            dbConn = connectionString;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserTest> Users { get; set; }

    }
}
