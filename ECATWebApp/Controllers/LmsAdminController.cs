using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Breeze.AspNetCore;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Common;
using Ecat.Data.Models.Designer;

namespace Ecat.Web.Controllers
{
    [Route("breeze/[controller]/[action]")]
    [Authorize(Policy = "LoggedInUser")]
    [Authorize(Policy = "LMSAdmin")]
    public class LmsAdminController: Controller
    {
        private readonly ILmsAdminCourseRepo courseRepo;
        private readonly ILmsAdminGroupRepo groupRepo;
        private IHttpContextAccessor httpCtx;

        public LmsAdminController(ILmsAdminCourseRepo lmsCourseOps, ILmsAdminGroupRepo lmsGroupOps, IHttpContextAccessor accessor)
        {
            courseRepo = lmsCourseOps;
            groupRepo = lmsGroupOps;

            //get the userId out of the token and set the userid in the repos
            httpCtx = accessor;
            int userId = int.Parse(httpCtx.HttpContext.User.Claims.First(c => c.Type == "sub").Value);
            courseRepo.loggedInUserId = userId;
            groupRepo.loggedInUserId = userId;
        }

        #region breeze methods
        [HttpGet]
        public string Metadata()
        {
            return courseRepo.Metadata();
        }

        [HttpPost]
        public SaveResult SaveChanges([FromBody] JObject saveBundle)
        {
            return courseRepo.SaveClientChanges(saveBundle);
        }
        #endregion breeze methods

        [HttpGet]
        public async Task<List<Course>> GetAllCourses()
        {
            return await courseRepo.GetAllCourses();
        }

        [HttpGet]
        public async Task<List<WorkGroupModel>> GetCourseModels(int courseId)
        {
            return await courseRepo.GetCourseModels(courseId);
        }

        [HttpGet]
        public async Task<Course> GetAllCourseMembers(int courseId)
        {
            return await courseRepo.GetAllCourseMembers(courseId);
        }

        [HttpGet]
        public async Task<Course> GetAllGroups(int courseId)
        {
            return await groupRepo.GetCourseGroups(courseId);
        }

        [HttpGet]
        public async Task<WorkGroup> GetGroupMembers(int workGroupId)
        {
            return await groupRepo.GetWorkGroupMembers(workGroupId);
        }

        [HttpGet]
        public async Task<CourseReconResult> PollCourses()
        {
            return await courseRepo.ReconcileCourses();
        }

        [HttpGet]
        public async Task<MemReconResult> PollCourseMembers(int courseId)
        {
            return await courseRepo.ReconcileCourseMembers(courseId);
        }

        [HttpGet]
        public async Task<GroupReconResult> PollGroups(int courseId)
        {
            return await groupRepo.ReconcileGroups(courseId);
        }

        [HttpGet]
        public async Task<GroupMemReconResult> PollGroupMembers(int workGroupId)
        {
            return await groupRepo.ReconcileGroupMembers(workGroupId);
        }

        [HttpGet]
        public async Task<List<GroupMemReconResult>> PollGroupCategory(int courseId, string category)
        {
            return await groupRepo.ReconcileGroupMembers(courseId, category);
        }

        [HttpGet]
        public async Task<SaveGradeResult> SyncBbGrades(int crseId, string wgCategory)
        {
            return await groupRepo.SyncBbGrades(crseId, wgCategory);
        }
    }
}
