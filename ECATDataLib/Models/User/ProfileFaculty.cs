using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Canvas;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.School;

namespace Ecat.Data.Models.User
{
    public class ProfileFaculty: IProfileBase
    {
        public int PersonId { get; set; }
        public string Bio { get; set; }
        public string HomeStation { get; set; }

        public bool IsCourseAdmin { get; set; }
        public bool IsReportViewer { get; set; }
        public string AcademyId { get; set; }

        public Person Person { get; set; }
        public ICollection<FacultyInCourse> Courses { get; set; }
        public CanvasLogin CanvasLogin { get; set; }
    }
}
