using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.School;

namespace Ecat.Data.Models.Common
{
    public class CourseReconcile
    {
        public Course Course { get; set; }
        public ICollection<UserReconcile> FacultyToReconcile { get; set; }
        public ICollection<UserReconcile> StudentsToReconcile { get; set; }
    }

    public class UserReconcile
    {
        public int PersonId { get; set; }
        public string BbUserId { get; set; }
        public bool CanDelete { get; set; }
    }

    public class GroupMemberReconcile
    {
        public string BbCrseId { get; set; }
        public int CrseId { get; set; }
        public ICollection<GmrGroup> WorkGroups { get; set; }
    }

    public class GmrGroup
    {
        public int WgId { get; set; }
        public string BbWgId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public GroupMemReconResult ReconResult { get; set; }
        public ICollection<GmrMember> Members { get; set; }
    }

    public class GmrMember
    {
        public int StudentId { get; set; }
        public string BbGroupMemId { get; set; }
        public string BbCrseMemId { get; set; }
        public bool IsDeleted { get; set; }
        public bool PendingRemoval { get; set; }
        public bool HasChildren { get; set; }
        public GmrMemberVo BbGmVo { get; set; }
    }

    public class GmrMemberVo
    {
        public string CourseMembershipId { get; set; }
        public string GroupId { get; set; }
        public string GroupMembershipId { get; set; }
    }
}
