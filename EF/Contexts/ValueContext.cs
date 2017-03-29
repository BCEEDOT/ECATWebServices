using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EF.Models;

namespace EF.Contexts
{
    public class ValueContext : DbContext
    {
        private const string LocalDb = "Server=(localdb)\\mssqllocaldb;Database=ecatlocaldev;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static string ContextName { get { return "ValueContext"; } }

        public ValueContext() : base(LocalDb) { }

        public ValueContext(string connectionString = LocalDb) : base(connectionString)
        { 

        }


        public DbSet<ValueItem> ValueItems { get; set; }


    }
}
