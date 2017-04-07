using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECATDataLib.Models.Interface
{
    public interface IAuditable
    {
        int? ModifiedById { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}
