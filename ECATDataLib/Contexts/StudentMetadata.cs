using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;
using Ecat.Data.Models.Interface;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.School;
using Ecat.Data.Models.School.Config;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.Student.Config;
using Ecat.Data.Contexts.Config;

namespace Ecat.Data.Contexts
{
    //Only used for metadata for breeze, cannot be used to query the database
    public class StudentMetadata: DbContext
    {
        static StudentMetadata()
        {
            Database.SetInitializer<StudentMetadata>(null);
        }

        protected override void OnModelCreating(DbModelBuilder mb)
        {

            mb.Configurations.Add(new ConfigSpResponse());
            mb.Configurations.Add(new ConfigSpResult());
            mb.Configurations.Add(new ConfigStratResponse());
            mb.Configurations.Add(new ConfigStudentInCourse());
            mb.Configurations.Add(new ConfigStratResult());
            mb.Configurations.Add(new StudConfigSanitizedComment());
            mb.Configurations.Add(new StudConfigSanitizedResponse());
            mb.Configurations.Add(new StudConfigSpInstrument());
            mb.Configurations.Add(new StudConfigSpInventory());
            mb.Configurations.Add(new StudConfigStudWrkGrp());
            mb.Configurations.Add(new StudConfigCrse());
            mb.Configurations.Add(new StudConfigProfileStudent());
            mb.Configurations.Add(new StudConfigPerson());
            mb.Configurations.Add(new StudConfigCrseStudInWg());
            mb.Configurations.Add(new StudConfigSpComment());
            mb.Configurations.Add(new ConfigSpCommentFlag());

            mb.Ignore(new List<Type>
            {
                //TODO: Update with model impls
                //typeof (ProfileExternal),
                //typeof (ProfileDesigner),
                typeof (ProfileFaculty),
                typeof (Security),
                //typeof (ProfileStaff),
                typeof (FacultyInCourse),
                //typeof (FacSpResponse),
                //typeof (FacStratResponse),
                //typeof (FacSpComment),
                //typeof (KcResponse),
                //typeof (KcResult)
            });

            mb.Types()
                .Where(t => typeof(IAuditable).IsAssignableFrom(t))
                .Configure(p => p.Ignore("ModifiedById"));

            mb.Types()
                .Where(t => typeof(IAuditable).IsAssignableFrom(t))
                .Configure(p => p.Ignore("ModifiedDate"));

            //Causing problems with model-generator, trying toadd configs it already added in ecatcontext
            //Added the Student-specific configs above, don't think we need to do this anymore
            //var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            //    .Where(type => !string.IsNullOrEmpty(type.Namespace))
            //    .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

            //foreach (var configurationInstance in typesToRegister.Select(Activator.CreateInstance))
            //{
            //    mb.Configurations.Add((dynamic)configurationInstance);
            //}

            //TODO: Update with api impl
            //mb.Ignore<CourseReconResult>();
            //mb.Ignore<MemReconResult>();
            //mb.Ignore<GroupMemReconResult>();
            //mb.Ignore<GroupReconResult>();
            mb.Entity<FacultyInCourse>().Ignore(p => p.ReconResultId);
            mb.Entity<StudentInCourse>().Ignore(p => p.ReconResultId);
            mb.Entity<CrseStudentInGroup>().Ignore(p => p.ReconResultId);
            mb.Entity<WorkGroup>().Ignore(p => p.ReconResultId);
            mb.Entity<Course>().Ignore(p => p.ReconResultId);

            base.OnModelCreating(mb);
        }

        public IDbSet<WorkGroup> WorkGroups { get; set; }
        public IDbSet<Course> Courses { get; set; }
        public IDbSet<CrseStudentInGroup> StudentInGroups { get; set; }
        public IDbSet<StudentInCourse> StudentInCourses { get; set; }
        public IDbSet<SpResponse> SpResponses { get; set; }
        public IDbSet<SpResult> SpResults { get; set; }
        public IDbSet<StudSpComment> StudSpComments { get; set; }
        public IDbSet<StratResponse> StratRepoResponses { get; set; }
        public IDbSet<StratResult> StratResults { get; set; }
        public IDbSet<SpInventory> Inventories { get; set; }
    }
}
