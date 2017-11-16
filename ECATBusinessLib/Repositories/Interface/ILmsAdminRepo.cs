using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Common;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.Canvas;

namespace Ecat.Business.Repositories.Interface
{
    public interface ILmsAdminCourseRepo
    {
        string Metadata();
        SaveResult SaveClientChanges(JObject saveBundle);

        int loggedInUserId { get; set; }
        //ProfileFaculty Faculty { get; set; }
        Task<List<Course>> GetAllCourses();
        Task<List<WorkGroupModel>> GetCourseModels(int courseId);
        Task<CourseReconResult> ReconcileCourses();
        Task<Course> GetAllCourseMembers(int courseId);
        Task<MemReconResult> ReconcileCourseMembers(int courseId);

        Task<CourseReconResult> ReconcileCanvasCourses();
        Task<MemReconResult> ReconcileCanvasCourseMems(int courseId);
    }

    public interface ILmsAdminGroupRepo
    {
        SaveResult SaveClientChanges(JObject saveBundle);

        int loggedInUserId { get; set; }
        //ProfileFaculty Faculty { get; set; }
        Task<Course> GetCourseGroups(int courseId);
        Task<WorkGroup> GetWorkGroupMembers(int workGroupId);
        Task<GroupReconResult> ReconcileGroups(int courseId);
        Task<GroupMemReconResult> ReconcileGroupMembers(int wgId);
        Task<List<GroupMemReconResult>> ReconcileAllGroupMembers(int courseId);//, string groupCategory);
        Task<SaveGradeResult> SyncBbGrades(int crseId, string wgCategory);
        Task<List<WorkGroup>> GetAllGroupSetMembers(int courseId, string categoryId);

        Task<SaveGradeResult> SyncCanvasGrades(int crseId);
    }

    public interface ILmsAdminTokenRepo
    {
        Task<bool> CheckCanvasTokenInfo();
        Task<bool> GetRefreshToken(string authCode);
        Task<string> GetAccessToken();
    }
}
