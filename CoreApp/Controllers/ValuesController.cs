using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ECATDataLib.Contexts;
using ECATDataLib.Models;
using Microsoft.AspNetCore.Authorization;

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
        //[Authorize(Policy = "ValuesUser")]
        public IEnumerable<ValueItem> GetAll()
        {
            return _valuesRepository.GetAll();  
        }
        
    }
}
