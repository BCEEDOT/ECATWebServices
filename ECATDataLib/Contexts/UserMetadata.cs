using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Ecat.Data.Models.User;
using Ecat.Data.Models.User.Config;
using Ecat.Data.Models.Cognitive;
using Ecat.Data.Models.Cognitive.Config;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.Designer.Config;
using Ecat.Data.Models.School;

namespace Ecat.Data.Contexts
{
    //Only used for metadata for breeze, cannot be used to query the database
    public class UserMetadata: DbContext
    {
        static UserMetadata() {
            Database.SetInitializer<UserMetadata>(null);
        }

        protected override void OnModelCreating(DbModelBuilder mb)
        {

            //TODO: Update as more implemented
            mb.Configurations.Add(new ConfigPerson());
            mb.Configurations.Add(new ConfigProfileStudent());
            mb.Configurations.Add(new ConfigProfileFaculty());
            //mb.Configurations.Add(new ConfigProfileStaff());
            //mb.Configurations.Add(new ConfigProfileDesigner());
            //mb.Configurations.Add(new ConfigProfileExternal());
            mb.Configurations.Add(new ConfigSecurity());
            mb.Configurations.Add(new ConfigRoadRunner());

            mb.Configurations.Add(new ConfigCogECMSPEResult());
            mb.Configurations.Add(new ConfigCogECPEResult());
            mb.Configurations.Add(new ConfigCogESALBResult());
            mb.Configurations.Add(new ConfigCogETMPREResult());
            mb.Configurations.Add(new ConfigCogInstrument());
            mb.Configurations.Add(new ConfigCogInventory());
            mb.Configurations.Add(new ConfigCogResponse());

            //mb.Entity<LoginToken>().HasKey(p => p.PersonId);


            //mb.Ignore<MeetingAttendee>();
            mb.Ignore(new List<Type>
            {
                //typeof(MemReconResult),
                typeof(FacultyInCourse),
                //typeof(MeetingAttendee),
                typeof(StudentInCourse),
                typeof(CrseStudentInGroup)
            });

            base.OnModelCreating(mb);
        }

        public IDbSet<Person> People { get; set; }
        public IDbSet<ProfileStudent> Students { get; set; }
        public IDbSet<ProfileFaculty> Facilitators { get; set; }
        //public IDbSet<ProfileExternal> Externals { get; set; }
        //public IDbSet<ProfileDesigner> Designers { get; set; }
        //public IDbSet<ProfileStaff> Staff { get; set; }
        //public IDbSet<LoginToken> LoginTokens { get; set; }
        public IDbSet<CogEcmspeResult> CogECMSPEResult { get; set; }
        public IDbSet<CogEcpeResult> CogECPEResult { get; set; }
        public IDbSet<CogEsalbResult> CogESALBResult { get; set; }
        public IDbSet<CogEtmpreResult> CogETMPREResult { get; set; }
        public IDbSet<CogInstrument> CogInstrument { get; set; }
        public IDbSet<CogInventory> CogInventory { get; set; }
        public IDbSet<CogResponse> CogResponse { get; set; }
        public IDbSet<RoadRunner> RoadRunnerAddresses { get; set; }
    }
}
