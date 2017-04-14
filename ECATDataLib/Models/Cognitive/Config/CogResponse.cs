using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Ecat.Data.Models.Cognitive.Config
{
    public class ConfigCogResponse : EntityTypeConfiguration<CogResponse>
    {
        public ConfigCogResponse()
        {
            HasKey(p => new
            {
                p.CogInventoryId,
                p.PersonId,
                p.Attempt
            });

            HasRequired(p => p.Person)
                .WithMany()
                .HasForeignKey(p => p.PersonId);

            HasRequired(p => p.InventoryItem)
                .WithMany()
                .HasForeignKey(p => p.CogInventoryId);
        }
    }
}
