using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.IO;
using Breeze.Persistence;
using Breeze.Persistence.EF6;
using Newtonsoft.Json.Linq;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Contexts;
using Ecat.Data.Models.User;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Common;
using Ecat.Data.Static;
using Ecat.Data.Models.Designer;
using Ecat.Business.BbWs.BbCourse;
using Ecat.Business.BbWs.BbCourseMembership;
using Ecat.Business.BbWs.BbUser;
using Ecat.Business.Utilities;
using Ecat.Business.Guards;

namespace Ecat.Business.Repositories
{
    public class LmsAdminCourseRepo: ILmsAdminCourseRepo
    {
        private readonly EFPersistenceManager<EcatContext> ctxManager;
        private readonly BbWsCnet bbWs;

        public int loggedInUserId { get; set; }
        private ProfileFaculty Faculty { get; set; }

        public LmsAdminCourseRepo(EcatContext mainCtx, BbWsCnet bbWsCnet)
        {
            ctxManager = new EFPersistenceManager<EcatContext>(mainCtx);
            bbWs = bbWsCnet;

            Faculty = ctxManager.Context.Faculty.Where(f => f.PersonId == loggedInUserId).SingleOrDefault();
        }

        #region breeze methods
        public string Metadata()
        {
            //Big problem with EF6 taking minutes to generate metadata at times worked around by just having static metadata strings served up
            //TODO: Change this if you ever change a model
            return StaticMetadatas.LmsAdminStatic;
            //return new EFPersistenceManager<LmsAdminMetadata>().Metadata();
        }



        public SaveResult SaveClientChanges(JObject saveBundle)
        {
            var guardian = new IsaGuard(ctxManager, loggedInUserId);
            ctxManager.BeforeSaveEntitiesDelegate += guardian.BeforeSaveEntities;
            return ctxManager.SaveChanges(saveBundle);
        }
        #endregion breeze methods

        public async Task GetProfile()
        {
            if (Faculty == null || Faculty.PersonId == 0)
            {
                Faculty = await ctxManager.Context.Faculty
                .Where(fac => fac.PersonId == loggedInUserId)
                .SingleOrDefaultAsync();
            }            
        }

        public async Task<List<Course>> GetAllCourses()
        {
            await GetProfile();

            return await ctxManager.Context.Courses
                .Where(course => course.AcademyId == Faculty.AcademyId)
                .ToListAsync();
        }

        public async Task<List<WorkGroupModel>> GetCourseModels(int courseId)
        {
            var course = await ctxManager.Context.Courses
                .Where(crs => crs.Id == courseId)
                .Include(crse => crse.WorkGroups)
                .SingleAsync();

            var edLevel = StaticAcademy.AcadLookupById
                .Where(acad => acad.Key == course.AcademyId)
                .Single()
                .Value
                .MpEdLevel;

            var models = await ctxManager.Context.WgModels
                .Where(mdl => mdl.MpEdLevel == edLevel && mdl.IsActive)
                .ToListAsync();

            models.ForEach(mdl => mdl.WorkGroups = course.WorkGroups.Where(grp => grp.WgModelId == mdl.Id).ToList());

            return models;
        }

        public async Task<List<CategoryVO>> GetBbCategories()
        {
            var filter = new CategoryFilter
            {
                filterType = (int)CategoryFilterTpe.GetAllCourseCategory,
                filterTypeSpecified = true
            };
            var autoRetry = new Retrier<CategoryVO[]>();
            var categories = await autoRetry.Try(() => bbWs.BbCourseCategories(filter), 3);

            return categories.ToList();
        }

