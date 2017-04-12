using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.User.Config
{
    public class ConfigSecurity : EntityTypeConfiguration<Security>
    {
        public ConfigSecurity()
        {
            HasKey(p => p.PersonId);
            Property(p => p.PasswordHash).HasMaxLength(400);
            HasRequired(p => p.Person)
             .WithOptional(p => p.Security);
        }
    }
}
