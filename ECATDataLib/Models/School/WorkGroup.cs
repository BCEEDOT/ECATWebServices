using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.Student;

namespace Ecat.Data.Models.School
{
    public class WorkGroup: IAuditable, IWorkGroupMonitored, ICourseMonitored
    {
        public int WorkGroupId { get; set; }
        public int CourseId { get; set; }
        public int WgModelId { get; set; }
        public string MpCategory { get; set; }
        public string GroupNumber { get; set; }
        public Guid? ReconResultId { get; set; }
        public int? AssignedSpInstrId { get; set; }
        public int? AssignedKcInstrId { get; set; }

        public string CustomName { get; set; }
        public string BbGroupId { get; set; }
        public string DefaultName { get; set; }
        public string MpSpStatus { get; set; }
        public bool IsPrimary { get; set; }

        //TODO:Update with fac response and comment impls
        public Course Course { get; set; }
        public WorkGroupModel WgModel { get; set; }
        //public GroupReconResult ReconResult { get; set; }

        //public ICollection<FacSpResponse> FacSpResponses { get; set; }
        //public ICollection<FacStratResponse> FacStratResponses { get; set; }
        //public ICollection<FacSpComment> FacSpComments { get; set; }

        public ICollection<CrseStudentInGroup> GroupMembers { get; set; }

        public ICollection<StudSpComment> SpComments { get; set; }
        public ICollection<SpResponse> SpResponses { get; set; }
        public ICollection<SpResult> SpResults { get; set; }
        public ICollection<StratResponse> SpStratResponses { get; set; }
        public ICollection<StratResult> SpStratResults { get; set; }

        public SpInstrument AssignedSpInstr { get; set; }
        public bool CanPublish { get; set; }
        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
