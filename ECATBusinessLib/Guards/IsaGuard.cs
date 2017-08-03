﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using Breeze.Persistence.EF6;
using Ecat.Data.Contexts;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Cognitive;
using Ecat.Data.Models.School;
using Ecat.Business.Utilities;
using Ecat.Data.Models.Common;
using Ecat.Data.Static;

namespace Ecat.Business.Guards
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public class IsaGuard
    {
        private readonly EFPersistenceManager<EcatContext> ctxManager;
        private readonly int loggedInUserId;
        private readonly Type tWg = typeof(WorkGroup);
        private readonly Type tStudInGroup = typeof(CrseStudentInGroup);

        public IsaGuard(EFPersistenceManager<EcatContext> efCtx, int userId)
        {
            ctxManager = efCtx;
            loggedInUserId = userId;
        }

        public SaveMap BeforeSaveEntities(SaveMap saveMap)
        {

            List<StudentOnTheMove> studentsPendingRemoval = new List<StudentOnTheMove>();

            if (saveMap.ContainsKey(tWg))
            {
                var grps = ProcessWorkGroup(saveMap[tWg]);
                saveMap.MergeMap(grps);
            }

            if (saveMap.ContainsKey(tStudInGroup))
            {
                var groupMembers = saveMap[tStudInGroup];

                //Need to account for adds when a new group is created. 

                var studentsOnTheMove = (from info in saveMap[tStudInGroup]
                                        let sig = info.Entity as CrseStudentInGroup
                                        where info.EntityState == EntityState.Modified
                                        where info.OriginalValuesMap.ContainsKey("WorkGroupId")
                                        select info).ToList();

      
                var unAssignedStudents = (from info in saveMap[tStudInGroup]
                                        let sig = info.Entity as CrseStudentInGroup
                                        where info.EntityState == EntityState.Modified
                                        where info.OriginalValuesMap.ContainsKey("IsDeleted")
                                        select info).ToList();

                if (unAssignedStudents.Any()) {
                    unAssignedStudents.ForEach(sm =>
                    {
                        var studentEntity = sm.Entity as CrseStudentInGroup;
                        var toWorkGroupId = studentEntity.WorkGroupId;

                        var memwithChildren = ctxManager.Context.StudentInGroups
                                                          .Where(sig => sig.StudentId == studentEntity.StudentId)
                                                          .Where(sig => sig.WorkGroupId == toWorkGroupId)
                                                          .Select(sig => new StudentOnTheMove{
                                                              Student = sig,
                                                              StudentId = sig.StudentId,
                                                              IsDeleted = sig.IsDeleted,
                                                              IsMoving = false,
                                                              HasChildren = sig.AuthorOfComments.Any() ||
                                                                           sig.AssesseeSpResponses.Any() ||
                                                                           sig.AssessorSpResponses.Any() ||
                                                                           sig.AssesseeStratResponse.Any() ||
                                                                           sig.AssessorStratResponse.Any() ||
                                                                           sig.RecipientOfComments.Any()
                                                          }).ToList();

                        studentsPendingRemoval.AddRange(memwithChildren);

                    });
                }

                if (studentsOnTheMove.Any()) {
                    
                    studentsOnTheMove.ForEach(sm =>
                    {
                        var studentEntity = sm.Entity as CrseStudentInGroup;
                        var fromWorkGroupId = Int32.Parse(sm.OriginalValuesMap.Values.FirstOrDefault().ToString());


                        var memwithChildren = ctxManager.Context.StudentInGroups
                                                          .Where(sig => sig.StudentId == studentEntity.StudentId)
                                                          .Where(sig => sig.WorkGroupId == fromWorkGroupId)
                                                          .Select(sig => new StudentOnTheMove {
                                                              Student = sig,
                                                              StudentId = sig.StudentId,
                                                              IsDeleted = sig.IsDeleted,
                                                              IsMoving = true,
                                                              ToWorkGroupId = studentEntity.WorkGroupId,
                                                              FromWorkGroupId = fromWorkGroupId,
                                                              CourseId = studentEntity.CourseId,
                                                              HasChildren = sig.AuthorOfComments.Any() ||
                                                                           sig.AssesseeSpResponses.Any() ||
                                                                           sig.AssessorSpResponses.Any() ||
                                                                           sig.AssesseeStratResponse.Any() ||
                                                                           sig.AssessorStratResponse.Any() ||
                                                                           sig.RecipientOfComments.Any()
                                                          }).ToList();

                        studentsPendingRemoval.AddRange(memwithChildren);
                    });                  
                }

                var studentsPendingRemovalWithChildren = studentsPendingRemoval
                                                                    .Where(spr => spr.HasChildren).ToList();

                var studentsPendingRemovalWithoutChildren = studentsPendingRemoval
                                                                    .Where(spr => !spr.HasChildren).ToList();
                                                                    

                if (studentsPendingRemovalWithChildren.Any())
                {
                    studentsPendingRemovalWithChildren.ForEach(sprwc => 
                    {
                        if (sprwc.IsMoving)
                        {
                            var authorOfComments = ctxManager.Context.StudSpComments
                                                    .Where(ssc => ssc.AuthorPersonId == sprwc.StudentId)
                                                    .Where(ssc => ssc.WorkGroupId == sprwc.FromWorkGroupId);

                            var recipientOfComments = ctxManager.Context.StudSpComments
                                                        .Where(ssc => ssc.RecipientPersonId == sprwc.StudentId)
                                                        .Where(ssc => ssc.WorkGroupId == sprwc.FromWorkGroupId);

                            //var authorCommentFlags = ctxManager.Context.

                            var assesseeSpResponses = ctxManager.Context.SpResponses
                                                    .Where(sr => sr.AssesseePersonId == sprwc.StudentId)
                                                    .Where(sr => sr.WorkGroupId == sprwc.FromWorkGroupId);

                            var assessorSpResponses = ctxManager.Context.SpResponses
                                                        .Where(sr => sr.AssessorPersonId == sprwc.StudentId)
                                                        .Where(sr => sr.WorkGroupId == sprwc.FromWorkGroupId);

                            var assesseeStratResponses = ctxManager.Context.SpStratResponses
                                                            .Where(ssr => ssr.AssesseePersonId == sprwc.StudentId)
                                                            .Where(ssr => ssr.WorkGroupId == sprwc.FromWorkGroupId);

                            var assessorStratResponses = ctxManager.Context.SpStratResponses
                                                            .Where(ssr => ssr.AssessorPersonId == sprwc.StudentId)
                                                            .Where(ssr => ssr.WorkGroupId == sprwc.FromWorkGroupId);

                            

                            if (authorOfComments.Any()) {
                                ctxManager.Context.StudSpComments.RemoveRange(authorOfComments);
                            }

                            if (recipientOfComments.Any()) {
                                ctxManager.Context.StudSpComments.RemoveRange(recipientOfComments);
                            }

                            if (assesseeSpResponses.Any()) {
                                ctxManager.Context.SpResponses.RemoveRange(assesseeSpResponses);
                            }

                            if (assessorSpResponses.Any()) {
                                ctxManager.Context.SpResponses.RemoveRange(assessorSpResponses);
                            }

                            if (assesseeStratResponses.Any()) {
                                ctxManager.Context.SpStratResponses.RemoveRange(assesseeStratResponses);
                            }

                            if (assessorStratResponses.Any()) {
                                ctxManager.Context.SpStratResponses.RemoveRange(assessorStratResponses);
                             }

                            
                        }

                        ctxManager.Context.StudentInGroups.Remove(sprwc.Student);
                     
                    });

                    ctxManager.Context.SaveChanges();
                }

                if (studentsPendingRemovalWithoutChildren.Any())
                {
                    studentsPendingRemovalWithoutChildren.ForEach(sprwoc =>
                    {
                        ctxManager.Context.Entry(sprwoc.Student).State = System.Data.Entity.EntityState.Deleted;
                    });

                    ctxManager.Context.SaveChanges();
                }

                var studentsToBeAddedBack = studentsPendingRemoval
                                                        .Where(spr => spr.IsMoving).ToList();


                //Students that were previously deleted with children. 
                var studentsToAdd = new List<CrseStudentInGroup>();


                if (studentsToBeAddedBack.Any())
                {

                    studentsToBeAddedBack.ForEach(stab =>
                    {
                        var studentWithPreviousRecord = ctxManager.Context.StudentInGroups
                                                              .Where(sig => sig.StudentId == stab.StudentId)
                                                              .Where(sig => sig.WorkGroupId == stab.ToWorkGroupId)
                                                              .Where(sig => sig.IsDeleted == true).ToList();

                        if (studentWithPreviousRecord.Any())
                        {
                            studentWithPreviousRecord.ForEach(swpr =>
                            {
                                swpr.IsDeleted = false;
                                swpr.DeletedDate = null;
                                swpr.DeletedById = null;
                                swpr.ModifiedById = loggedInUserId;
                                swpr.ModifiedDate = DateTime.Now;

                            });
                        
                        }
                        else
                        {
                            var toAdd = new CrseStudentInGroup
                            {
                                StudentId = stab.StudentId,
                                CourseId = stab.CourseId,
                                WorkGroupId = stab.ToWorkGroupId,
                                HasAcknowledged = false,
                                IsDeleted = false,
                                ModifiedById = loggedInUserId,
                                ModifiedDate = DateTime.Now
                            };

                            studentsToAdd.Add(toAdd);
                        }

                    });

                    ctxManager.Context.SaveChanges();
                }


                if (studentsToAdd.Any()) {
                    studentsToAdd.ForEach(sta =>
                    {
                        ctxManager.Context.StudentInGroups.Add(sta);
                    });

                    ctxManager.Context.SaveChanges();
                }
            
                studentsOnTheMove.ForEach(info => { saveMap.Remove(tStudInGroup); });
                unAssignedStudents.ForEach(info => { saveMap.Remove(tStudInGroup); });

                //saveMap[tStudInGroup] = deleteOnly;



                //if (deleteOnly.Any())
                //{
                //    foreach ( var delete in deleteOnly)
                //    {
                //       var gmsPendingRemovalWithChildren = delete.select
                //    }

                //    saveMap[tStudInGroup] = infosToDeleteOnly;
                //}
                //else {
                //    saveMap.Remove(tStudInGroup);
                //}
            }


            return saveMap;
        }

        private SaveMap ProcessWorkGroup(List<EntityInfo> workGroupInfos)
        {

            var publishingWgs = workGroupInfos
                .Where(info => info.OriginalValuesMap.ContainsKey("MpSpStatus"))
                .Select(info => info.Entity)
                .OfType<WorkGroup>()
                .Where(wg => wg.MpSpStatus == MpSpStatus.Published).ToList();

            var wgSaveMap = new Dictionary<Type, List<EntityInfo>> { { tWg, workGroupInfos } };

            if (!publishingWgs.Any()) return wgSaveMap;


            var svrWgIds = publishingWgs.Select(wg => wg.WorkGroupId);
            var publishResultMap = WorkGroupPublish.Publish(wgSaveMap, svrWgIds, loggedInUserId, ctxManager);

            wgSaveMap.MergeMap(publishResultMap);

            return wgSaveMap;
        }
    }

    //private SaveMap ProcessMembers(List<EntityInfo> groupMembers)

   // {
   
        //var gmsPendingRemovalWithChildren = groupMembers.Select(gm => new GmrMember
        //{
        //    StudentId = gm.Entity.StudentId,
        //    BbGroupMemId = gm.BbCrseStudGroupId,
        //    BbCrseMemId = gm.StudentInCourse.BbCourseMemId,
        //    IsDeleted = gm.IsDeleted,
        //    HasChildren = gm.AuthorOfComments.Any() ||
        //                                      gm.AssesseeSpResponses.Any() ||
        //                                      gm.AssessorSpResponses.Any() ||
        //                                      gm.AssesseeStratResponse.Any() ||
        //                                      gm.AssessorStratResponse.Any() ||
        //                                      gm.RecipientOfComments.Any()

        //})



        //    foreach (var member in groupMembers)
        //{
        //    var gmsPendingRemovalWithChildren = member.Where(mem => mem.PendingRemoval && mem.HasChildren).Select(mem => mem.StudentId).ToList();

        //    if (gmsPendingRemovalWithChildren.Any())
        //    {
        //        var existingStudToFlag =
        //            await ctxManager.Context.StudentInGroups.Where(
        //                sig =>
        //                    gmsPendingRemovalWithChildren.Contains(sig.StudentId) && sig.WorkGroupId == group.WgId)
        //                .ToListAsync();

        //        foreach (var sig in existingStudToFlag)
        //        {
        //            sig.IsDeleted = true;
        //            sig.DeletedById = Faculty?.PersonId;
        //            sig.DeletedDate = DateTime.Now;
        //            sig.ModifiedById = Faculty?.PersonId;
        //            sig.ModifiedDate = DateTime.Now;
        //        }
        //    }

        //    foreach (
        //        var gmrMember in
        //            group.Members.Where(mem => mem.PendingRemoval && !mem.HasChildren)
        //                .Select(mem => new CrseStudentInGroup
        //                {
        //                    WorkGroupId = group.WgId,
        //                    CourseId = crseId,
        //                    StudentId = mem.StudentId,
        //                }))
        //    {
        //        ctxManager.Context.Entry(gmrMember).State = System.Data.Entity.EntityState.Deleted;
        //    }

        //    group.ReconResult.NumRemoved = group.Members.Count(mem => mem.PendingRemoval);
        //}
        //await ctxManager.Context.SaveChangesAsync();
        //return grpWithMems;
        //}
    }