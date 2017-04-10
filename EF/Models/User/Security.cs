using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Ecat.Data.Models.Interface;

namespace Ecat.Data.Models.User
{
    public class Security: IAuditable
    {
        public int PersonId { get; set; }
        public int BadPasswordCount { get; set; }
        public string PasswordHash { get; set; }

        public Person Person { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
