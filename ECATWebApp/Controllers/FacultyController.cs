using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.AspNetCore;
using Breeze.Persistence;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.Student;

namespace Ecat.Web.Controllers
{
    [Route("breeze/[controller]/[action]")]
    [BreezeController]
    [Authorize(Policy = "LoggedInUser")]
    [Authorize(Policy = "Faculty")]
    public class FacultyController: Controller
    {
        private readonly IFacultyRepo facultyRepo;
        private IHttpContextAccessor httpCtx;

        public FacultyController(IFacultyRepo repo, IHttpContextAccessor accessor)
        {
            facultyRepo = repo;
            //get the userId out of the token and set the userid in the repo
            httpCtx = accessor;
            facultyRepo.loggedInUserId = int.Parse(httpCtx.HttpContext.User.Claims.First(c => c.Type == "sub").Value);
        }

        #region breeze methods
        [HttpGet]
        public string Metadata()
        {
            return facultyRepo.MetaData();
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return facultyRepo.ClientSave(saveBundle);
        }
        #endregion breeze methods

        [HttpGet]
        public async Task<List<Course>> GetCourses()
        {
            return await facultyRepo.GetActiveCourse();
        }

        [HttpGet]
        public async Task<List<Course>> ActiveCourse(int courseId)
        {
            return await facultyRepo.GetActiveCourse(courseId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWorkGroup(int courseId, int workGroupId)
        {
            return await facultyRepo.GetActiveWorkGroup(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<List<WorkGroup>> GetRoadRunnerWorkGroups(int courseId)
        {
            return await facultyRepo.GetRoadRunnerWorkGroups(courseId);
        }

        [HttpGet]
        public async Task<SpInstrument> SpInstrument(int instrumentId)
        {
            return await facultyRepo.GetSpInstrument(instrumentId);
        }

        [HttpGet]
        public async Task<List<StudSpComment>> ActiveWgSpComment(int courseId, int workGroupId)
        {
            return await facultyRepo.GetStudSpComments(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<List<FacSpComment>> ActiveWgFacComment(int courseId, int workGroupId)
        {
            return await facultyRepo.GetFacSpComment(courseId, workGroupId);
        }

        [HttpGet]
        public async Task<WorkGroup> ActiveWgSpResult(int courseId, int workGroupId)
        {
            return await facultyRepo.GetSpResult(courseId, workGroupId);
        }
    }
}
