using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasCourse
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public string name { get; set; }
        public string course_code { get; set; }
        public string workflow_state { get; set; }
        public int account_id { get; set; }
        public int root_account_id { get; set; }
        public int enrollment_term_id { get; set; }
        //public int grading_standard_id { get; set; }
        public string start_at { get; set; }
        public string end_at { get; set; }
    }
}
