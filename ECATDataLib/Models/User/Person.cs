using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Interface;

namespace Ecat.Data.Models.User
{
    //TODO: Bring over the Person validation class?
    //[PersonMappedPropValid]
    public class Person: IAuditable
    {
        public int PersonId { get; set; }
        public bool IsActive { get; set; }

        public string BbUserId { get; set; }
        public string BbUserName { get; set; }
        //TODO: change to LMS 
        //public string LmsUserId { get; set; }
        //public string LmsUserName { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AvatarLocation { get; set; }
        public string GoByName { get; set; }
        public string MpGender { get; set; }
        public string MpAffiliation { get; set; }
        public string MpPaygrade { get; set; }
        public string MpComponent { get; set; }
        public string Email { get; set; }
        public bool RegistrationComplete { get; set; }
        public string MpInstituteRole { get; set; }

        public virtual ProfileStudent Student { get; set; }
        public virtual ProfileFaculty Faculty { get; set; }
        //public virtual ProfileDesigner Designer { get; set; }
        //public virtual ProfileExternal External { get; set; }
        //public virtual ProfileStaff HqStaff { get; set; }
        public ICollection<RoadRunner> RoadRunnerAddresses { get; set; }
        public virtual Security Security { get; set; }

        public int? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
