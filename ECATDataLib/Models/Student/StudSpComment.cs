using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.School;

namespace Ecat.Data.Models.Student
{
    //TODO: Implement deleteableguard attribute?
    //[DeletableGuard(AuthorizedDeleters = new[] { RoleMap.Student })]
    public class StudSpComment : IAuditable, IWorkGroupMonitored, ICourseMonitored
    {
        public string EntityId => $"{AuthorPersonId}|{RecipientPersonId}|{CourseId}|{WorkGroupId}";
        public int AuthorPersonId { get; set; }
        public int RecipientPersonId { get; set; }
        public int WorkGroupId { get; set; }
        public int? FacultyPersonId { get; set; }
        public int CourseId { get; set; }
        public bool RequestAnonymity { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedDate { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public CrseStudentInGroup Author { get; set; }
        public CrseStudentInGroup Recipient { get; set; }
        public WorkGroup WorkGroup { get; set; }
        public Course Course { get; set; }
        public StudSpCommentFlag Flag { get; set; }
    }
}
