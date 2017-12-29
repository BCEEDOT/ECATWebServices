﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Ecat.Business.Guards;
using Breeze.Persistence;
using Breeze.Persistence.EF6;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Contexts;
using Ecat.Data.Models.Common;
using Ecat.Data.Models.User;
using Ecat.Data.Models.School;
using Ecat.Data.Static;
using Ecat.Business.BbWs.BbCourse;
using Ecat.Business.Utilities;
using Ecat.Business.BbWs.BbCourseMembership;
using Ecat.Business.BbWs.BbGradebook;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Ecat.Data.Models.Canvas;
using Ecat.Data.Models.Designer;

namespace Ecat.Business.Repositories
{
    using GroupMemberMap = Dictionary<GmrGroup, List<GroupMembershipVO>>;
    public class LmsAdminGroupRepo: ILmsAdminGroupRepo
    {
        private readonly EFPersistenceManager<EcatContext> ctxManager;
        private readonly BbWsCnet bbWs;
        //TODO: Update once we have production Canvas
        private readonly string canvasApiUrl = "https://ec2-34-215-69-52.us-west-2.compute.amazonaws.com/api/v1/";

        public int loggedInUserId { get; set; }
        private ProfileFaculty Faculty { get; set; }

        public LmsAdminGroupRepo(EcatContext mainCtx, BbWsCnet bbWsCnet)
        {
            ctxManager = new EFPersistenceManager<EcatContext>(mainCtx);
            bbWs = bbWsCnet;

            Faculty = ctxManager.Context.Faculty.Where(f => f.PersonId == loggedInUserId).SingleOrDefault();
        }

        public SaveResult SaveClientChanges(JObject saveBundle)
        {
            var guardian = new IsaGuard(ctxManager, loggedInUserId);
            ctxManager.BeforeSaveEntitiesDelegate += guardian.BeforeSaveEntities;
            return ctxManager.SaveChanges(saveBundle);
        }

        public async Task GetProfile()
        {
            if (Faculty == null || Faculty.PersonId == 0)
            {
                Faculty = await ctxManager.Context.Faculty
                .Where(fac => fac.PersonId == loggedInUserId)
                .SingleOrDefaultAsync();
            }
        }

        public async Task<Course> GetCourseGroups(int courseId)
        {
            var query = await ctxManager.Context.Courses
                .Where(crse => crse.Id == courseId)
                .Select(crse => new
                {
                    crse,
                    crse.WorkGroups,
                    Faculty = crse.Faculty
                        .Where(fic => crse.WorkGroups.Select(wg => wg.ModifiedById).Contains(fic.FacultyPersonId))
                        .Select(fic => new
                        {
                            fic,
                            fic.FacultyProfile,
                            fic.FacultyProfile.Person
                        })
                }).SingleAsync();

            var course = query.crse;
            course.Faculty = query.Faculty.Select(f => f.fic).ToList();
            return course;
        }

        public async Task<WorkGroup> GetWorkGroupMembers(int workGroupId)
        {
            var query = await ctxManager.Context.WorkGroups
                .Where(wg => wg.WorkGroupId == workGroupId)
                .Select(wg => new
                {
                    wg,
                    Gm = wg.GroupMembers.Where(gm => !gm.IsDeleted)
                        .Select(gm => new
                        {
                            gm,
                            gm.StudentProfile,
                            gm.StudentProfile.Person
                        })
                }).SingleAsync();

            var workGroup = query.wg;
            workGroup.GroupMembers = query.Gm.Select(g => g.gm).ToList();
            return workGroup;
        }


