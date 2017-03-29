using System;
using System.Collections.Generic;
using EF.Contexts;
using System.Linq;


namespace EF.Models
{
    public class ValueRepository : IValueRepository

    {
        private readonly ValueContext _context;

        public ValueRepository(ValueContext context)
        {
            _context = context;

            Add(new ValueItem
            {
                Value = "Item1"
            });
        }
        public void Add(ValueItem item)
        {
            _context.ValueItems.Add(item);
            _context.SaveChanges();
        }

        public ValueItem Find(long key)
        {
            return _context.ValueItems.FirstOrDefault(t => t.Key == key);
        }

        public IEnumerable<ValueItem> GetAll()
        {
            return _context.ValueItems.ToList();
        }

        public void Remove(long key)
        {
            var entity = _context.ValueItems.FirstOrDefault(t => t.Key == key);
            _context.ValueItems.Remove(entity);
            _context.SaveChanges();
        }
    }
    
}
