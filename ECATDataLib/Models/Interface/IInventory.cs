using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Interface
{
    public interface IInventory<T> where T : IInstrument, IAuditable
    {
        int Id { get; set; }
        int InstrumentId { get; set; }
        int DisplayOrder { get; set; }
        bool IsScored { get; set; }

        T Instrument { get; set; }
    }
}
