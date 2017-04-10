using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Ecat.Data.Models;

namespace Ecat.Data.Contexts
{
    public class ValueContext : DbContext
    {
        //private const string LocalDb = "Server=(localdb)\\mssqllocaldb;Database=ecatlocaldev;Trusted_Connection=True;MultipleActiveResultSets=true";

        //public static string ContextName { get { return "ValueContext"; } }

        public static string dbConn;

        public ValueContext() : base("ecat") { }

        public ValueContext(string connectionString) : base(connectionString)
        {
            dbConn = connectionString;
        }


        public DbSet<ValueItem> ValueItems { get; set; }


    }
}
