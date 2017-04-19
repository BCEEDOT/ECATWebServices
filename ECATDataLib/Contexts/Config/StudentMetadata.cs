using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.Faculty;

namespace Ecat.Data.Contexts.Config
{
    public class StudConfigSanitizedComment : EntityTypeConfiguration<SanitizedSpComment>
    {
        public StudConfigSanitizedComment()
        {
            HasKey(p => p.Id);

            HasRequired(p => p.Result)
                .WithMany(p => p.SanitizedComments)
                .HasForeignKey(p => new { p.RecipientId, p.CourseId, p.WorkGroupId });
        }
    }

    public class StudConfigSanitizedResponse : EntityTypeConfiguration<SanitizedSpResponse>
    {
        public StudConfigSanitizedResponse()
        {
            HasKey(p => p.Id);

            HasRequired(p => p.Result)
                .WithMany(p => p.SanitizedResponses)
                .HasForeignKey(p => new { p.AssesseeId, p.CourseId, p.WorkGroupId });
        }
    }

    public class StudConfigSpInstrument : EntityTypeConfiguration<SpInstrument>
    {
        public StudConfigSpInstrument()
        {
            Ignore(p => p.Version);
            Ignore(p => p.FacultyInstructions);
            Ignore(p => p.ModifiedById);
            Ignore(p => p.ModifiedDate);
            Property(p => p.StudentInstructions).IsMaxLength();
            HasMany(p => p.AssignedGroups)
                .WithOptional(p => p.AssignedSpInstr)
                .HasForeignKey(p => p.AssignedSpInstrId);
        }
    }

    public class StudConfigSpInventory : EntityTypeConfiguration<SpInventory>
    {
        public StudConfigSpInventory()
        {
            Ignore(p => p.IsScored);
            Ignore(p => p.ModifiedById);
            Ignore(p => p.ModifiedDate);
        }
    }

    public class StudConfigStudWrkGrp : EntityTypeConfiguration<WorkGroup>
    {
        public StudConfigStudWrkGrp()
        {
            Ignore(p => p.WgModel);
            Ignore(p => p.FacSpComments);
            Ignore(p => p.FacSpResponses);
            Ignore(p => p.FacStratResponses);
            Ignore(p => p.BbGroupId);
            Ignore(p => p.CanPublish);
        }
    }

    public class StudConfigCrse : EntityTypeConfiguration<Course>
    {
        public StudConfigCrse()
        {
            Ignore(p => p.BbCourseId);
        }
    }

    internal class StudConfigProfileStudent : EntityTypeConfiguration<ProfileStudent>
    {
        public StudConfigProfileStudent()
        {
            Ignore(p => p.Commander);
            Ignore(p => p.CommanderEmail);
            Ignore(p => p.Shirt);
            Ignore(p => p.ShirtEmail);
            Ignore(p => p.ContactNumber);

            HasKey(p => p.PersonId)
                .HasRequired(p => p.Person)
                .WithOptional(p => p.Student);
        }
    }

    internal class StudConfigPerson : EntityTypeConfiguration<Person>
    {
        public StudConfigPerson()
        {
            HasKey(p => p.PersonId);

            Ignore(p => p.BbUserId);
            Ignore(p => p.IsActive);
            Ignore(p => p.ModifiedById);
        }
    }

    internal class StudConfigCrseStudInWg : EntityTypeConfiguration<CrseStudentInGroup>
    {
        public StudConfigCrseStudInWg()
        {
            HasOptional(p => p.SpResult)
              .WithRequired(p => p.ResultFor);

            HasOptional(p => p.StratResult)
                .WithRequired(p => p.ResultFor);

            Ignore(p => p.GroupPeers);
            Ignore(p => p.NumOfStratIncomplete);
            Ignore(p => p.NumberOfAuthorComments);

            HasKey(p => new { p.StudentId, p.CourseId, p.WorkGroupId });

            HasRequired(p => p.StudentInCourse)
                .WithMany(p => p.WorkGroupEnrollments)
                .HasForeignKey(p => new { p.StudentId, p.CourseId });

            HasRequired(p => p.WorkGroup)
                .WithMany(p => p.GroupMembers)
                .HasForeignKey(p => p.WorkGroupId);

            HasRequired(p => p.StudentProfile)
                .WithMany(p => p.CourseGroupMemberships)
                .HasForeignKey(p => p.StudentId);

            HasRequired(p => p.Course)
                .WithMany(p => p.StudentInCrseGroups)
                .HasForeignKey(p => p.CourseId);
        }
    }

    internal class StudConfigSpComment : EntityTypeConfiguration<StudSpComment>
    {
        public StudConfigSpComment()
        {
            HasKey(p => new
            {
                p.AuthorPersonId,
                p.RecipientPersonId,
                p.CourseId,
                p.WorkGroupId
            });

            Property(p => p.CommentText).IsMaxLength();

            HasRequired(p => p.Author)
             .WithMany(p => p.AuthorOfComments)
             .HasForeignKey(p => new { p.AuthorPersonId, p.CourseId, p.WorkGroupId })
             .WillCascadeOnDelete(false);

            HasRequired(p => p.Recipient)
                .WithMany(p => p.RecipientOfComments)
                .HasForeignKey(p => new { p.RecipientPersonId, p.CourseId, p.WorkGroupId })
                .WillCascadeOnDelete(false);

            HasRequired(p => p.WorkGroup)
               .WithMany(p => p.SpComments)
               .HasForeignKey(p => p.WorkGroupId)
               .WillCascadeOnDelete(false);

        }

        internal class ConfigSpCommentFlag : EntityTypeConfiguration<StudSpCommentFlag>
        {
            public ConfigSpCommentFlag()
            {

                HasKey(p => new
                {
                    p.AuthorPersonId,
                    p.RecipientPersonId,
                    p.CourseId,
                    p.WorkGroupId
                });

                Ignore(p => p.FlaggedByFaculty);

                HasRequired(p => p.Comment)
                    .WithOptional(p => p.Flag);
            }
        }
    }
}
