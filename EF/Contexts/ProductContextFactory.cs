using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Contexts
{
    public class ProductContextFactory : IDbContextFactory<ProductContext>
    {
        public ProductContext Create()
        {
            return new ProductContext("Server=(localdb)\\mssqllocaldb;Database=ecatlocaldev;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