        public async Task<CourseReconResult> ReconcileCourses()
        {
            await GetProfile();

            var courseFilter = new CourseFilter
            {
                filterTypeSpecified = true,
                filterType = (int)CourseFilterType.LoadByCatId
            };

            if (Faculty != null)
            {
                var academy = StaticAcademy.AcadLookupById[Faculty.AcademyId];
                courseFilter.categoryIds = new[] { academy.BbCategoryId };
            }
            else
            {
                var ids = StaticAcademy.AcadLookupById.Select(acad => acad.Value.BbCategoryId).ToArray();
                courseFilter.categoryIds = ids;
            }

            var autoRetry = new Retrier<CourseVO[]>();
            var bbCoursesResult = await autoRetry.Try(() => bbWs.BbCourses(courseFilter), 3);

            if (bbCoursesResult == null) throw new InvalidDataException("No Bb Responses received");

            var queryKnownCourses = ctxManager.Context.Courses.AsQueryable();

            queryKnownCourses = Faculty == null
                ? queryKnownCourses
                : queryKnownCourses.Where(crse => crse.AcademyId == Faculty.AcademyId);

            var knownCoursesIds = queryKnownCourses.Select(crse => crse.BbCourseId).ToList();

            var reconResult = new CourseReconResult
            {
                Id = Guid.NewGuid(),
                AcademyId = Faculty?.AcademyId,
                Courses = new List<Course>()
            };

            foreach (var nc in bbCoursesResult
                .Where(bbc => !knownCoursesIds.Contains(bbc.id))
                .Select(bbc => new Course
                {
                    BbCourseId = bbc.id,
                    AcademyId = Faculty.AcademyId,
                    Name = bbc.name,
                    StartDate = DateTime.Now,
                    GradDate = DateTime.Now.AddDays(25)
                }))
            {
                reconResult.NumAdded += 1;
                reconResult.Courses.Add(nc);
                ctxManager.Context.Courses.Add(nc);
            }

            await ctxManager.Context.SaveChangesAsync();

            foreach (var course in reconResult.Courses)
            {
                course.ReconResultId = reconResult.Id;
            }

            return reconResult;
        }

        public async Task<Course> GetAllCourseMembers(int courseId)
        {
            var query = await ctxManager.Context.Courses
                 .Where(c => c.Id == courseId)
                 .Select(c => new
                 {
                     c,
                     Students = c.Students
                     .Where(sic => !sic.IsDeleted)
                     .Select(sic => new
                     {
                         sic,
                         sic.Student,
                         sic.Student.Person
                     }),
                     Faculty = c.Faculty.Where(fic => !fic.IsDeleted)
                     .Select(fic => new
                     {
                         fic,
                         fic.FacultyProfile,
                         fic.FacultyProfile.Person
                     })
                 })
                 .SingleAsync();

            return query.c;
        }

        public async Task<MemReconResult> ReconcileCourseMembers(int courseId)
        {
            await GetProfile();

            var ecatCourse = await ctxManager.Context.Courses
                .Where(crse => crse.Id == courseId)
                .Select(crse => new CourseReconcile
                {
                    Course = crse,
                    FacultyToReconcile = crse.Faculty.Select(fac => new UserReconcile
                    {
                        PersonId = fac.FacultyPersonId,
                        BbUserId = fac.FacultyProfile.Person.BbUserId,
                        CanDelete = !fac.FacSpComments.Any() &&
                        !fac.FacSpResponses.Any() &&
                        !fac.FacStratResponse.Any()
                    }).ToList(),
                    StudentsToReconcile = crse.Students.Select(sic => new UserReconcile
                    {
                        PersonId = sic.StudentPersonId,
                        BbUserId = sic.Student.Person.BbUserId,
                        CanDelete = !sic.WorkGroupEnrollments.Any()
                    }).ToList()
                }).SingleOrDefaultAsync();

            Contract.Assert(ecatCourse != null);

            var reconResult = new MemReconResult
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                AcademyId = Faculty?.AcademyId
            };

            var autoRetryCm = new Retrier<CourseMembershipVO[]>();

            var courseMemFilter = new MembershipFilter
            {
                filterTypeSpecified = true,
                filterType = (int)CrseMembershipFilterType.LoadByCourseId,
            };

            var bbCourseMems = await autoRetryCm.Try(() => bbWs.BbCourseMembership(ecatCourse.Course.BbCourseId, courseMemFilter), 3);

            var existingCrseUserIds = ecatCourse.FacultyToReconcile
                .Select(fac => fac.BbUserId).ToList();

            existingCrseUserIds.AddRange(ecatCourse.StudentsToReconcile.Select(sic => sic.BbUserId));

            var newMembers = bbCourseMems
                .Where(cm => !existingCrseUserIds.Contains(cm.userId))
                .Where(cm => cm.available == true)
                .ToList();

            if (newMembers.Any())
            {
                //var queryCr = await autoRetryCr.Try(client.getCourseRolesAsync(bbCourseMems.Select(bbcm => bbcm.roleId).ToArray()), 3);

                //var bbCourseRoles = queryCr.@return.ToList();
                reconResult = await AddNewUsers(newMembers, reconResult);
                reconResult.NumAdded = newMembers.Count();
            }

            var usersBbIdsToRemove = existingCrseUserIds.Where(ecu => !bbCourseMems.Select(cm => cm.userId).Contains(ecu)).ToList();

