using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.School;

namespace Ecat.Data.Models.Faculty
{
    public class FacSpComment : IAuditable, ICompositeEntity, IWorkGroupMonitored, ICourseMonitored
    {
        public string EntityId => $"{RecipientPersonId}|{CourseId}|{WorkGroupId}";
        public int RecipientPersonId { get; set; }
        public int WorkGroupId { get; set; }
        public int FacultyPersonId { get; set; }
        public int CourseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CommentText { get; set; }

        public FacultyInCourse FacultyCourse { get; set; }
        public CrseStudentInGroup Recipient { get; set; }
        public WorkGroup WorkGroup { get; set; }
        public Course Course { get; set; }
        public FacSpCommentFlag Flag { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
