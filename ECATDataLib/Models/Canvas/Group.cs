using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasGroup
    {
        public int id { get; set; }
        public string name { get; set; }
        public int course_id { get; set; }
        public int members_count { get; set; }

        public CanvasCourse Course { get; set; }
    }
}
