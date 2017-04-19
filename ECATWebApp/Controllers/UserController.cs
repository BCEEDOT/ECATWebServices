﻿using System;
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
    [BreezeQueryFilter]
    [Authorize(Policy = "LoggedInUser")]
    public class UserController: Controller
    {
        private readonly IUserRepo _userRepo;
        private IHttpContextAccessor httpCtx;

        public UserController(IUserRepo userRepo, IHttpContextAccessor accessor)
        {
            _userRepo = userRepo;
            //get the userId out of the token and set the userid in the repo
            httpCtx = accessor;
            int usrId;
            int.TryParse(httpCtx.HttpContext.User.Claims.First(c => c.Type == "sub").Value, out usrId);
            _userRepo.loggedInUserId = usrId;
        }

        #region breeze methods
        [HttpGet]
        [AllowAnonymous]
        public string Metadata()
        {
            return _userRepo.MetaData();
        }

        [HttpPost]
        [AllowAnonymous]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _userRepo.ClientSave(saveBundle);
        }
        #endregion breeze methods

        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> CheckUserEmail(string email)
        {
            var emailChecker = new ValidEmailChecker();
            return !emailChecker.IsValidEmail(email) && await _userRepo.UniqueEmailCheck(email);
        }

        [HttpGet]
        public async Task<object> Profiles()
        {
            return await _userRepo.GetProfile();
        }

        [HttpGet]
        public async Task<CogInstrument> GetCogInst(string type)
        {
            return await _userRepo.GetCogInst(type);
        }

        [HttpGet]
        public async Task<List<object>> GetCogResults(bool? all)
        {
            return await _userRepo.GetCogResults(all);
        }

        [HttpGet]
        public async Task<List<RoadRunner>> RoadRunnerInfos()
        {
            return await _userRepo.GetRoadRunnerInfo();
        }
    }
}
