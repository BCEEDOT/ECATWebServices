using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Designer;

namespace Ecat.Data.Models.Student
{
    public class SanitizedSpResponse
    {
        public Guid Id { get; set; }
        public int CourseId { get; set; }
        //public int StudentId { get; set; }
        public int AssesseeId { get; set; }
        public int WorkGroupId { get; set; }
        public bool IsSelfResponse { get; set; }
        public string PeerGenericName { get; set; }
        public string MpItemResponse { get; set; }
        public int ItemModelScore { get; set; }
        public int InventoryItemId { get; set; }
        public SpInventory InventoryItem { get; set; }
        public SpResult Result { get; set; }
    }
}
