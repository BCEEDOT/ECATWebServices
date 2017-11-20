using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.User;

namespace Ecat.Data.Models.Canvas
{
    public class CanvasLogin
    {
        public int PersonId { get; set; }
        [MaxLength(100)]
        public string AccessToken { get; set; }
        [MaxLength(100)]
        public string RefreshToken { get; set; }
        public DateTime? TokenExpires  { get; set; }

        public ProfileFaculty FacultyProfile { get; set; }
    }
}
