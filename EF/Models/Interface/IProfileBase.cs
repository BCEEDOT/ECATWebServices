using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECATDataLib.Models.User;

namespace ECATDataLib.Models.Interface
{
    public interface IProfileBase
    {
        int PersonId { get; set; }
        string Bio { get; set; }
        string HomeStation { get; set; }

        Person Person { get; set; }
    }
}
