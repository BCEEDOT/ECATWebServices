using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.School.Config
{
    public class ConfigStudentInCourse : EntityTypeConfiguration<StudentInCourse>
    {
        public ConfigStudentInCourse()
        {
            HasKey(p => new { p.StudentPersonId, p.CourseId });

            HasRequired(p => p.Student)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.StudentPersonId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Course)
                .WithMany(p => p.Students)
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false);

            //Ignore(p => p.KcResponses);
        }
    }

    public class ConfigFacultyInCourse : EntityTypeConfiguration<FacultyInCourse>
    {
        public ConfigFacultyInCourse()
        {
            HasKey(p => new { p.FacultyPersonId, p.CourseId });
            //TODO:Update as more implemented
            //Ignore(p => p.FlaggedSpComments);

            HasRequired(p => p.FacultyProfile)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.FacultyPersonId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Course)
                .WithMany(p => p.Faculty)
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false);
        }
    }
}
