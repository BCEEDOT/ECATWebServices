using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace ECATDataLib.Models.User.Config
{
    public class ConfigProfileStudent : EntityTypeConfiguration<ProfileStudent>
    {
        public ConfigProfileStudent()
        {
            HasKey(p => p.PersonId);

            Property(p => p.Bio)
                .HasMaxLength(6000);
        }
    }

    public class ConfigProfileFaculty : EntityTypeConfiguration<ProfileFaculty>
    {
        public ConfigProfileFaculty()
        {
            HasKey(p => p.PersonId);

            Property(p => p.Bio)
                .HasMaxLength(6000);
        }
    }

    //public class ConfigProfileDesigner : EntityTypeConfiguration<ProfileDesigner>
    //{
    //    public ConfigProfileDesigner()
    //    {
    //        HasKey(p => p.PersonId);

    //        Property(p => p.Bio)
    //            .HasMaxLength(6000);
    //    }
    //}

    //public class ConfigProfileStaff : EntityTypeConfiguration<ProfileStaff>
    //{
    //    public ConfigProfileStaff()
    //    {
    //        HasKey(p => p.PersonId);

    //        Property(p => p.Bio)
    //            .HasMaxLength(6000);
    //    }
    //}

    //public class ConfigProfileExternal : EntityTypeConfiguration<ProfileExternal>
    //{
    //    public ConfigProfileExternal()
    //    {
    //        HasKey(p => p.PersonId);

    //        Property(p => p.Bio)
    //            .HasMaxLength(6000);
    //    }
    //}
}
