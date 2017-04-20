using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;
using Ecat.Data.Models.School;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Student;

namespace Ecat.Business.Repositories.Interface
{
    public interface IStudentRepo
    {
        string MetaData();
        SaveResult ClientSave(JObject saveBundle);

        int loggedInUserId { get; set; }
        Task<List<Course>> GetCourses(int? crseId);
        Task<SpResult> GetWrkGrpResult(int wgId, bool addInstrument);
        Task<WorkGroup> GetWorkGroup(int wgId, bool addInstrument);
    }
}
