using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasEnrollment
    {
        public int id { get; set; }
        public int course_id { get; set; }
        public int course_section_id { get; set; }
        public string enrollment_state { get; set; }
        public int root_account_id { get; set; }
        public string type { get; set; }
        public int user_id { get; set; }
        public string role { get; set; }
        public int role_id { get; set; }
        //public DateTime created_at { get; set; }
        //public DateTime updated_at { get; set; }
        //public DateTime start_at { get; set; }
        //public DateTime end_at { get; set; }
        //public DateTime last_activity_at { get; set; }
        public int total_activity_time { get; set; }

        public CanvasCourse Course { get; set; }
        public CanvasUser user { get; set; }
        public CanvasSection Section { get; set; }
    }
}
