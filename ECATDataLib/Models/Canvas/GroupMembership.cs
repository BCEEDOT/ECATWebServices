using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasGroupMem
    {
        public int id { get; set; }
        public int group_id { get; set; }
        public int user_id { get; set; }
        public string workflow_state { get; set; }
        public bool moderator { get; set; }
        public bool just_created { get; set; }
        public int sis_import_id { get; set; }

        public CanvasGroup Group { get; set; }
        public CanvasUser User { get; set; }
    }
}