            if (usersBbIdsToRemove.Any())
            {
                reconResult.RemovedIds = await RemoveOrFlagUsers(ecatCourse, usersBbIdsToRemove);
                reconResult.NumRemoved = reconResult.RemovedIds.Count();
            }

            return reconResult;
        }

        private async Task<MemReconResult> AddNewUsers(IEnumerable<CourseMembershipVO> bbCmsVo, MemReconResult reconResult)
        {
            var bbCms = bbCmsVo.ToList();
            var bbCmUserIds = bbCms.Select(bbcm => bbcm.userId).ToList();

            var usersWithAccount = await ctxManager.Context.People
                .Where(p => bbCmUserIds.Contains(p.BbUserId))
                .Select(p => new
                {
                    p.BbUserId,
                    p.PersonId,
                    p.MpInstituteRole
                })
                .ToListAsync();

            var accountsNeedToCreate = bbCms.Where(cm => !usersWithAccount.Select(uwa => uwa.BbUserId).Contains(cm.userId)).ToList();

            if (accountsNeedToCreate.Any())
            {
                var userFilter = new UserFilter
                {
                    filterTypeSpecified = true,
                    filterType = (int)UserFilterType.UserByIdWithAvailability,
                    available = true,
                    availableSpecified = true,
                    id = accountsNeedToCreate.Select(bbAccount => bbAccount.userId).ToArray()
                };

                var autoRetryUsers = new Retrier<UserVO[]>();
                var bbUsers = await autoRetryUsers.Try(() => bbWs.BbCourseUsers(userFilter), 3);
                var courseMems = bbCms;

                if (bbUsers != null)
                {

                    var users = bbUsers.Select(bbu =>
                    {
                        var cm = courseMems.First(bbcm => bbcm.userId == bbu.id);
                        return new Person
                        {
                            MpInstituteRole = MpRoleTransform.BbWsRoleToEcat(cm.roleId),
                            BbUserId = bbu.id,
                            BbUserName = bbu.name,
                            Email = $"{cm.id}-{bbu.extendedInfo.emailAddress}",
                            IsActive = true,
                            LastName = bbu.extendedInfo.familyName,
                            FirstName = bbu.extendedInfo.givenName,
                            MpGender = MpGender.Unk,
                            MpAffiliation = MpAffiliation.Unk,
                            MpComponent = MpComponent.Unk,
                            RegistrationComplete = false,
                            MpPaygrade = MpPaygrade.Unk,
                            ModifiedById = Faculty.PersonId,
                            ModifiedDate = DateTime.Now
                        };
                    }).ToList();

                    foreach (var user in users)
                    {
                        ctxManager.Context.People.Add(user);

                    }

                    reconResult.NumOfAccountCreated = await ctxManager.Context.SaveChangesAsync();

                    foreach (var user in users)
                    {
                        usersWithAccount.Add(new
                        {
                            user.BbUserId,
                            user.PersonId,
                            user.MpInstituteRole
                        });

                        switch (user.MpInstituteRole)
                        {
                            case MpInstituteRoleId.Faculty:
                                user.Faculty = new ProfileFaculty
                                {
                                    PersonId = user.PersonId,
                                    AcademyId = reconResult.AcademyId,
                                    HomeStation = StaticAcademy.AcadLookupById[reconResult.AcademyId].Base.ToString(),
                                    IsReportViewer = false,
                                    IsCourseAdmin = false
                                };
                                break;
                            case MpInstituteRoleId.Student:
                                user.Student = new ProfileStudent
                                {
                                    PersonId = user.PersonId
                                };
                                break;
                            default:
                                user.Student = new ProfileStudent
                                {
                                    PersonId = user.PersonId
                                };
                                break;
                        }
                    }
                    await ctxManager.Context.SaveChangesAsync();
                }
            }

            reconResult.Students = usersWithAccount
                .Where(ecm => {
                    var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
                    return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) != MpInstituteRoleId.Faculty;
                })
                .Select(ecm => new StudentInCourse
                {
                    StudentPersonId = ecm.PersonId,
                    CourseId = reconResult.CourseId,
                    ReconResultId = reconResult.Id,
                    BbCourseMemId = bbCms.First(bbcm => bbcm.userId == ecm.BbUserId).id
                }).ToList();

