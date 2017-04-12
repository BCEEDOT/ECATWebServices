using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.User.Config
{
    public class ConfigRoadRunner: EntityTypeConfiguration<RoadRunner>
    {
        public ConfigRoadRunner()
        {
            //Foriegn key
            HasRequired(p => p.Person)
                .WithMany(g => g.RoadRunnerAddresses);

            Property(p => p.Location)
                .HasMaxLength(200);
        }
    }
}
