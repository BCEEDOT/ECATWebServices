using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Designer.Config
{
    public class ConfigSpInstrument : EntityTypeConfiguration<SpInstrument>
    {
        public ConfigSpInstrument()
        {
            Property(p => p.FacultyInstructions).IsMaxLength();
            Property(p => p.StudentInstructions).IsMaxLength();
            HasMany(p => p.AssignedGroups)
                .WithOptional(p => p.AssignedSpInstr)
                .HasForeignKey(p => p.AssignedSpInstrId)
                .WillCascadeOnDelete(false);
        }
    }

    public class ConfigCogInstrument : EntityTypeConfiguration<CogInstrument>
    {
        public ConfigCogInstrument()
        {
            Property(p => p.CogInstructions).IsMaxLength();

        }
    }
}
