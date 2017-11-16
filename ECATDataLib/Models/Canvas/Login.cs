using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.User;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasLogin
    {
        public int PersonId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpires  { get; set; }

        public Person Person { get; set; }
    }
}
