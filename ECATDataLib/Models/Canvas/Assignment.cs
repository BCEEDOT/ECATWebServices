using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasAssignment
    {
        public int id { get; set; }
        public string name { get; set; }
        public int course_id { get; set; }
        public int assignment_group_id { get; set; }
        //...
    }
}
