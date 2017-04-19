using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Faculty.Config
{
    public class ConfigFacSpComment : EntityTypeConfiguration<FacSpComment>
    {
        public ConfigFacSpComment()
        {
            HasKey(p => new { p.RecipientPersonId, p.CourseId, p.WorkGroupId });

            Property(p => p.CommentText).IsMaxLength();

            HasRequired(p => p.FacultyCourse)
                 .WithMany(p => p.FacSpComments)
                 .HasForeignKey(p => new { p.FacultyPersonId, p.CourseId })
                 .WillCascadeOnDelete(false);

            HasRequired(p => p.Recipient)
                .WithOptional(p => p.FacultyComment)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.WorkGroup)
                .WithMany(p => p.FacSpComments)
                .HasForeignKey(p => p.WorkGroupId)
                .WillCascadeOnDelete(false);
        }
    }
}
