using System;
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

            if (saveMap.ContainsKey(tWg))
            {
                var grps = ProcessWorkGroup(saveMap[tWg]);
                saveMap.MergeMap(grps);
            }

            if (saveMap.ContainsKey(tStudInGroup))
            {
                var groupMembers = saveMap[tStudInGroup];

                //Need to account for adds when a new group is created. 


                var studentOnTheMove = (from info in saveMap[tStudInGroup]
                             let sig = info.Entity as CrseStudentInGroup
                             where info.EntityState == EntityState.Modified
                             where info.OriginalValuesMap.ContainsKey("WorkGroupId")
                             select info).ToList();

                var deleteOnly = (from info in saveMap[tStudInGroup]
                                        let sig = info.Entity as CrseStudentInGroup
                                        where info.EntityState == EntityState.Modified
                                        where info.OriginalValuesMap.ContainsKey("IsDeleted")
                                        select info).ToList();



                //var deleteOnly = groupMembers.Where(info => info.OriginalValuesMap.ContainsKey("IsDeleted"))
                //                             .Select(info => info.Entity)
                //                             .OfType<CrseStudentInGroup>().ToList();

                //var gmsPendingRemovalWithChildren = deleteOnly.Select(gm => new GmrMember
                //{
                //    StudentId = gm.StudentId,
                //    BbGroupMemId = gm.BbCrseStudGroupId,
                //    BbCrseMemId = gm.StudentInCourse.BbCourseMemId,
                //    IsDeleted = gm.IsDeleted,
                //    HasChildren = gm.AuthorOfComments.Any() ||
                //                  gm.AssesseeSpResponses.Any() ||
                //                  gm.AssesseeSpResponses.Any() ||
                //                  gm.AssesseeStratResponse.Any() ||
                //                  gm.AssessorStratResponse.Any() ||
                //                  gm.RecipientOfComments.Any()
                
                //}).ToList();

                studentOnTheMove.ForEach(info => { saveMap.Remove(tStudInGroup); });

                saveMap[tStudInGroup] = deleteOnly;



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
