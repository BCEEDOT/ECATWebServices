using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EF.Contexts;
using EF.Models;

namespace CoreApp.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IValueRepository _valuesRepository;

        public ValuesController(IValueRepository valuesRepository)
        {
            _valuesRepository = valuesRepository;
        }

        public IValueRepository ValueItems { get; set; }

        [HttpGet]
        public IEnumerable<ValueItem> GetAll()
        {
            return _valuesRepository.GetAll();  
        }
        
    }
}
