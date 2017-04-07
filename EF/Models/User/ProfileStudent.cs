using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECATDataLib.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace ECATDataLib.Models.User
{
    public class ProfileStudent: IProfileBase
    {
        //[Key]
        public int PersonId { get; set; }
        public string Bio { get; set; }
        public string HomeStation { get; set; }

        public string ContactNumber { get; set; }
        public string Commander { get; set; }
        public string Shirt { get; set; }
        public string CommanderEmail { get; set; }
        public string ShirtEmail { get; set; }

        public Person Person { get; set; }

        //public ICollection<StudentInCourse> Courses { get; set; }
        //public ICollection<CrseStudentInGroup> CourseGroupMemberships { get; set; }
    }
}
