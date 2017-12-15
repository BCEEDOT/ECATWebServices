using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasUser
    {
        public int id { get; set; }
        public string name { get; set; }
        public string sortable_name { get; set; }
        public string short_name { get; set; }
        public string login_id { get; set; }
        public ICollection<CanvasEnrollment> enrollments { get; set; }
        public string email { get; set; }
        public string avatar_url { get; set; }
        public ICollection<int> group_ids { get; set; }
    }
}
