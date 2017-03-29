using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;

namespace EF.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        void Add(Product product);
        void Add(string descript);

        //Breeze specific methods
        string Metadata();
        SaveResult SaveChanges(JObject saveBundle);
    }
}
