using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.Faculty;

namespace Ecat.Data.Models.School
{
    public class FacultyInCourse: ICompositeEntity, ISoftDelete
    {
        public string EntityId => $"{FacultyPersonId}, {CourseId}";
        public int CourseId { get; set; }
        public int FacultyPersonId { get; set; }
        public string BbCourseMemId { get; set; }
        public Course Course { get; set; }
        public ProfileFaculty FacultyProfile { get; set; }

        public ICollection<FacSpResponse> FacSpResponses { get; set; }
        public ICollection<FacSpComment> FacSpComments { get; set; }
        public ICollection<FacStratResponse> FacStratResponse { get; set; }
        public ICollection<StudSpCommentFlag> FlaggedSpComments { get; set; }

        public bool IsDeleted { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedDate { get; set; }

        public Guid? ReconResultId { get; set; }
        //TODO: Update with api impl
        //public MemReconResult ReconResult { get; set; }
    }
}
