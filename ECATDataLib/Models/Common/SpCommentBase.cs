using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;

namespace Ecat.Data.Models.Common
{
    public class SpCommentBase : IAuditable
    {
        public DateTime CreatedDate { get; set; }
        public bool Anonymity { get; set; }
        public string CommentText { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
