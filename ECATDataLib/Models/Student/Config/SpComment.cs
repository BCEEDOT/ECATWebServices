using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Student.Config
{
    public class ConfigSpComment : EntityTypeConfiguration<StudSpComment>
    {
        public ConfigSpComment()
        {
            HasKey(p => new
            {
                p.AuthorPersonId,
                p.RecipientPersonId,
                p.CourseId,
                p.WorkGroupId
            });

            Property(p => p.CommentText).IsMaxLength();

            HasRequired(p => p.Author)
                .WithMany(p => p.AuthorOfComments)
                .HasForeignKey(p => new { p.AuthorPersonId, p.CourseId, p.WorkGroupId })
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Recipient)
                .WithMany(p => p.RecipientOfComments)
                .HasForeignKey(p => new { p.RecipientPersonId, p.CourseId, p.WorkGroupId })
                .WillCascadeOnDelete(false);

            HasRequired(p => p.WorkGroup)
               .WithMany(p => p.SpComments)
               .HasForeignKey(p => p.WorkGroupId)
               .WillCascadeOnDelete(false);

            HasOptional(p => p.Flag)
                .WithRequired()
                .WillCascadeOnDelete(false);
        }
    }
}
