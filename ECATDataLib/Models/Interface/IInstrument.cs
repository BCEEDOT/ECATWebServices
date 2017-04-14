using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Interface
{
    public interface IInstrument : IAuditable
    {
        int Id { get; set; }
        string Version { get; set; }
        bool IsActive { get; set; }
    }
}
