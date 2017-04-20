using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Common;

namespace Ecat.Data.Models.School
{
    public class StudentInCourse: ICompositeEntity, ISoftDelete
    {
        public string EntityId => $"{StudentPersonId}|{CourseId}";
        public int CourseId { get; set; }
        public int StudentPersonId { get; set; }
        public string BbCourseMemId { get; set; }

        public Course Course { get; set; }
        public ProfileStudent Student { get; set; }

        public ICollection<CrseStudentInGroup> WorkGroupEnrollments { get; set; }
        //public ICollection<KcResponse> KcResponses { get; set; }

        public bool IsDeleted { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedDate { get; set; }

        public Guid? ReconResultId { get; set; }
        public MemReconResult ReconResult { get; set; }
    }
}
