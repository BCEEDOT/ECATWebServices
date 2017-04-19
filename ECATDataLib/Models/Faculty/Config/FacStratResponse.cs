using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Faculty.Config
{
    public class ConfigFacStratResponse : EntityTypeConfiguration<FacStratResponse>
    {
        public ConfigFacStratResponse()
        {
            HasKey(p => new { p.AssesseePersonId, p.CourseId, p.WorkGroupId });

            HasRequired(p => p.FacultyAssessor)
                .WithMany(p => p.FacStratResponse)
                .HasForeignKey(p => new { p.FacultyPersonId, p.CourseId })
                .WillCascadeOnDelete(false);

            HasRequired(p => p.WorkGroup)
                 .WithMany(p => p.FacStratResponses)
                 .HasForeignKey(p => p.WorkGroupId)
                 .WillCascadeOnDelete(false);

        }
    }
}