            reconResult.Faculty = usersWithAccount
              .Where(ecm => {
                  var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
                  return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) == MpInstituteRoleId.Faculty;
              })
              .Select(ecm => new FacultyInCourse
              {
                  FacultyPersonId = ecm.PersonId,
                  CourseId = reconResult.CourseId,
                  ReconResultId = reconResult.Id,
                  BbCourseMemId = bbCms.First(bbcm => bbcm.userId == ecm.BbUserId).id
              }).ToList();

            var neededFacultyProfiles = usersWithAccount.Where(ecm =>
            {
                var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
                return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) == MpInstituteRoleId.Faculty;
            }).Select(ecm => ecm.PersonId);

            var existingFacultyProfiles = ctxManager.Context.Faculty
                .Where(fac => neededFacultyProfiles.Contains(fac.PersonId))
                .Select(fac => fac.PersonId);

            var newFacultyProfiles = neededFacultyProfiles
                .Where(id => !existingFacultyProfiles.Contains(id))
                .Select(id => new ProfileFaculty
                {
                    PersonId = id,
                    AcademyId = reconResult.AcademyId,
                    HomeStation = StaticAcademy.AcadLookupById[reconResult.AcademyId].Base.ToString(),
                    IsReportViewer = false,
                    IsCourseAdmin = false
                });

            var neededStudentProfiles = usersWithAccount.Where(ecm =>
            {
                var bbCM = bbCmsVo.First(cm => cm.userId == ecm.BbUserId);
                return MpRoleTransform.BbWsRoleToEcat(bbCM.roleId) != MpInstituteRoleId.Faculty;
            }).Select(ecm => ecm.PersonId);

            var existingStudentProfiles = ctxManager.Context.Students
                .Where(stud => neededStudentProfiles.Contains(stud.PersonId))
                .Select(stud => stud.PersonId);

            var newStudentProfiles = neededStudentProfiles
                .Where(id => !existingStudentProfiles.Contains(id))
                .Select(id => new ProfileStudent
                {
                    PersonId = id,
                });

            if (newFacultyProfiles.Any()) ctxManager.Context.Faculty.AddRange(newFacultyProfiles);
            if (newStudentProfiles.Any()) ctxManager.Context.Students.AddRange(newStudentProfiles);

            if (reconResult.Students.Any()) ctxManager.Context.StudentInCourses.AddRange(reconResult.Students);

            if (reconResult.Faculty.Any()) ctxManager.Context.FacultyInCourses.AddRange(reconResult.Faculty);

            reconResult.NumAdded = reconResult.Faculty.Any() || reconResult.Students.Any()
                ? await ctxManager.Context.SaveChangesAsync()
                : 0;

            return reconResult;
        }

        private async Task<List<int>> RemoveOrFlagUsers(CourseReconcile courseToReconcile, IEnumerable<string> bbIdsToRemove)
        {
            var idsRemoved = new List<int>();

            foreach (var studReconcile in courseToReconcile.StudentsToReconcile.Where(str => bbIdsToRemove.Contains(str.BbUserId)))
            {
                idsRemoved.Add(studReconcile.PersonId);
                var sic = new StudentInCourse
                {
                    StudentPersonId = studReconcile.PersonId,
                    CourseId = courseToReconcile.Course.Id
                };

                if (studReconcile.CanDelete)
                {
                    ctxManager.Context.Entry(sic).State = System.Data.Entity.EntityState.Deleted;
                }
                else
                {
                    sic.DeletedDate = DateTime.Now;
                    sic.DeletedById = Faculty.PersonId;
                    sic.IsDeleted = true;
                    ctxManager.Context.Entry(sic).State = System.Data.Entity.EntityState.Modified;
                }
            }

            foreach (var facReconcile in courseToReconcile.FacultyToReconcile.Where(str => bbIdsToRemove.Contains(str.BbUserId)))
            {
                idsRemoved.Add(facReconcile.PersonId);
                var fic = new FacultyInCourse
                {
                    FacultyPersonId = facReconcile.PersonId,
                    CourseId = courseToReconcile.Course.Id
                };

                if (facReconcile.CanDelete)
                {
                    ctxManager.Context.Entry(fic).State = System.Data.Entity.EntityState.Deleted;
                }
                else
                {
                    fic.DeletedDate = DateTime.Now;
                    fic.DeletedById = Faculty.PersonId;
                    fic.IsDeleted = true;
                    ctxManager.Context.Entry(fic).State = System.Data.Entity.EntityState.Modified;
                }
            }

            await ctxManager.Context.SaveChangesAsync();

            return idsRemoved;
        }
    }
}
