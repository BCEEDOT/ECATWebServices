using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Faculty.Config
{
    public class ConfigFacSpCommentFlag : EntityTypeConfiguration<FacSpCommentFlag>
    {
        public ConfigFacSpCommentFlag()
        {
            HasKey(p => new
            {
                p.RecipientPersonId,
                p.CourseId,
                p.WorkGroupId
            });

            HasRequired(p => p.Comment)
                .WithOptional(p => p.Flag);
        }
    }
}
