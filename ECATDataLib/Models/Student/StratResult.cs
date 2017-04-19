using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Faculty;

namespace Ecat.Data.Models.Student
{
    public class StratResult : ICompositeEntity, IAuditable
    {
        public string EntityId => $"{StudentId}|{CourseId}|{WorkGroupId}";
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int WorkGroupId { get; set; }

        public int OriginalStratPosition { get; set; }
        public int FinalStratPosition { get; set; }
        public decimal StratCummScore { get; set; }
        public decimal StudStratAwardedScore { get; set; }
        public decimal FacStratAwardedScore { get; set; }

        public Course Course { get; set; }
        public CrseStudentInGroup ResultFor { get; set; }
        public FacStratResponse FacStrat { get; set; }
        public ICollection<StratResponse> StratResponses { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
