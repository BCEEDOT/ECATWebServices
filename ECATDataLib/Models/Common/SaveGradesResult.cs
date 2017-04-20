using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Common
{
    public class SaveGradeResult
    {
        public int CourseId { get; set; }
        public string WgCategory { get; set; }
        public bool Success { get; set; }
        public int NumOfStudents { get; set; }
        public int SentScores { get; set; }
        public int ReturnedScores { get; set; }
        public string Message { get; set; }
    }
}
