using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.School;

namespace Ecat.Data.Models.Common
{
    public abstract class ReconcileResult
    {
        public Guid Id { get; set; }
        public string AcademyId { get; set; }
        public int NumAdded { get; set; }
        public int NumRemoved { get; set; }
        public bool HasToken { get; set; }
    }

    public class CourseReconResult : ReconcileResult
    {
        public ICollection<Course> Courses { get; set; }
    }

    public class GroupReconResult : ReconcileResult
    {
        public ICollection<WorkGroup> Groups { get; set; }
    }
    
    public class MemReconResult : ReconcileResult
    {
        public int CourseId { get; set; }
        public int NumOfAccountCreated { get; set; }
        public ICollection<FacultyInCourse> Faculty { get; set; }
        public ICollection<StudentInCourse> Students { get; set; }
        public ICollection<int> RemovedIds { get; set; }
    }
    
    public class GroupMemReconResult : ReconcileResult
    {
        public int CourseId { get; set; }
        public int WorkGroupId { get; set; }
        public string WorkGroupName { get; set; }
        public string GroupType { get; set; }
        public ICollection<CrseStudentInGroup> GroupMembers { get; set; }
    }
}
