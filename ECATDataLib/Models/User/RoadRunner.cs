using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.User
{
    public class RoadRunner
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LeaveDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool SignOut { get; set; }
        public bool PrevSignOut { get; set; }
        public int PersonId { get; set; }

        public Person Person { get; set; }
    }
}
