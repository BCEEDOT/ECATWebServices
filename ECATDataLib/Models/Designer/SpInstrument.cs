using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.School;

namespace Ecat.Data.Models.Designer
{
    public class SpInstrument : IInstrument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }
        public string StudentInstructions { get; set; }
        public string FacultyInstructions { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedById { get; set; }

        public ICollection<SpInventory> InventoryCollection { get; set; }
        public ICollection<WorkGroup> AssignedGroups { get; set; }
    }
}
