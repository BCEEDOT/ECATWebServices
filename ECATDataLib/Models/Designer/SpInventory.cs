using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.Student;

namespace Ecat.Data.Models.Designer
{
    public class SpInventory : IAuditable, IInventory<SpInstrument>
    {
        public int Id { get; set; }
        public int InstrumentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsScored { get; set; }
        public bool IsDisplayed { get; set; }
        public string Behavior { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public SpInstrument Instrument { get; set; }
        public ICollection<SpResponse> ItemResponses { get; set; }
    }
}