        public async Task<List<WorkGroup>> GetAllGroupSetMembers(int courseId, string categoryId)
        {
            var latestCourse = ctxManager.Context.Courses.Where(crse => crse.Id == courseId).Single();

            var workGroupsInCourse = await (from workGroup in ctxManager.Context.WorkGroups

                                            where workGroup.CourseId == latestCourse.Id && workGroup.MpCategory == categoryId

                                            select new
                                            {
                                                workGroup,
                                                //Return all GroupMembers even deleted. Schools must have the ability to keep a student in a course but 
                                                //not in a group.
                                                //GroupMembers = workGroup.GroupMembers.Where(gm => gm.IsDeleted == false).Select(gm => new
                                                GroupMembers = workGroup.GroupMembers.Select(gm => new
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
                //if (wg.GroupMembers.Any())
                //{
                    currentWorkgroups.Add(wg.workGroup);
                //}
            });

            return currentWorkgroups;
        }


        //lms web service stuff... Bb specific here
        public async Task<GroupReconResult> ReconcileGroups(int courseId)
        {
            await GetProfile();

            var ecatCourse = await ctxManager.Context.Courses
                .Where(course => course.Id == courseId)
                .Include(course => course.WorkGroups)
                .SingleAsync();

            var groupFilter = new GroupFilter
            {
                filterType = (int)GroupFilterType.GetGroupByCourseId,
                filterTypeSpecified = true,
                availableSpecified = true,
                available = true,
            };

            var autoRetry = new Retrier<GroupVO[]>();
            var bbGroups = await autoRetry.Try(() => bbWs.BbWorkGroups(ecatCourse.BbCourseId, groupFilter), 3);

            var courseGroups = ecatCourse.WorkGroups;

            var reconResult = new GroupReconResult
            {
                Id = Guid.NewGuid(),
                AcademyId = Faculty?.AcademyId,
                Groups = new List<WorkGroup>()
            };

            if (bbGroups == null) return reconResult;

            //var groupNeedToCreate = bbGroups
            //    .Where(bbg => !courseGroups.Select(wg => wg.BbGroupId).Contains(bbg.id))
            //    .Where(bbg => bbg.title.ToLower().StartsWith("bc")).ToList();

            //ECAT 2.0 only get BC1 groups from LMS
            var groupNeedToCreate = bbGroups
                .Where(bbg => !courseGroups.Select(wg => wg.BbGroupId).Contains(bbg.id))
                .Where(bbg => bbg.title.ToLower().StartsWith("bc1")).ToList();

            if (!groupNeedToCreate.Any()) return reconResult;

            var edLevel = StaticAcademy.AcadLookupById
                .First(acad => acad.Key == Faculty.AcademyId)
                .Value
                .MpEdLevel;

            var wgModels = await ctxManager.Context.WgModels
                .Where(wg => wg.IsActive && wg.MpEdLevel == edLevel).ToListAsync();

            var newGroups = groupNeedToCreate.Select(bbg =>
            {
                var groupMapper = bbg.title.Split('-');
                string category;
                switch (groupMapper[0])
                {
                    case "BC1":
                        category = MpGroupCategory.Wg1;
                        break;
                    //ECAT 2.0 only get BC1 groups from LMS
                    //case "BC2":
                    //    category = MpGroupCategory.Wg2;
                    //    break; ;
                    //case "BC3":
                    //    category = MpGroupCategory.Wg3;
                    //    break;
                    //case "BC4":
                    //    category = MpGroupCategory.Wg4;
                    //    break;
                    default:
                        category = MpGroupCategory.None;
                        break;
                }
                return new WorkGroup
                {
                    BbGroupId = bbg.id,
                    CourseId = ecatCourse.Id,
                    MpCategory = category,
                    GroupNumber = groupMapper[1].Substring(1),
                    DefaultName = groupMapper[2],
                    MpSpStatus = MpSpStatus.Created,
                    ModifiedById = Faculty.PersonId,
                    ModifiedDate = DateTime.Now,
                    ReconResultId = reconResult.Id,
                    AssignedSpInstrId = wgModels.FindLast(wgm => wgm.MpWgCategory == category).AssignedSpInstrId,
                    WgModel = wgModels.First(wgm => wgm.MpWgCategory == category)
                };
            });

            foreach (var grp in newGroups)
            {
                reconResult.NumAdded += 1;
                reconResult.Groups.Add(grp);
                ctxManager.Context.WorkGroups.Add(grp);
            }
            await ctxManager.Context.SaveChangesAsync();
            return reconResult;
        }

        public async Task<GroupReconResult> PollCanvasGroups(int crseId)
        {
            var course = await ctxManager.Context.Courses.Where(c => c.Id == crseId)
                .Include(c => c.Students)
                .Include(c => c.WorkGroups)
                .SingleAsync();
            if (course == null) { return null; }
            var academy = StaticAcademy.AcadLookupById[course.AcademyId];
            var workGroupModel = await ctxManager.Context.WgModels.Where(wgm => wgm.IsActive && wgm.MpEdLevel == academy.MpEdLevel && wgm.MpWgCategory == MpGroupCategory.Wg1).SingleAsync();

            var canvasLogin = await ctxManager.Context.CanvasLogins.Where(cl => cl.PersonId == loggedInUserId).SingleOrDefaultAsync();

            if (canvasLogin.AccessToken == null)
            {
                return null;
            }

            var reconResult = new GroupReconResult
            {
                Id = Guid.NewGuid(),
                AcademyId = Faculty?.AcademyId,
                Groups = new List<WorkGroup>()
            };

            var client = new HttpClient();
            var apiAddr = new Uri(canvasApiUrl + "courses/" + course.BbCourseId + "/groups?per_page=1000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + canvasLogin.AccessToken);

            var response = await client.GetAsync(apiAddr);

            var apiResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var groupsReturned = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasGroup>>(apiResponse);

                groupsReturned.ForEach(gr =>
                {
                    var existingGroup = course.WorkGroups.Where(grp => grp.BbGroupId == gr.id.ToString()).Single();
                    if (existingGroup == null)
                    {
                        var newGroup = new WorkGroup()
                        {
                            BbGroupId = gr.id.ToString(),
                            DefaultName = gr.name,
                            CourseId = crseId,
                            WgModelId = workGroupModel.Id,
                            AssignedSpInstrId = workGroupModel.AssignedSpInstrId,
                            MpCategory = MpGroupCategory.Wg1,
                            MpSpStatus = MpSpStatus.Created,
                            ModifiedById = loggedInUserId,
                            ModifiedDate = DateTime.Now,
                            ReconResultId = reconResult.Id
                        };

                        var nameNum = gr.name.Split(' ')[1];
                        if (!nameNum.StartsWith("0") && nameNum.Length == 1)
                        {
                            nameNum = "0" + nameNum;
                        }
                        newGroup.GroupNumber = nameNum;

                        ctxManager.Context.WorkGroups.Add(newGroup);
                        reconResult.Groups.Add(newGroup);
                    }
                });

                await ctxManager.Context.SaveChangesAsync();
            }

            return reconResult;
        }

        public async Task<List<GroupMemReconResult>> PollCanvasGroupMems (int crseId)
        {
            var course = await ctxManager.Context.Courses.Where(c => c.Id == crseId)
                .Include(c => c.Students)
                .Include(c => c.WorkGroups)
                .SingleAsync();
            if (course == null) { return null; }

            var canvasLogin = await ctxManager.Context.CanvasLogins.Where(cl => cl.PersonId == loggedInUserId).SingleOrDefaultAsync();

            if (canvasLogin.AccessToken == null)
            {
                return null;
            }

            var results = new List<GroupMemReconResult>();

            var client = new HttpClient();
            var apiAddr = new Uri(canvasApiUrl + "courses/" + course.BbCourseId + "/enrollments?include[]=groupd_ids&per_page=1000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + canvasLogin.AccessToken);

            var response = await client.GetAsync(apiAddr);

            var apiResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var enrollmentsReturned = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasEnrollment>>(apiResponse);

                var groupsWithMems = await ctxManager.Context.WorkGroups.Where(grp => grp.CourseId == course.Id && grp.MpCategory == MpGroupCategory.Wg1).Include(grp => grp.GroupMembers).ToListAsync();

                var studentIds = course.Students.Select(sic => sic.StudentPersonId).ToList();
                var students = await ctxManager.Context.People.Where(p => studentIds.Contains(p.PersonId)).ToListAsync();

                var remLmsGrpMemIds = new List<string>();

                groupsWithMems.ForEach(grp =>
                {
                    var grpMemRecon = new GroupMemReconResult()
                    {
                        CourseId = course.Id,
                        Id = new Guid(),
                        WorkGroupId = grp.WorkGroupId,
                        WorkGroupName = grp.DefaultName,
                        GroupType = grp.MpCategory
                    };

                    var returnedMems = enrollmentsReturned.Where(er => er.user.group_ids.Count == 1 && er.user.group_ids.First().ToString() == grp.BbGroupId).ToList();
                    var retMemIds = new List<string>();

                    returnedMems.ForEach(rm =>
                    {
                        retMemIds.Add(rm.id.ToString());
                        var memExists = grp.GroupMembers.Where(gm => gm.BbCrseStudGroupId == rm.id.ToString()).Single();

                        if (memExists == null)
                        {
                            var stuPerson = students.Where(s => s.BbUserId == rm.user_id.ToString()).Single();

                            var grpMembership = new CrseStudentInGroup()
                            {
                                CourseId = grp.CourseId,
                                WorkGroupId = grp.WorkGroupId,
                                StudentId = stuPerson.PersonId,
                                ModifiedById = loggedInUserId,
                                ModifiedDate = DateTime.Now
                            };

                            ctxManager.Context.StudentInGroups.Add(grpMembership);
                            grpMemRecon.NumAdded++;
                        }
                        else
                        {
                            if (memExists.IsDeleted)
                            {
                                memExists.IsDeleted = false;
                                ctxManager.Context.Entry(memExists).State = System.Data.Entity.EntityState.Modified;
                                grpMemRecon.NumAdded++;
                            }
                        }
                    });

                    var memNotReturned = grp.GroupMembers.Where(gm => !gm.IsDeleted && !retMemIds.Contains(gm.BbCrseStudGroupId)).ToList();

                    if (memNotReturned.Any())
                    {
                        //memNotReturned.ForEach(mr =>
                        //{
                            //mr.IsDeleted = true;
                            //ctxManager.Context.Entry(mr).State = System.Data.Entity.EntityState.Modified;
                            //grpMemRecon.NumRemoved++;
                        //});

                        remLmsGrpMemIds.AddRange(memNotReturned.Select(csig => csig.BbCrseStudGroupId).ToList());
                    }

                    if (grpMemRecon.NumAdded > 0 || grpMemRecon.NumRemoved > 0)
                    {
                        results.Add(grpMemRecon);
                    }
                });

                if (remLmsGrpMemIds.Any())
                {
                    //group memberships are always deleted in 2.0
                    var remMemsWithChildren = await ctxManager.Context.StudentInGroups.Where(csig => remLmsGrpMemIds.Contains(csig.BbCrseStudGroupId) && !csig.IsDeleted)
                        .Include(csig => csig.WorkGroup)
                        .Include(csig => csig.AssesseeSpResponses)
                        .Include(csig => csig.AssesseeStratResponse)
                        .Include(csig => csig.AssessorSpResponses)
                        .Include(csig => csig.AssessorStratResponse)
                        .Include(csig => csig.AuthorOfComments)
                        .Include(csig => csig.RecipientOfComments)
                        .Include(csig => csig.FacultyComment)
                        .Include(csig => csig.FacultySpResponses)
                        .Include(csig => csig.FacultyStrat)
                        .ToListAsync();
                    var remStuIdList = remMemsWithChildren.Select(mem => mem.StudentId).ToList();
                    //var commFlags = await ctxManager.Context.StudSpCommentFlag.Where(flag => groupIdList.Contains(flag.WorkGroupId) && (studIdList.Contains(flag.AuthorPersonId) || studIdList.Contains(flag.RecipientPersonId))).ToListAsync();
                    var authorFlags = await ctxManager.Context.StudSpCommentFlag.Where(flag => flag.WorkGroupId == remMemsWithChildren[0].WorkGroupId && remStuIdList.Contains(flag.AuthorPersonId)).ToListAsync();
                    var recipFlags = await ctxManager.Context.StudSpCommentFlag.Where(flag => flag.WorkGroupId == remMemsWithChildren[0].WorkGroupId && remStuIdList.Contains(flag.RecipientPersonId)).ToListAsync();
                    var facFlags = await ctxManager.Context.facSpCommentsFlag.Where(flag => flag.WorkGroupId == remMemsWithChildren[0].WorkGroupId && remStuIdList.Contains(flag.RecipientPersonId)).ToListAsync();

                    remMemsWithChildren.ForEach(gm =>
                    {
                        if (gm.WorkGroup.MpSpStatus != MpSpStatus.Published)
                        {
                            if (gm.AssesseeSpResponses.Any()) { ctxManager.Context.SpResponses.RemoveRange(gm.AssesseeSpResponses); }
                            if (gm.AssesseeStratResponse.Any()) { ctxManager.Context.SpStratResponses.RemoveRange(gm.AssesseeStratResponse); }
                            if (gm.AssessorSpResponses.Any()) { ctxManager.Context.SpResponses.RemoveRange(gm.AssessorSpResponses); }
                            if (gm.AssessorStratResponse.Any()) { ctxManager.Context.SpStratResponses.RemoveRange(gm.AssessorStratResponse); }

                            if (gm.AuthorOfComments.Any())
                            {
                                var gmAuthorFlags = authorFlags.Where(flag => flag.AuthorPersonId == gm.StudentId && flag.WorkGroupId == gm.WorkGroupId).ToList();
                                ctxManager.Context.StudSpCommentFlag.RemoveRange(gmAuthorFlags);
                                ctxManager.Context.StudSpComments.RemoveRange(gm.AuthorOfComments);
                            }
                            if (gm.RecipientOfComments.Any())
                            {
                                var gmRecipFlags = recipFlags.Where(flag => flag.RecipientPersonId == gm.StudentId && flag.WorkGroupId == gm.WorkGroupId).ToList();
                                ctxManager.Context.StudSpCommentFlag.RemoveRange(gmRecipFlags);
                                ctxManager.Context.StudSpComments.RemoveRange(gm.RecipientOfComments);
                            }

                            if (gm.FacultyComment != null)
                            {
                                var gmFacFlag = facFlags.Where(flag => flag.RecipientPersonId == gm.StudentId && flag.WorkGroupId == gm.WorkGroupId).Single();
                                ctxManager.Context.facSpCommentsFlag.Remove(gmFacFlag);
                                ctxManager.Context.FacSpComments.Remove(gm.FacultyComment);
                            }

                            if (gm.FacultySpResponses.Any()) { ctxManager.Context.FacSpResponses.RemoveRange(gm.FacultySpResponses); }
                            if (gm.FacultyStrat != null) { ctxManager.Context.FacStratResponses.Remove(gm.FacultyStrat); }


                            ctxManager.Context.StudentInGroups.Remove(gm);

                            var recon = results.Where(res => res.WorkGroupId == gm.WorkGroupId).Single();
                            recon.NumRemoved++;
                        }
                    });
                }

                await ctxManager.Context.SaveChangesAsync();
            }

            return results;
        }

        public async Task<GroupReconResult> PushCanvasSections(int crseId)
        {
            //incomplete, not sure if we are going to do this
            var course = await ctxManager.Context.Courses.Where(c => c.Id == crseId).SingleAsync();
            if (course == null) { return null; }
            var academy = StaticAcademy.AcadLookupById[course.AcademyId];

            var canvasLogin = await ctxManager.Context.CanvasLogins.Where(cl => cl.PersonId == loggedInUserId).SingleOrDefaultAsync();

            if (canvasLogin.AccessToken == null)
            {
                return null;
            }

            var reconResult = new GroupReconResult
            {
                Id = Guid.NewGuid(),
                AcademyId = Faculty?.AcademyId,
                Groups = new List<WorkGroup>()
            };

            var client = new HttpClient();
            var apiAddr = new Uri(canvasApiUrl + "courses/" + course.BbCourseId + "/sections?include[]=students&per_page=1000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + canvasLogin.AccessToken);

            var response = await client.GetAsync(apiAddr);

            var apiResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var sectionsReturned = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasSection>>(apiResponse);

                var groupsWithMems = await ctxManager.Context.WorkGroups.Where(grp => grp.CourseId == crseId && grp.MpCategory == MpGroupCategory.Wg1 && grp.GroupMembers.Any()).Include(grp => grp.GroupMembers).ToListAsync();
                var studentIds = new List<int>();
                groupsWithMems.ForEach(grp => {
                    grp.GroupMembers.ToList().ForEach(gm => {
                        studentIds.Add(gm.StudentId);
                    });
                });

                var students = ctxManager.Context.People.Where(p => studentIds.Contains(p.PersonId)).ToListAsync();

                groupsWithMems.ForEach(grp =>
                {
                    var retSection = sectionsReturned.Where(sr => sr.name == grp.DefaultName).First();
                    if (retSection == null)
                    {
                        var newSection = new CanvasSection()
                        {
                            name = grp.DefaultName
                        };
                    }
                });
            }

            return reconResult;
        }

        public async Task<GroupMemReconResult> ReconcileGroupMembers(int wgId)
        {

            var crseWithWorkgroup = await ctxManager.Context.Courses
                .Where(crse => crse.WorkGroups.Any(wg => wg.WorkGroupId == wgId))
                .Select(crse => new GroupMemberReconcile
                {
                    CrseId = crse.Id,
                    BbCrseId = crse.BbCourseId,
                    WorkGroups = crse.WorkGroups
                    .Where(wg => wg.MpSpStatus != MpSpStatus.Published)
                    .Where(wg => wg.WorkGroupId == wgId).Select(wg => new GmrGroup
                    {
                        WgId = wg.WorkGroupId,
                        BbWgId = wg.BbGroupId,
                        Category = wg.MpCategory,
                        Name = wg.DefaultName,
                        Members = wg.GroupMembers.Select(gm => new GmrMember
                        {
                            StudentId = gm.StudentId,
                            BbGroupMemId = gm.BbCrseStudGroupId,
                            BbCrseMemId = gm.StudentInCourse.BbCourseMemId,
                            IsDeleted = gm.IsDeleted,
                            HasChildren = gm.AuthorOfComments.Any() ||
                                          gm.AssesseeSpResponses.Any() ||
                                          gm.AssessorSpResponses.Any() ||
                                          gm.AssesseeStratResponse.Any() ||
                                          gm.AssessorStratResponse.Any() ||
                                          gm.RecipientOfComments.Any()
                        }).ToList()
                    }).ToList()
                })
                .FirstAsync();

            var reconResults = await DoReconciliation(crseWithWorkgroup);

            return reconResults.SingleOrDefault();
        }

        public async Task<List<GroupMemReconResult>> ReconcileAllGroupMembers(int courseId)//, string groupCategory)
        {
            //ECAT 2.0 only get BC1 groups from LMS
            var crseWithWorkgroup = await ctxManager.Context.Courses
                .Where(crse => crse.Id == courseId)
                .Select(crse => new GroupMemberReconcile
                {
                    CrseId = crse.Id,
                    BbCrseId = crse.BbCourseId,
                    WorkGroups = crse.WorkGroups
                    .Where(wg => wg.MpSpStatus != MpSpStatus.Published)
                    .Where(wg => wg.MpCategory == MpGroupCategory.Wg1)
                        .Select(wg => new GmrGroup
                        {
                            WgId = wg.WorkGroupId,
                            BbWgId = wg.BbGroupId,
                            Category = wg.MpCategory,
                            Name = wg.DefaultName,
                            Members = wg.GroupMembers.Select(gm => new GmrMember
                            {
                                StudentId = gm.StudentId,
                                BbGroupMemId = gm.BbCrseStudGroupId,
                                BbCrseMemId = gm.StudentInCourse.BbCourseMemId,
                                IsDeleted = gm.IsDeleted,
                                HasChildren = gm.AuthorOfComments.Any() ||
                                              gm.AssesseeSpResponses.Any() ||
                                              gm.AssessorSpResponses.Any() ||
                                              gm.AssesseeStratResponse.Any() ||
                                              gm.AssessorStratResponse.Any() ||
                                              gm.RecipientOfComments.Any()
                            }).ToList()
                        }).ToList()
                })
                .FirstAsync();

            return await DoReconciliation(crseWithWorkgroup);
        }

        private async Task<List<GroupMemReconResult>> DoReconciliation(GroupMemberReconcile crseGroupToReconcile)
        {
            await GetProfile();
            //if (crseGroupToReconcile.WorkGroups == null || crseGroupToReconcile.WorkGroups.Count == 0)
            //{
            //    return null;
            //}

            var allGroupBbIds = crseGroupToReconcile.WorkGroups.Select(wg => wg.BbWgId).ToArray();

            var autoRetry = new Retrier<GroupMembershipVO[]>();
            var filter = new MembershipFilter
            {
                filterTypeSpecified = true,
                filterType = (int)GrpMemFilterType.LoadByGrpId,
                groupIds = allGroupBbIds
            };

            var allBbGrpMems = await autoRetry.Try(() => bbWs.BbGroupMembership(crseGroupToReconcile.BbCrseId, filter), 3);

            var wgsWithChanges = new Dictionary<GmrGroup, List<GroupMembershipVO>>();

            var wgsWithoutBbMems = new List<GmrGroup>(crseGroupToReconcile.WorkGroups);

            if (allBbGrpMems != null)
            {
                foreach (var grpMem in allBbGrpMems.GroupBy(bbgm => bbgm.groupId))
                {
                    var relatedWg = crseGroupToReconcile.WorkGroups.Single(wg => wg.BbWgId == grpMem.Key);
                    wgsWithoutBbMems.Remove(relatedWg);

                    var existingCrseMemBbIds = relatedWg.Members
                        .Where(mem => !mem.IsDeleted)
                        .Select(mem => mem.BbCrseMemId)
                        .ToList();

                    var changeGrpMem = grpMem.Where(gm => !existingCrseMemBbIds.Contains(gm.courseMembershipId))
                        .Select(gm => gm)
                        .ToList();

                    var currentBbGmIds = grpMem.Select(gm => gm.courseMembershipId);
                    var removedGroupMem = existingCrseMemBbIds.Where(ecmid => !currentBbGmIds.Contains(ecmid)).ToList();

                    if (!changeGrpMem.Any() && !removedGroupMem.Any()) continue;

                    relatedWg.ReconResult = new GroupMemReconResult
                    {
                        Id = Guid.NewGuid(),
                        WorkGroupId = relatedWg.WgId,
                        AcademyId = Faculty.AcademyId,
                        GroupType = relatedWg.Category,
                        WorkGroupName = relatedWg.Name,
                        CourseId = crseGroupToReconcile.CrseId,
                        GroupMembers = new List<CrseStudentInGroup>()
                    };

                    foreach (var gm in removedGroupMem.Select(rgmId => relatedWg.Members.Single(mem => mem.BbCrseMemId == rgmId)))
                    {
                        gm.PendingRemoval = true;
                    }

                    wgsWithChanges.Add(relatedWg, changeGrpMem);
                }
            }

            if (wgsWithoutBbMems.Any())
            {
                foreach (var grp in wgsWithoutBbMems)
                {
                    var activeMems = grp.Members.Where(mem => !mem.IsDeleted);
                    if (activeMems.Any())
                    {
                        grp.ReconResult = new GroupMemReconResult
                        {
                            Id = Guid.NewGuid(),
                            WorkGroupId = grp.WgId,
                            AcademyId = Faculty.AcademyId,
                            GroupType = grp.Category,
                            WorkGroupName = grp.Name,
                            CourseId = crseGroupToReconcile.CrseId,
                            GroupMembers = new List<CrseStudentInGroup>()
                        };

                        foreach (var gm in activeMems)
                        {
                            gm.PendingRemoval = true;
                        }

                        wgsWithChanges.Add(grp, new List<GroupMembershipVO>());
                    }
                }
            }

            if (wgsWithChanges.Any(wgwc => wgwc.Value.Any()))
                wgsWithChanges = await AddGroupMembers(crseGroupToReconcile.CrseId, wgsWithChanges);

            if (wgsWithChanges.SelectMany(wg => wg.Key.Members).Any(gm => gm.PendingRemoval))
                wgsWithChanges = await RemoveOrFlag(crseGroupToReconcile.CrseId, wgsWithChanges);

            if (wgsWithChanges.Any())
            {
                var ids = wgsWithChanges.Select(wg => wg.Key.WgId);
                var svrWgMembers = await (from wg in ctxManager.Context.WorkGroups
                                          where ids.Contains(wg.WorkGroupId)
                                          select new
                                          {
                                              wg,
                                              GroupMembers = wg.GroupMembers.Where(gm => !gm.IsDeleted).Select(gm => new
                                              {
                                                  gm,
                                                  gm.StudentProfile,
                                                  gm.StudentProfile.Person
                                              })
                                          }).ToListAsync();

                foreach (var wg in wgsWithChanges)
                {
                    wg.Key.ReconResult.GroupMembers = svrWgMembers.Single(swg => swg.wg.WorkGroupId == wg.Key.WgId)
                            .GroupMembers.Select(gm => gm.gm).ToList();
                }
            }

            var reconResults = wgsWithChanges.Select(wg => wg.Key.ReconResult).ToList();

            return reconResults;
        }

        private async Task<GroupMemberMap> AddGroupMembers(int crseId, GroupMemberMap grpsWithMemToAdd)
        {
            //Deal with members that were previously removed from the group, flagged as deleted and then added
            //back to the group

            var studentsToReactivate = new List<CrseStudentInGroup>();
            var reActivateStudIds = new List<string>();

            foreach (var gmrGroup in grpsWithMemToAdd)
            {
                var newMemberCrseIds = gmrGroup.Value.Select(gmvo => gmvo.courseMembershipId);

                var exisitingMembersWithDeletedFlag = gmrGroup.Key.Members
                    .Where(mem => newMemberCrseIds.Contains(mem.BbCrseMemId) && mem.IsDeleted)
                    .ToList();

                if (!exisitingMembersWithDeletedFlag.Any()) continue;

                gmrGroup.Key.ReconResult.NumAdded += exisitingMembersWithDeletedFlag.Count;

                var studentsInGroup = exisitingMembersWithDeletedFlag.Select(emwdg => new CrseStudentInGroup
                {
                    StudentId = emwdg.StudentId,
                    CourseId = crseId,
                    WorkGroupId = gmrGroup.Key.WgId,
                    BbCrseStudGroupId = emwdg.BbGroupMemId,
                    IsDeleted = false,
                    DeletedDate = null,
                    DeletedById = null,
                    ModifiedById = Faculty.PersonId,
                    ModifiedDate = DateTime.Now
                });

                studentsToReactivate.AddRange(studentsInGroup);
                reActivateStudIds.AddRange(exisitingMembersWithDeletedFlag.Select(emwdg => emwdg.BbCrseMemId));
            }

            if (studentsToReactivate.Any())
            {
                foreach (var str in studentsToReactivate)
                {
                    ctxManager.Context.Entry(str).State = System.Data.Entity.EntityState.Modified;
                }

                await ctxManager.Context.SaveChangesAsync();
            }

            var studentToLookUp = grpsWithMemToAdd
                .SelectMany(gm => gm.Value)
                .Where(gm => !reActivateStudIds.Contains(gm.courseMembershipId))
                .Select(gm => gm.courseMembershipId)
                .Distinct();

            var students = await ctxManager.Context.StudentInCourses
                .Where(stud => stud.CourseId == crseId)
                .Where(stud => studentToLookUp.Contains(stud.BbCourseMemId))
                .Where(stud => !stud.IsDeleted)
                .Select(stud => new
                {
                    stud.BbCourseMemId,
                    stud.Student.Person.BbUserId,
                    stud.Student.PersonId
                })
                .ToListAsync();

            var additions = new List<CrseStudentInGroup>();
            foreach (var gwmta in grpsWithMemToAdd)
            {
                foreach (var studInGroup in
                    from memVo in gwmta.Value
                    let relatedStudent =
                        students.SingleOrDefault(stud => stud.BbCourseMemId == memVo.courseMembershipId)
                    where relatedStudent != null
                    select new CrseStudentInGroup
                    {
                        StudentId = relatedStudent.PersonId,
                        CourseId = crseId,
                        WorkGroupId = gwmta.Key.WgId,
                        HasAcknowledged = false,
                        BbCrseStudGroupId = memVo.groupMembershipId,
                        IsDeleted = false,
                        ModifiedById = Faculty?.PersonId,
                        ModifiedDate = DateTime.Now,
                        ReconResultId = gwmta.Key.ReconResult.Id
                    })
                {
                    additions.Add(studInGroup);
                    gwmta.Key.ReconResult.NumAdded += 1;
                    gwmta.Key.ReconResult.GroupMembers.Add(studInGroup);
                }
            }
            ctxManager.Context.StudentInGroups.AddRange(additions);
            await ctxManager.Context.SaveChangesAsync();

            return grpsWithMemToAdd;
        }

        private async Task<GroupMemberMap> RemoveOrFlag(int crseId, GroupMemberMap grpWithMems)
        {
            foreach (var group in grpWithMems.Keys)
            {
                var gmsPendingRemovalWithChildren = group.Members.Where(mem => mem.PendingRemoval && mem.HasChildren).Select(mem => mem.StudentId).ToList();

                if (gmsPendingRemovalWithChildren.Any())
                {
                    var existingStudToFlag =
                        await ctxManager.Context.StudentInGroups.Where(
                            sig =>
                                gmsPendingRemovalWithChildren.Contains(sig.StudentId) && sig.WorkGroupId == group.WgId)
                            .ToListAsync();

                    foreach (var sig in existingStudToFlag)
                    {
                        sig.IsDeleted = true;
                        sig.DeletedById = Faculty?.PersonId;
                        sig.DeletedDate = DateTime.Now;
                        sig.ModifiedById = Faculty?.PersonId;
                        sig.ModifiedDate = DateTime.Now;
                    }
                }

                foreach (
                    var gmrMember in
                        group.Members.Where(mem => mem.PendingRemoval && !mem.HasChildren)
                            .Select(mem => new CrseStudentInGroup
                            {
                                WorkGroupId = group.WgId,
                                CourseId = crseId,
                                StudentId = mem.StudentId,
                            }))
                {
                    ctxManager.Context.Entry(gmrMember).State = System.Data.Entity.EntityState.Deleted;
                }

                group.ReconResult.NumRemoved = group.Members.Count(mem => mem.PendingRemoval);
            }
            await ctxManager.Context.SaveChangesAsync();
            return grpWithMems;
        }

        public async Task<List<GroupMemReconResult>> PollCanvasSections(int crseId)
        {
            //incomplete, but I'm leaving all this in for now, because if the new SIS ever happens we might have to get sections

            var course = await ctxManager.Context.Courses.Where(c => c.Id == crseId)
                .Include(c => c.Students)
                .SingleAsync();
            if (course == null) { return null; }
            var academy = StaticAcademy.AcadLookupById[course.AcademyId];
            var workGroupModel = await ctxManager.Context.WgModels.Where(wgm => wgm.IsActive && wgm.MpEdLevel == academy.MpEdLevel && wgm.MpWgCategory == MpGroupCategory.Wg1).SingleAsync();

            var canvasLogin = await ctxManager.Context.CanvasLogins.Where(cl => cl.PersonId == loggedInUserId).SingleOrDefaultAsync();

            if (canvasLogin.AccessToken == null)
            {
                return null;
            }

            var results = new List<GroupMemReconResult>();

            var client = new HttpClient();
            var apiAddr = new Uri(canvasApiUrl + "courses/" + course.BbCourseId + "/sections?include[]=students&per_page=1000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + canvasLogin.AccessToken);

            var response = await client.GetAsync(apiAddr);

            var apiResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var sectionsReturned = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasSection>>(apiResponse);

                if (sectionsReturned == null || sectionsReturned.Count == 0)
                {
                    return results;
                }
                else
                {
                    //var returnedSectionIds = new List<string>();
                    //sectionsReturned.ForEach(sr => returnedSectionIds.Add(sr.id.ToString()));
                    var courseEnrollPersonIds = new List<int>();

                    course.Students.ToList().ForEach(cs => courseEnrollPersonIds.Add(cs.StudentPersonId));

                    var existingGroups = await ctxManager.Context.WorkGroups.Where(wg => wg.CourseId == crseId)
                        .Include(wg => wg.GroupMembers)
                        .ToListAsync();

                    var existingGroupLMSIds = new List<string>();
                    existingGroups.ForEach(g => {
                        existingGroupLMSIds.Add(g.BbGroupId);
                        //g.GroupMembers.ToList().ForEach(gm => studentsInGroupsIds.Add(gm.StudentId));
                    });
                    var newGroups = new List<CanvasSection>();
                    sectionsReturned.ForEach(sr =>
                    {
                        if (!existingGroupLMSIds.Contains(sr.id.ToString()))
                        {
                            newGroups.Add(sr);
                        }
                    });

                    var createdGroups = await AddCanvasGroups(newGroups, course.Id, workGroupModel);

                    var courseEnrolls = new List<Person>();
                    if (courseEnrollPersonIds.Count > 0)
                    {
                        courseEnrolls = await ctxManager.Context.People.Where(p => courseEnrollPersonIds.Contains(p.PersonId)).ToListAsync();
                    }
                    var courseEnrollLMSIds = new List<string>();
                    courseEnrolls.ForEach(ce => courseEnrollLMSIds.Add(ce.BbUserId));




                    
                    sectionsReturned.ForEach(sec =>
                    {
                        if (existingGroupLMSIds.Contains(sec.id.ToString()))
                        {
                            var groupRecord = existingGroups.Where(g => g.BbGroupId == sec.id.ToString()).Single();

                            if (groupRecord.GroupMembers.Count > 0)
                            {
                                var grpMemList = groupRecord.GroupMembers.ToList();
                                var groupMemIds = new List<int>();
                                grpMemList.ForEach(gm => groupMemIds.Add(gm.StudentId));

                                var groupMemPersons = courseEnrolls.Where(ce => {
                                    if (groupMemIds.Contains(ce.PersonId)) { return true; }
                                    return false;
                                });

                                var returnedGrpStuds = sec.students.ToList();
                                

                                
                                returnedGrpStuds.ForEach(rgs =>
                                {
                                    var existingMem = groupMemPersons.Where(gmp => gmp.BbUserId == rgs.id.ToString()).Single();
                                    
                                    if (existingMem == null)
                                    {
                                        //var user = courseEnrolls.Where()
                                        var newCSIG = new CrseStudentInGroup()
                                        {
                                            CourseId = course.Id,
                                            WorkGroupId = groupRecord.WorkGroupId
                                        };

                                    }
                                });
                            }
                        }
                    });
                }
            }



            return results;
        }

        public async Task<List<WorkGroup>> AddCanvasGroups(List<CanvasSection> sections, int crseId, WorkGroupModel model)
        {
            var newGroups = new List<WorkGroup>();

            sections.ForEach(sec => {
                var group = new WorkGroup()
                {
                    CourseId = crseId,
                    BbGroupId = sec.id.ToString(),
                    DefaultName = sec.name,
                    WgModelId = model.Id,
                    ModifiedById = loggedInUserId,
                    ModifiedDate = DateTime.Now,
                    AssignedSpInstrId = model.AssignedSpInstrId,
                    MpCategory = MpGroupCategory.Wg1,
                    MpSpStatus = MpSpStatus.Created
                    //TODO: figure out how to figure out group number (will it be in name somewhere?)
                    //GroupNumber = 
                };

                newGroups.Add(group);
            });

            ctxManager.Context.WorkGroups.AddRange(newGroups);
            var saved = await ctxManager.Context.SaveChangesAsync();

            return newGroups;
        }

        public async Task<SaveGradeResult> SyncBbGrades(int crseId, string wgCategory)
        {

            var result = new SaveGradeResult()
            {
                CourseId = crseId,
                WgCategory = wgCategory,
                Success = false,
                SentScores = 0,
                ReturnedScores = 0,
                NumOfStudents = 0,
                Message = "Bb Gradebook Sync"
            };

            var groups = await ctxManager.Context.WorkGroups.Where(wg => wg.CourseId == crseId && wg.MpCategory == wgCategory)
                .Include(wg => wg.WgModel)
                .Include(wg => wg.Course)
                .Include(wg => wg.GroupMembers)
                .ToListAsync();

            var unusedGrps = groups.Where(grp => !grp.GroupMembers.Any()).ToList();
            var usedGrps = groups.Where(grp => grp.GroupMembers.Any()).ToList();

            if (!usedGrps.Any())
            {
                result.Success = false;
                result.Message = "No flights with enrolled students found ";
                return result;
            }

            var grpIds = new List<int>();
            foreach (var group in usedGrps)
            {
                if (group.MpSpStatus != MpSpStatus.Published)
                {
                    var notDeleted = group.GroupMembers.Where(mem => mem.IsDeleted == false).Count();
                    if (notDeleted > 0)
                    {
                        result.Success = false;
                        result.Message = "All flights with enrollments must be published to sync grades";
                        return result;
                    }
                    else { continue; }
                }
                grpIds.Add(group.WorkGroupId);
            }

            var bbCrseId = usedGrps[0].Course.BbCourseId;

            var model = usedGrps[0].WgModel;
            if (model.StudStratCol == null)
            {
                result.Success = false;
                result.Message = "Error matching ECAT and LMS Columns";
                return result;
            }

            string[] name = { model.StudStratCol };

            //var columnFilter = new ColumnFilter
            //{
            //    filterType = (int)ColumnFilterType.GetColumnByCourseIdAndColumnName,
            //    filterTypeSpecified = true,
            //    names = name
            //};

            var columnFilter = new ColumnFilter
            {
                filterType = (int)ColumnFilterType.GetColumnByCourseId,
                filterTypeSpecified = true
            };

            var columns = new List<ColumnVO>();
            var autoRetry = new Retrier<ColumnVO[]>();
            var wsColumn = await autoRetry.Try(() => bbWs.BbColumns(bbCrseId, columnFilter), 3);

            //if (wsColumn[0] == null || wsColumn.Length > 1)
            //{
            //    result.Success = false;
            //    result.Message = "Error matching ECAT and LMS Columns";
            //    return result;
            //}

            //columns.Add(wsColumn[0]);

            if (wsColumn == null || wsColumn.Length == 0)
            {
                result.Success = false;
                result.Message = "Error matching ECAT and LMS Columns";
                return result;
            }

            var studCol = wsColumn.Where(col => col.columnName == model.StudStratCol).ToList();

            if (studCol[0] == null || studCol.Count > 1)
            {
                result.Success = false;
                result.Message = "Error matching ECAT and LMS Columns";
                return result;
            }

            columns.Add(studCol[0]);

            var facCol = new List<ColumnVO>();
            if (model.MaxStratFaculty > 0 && model.FacStratCol != null)
            {
                facCol = wsColumn.Where(col => col.columnName == model.FacStratCol).ToList();
                if (facCol[0] == null || facCol.Count > 1)
                {
                    result.Success = false;
                    result.Message = "Error matching ECAT and LMS Columns";
                    return result;
                }

                columns.Add(facCol[0]);
            }

            //If you specify column names in the column filter, Bb only brings back the column that matches the first name in the names array for some reason
            //So either we go get all 100+ columns and filter it for what we want or we hit the WS twice...
            //if (model.MaxStratFaculty > 0 && model.FacStratCol != null)
            //{
            //    name[0] = model.FacStratCol;
            //    columnFilter.names = name;
            //    wsColumn = await autoRetry.Try(() => bbWs.BbColumns(bbCrseId, columnFilter), 3);

            //    if (wsColumn[0] == null || wsColumn.Length > 1)
            //    {
            //        result.Success = false;
            //        result.Message = "Error matching ECAT and LMS Columns";
            //        return result;
            //    }

            //    columns.Add(wsColumn[0]);
            //}

            var stratResults = await (from str in ctxManager.Context.SpStratResults
                                      where grpIds.Contains(str.WorkGroupId)
                                      select new
                                      {
                                          stratResult = str,
                                          person = str.ResultFor.StudentProfile.Person
                                      }).ToListAsync();

            var scoreVOs = new List<ScoreVO>();
            foreach (var str in stratResults)
            {
                var studScore = new ScoreVO
                {
                    userId = str.person.BbUserId,
                    courseId = bbCrseId,
                    columnId = studCol[0].id,
                    //columnId = columns[0].id,
                    manualGrade = str.stratResult.StudStratAwardedScore.ToString(),
                    manualScore = decimal.ToDouble(str.stratResult.StudStratAwardedScore),
                    manualScoreSpecified = true
                };
                result.SentScores += 1;
                scoreVOs.Add(studScore);

                if (model.MaxStratFaculty > 0 && model.FacStratCol != null)
                {
                    var facScore = new ScoreVO
                    {
                        userId = str.person.BbUserId,
                        courseId = bbCrseId,
                        columnId = facCol[0].id,
                        //columnId = columns[1].id,
                        manualGrade = str.stratResult.FacStratAwardedScore.ToString(),
                        manualScore = decimal.ToDouble(str.stratResult.FacStratAwardedScore),
                        manualScoreSpecified = true
                    };
                    result.SentScores += 1;
                    scoreVOs.Add(facScore);
                }

                result.NumOfStudents += 1;
            }

            //send to Bb
            var autoRetry2 = new Retrier<saveGradesResponse>();
            var scoreReturn = await autoRetry2.Try(() => bbWs.SaveGrades(bbCrseId, scoreVOs.ToArray()), 3);

            result.ReturnedScores = scoreReturn.@return.Length;
            if (result.ReturnedScores != result.SentScores)
            {
                result.Message += " recieved a different number of scores than sent";
                if (scoreReturn.@return[0] == null)
                {
                    result.Success = false;
                    result.Message = "Something went wrong with the connection to the LMS";
                    return result;
                }
            }
            result.Success = true;

            if (unusedGrps.Any())
            {
                ctxManager.Context.WorkGroups.RemoveRange(unusedGrps);
                await ctxManager.Context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<SaveGradeResult> SyncCanvasGrades(int crseId)
        {
            var result = new SaveGradeResult();
            result.Success = false;
            result.Message = "";
            result.NumOfStudents = 0;
            result.SentScores = 0;

            var groups = await ctxManager.Context.WorkGroups.Where(wg => wg.CourseId == crseId && wg.GroupMembers.Any())
                .Include(wg => wg.WgModel)
                .Include(wg => wg.Course)
                .ToListAsync();

            if (!groups.Any())
            {
                result.Success = false;
                result.Message = "No groups with members found.";
                return result;
            }

            var unpubGroups = groups.Where(wg => wg.MpSpStatus != MpSpStatus.Published).ToList();

            if (unpubGroups.Any())
            {
                result.Success = false;
                result.Message = "All groups with members must be published to push eval scores to the LMS.";
                return result;
            }

            var canvasLogin = await ctxManager.Context.CanvasLogins.Where(cl => cl.PersonId == loggedInUserId).SingleOrDefaultAsync();

            if (canvasLogin.AccessToken == null)
            {
                result.Success = false;
                result.HasToken = false;
                result.Message = "There was an problem with your LMS authorization information.";
                return result;
            }

            var acad = StaticAcademy.AcadLookupById[groups.First().Course.AcademyId];

            var models = await ctxManager.Context.WgModels.Where(wgm => wgm.MpEdLevel == acad.MpEdLevel && wgm.IsActive).ToListAsync();
            models.OrderBy(wgm => wgm.MpWgCategory);

            var client = new HttpClient();
            var apiAddr = new Uri(canvasApiUrl + "courses/" + groups[0].Course.BbCourseId + "/assignments?per_page=1000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + canvasLogin.AccessToken);

            var response = await client.GetAsync(apiAddr);
            if (!response.IsSuccessStatusCode)
            {
                result.Success = false;
                result.Message = "There was an error calling the LMS API for assignment information.";
                return result;
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var canvAssignments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanvasAssignment>>(apiResponse);

            //TODO: Update based on what we are doing with prod canvas: pushing all grades back or pushing one grade back?
            var canvasEvalAssign = canvAssignments.Where(ca => ca.name == models.Last().StudStratCol).Single();

            if (canvasEvalAssign == null)
            {
                result.Success = false;
                result.Message = "There was an error finding the proper assignment in the LMS to push scores.";
                return result;
            }

            var grpIds = new List<int>();
            groups.ForEach(grp => grpIds.Add(grp.WorkGroupId));
            var grpMemsWithPerson = await (from csig in ctxManager.Context.StudentInGroups
                                    where csig.CourseId == crseId && grpIds.Contains(csig.WorkGroupId) && !csig.IsDeleted
                                    select new
                                    {
                                        csig,
                                        csig.StudentProfile,
                                        csig.StudentProfile.Person,
                                        csig.StratResult
                                    }).ToListAsync();
            var crseMems = await ctxManager.Context.StudentInCourses.Where(sic => sic.CourseId == crseId && !sic.IsDeleted).ToListAsync();

            string gradeData = "{\"grade_data\":{";
            crseMems.ForEach(cm =>
            {
                result.NumOfStudents += 1;
                var grpMems = grpMemsWithPerson.Where(gm => gm.Person.PersonId == cm.StudentPersonId).ToList();
                decimal totalEval = 0;
                string canvasId = "0";
                grpMems.ForEach(mem =>
                {
                    canvasId = mem.Person.BbUserId;
                    totalEval += mem.StratResult.StudStratAwardedScore;
                    totalEval += mem.StratResult.FacStratAwardedScore;
                });

                if (canvasId == "0") {  }
                string score = "\"" + canvasId + "\":{\"posted_grade\":\"" + totalEval + "\"},";
                gradeData += score;
                result.SentScores += 1;
            });

            gradeData += "}}";

            apiAddr = new Uri(canvasApiUrl + "courses/" + crseId + "assignments/" + canvasEvalAssign.id + "submissions/update_grades");
            var content = new StringContent(gradeData, Encoding.UTF8, "application/json");

            var postResponse = await client.PostAsync(apiAddr, content);
            if (!postResponse.IsSuccessStatusCode)
            {
                result.Success = false;
                result.Message = "There was an error attempting to post scores to the LMS API.";
                return result;
            }

            var postRespContent = await postResponse.Content.ReadAsStringAsync();
            var progress = Newtonsoft.Json.JsonConvert.DeserializeObject<CanvasProgress>(postRespContent); 

            if (progress.workflow_state == "queued" || progress.workflow_state == "running")
            {
                result.Success = true;
                result.Message = result.SentScores + " scores have been sent to the LMS and are now being processed. ";//Check the status at " + canvasApiUrl +"progress/" + progress.id;
            }

            if (progress.workflow_state == "failed")
            {
                result.Success = false;
                result.Message = "LMS returned failed on API call: " + progress.message;
            }

            return result;
        }
    }
}
