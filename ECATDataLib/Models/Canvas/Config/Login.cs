using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Models.Canvas.Config
{
    public class ConfigCanvasLogin: EntityTypeConfiguration<CanvasLogin>
    {
        public ConfigCanvasLogin()
        {
            HasKey(cl => cl.PersonId);

            HasRequired(cl => cl.FacultyProfile)
                .WithOptional(fp => fp.CanvasLogin)
                //.HasForeignKey(p => p.FacultyPersonId)
                .WillCascadeOnDelete(false);
        }
    }
}
