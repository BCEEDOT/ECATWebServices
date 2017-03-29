using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Contexts
{
    public class ValueContextFactory : IDbContextFactory<ValueContext>
    {
        public ValueContext Create()
        {
            return new ValueContext("Server=(localdb)\\mssqllocaldb;Database=ecatlocaldev;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
