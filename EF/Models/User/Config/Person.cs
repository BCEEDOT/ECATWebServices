using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;

namespace ECATDataLib.Models.User.Config
{
    public class ConfigPerson: EntityTypeConfiguration<Person>
    {
        public ConfigPerson()
        {
            HasKey(p => p.PersonId);
            Property(p => p.LastName).IsRequired();
            Property(p => p.FirstName).IsRequired();
            Property(p => p.MpAffiliation).IsRequired();
            Property(p => p.MpComponent).IsRequired();
            Property(p => p.MpPaygrade).IsRequired();
            Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_UniqueEmailAddress") { IsUnique = true }));

            Property(p => p.MpGender).IsRequired();
            Property(p => p.MpInstituteRole).IsRequired();

            //HasOptional(p => p.External)
            //    .WithRequired(p => p.Person)
            //    .WillCascadeOnDelete(true);

            //HasOptional(p => p.HqStaff)
            //    .WithRequired(p => p.Person)
            //    .WillCascadeOnDelete(true);

            HasOptional(p => p.Student)
                .WithRequired(p => p.Person)
               .WillCascadeOnDelete(true);

            HasOptional(p => p.Faculty)
                .WithRequired(p => p.Person)
                .WillCascadeOnDelete(true);

            //HasOptional(p => p.Designer)
            //    .WithRequired(p => p.Person)
            //    .WillCascadeOnDelete(true);
        }
    }
}
