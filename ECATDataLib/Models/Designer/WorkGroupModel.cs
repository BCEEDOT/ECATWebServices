using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Interface;

namespace Ecat.Data.Models.Designer
{
    public class WorkGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? AssignedSpInstrId { get; set; }
        public string MpEdLevel { get; set; }
        public string MpWgCategory { get; set; }
        public decimal MaxStratStudent { get; set; }
        public decimal MaxStratFaculty { get; set; }
        public bool IsActive { get; set; }
        public int StratDivisor { get; set; }
        public string StudStratCol { get; set; }
        public string FacStratCol { get; set; }

        public SpInstrument AssignedSpInstr { get; set; }
        public ICollection<WorkGroup> WorkGroups { get; set; }
    }
}
