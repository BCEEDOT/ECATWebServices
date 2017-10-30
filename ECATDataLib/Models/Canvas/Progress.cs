using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasProgress
    {
        public int id { get; set; }
        public int context_id { get; set; }
        public string context_type { get; set; }
        public int user_id { get; set; }
        public string tag { get; set; }
        public int completion { get; set; }
        public string workflow_state { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string message { get; set; }
        public string url { get; set; }
    }
}
