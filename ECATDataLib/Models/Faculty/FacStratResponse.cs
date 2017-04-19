using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.School;

namespace Ecat.Data.Models.Faculty
{
    public class FacStratResponse : IAuditable, ICompositeEntity, IWorkGroupMonitored, ICourseMonitored
    {
        public string EntityId => $"{AssesseePersonId}|{CourseId}|{WorkGroupId}";
        public int StratPosition { get; set; }
        public int? StratResultId { get; set; }

        public int CourseId { get; set; }
        public int FacultyPersonId { get; set; }
        public int AssesseePersonId { get; set; }
        public int WorkGroupId { get; set; }

        public CrseStudentInGroup StudentAssessee { get; set; }
        public WorkGroup WorkGroup { get; set; }
        public FacultyInCourse FacultyAssessor { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
