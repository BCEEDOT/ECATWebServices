using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF.Contexts;
using Breeze.Persistence.EF6;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;

namespace EF.Models
{
    public class ProductRepository : IProductRepository
    {
        //breeze manager
        private readonly EFPersistenceManager<ProductContext> _persistenceManager;

        //inject the context
        public ProductRepository(ProductContext productContext) {
            //new up the breeze manager using the context
            _persistenceManager = new EFPersistenceManager<ProductContext>(productContext);
        }

        public IEnumerable<Product> GetProducts() {
            return _persistenceManager.Context.Products;
        }

        public void Add(Product product) {
            _persistenceManager.Context.Products.Add(product);
            _persistenceManager.Context.SaveChanges();
        }

        public void Add(string descript) {
            _persistenceManager.Context.Products.Add(new Product { Description = descript });
            _persistenceManager.Context.SaveChanges();
        }

        //breeze specific methods
        public string Metadata() {
            return _persistenceManager.Metadata();
        }

        public SaveResult SaveChanges(JObject saveBundle) {
            return _persistenceManager.SaveChanges(saveBundle);
        }

    }
}
