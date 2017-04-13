using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.User;

namespace Ecat.Data.Models.School
{
    public class CrseStudentInGroup: ICompositeEntity, IAuditable, ISoftDelete
    {
        //TODO:Update as more implemented
        public string EntityId => $"{StudentId}|{CourseId}|{WorkGroupId}";
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int WorkGroupId { get; set; }
        public bool HasAcknowledged { get; set; }
        public int NumOfStratIncomplete { get; set; }
        public string BbCrseStudGroupId { get; set; }
        public Guid? ReconResultId { get; set; }
        public WorkGroup WorkGroup { get; set; }
        public ProfileStudent StudentProfile { get; set; }
        public Course Course { get; set; }
        public StudentInCourse StudentInCourse { get; set; }
        //public GroupMemReconResult ReconResult { get; set; }
        public ICollection<CrseStudentInGroup> GroupPeers { get; set; }
        //public ICollection<SpResponse> AssessorSpResponses { get; set; }
        //public ICollection<SpResponse> AssesseeSpResponses { get; set; }
        //public ICollection<StudSpComment> AuthorOfComments { get; set; }
        //public ICollection<StudSpComment> RecipientOfComments { get; set; }
        //public ICollection<StratResponse> AssessorStratResponse { get; set; }
        //public ICollection<StratResponse> AssesseeStratResponse { get; set; }
        //public ICollection<FacSpResponse> FacultySpResponses { get; set; }

        //public SpResult SpResult { get; set; }
        //public StratResult StratResult { get; set; }
        //public FacSpComment FacultyComment { get; set; }
        //public FacStratResponse FacultyStrat { get; set; }

        public int NumberOfAuthorComments { get; set; }

        public bool IsDeleted { get; set; }
        public int? DeletedById { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
