using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Breeze.AspNetCore;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;
using Ecat.Business.Repositories.Interface;
using Ecat.Business.Utilities;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Designer;
using Microsoft.AspNetCore.Http;

namespace Ecat.Web.Controllers
{
    [Route("breeze/[controller]/[action]")]
    [Authorize(Policy = "LoggedInUser")]
    [Authorize(Policy = "Student")]
    public class UserController: Controller
    {
        private readonly IUserRepo userRepo;
        private IHttpContextAccessor httpCtx;

        public UserController(IUserRepo repo, IHttpContextAccessor accessor)
        {
            userRepo = repo;
            //get the userId out of the token and set the userid in the repo
            //if we get to this point we already know the the sub claim exists and the value is an int because it checks in it auth policy handler
            httpCtx = accessor;
            userRepo.loggedInUserId = int.Parse(httpCtx.HttpContext.User.Claims.First(c => c.Type == "sub").Value);
        }

        #region breeze methods
        [HttpGet]
        public string Metadata()
        {
            return userRepo.MetaData();
        }

        [HttpPost]
        //[FromBody] makes ASPNet Core model binding read the model from the request body
        public SaveResult SaveChanges([FromBody] JObject saveBundle)
        {
            return userRepo.ClientSave(saveBundle);
        }
        #endregion breeze methods


        //TODO: Remove for production
        [HttpGet]
        public IEnumerable<Person> GetUsers() {
            return userRepo.GetUsers();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> CheckUserEmail(string email)
        {
            var emailChecker = new ValidEmailChecker();
            return !emailChecker.IsValidEmail(email) && await userRepo.UniqueEmailCheck(email);
        }

        [HttpGet]
        public async Task<object> Profiles()
        {
            return await userRepo.GetProfile();
        }

        [HttpGet]
        public async Task<CogInstrument> GetCogInst(string type)
        {
            return await userRepo.GetCogInst(type);
        }

        [HttpGet]
        public async Task<List<object>> GetCogResults(bool? all)
        {
            return await userRepo.GetCogResults(all);
        }

        [HttpGet]
        public async Task<List<RoadRunner>> RoadRunnerInfos()
        {
            return await userRepo.GetRoadRunnerInfo();
        }
    }
}
