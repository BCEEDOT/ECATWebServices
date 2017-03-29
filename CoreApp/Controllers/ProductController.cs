using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EF.Contexts;
using EF.Models;
using Breeze.AspNetCore;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;
using Breeze.Persistence.EF6;

namespace CoreApp.Controllers
{
    //standard breeze routing and attribute
    [Route("breeze/[controller]/[action]")]
    [BreezeQueryFilter]
    public class ProductController : Controller
    {

        private readonly IProductRepository _productRepository;

        //inject the repository
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return _productRepository.GetProducts();
        }

        [HttpGet]
        public void AddNewTest() {
            _productRepository.Add("test");
        }

        //breeze specific methods
        [HttpGet]
        public string Metadata()
        {
            return _productRepository.Metadata();
        }

        [HttpPost]
        public SaveResult SaveChanges([FromBody] JObject saveBundle)
        {
            return _productRepository.SaveChanges(saveBundle);
        }
    }
}