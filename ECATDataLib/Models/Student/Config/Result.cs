using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Student.Config
{
    public class ConfigSpResult : EntityTypeConfiguration<SpResult>
    {
        public ConfigSpResult()
        {
            HasKey(p => new { p.StudentId, p.CourseId, p.WorkGroupId });

            Ignore(p => p.SpResponses);
            Ignore(p => p.FacultyResponses);

            HasRequired(p => p.Course)
              .WithMany(p => p.SpResults)
              .HasForeignKey(p => p.CourseId)
              .WillCascadeOnDelete(false);

            HasRequired(p => p.WorkGroup)
              .WithMany(p => p.SpResults)
              .HasForeignKey(p => p.WorkGroupId)
              .WillCascadeOnDelete(false);

            HasRequired(p => p.AssignedInstrument)
                .WithMany()
                .HasForeignKey(p => p.AssignedInstrumentId)
                .WillCascadeOnDelete(false);

        }
    }

    public class ConfigStratResult : EntityTypeConfiguration<StratResult>
    {
        public ConfigStratResult()
        {
            HasKey(p => new { p.StudentId, p.CourseId, p.WorkGroupId });
            Property(p => p.StudStratAwardedScore).HasPrecision(18, 3);
            Property(p => p.FacStratAwardedScore).HasPrecision(18, 3);
            Ignore(p => p.StratResponses);
            Ignore(p => p.FacStrat);

            HasRequired(p => p.Course)
                .WithMany(p => p.StratResults)
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false);
        }
    }
}
