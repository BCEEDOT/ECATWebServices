using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Designer;

namespace Ecat.Data.Models.Cognitive
{
    public class CogResponse
    {
        public int PersonId { get; set; }
        public int CogInventoryId { get; set; }
        public double ItemScore { get; set; }
        public int Attempt { get; set; }
        //public bool IsDeleted { get; set; }
        //public int? DeletedById { get; set; }
        //public DateTime? DeletedDate { get; set; }
        public CogInventory InventoryItem { get; set; }
        public Person Person { get; set; }
    }
}
