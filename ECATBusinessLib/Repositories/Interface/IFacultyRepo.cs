using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.Faculty;

namespace Ecat.Business.Repositories.Interface
{
    public interface IFacultyRepo
    {
        string MetaData();
        SaveResult ClientSave(JObject saveBundle);

        int loggedInUserId { get; set; }
        Task<List<Course>> GetActiveCourse(int? courseId = null);
        Task<WorkGroup> GetActiveWorkGroup(int courseId, int workGroupId);
        Task<SpInstrument> GetSpInstrument(int instrumentId);
        Task<List<StudSpComment>> GetStudSpComments(int courseId, int workGroupId);
        Task<List<FacSpComment>> GetFacSpComment(int courseId, int workGroupId);
        Task<WorkGroup> GetSpResult(int courseId, int workGroupId);
        Task<List<WorkGroup>> GetRoadRunnerWorkGroups(int courseId);
        Task<Course> GetAllCourseMembers(int courseId);
    }
}
