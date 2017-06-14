using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Student.Config
{
    public class ConfigSpResponse : EntityTypeConfiguration<SpResponse>
    {
        public ConfigSpResponse()
        {
            HasKey(p => new
            {
                p.AssessorPersonId,
                p.AssesseePersonId,
                p.CourseId,
                p.WorkGroupId,
                p.InventoryItemId
            });

            HasRequired(p => p.Assessor)
                .WithMany(p => p.AssessorSpResponses)
                .HasForeignKey(p => new { p.AssessorPersonId, p.CourseId, p.WorkGroupId })
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Assessee)
                .WithMany(p => p.AssesseeSpResponses)
                .HasForeignKey(p => new { p.AssesseePersonId, p.CourseId, p.WorkGroupId })
                .WillCascadeOnDelete(false);

            HasRequired(p => p.InventoryItem)
                .WithMany()
                .HasForeignKey(p => p.InventoryItemId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Course)
                .WithMany(p => p.SpResponses)
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.WorkGroup)
                .WithMany(p => p.SpResponses)
                .HasForeignKey(p => p.WorkGroupId)
                .WillCascadeOnDelete(false);
        }
    }

    public class ConfigStratResponse : EntityTypeConfiguration<StratResponse>
    {
        public ConfigStratResponse()
        {
            HasKey(p => new
            {
                p.AssessorPersonId,
                p.AssesseePersonId,
                p.CourseId,
                p.WorkGroupId,
            });

            HasRequired(p => p.Assessor)
                .WithMany(p => p.AssessorStratResponse)
                .HasForeignKey(p => new { p.AssessorPersonId, p.CourseId, p.WorkGroupId })
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Assessee)
                .WithMany(p => p.AssesseeStratResponse)
                .HasForeignKey(p => new { p.AssesseePersonId, p.CourseId, p.WorkGroupId })
                .WillCascadeOnDelete(false);

        }
    }
}
