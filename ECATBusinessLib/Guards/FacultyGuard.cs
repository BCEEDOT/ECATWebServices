using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using Breeze.Persistence.EF6;
using Ecat.Data.Contexts;
using Ecat.Data.Models.School;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.Interface;
using Ecat.Data.Static;
using Ecat.Business.Utilities;

namespace Ecat.Business.Guards
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public class FacultyGuard
    {
        private readonly EFPersistenceManager<EcatContext> ctxManager;
        private readonly int loggedInUserId;
        private readonly Type tWg = typeof(WorkGroup);
        private readonly Type tFacComment = typeof(FacSpComment);
        private readonly Type tFacStratResp = typeof(FacStratResponse);
        private readonly Type tFacCommentFlag = typeof(FacSpCommentFlag);
        private readonly Type tStudCommentFlag = typeof(StudSpCommentFlag);
        private readonly Type tFacSpResp = typeof(FacSpResponse);

        public FacultyGuard(EFPersistenceManager<EcatContext> efCtx, int userId)
        {
            ctxManager = efCtx;
            loggedInUserId = userId;
        }

        public SaveMap BeforeSaveEntities(SaveMap saveMap)
        {

            var unAuthorizedMaps = saveMap.Where(map => map.Key != tWg &&
                                                        map.Key != tFacComment &&
                                                        map.Key != tFacStratResp &&
                                                        map.Key != tFacSpResp &&
                                                        map.Key != tStudCommentFlag &&
                                                        map.Key != tFacCommentFlag)
                                                        .ToList();
            //.Select(map => map.Key);

            saveMap.RemoveMaps(unAuthorizedMaps);

            var courseMonitorEntities = saveMap.MonitorCourseMaps()?.ToList();
            var workGroupMonitorEntities = saveMap.MonitorWgMaps()?.ToList();

            //moved processing for monitored entities to monitoredguard
            if (courseMonitorEntities != null || workGroupMonitorEntities != null) {
                var monitoredGuard = new MonitoredGuard(ctxManager);

                if (courseMonitorEntities != null) {
                    monitoredGuard.ProcessCourseMonitoredMaps(courseMonitorEntities);
                }

                if (workGroupMonitorEntities != null) {
                    monitoredGuard.ProcessFacultyWorkGroupMonitoredMaps(workGroupMonitorEntities);
                }
            }         

            if (saveMap.ContainsKey(tWg))
            {
                var workGroupMap = ProcessWorkGroup(saveMap[tWg]);
                saveMap.MergeMap(workGroupMap);
            }

            if (saveMap.ContainsKey(tFacComment))
            {
                ProcessComments(saveMap[tFacComment]);
            }

            saveMap.AuditMap(loggedInUserId);
            saveMap.SoftDeleteMap(loggedInUserId);
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

        private void ProcessComments(List<EntityInfo> commentInfos)
        {
            var newComments = commentInfos
                .Where(info => info.EntityState == EntityState.Added)
                .Select(info => info.Entity)
                .OfType<FacSpComment>()
                .ToList();

            foreach (var newComment in newComments)
            {
                newComment.CreatedDate = DateTime.UtcNow;
            }

            var modifiedComments = commentInfos
                .Where(info => info.EntityState == EntityState.Modified)
                .ToList();

            //for now modifying a comment makes the modifier the comment owner, no history of who originally created it
            //future plan for audit tables to track changes
            foreach (var info in modifiedComments)
            {
                info.OriginalValuesMap["FacultyPersonId"] = null;
                var comment = info.Entity as FacSpComment;
                comment.FacultyPersonId = loggedInUserId;
            }
        }
    }
}
