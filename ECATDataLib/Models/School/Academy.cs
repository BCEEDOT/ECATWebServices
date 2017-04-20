using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Static;

namespace Ecat.Data.Models.School
{
    public class Academy
    {
        public string Id { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string MpEdLevel { get; set; }
        public AcademyBase Base { get; set; }
        public string BbCategoryId { get; set; }
        public string ParentBbCategoryId { get; set; }
    }
}
