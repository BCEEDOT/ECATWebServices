using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.AspNetCore;
using Breeze.Persistence;
using Breeze.WebApi2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Student;

namespace Ecat.Web.Controllers
{
    [Route("breeze/[controller]/[action]")]
    [BreezeController]
    [Authorize(Policy = "LoggedInUser")]
    [Authorize(Policy = "Student")]
    public class StudentController: Controller
    {
        private readonly IStudentRepo studentRepo;
        private IHttpContextAccessor httpCtx;

        public StudentController(IStudentRepo repo, IHttpContextAccessor accessor)
        {
            studentRepo = repo;
            //get the userId out of the token and set the userid in the repo
            httpCtx = accessor;
            studentRepo.loggedInUserId = int.Parse(httpCtx.HttpContext.User.Claims.First(c => c.Type == "sub").Value);
        }

        #region breeze methods
        [HttpGet]
        public string Metadata()
        {
            return studentRepo.MetaData();
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return studentRepo.ClientSave(saveBundle);
        }
        #endregion breeze methods

        [HttpGet]
        public Task<List<Course>> GetCourses()
        {
            return studentRepo.GetCourses(null);
        }

        [HttpGet]
        public Task<List<Course>> ActiveCourse(int crseId)
        {
            return studentRepo.GetCourses(crseId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWorkGroup(int wgId, bool addAssessment)
        {
            return await studentRepo.GetWorkGroup(wgId, addAssessment);
        }

        [HttpGet]
        public async Task<SpResult> GetMyWgResult(int wgId, bool addInstrument)
        {
            return await studentRepo.GetWrkGrpResult(wgId, addInstrument);
        }
    }
}
