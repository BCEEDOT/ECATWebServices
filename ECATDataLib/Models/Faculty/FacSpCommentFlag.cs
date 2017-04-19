using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;

namespace Ecat.Data.Models.Faculty
{
    public class FacSpCommentFlag : IWorkGroupMonitored, ICourseMonitored
    {
        public int FacultyId { get; set; }
        public string MpAuthor { get; set; }
        public string MpRecipient { get; set; }
        public int RecipientPersonId { get; set; }
        public int CourseId { get; set; }
        public int WorkGroupId { get; set; }

        public FacSpComment Comment { get; set; }
    }
}
