using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Student
{
    public class SanitizedSpComment
    {
        //TODO: Fac comment impl
        public Guid Id { get; set; }
        public int RecipientId { get; set; }
        public int CourseId { get; set; }
        public int WorkGroupId { get; set; }
        public string AuthorName { get; set; }
        public string CommentText { get; set; }
        public StudSpCommentFlag Flag { get; set; }
        //public FacSpCommentFlag FacFlag { get; set; }

        public string MpCommentFlagRecipient { get; set; }

        public SpResult Result { get; set; }
    }
}
