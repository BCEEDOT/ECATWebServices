using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Ecat.Business.Repositories.Interface;
using Breeze.Persistence;
using Breeze.Persistence.EF6;
using Newtonsoft.Json.Linq;
using Ecat.Data.Contexts;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.Student;
using Ecat.Data.Static;
using Ecat.Business.Guards;

namespace Ecat.Business.Repositories
{
    public class FacultyRepo : IFacultyRepo
    {
        private readonly EFPersistenceManager<EcatContext> ctxManager;

        public int loggedInUserId { get; set; }

        public FacultyRepo(EcatContext context)
        {
            ctxManager = new EFPersistenceManager<EcatContext>(context);
        }

        #region breeze methods
        public string MetaData()
        {
            return new EFPersistenceManager<FacultyMetadata>().Metadata();
        }

        public SaveResult ClientSave(JObject saveBundle)
        {
            var guardian = new FacultyGuard(ctxManager, loggedInUserId);
            ctxManager.BeforeSaveEntitiesDelegate += guardian.BeforeSaveEntities;
            return ctxManager.SaveChanges(saveBundle);
        }
        #endregion breeze methods

        async Task<List<WorkGroup>> IFacultyRepo.GetRoadRunnerWorkGroups(int courseId)
        {

            var latestCourse = ctxManager.Context.Courses.Where(crse => crse.Id == courseId).Single();

            var workGroupsInCourse = await (from workGroup in ctxManager.Context.WorkGroups

                                            where workGroup.CourseId == latestCourse.Id && workGroup.MpCategory == MpGroupCategory.Wg1

                                            select new
                                            {
                                                workGroup,
                                                GroupMembers = workGroup.GroupMembers.Where(gm => gm.IsDeleted == false).Select(gm => new
                                                {
                                                    gm,
                                                    StudProfile = gm.StudentProfile,
                                                    StudPerson = gm.StudentProfile.Person,
                                                    RoadRunner = gm.StudentProfile.Person.RoadRunnerAddresses
                                                })



                                            }).ToListAsync();

            var currentWorkgroups = new List<WorkGroup>();

            workGroupsInCourse.ForEach(wg =>
            {
                if (wg.GroupMembers.Any())
                {
                    currentWorkgroups.Add(wg.workGroup);
                }
            });

            return currentWorkgroups;


        }

        async Task<List<Course>> IFacultyRepo.GetActiveCourse(int? courseId)
        {
            var query = courseId == null
                ? ctxManager.Context.Courses
                : ctxManager.Context.Courses.Where(course => course.Id == courseId);

            var coursesProj = await (from course in query
                                     where course.Faculty.Any(fac => fac.FacultyPersonId == loggedInUserId && !fac.IsDeleted)
                                     select new
                                     {
                                         course,
                                         FacultyCourses = course.Faculty.Where(fic => fic.FacultyPersonId == loggedInUserId),
                                     }).ToListAsync();

            if (coursesProj == null) return null;

            var courses = coursesProj.OrderByDescending(c => c.course.StartDate).Select(c => c.course).ToList();

            var latestCourse = courses.First();

            var wgProj = await (from wg in ctxManager.Context.WorkGroups
                                let inventoryCount = wg.AssignedSpInstr.InventoryCollection.Count
                                let activeGroupCount = wg.GroupMembers.Count(gm => !gm.IsDeleted)
                                where wg.CourseId == latestCourse.Id && activeGroupCount > 0
                                select new
                                {
                                    wg,
                                    CanPublish = wg.GroupMembers.Where(gm => !gm.IsDeleted).All(
                                            gm =>
                                                gm.AssessorSpResponses.Count(assess => !assess.Assessee.IsDeleted) ==
                                                inventoryCount * activeGroupCount &&
                                                gm.AssessorStratResponse.Count(strat => !strat.Assessee.IsDeleted) == activeGroupCount &&
                                                gm.AssessorStratResponse.Where(strat => !strat.Assessee.IsDeleted).Max(strat => strat.StratPosition) <= activeGroupCount)
                                }).ToListAsync();

            latestCourse.WorkGroups = new List<WorkGroup>();

            if (wgProj == null) return courses;

            foreach (var wgp in wgProj)
            {
                var workGroup = wgp.wg;
                workGroup.CanPublish = wgp.CanPublish;
                latestCourse.WorkGroups.Add(workGroup);
            }

            latestCourse.WorkGroups = latestCourse.WorkGroups.OrderBy(wg => wg.GroupNumber).ToList();

            return courses;
        }

