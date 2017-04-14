using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;

namespace Ecat.Data.Models.Designer
{
    public class CogInventory : IInventory<CogInstrument>, IAuditable
    {
        public int Id { get; set; }
        public int InstrumentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsScored { get; set; }
        public bool IsDisplayed { get; set; }
        public string AdaptiveDescription { get; set; }
        public string InnovativeDescription { get; set; }
        public string ItemType { get; set; }
        public string ItemDescription { get; set; }
        public bool? IsReversed { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public CogInstrument Instrument { get; set; }
    }
}
