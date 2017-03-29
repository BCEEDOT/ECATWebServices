using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EF.Models
{
    public interface IValueRepository
    {
        void Add(ValueItem item);
        IEnumerable<ValueItem> GetAll();
        ValueItem Find(long key);
        void Remove(long key);
    }
}
