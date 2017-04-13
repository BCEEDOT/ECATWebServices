using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.School
{
    public class Course
    {
        public int Id { get; set; }
        public string AcademyId { get; set; }
        public string BbCourseId { get; set; }
        public string Name { get; set; }
        public string ClassNumber { get; set; }
        public string Term { get; set; }
        public bool GradReportPublished { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime GradDate { get; set; }
        //TODO:Update as more implemented
        //public ICollection<SpResult> SpResults { get; set; }
        //public ICollection<StratResult> StratResults { get; set; }
        public ICollection<StudentInCourse> Students { get; set; }
        public ICollection<CrseStudentInGroup> StudentInCrseGroups { get; set; }
        //public ICollection<SpResponse> SpResponses { get; set; }
        public ICollection<FacultyInCourse> Faculty { get; set; }
        public ICollection<WorkGroup> WorkGroups { get; set; }


        public Guid? ReconResultId { get; set; }
        //public CourseReconResult ReconResult { get; set; }
    }
}
