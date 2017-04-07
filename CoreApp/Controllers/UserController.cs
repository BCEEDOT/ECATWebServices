using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Breeze.AspNetCore;
using Breeze.Persistence;
using ECATDataLib.Models;
using ECATBusinessLib.Repositories.Interface;
using ECATBusinessLib.Repositories.User;
using Newtonsoft.Json.Linq;

namespace CoreApp.Controllers
{
    [Route("breeze/[controller]/[action]")]
    [BreezeQueryFilter]
    public class UserController: Controller
    {
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public string Metadata()
        {
            return _userRepo.MetaData();
        }
    }
}