        async Task<WorkGroup> IFacultyRepo.GetActiveWorkGroup(int courseId, int workGroupId)
        {
            var requestedWg = await (from wg in ctxManager.Context.WorkGroups
                                     let inventoryCount = wg.AssignedSpInstr.InventoryCollection.Count
                                     let activeGroupCount = wg.GroupMembers.Count(gm => !gm.IsDeleted)
                                     where wg.WorkGroupId == workGroupId &&
                            wg.CourseId == courseId &&
                            wg.Course.Faculty.Any(fac => fac.FacultyPersonId == loggedInUserId)
                                     select new
                                     {
                                         ActiveWg = wg,
                                         CanPublish = wg.GroupMembers.Where(gm => !gm.IsDeleted).All(
                                                 gm =>
                                                     gm.AssessorSpResponses.Count(assess => !assess.Assessee.IsDeleted) ==
                                                     inventoryCount * activeGroupCount &&
                                                     gm.AssessorStratResponse.Count(strat => !strat.Assessee.IsDeleted) == activeGroupCount &&
                                                     gm.AssessorStratResponse.Where(strat => !strat.Assessee.IsDeleted).Max(strat => strat.StratPosition) <= activeGroupCount),

                                         GroupMembers = wg.GroupMembers.Where(gm => !gm.IsDeleted).Select(gm => new
                                         {
                                             gm,
                                             SpRepsonses = gm.AssessorSpResponses.Where(aos => !aos.Assessee.IsDeleted),
                                             CommentCount = gm.AuthorOfComments.Count(aos => !aos.Recipient.IsDeleted),
                                             FacComment = wg.FacSpComments.FirstOrDefault(fc => fc.RecipientPersonId == gm.StudentId),
                                             FacCourse = wg.FacSpComments.FirstOrDefault(fc => fc.RecipientPersonId == gm.StudentId).FacultyCourse,
                                             FacProfile = wg.FacSpComments.FirstOrDefault(fc => fc.RecipientPersonId == gm.StudentId).FacultyCourse.FacultyProfile,
                                             FacPerson = wg.FacSpComments.FirstOrDefault(fc => fc.RecipientPersonId == gm.StudentId).FacultyCourse.FacultyProfile.Person,
                                             FacFlag = wg.FacSpComments.FirstOrDefault(fc => fc.RecipientPersonId == gm.StudentId).Flag,
                                             FacResponse = wg.FacSpResponses.Where(fr => fr.AssesseePersonId == gm.StudentId),
                                             FacStrats = wg.FacStratResponses.FirstOrDefault(fs => fs.AssesseePersonId == gm.StudentId),
                                             StudProfile = gm.StudentProfile,
                                             StudPerson = gm.StudentProfile.Person,
                                             MissingStratCount = wg.GroupMembers.Where(peer => !peer.IsDeleted).Count(
                                                 peer => peer.AssesseeStratResponse.Count(strat => strat.AssessorPersonId == gm.StudentId && strat.StratPosition <= activeGroupCount) == 0),
                                             StratSum = gm.AssessorStratResponse.Where(aos => !aos.Assessee.IsDeleted).Sum(strat => (int?)strat.StratPosition) ?? 0
                                         }),

                                         InstrumentId = (int?)wg.AssignedSpInstrId ?? 0
                                     }).SingleAsync();

            var workGroup = requestedWg.ActiveWg;
            workGroup.CanPublish = requestedWg.CanPublish;

            var totalStratPos = 0;
            for (var i = 1; i <= workGroup.GroupMembers.Count(); i++)
            {
                totalStratPos += i;
            }

            foreach (var grpMem in workGroup.GroupMembers)
            {
                var studInGroup = requestedWg.GroupMembers.Single(gm => gm.gm.StudentId == grpMem.StudentId);
                grpMem.NumOfStratIncomplete = studInGroup.MissingStratCount;
                if (grpMem.NumOfStratIncomplete == 0 && studInGroup.StratSum != totalStratPos)
                {
                    grpMem.NumOfStratIncomplete = 2;
                    workGroup.CanPublish = false;
                }
                grpMem.NumberOfAuthorComments = studInGroup.CommentCount;
            }

            var test = await ctxManager.Context.SpInstruments
                .Where(instr => instr.Id == requestedWg.InstrumentId)
                .Select(instr => new
                {
                    instr,
                    inventory = instr.InventoryCollection.Where(collection => collection.IsDisplayed)
                }).SingleAsync();

            workGroup.AssignedSpInstr = test.instr;

            return workGroup;
        }

