using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Student.Config
{
    public class ConfigSpCommentFlag : EntityTypeConfiguration<StudSpCommentFlag>
    {
        public ConfigSpCommentFlag()
        {

            HasKey(p => new
            {
                p.AuthorPersonId,
                p.RecipientPersonId,
                p.CourseId,
                p.WorkGroupId
            });

            Ignore(p => p.FlaggedByFaculty);

            HasRequired(p => p.Comment)
                .WithOptional(p => p.Flag);
        }
    }
}
