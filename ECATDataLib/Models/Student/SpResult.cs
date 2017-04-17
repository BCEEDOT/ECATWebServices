using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.School;

namespace Ecat.Data.Models.Student
{
    public class SpResult : ICompositeEntity, IAuditable, IWorkGroupMonitored, ICourseMonitored
    {
        //TODO: Fac resposne impl
        public string EntityId => $"{StudentId}|{CourseId}|{WorkGroupId}";
        public int CourseId { get; set; }
        public int WorkGroupId { get; set; }
        public int StudentId { get; set; }
        public int? AssignedInstrumentId { get; set; }
        public string MpAssessResult { get; set; }
        public int CompositeScore { get; set; }
        public SpResultBreakOut BreakOut { get; set; }
        public CrseStudentInGroup ResultFor { get; set; }
        public SpInstrument AssignedInstrument { get; set; }
        public WorkGroup WorkGroup { get; set; }
        public Course Course { get; set; }
        //public ICollection<FacSpResponse> FacultyResponses { get; set; }
        public ICollection<SpResponse> SpResponses { get; set; }
        public ICollection<SanitizedSpResponse> SanitizedResponses { get; set; }
        public ICollection<SanitizedSpComment> SanitizedComments { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class SpResultBreakOut
    {
        public int IneffA { get; set; }
        public int IneffU { get; set; }
        public int EffA { get; set; }
        public int EffU { get; set; }
        public int HighEffU { get; set; }
        public int HighEffA { get; set; }
        public int NotDisplay { get; set; }
    }
}
