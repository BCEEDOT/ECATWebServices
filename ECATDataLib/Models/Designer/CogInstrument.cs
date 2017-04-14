using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;

namespace Ecat.Data.Models.Designer
{
    public class CogInstrument : IInstrument
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }

        public string CogInstructions { get; set; }
        public string MpCogInstrumentType { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public ICollection<CogInventory> InventoryCollection { get; set; }
    }
}
