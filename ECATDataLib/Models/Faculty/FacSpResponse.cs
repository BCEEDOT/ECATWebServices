using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Designer;

namespace Ecat.Data.Models.Faculty
{
    public class FacSpResponse : IAuditable, ICompositeEntity, IWorkGroupMonitored, ICourseMonitored
    {
        public string EntityId => $"{AssesseePersonId}|{CourseId}|{WorkGroupId}|{InventoryItemId}";
        public int InventoryItemId { get; set; }

        public int WorkGroupId { get; set; }
        public int FacultyPersonId { get; set; }
        public int AssesseePersonId { get; set; }
        public int CourseId { get; set; }

        public string MpItemResponse { get; set; }
        public float ItemModelScore { get; set; }
        //public int ScoreModelVersion { get; set; }

        public WorkGroup WorkGroup { get; set; }
        public CrseStudentInGroup Assessee { get; set; }
        public FacultyInCourse FacultyAssessor { get; set; }
        public SpInventory InventoryItem { get; set; }

        //[TsIgnore]
        public bool IsDeleted { get; set; }
        //[TsIgnore]
        public int? DeletedById { get; set; }
        //[TsIgnore]
        public DateTime? DeletedDate { get; set; }
        //[TsIgnore]
        public int? ModifiedById { get; set; }
        //[TsIgnore]
        public DateTime? ModifiedDate { get; set; }
    }
}
