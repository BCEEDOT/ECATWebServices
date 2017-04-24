using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Ecat.Data.Models.School;
using Ecat.Data.Models.School.Config;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.Student.Config;
using Ecat.Data.Models.User;
using Ecat.Data.Models.User.Config;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.Faculty.Config;
using Ecat.Data.Contexts.Config;

namespace Ecat.Data.Contexts
{
    public class LmsAdminMetadata: DbContext
    {
        //Only used for metadata for breeze, cannot be used to query the database
        //TODO: Make sure this is everything we need for LMS Admin...
        static LmsAdminMetadata()
        {
            Database.SetInitializer<LmsAdminMetadata>(null);
        }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            mb.Configurations.Add(new ConfigProfileStudent());
            mb.Configurations.Add(new ConfigProfileFaculty());
            mb.Configurations.Add(new ConfigSpResult());
            mb.Configurations.Add(new ConfigStratResult());
            mb.Configurations.Add(new ConfigCrseStudInGroup());
            mb.Configurations.Add(new ConfigFacultyInCourse());
            mb.Configurations.Add(new ConfigStudentInCourse());
            mb.Configurations.Add(new ConfigFacStratResponse());
            mb.Configurations.Add(new ConfigStratResponse());
            mb.Configurations.Add(new ConfigPerson());

            //mb.Ignore<Academy>();
            mb.Ignore<Security>();
            mb.Ignore<SanitizedSpComment>();
            mb.Ignore<SanitizedSpResponse>();
            mb.Ignore<SpResponse>();
            mb.Ignore<FacSpResponse>();
            //mb.Ignore<SpResult>();
            //mb.Ignore<StratResponse>();
            //mb.Ignore<FacStratResponse>();
            mb.Ignore<StudSpComment>();
            mb.Ignore<StudSpCommentFlag>();
            mb.Ignore<FacSpCommentFlag>();
            mb.Ignore<FacSpComment>();
            mb.Ignore<RoadRunner>();

            mb.Entity<WorkGroup>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.Groups)
                .HasForeignKey(p => p.ReconResultId);

            mb.Entity<Course>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.ReconResultId);

            mb.Entity<CrseStudentInGroup>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.GroupMembers)
                .HasForeignKey(p => p.ReconResultId);

            mb.Entity<FacultyInCourse>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.Faculty)
                .HasForeignKey(p => p.ReconResultId);

            mb.Entity<StudentInCourse>()
                .HasOptional(p => p.ReconResult)
                .WithMany(p => p.Students)
                .HasForeignKey(p => p.ReconResultId);
        }

        public IDbSet<WorkGroup> WorkGroups { get; set; }
        public IDbSet<Course> Courses { get; set; }
        public IDbSet<CrseStudentInGroup> StudentInGroups { get; set; }
        public IDbSet<FacultyInCourse> FacultyInCourses { get; set; }
        public IDbSet<StudentInCourse> StudentInCourses { get; set; }
    }
}