        async Task<SpInstrument> IFacultyRepo.GetSpInstrument(int instrumentId)
        {
            var instrument = await ctxManager.Context.SpInstruments
                .Where(instr => instr.Id == instrumentId)
                .Select(instr => new
                {
                    instr,
                    inventory = instr.InventoryCollection.Where(collection => collection.IsDisplayed)
                }).SingleAsync();

            return instrument.instr;
        }

        async Task<List<StudSpComment>> IFacultyRepo.GetStudSpComments(int courseId, int workGroupId)
        {
            var comments = await ctxManager.Context.WorkGroups
                .Where(wg => wg.CourseId == courseId && wg.WorkGroupId == workGroupId)
                .SelectMany(wg => wg.SpComments)
                .Include(comment => comment.Flag)
                .Include(comment => comment.Author)
                .Include(comment => comment.Recipient).ToListAsync();

            var activeComments = comments.Where(comment => !comment.Author.IsDeleted && !comment.Recipient.IsDeleted).ToList();
            foreach (var comment in activeComments)
            {
                comment.Author = null;
                comment.Recipient = null;
            }

            return activeComments;
        }

        async Task<List<FacSpComment>> IFacultyRepo.GetFacSpComment(int courseId, int workGroupId)
        {
            var comments = ctxManager.Context.WorkGroups
                .Where(wg => wg.CourseId == courseId && wg.WorkGroupId == workGroupId)
                .SelectMany(wg => wg.FacSpComments)
                .Include(comment => comment.Flag)
                .Include(comment => comment.Recipient)
                .Include(comment => comment.FacultyCourse.FacultyProfile.Person)
                .Include(comment => comment.FacultyCourse.FacultyProfile);

            return await comments.Where(comment => !comment.Recipient.IsDeleted).ToListAsync();
        }

        async Task<WorkGroup> IFacultyRepo.GetSpResult(int courseId, int workGroupId)
        {
            var requestedWg = await (from wg in ctxManager.Context.WorkGroups
                                     let inventoryCount = wg.AssignedSpInstr.InventoryCollection.Count
                                     let activeGroupCount = wg.GroupMembers.Count(gm => !gm.IsDeleted)
                                     where wg.WorkGroupId == workGroupId &&
                                           wg.CourseId == courseId &&
                                           wg.Course.Faculty.Any(fac => fac.FacultyPersonId == loggedInUserId)
                                     select new
                                     {
                                         ActiveWg = wg,
                                         CanPublish = wg.GroupMembers.All(
                                             gm =>
                                                 gm.AssessorSpResponses.Count(assess => !assess.Assessor.IsDeleted) ==
                                                 inventoryCount * activeGroupCount &&
                                                 gm.AssessorStratResponse.Count(strat => !strat.Assessor.IsDeleted) == activeGroupCount &&
                                                 gm.AssessorStratResponse.Max(strat => strat.StratPosition) <= activeGroupCount),

                                         GroupMembers = wg.GroupMembers.Where(gm => !gm.IsDeleted).Select(gm => new
                                         {
                                             gm,
                                             StratResult = wg.SpStratResults.FirstOrDefault(sr => sr.StudentId == gm.StudentId),
                                             SpResult = wg.SpResults.FirstOrDefault(sr => sr.StudentId == gm.StudentId),
                                             Strats = gm.AssesseeStratResponse.Where(asr => !asr.Assessor.IsDeleted),
                                             SpRepsonses = gm.AssessorSpResponses.Where(aos => !aos.Assessee.IsDeleted),
                                             FacFacultyInCourse = wg.FacSpComments.Select(comment => comment.FacultyCourse),
                                             FacFacultyPerson =
                                                 wg.FacSpComments.Select(comment => comment.FacultyCourse.FacultyProfile.Person),
                                             FacFacultyProfile =
                                                 wg.FacSpComments.Select(comment => comment.FacultyCourse.FacultyProfile),
                                             FacComment = wg.FacSpComments.FirstOrDefault(fc => fc.RecipientPersonId == gm.StudentId),
                                             FacFlag = wg.FacSpComments.FirstOrDefault(fc => fc.RecipientPersonId == gm.StudentId).Flag,
                                             FacResponse = wg.FacSpResponses.Where(fr => fr.AssesseePersonId == gm.StudentId),
                                             FacStrats = wg.FacStratResponses.FirstOrDefault(fs => fs.AssesseePersonId == gm.StudentId),
                                             StudProfile = gm.StudentProfile,
                                             StudPerson = gm.StudentProfile.Person
                                         })
                                     }).SingleAsync();

            var workGroup = requestedWg.ActiveWg;
            workGroup.CanPublish = requestedWg.CanPublish;
            return workGroup;
        }
    }
}
