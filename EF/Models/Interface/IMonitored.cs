using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Interface
{
    public interface ICourseMonitored
    {
        int CourseId { get; set; }
    }

    public interface IWorkGroupMonitored
    {
        int WorkGroupId { get; set; }
    }
}
