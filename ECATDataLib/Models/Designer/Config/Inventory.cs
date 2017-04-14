using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Designer.Config
{
    public class ConfigSpInventory : EntityTypeConfiguration<SpInventory>
    {
        public ConfigSpInventory()
        {
            Property(p => p.Behavior).IsMaxLength();
        }
    }

    public class ConfigCogInventory : EntityTypeConfiguration<CogInventory>
    {
        public ConfigCogInventory()
        {
            Property(p => p.ItemDescription).IsMaxLength();
            Property(p => p.AdaptiveDescription).IsMaxLength();
            Property(p => p.InnovativeDescription).IsMaxLength();
        }
    }
}
