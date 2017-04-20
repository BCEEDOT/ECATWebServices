using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence.EF6;
using Breeze.Persistence;
using Ecat.Data.Contexts;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.User;
using Ecat.Data.Models.School;
using Ecat.Business.Utilities;

namespace Ecat.Business.Guards
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public class StudentGuard
    {

        private readonly EFPersistenceManager<EcatContext> ctxManager;
        private readonly int loggedInUserId;
        private readonly Type tStudInGroup = typeof(CrseStudentInGroup);
        private readonly Type tStudComment = typeof(StudSpComment);
        private readonly Type tStudCommentFlag = typeof(StudSpCommentFlag);
        private readonly Type tSpResponse = typeof(SpResponse);
        private readonly Type tStratResponse = typeof(StratResponse);

        public StudentGuard(EFPersistenceManager<EcatContext> efCtx, int userId)
        {
            ctxManager = efCtx;
            loggedInUserId = userId;
        }

        public SaveMap BeforeSaveEntities(SaveMap saveMap)
        {

            var unAuthorizedMaps = saveMap.Where(map => map.Key != tStudComment &&
                                                        map.Key != tSpResponse &&
                                                        map.Key != tStudInGroup &&
                                                        map.Key != tStratResponse &&
                                                        map.Key != tStudCommentFlag)
                                                        .ToList();

            saveMap.RemoveMaps(unAuthorizedMaps);

            //Process any monitored entities to see if saves are allowed.
            var courseMonitorEntities = saveMap.MonitorCourseMaps()?.ToList();
            var workGroupMonitorEntities = saveMap.MonitorWgMaps()?.ToList();

            if (courseMonitorEntities != null || workGroupMonitorEntities != null)
            {
                var monitorGuard = new MonitoredGuard(ctxManager);
                if (courseMonitorEntities != null) monitorGuard.ProcessCourseMonitoredMaps(courseMonitorEntities);
                if (workGroupMonitorEntities != null) monitorGuard.ProcessWorkGroupMonitoredMaps(workGroupMonitorEntities);
            }

            //Process studInGroup to ensure that only the logged student' is being handled.
            if (saveMap.ContainsKey(tStudInGroup))
            {
                var infos = (from info in saveMap[tStudInGroup]
                             let sig = info.Entity as CrseStudentInGroup
                             where sig != null && sig.StudentId == loggedInUserId
                             where info.EntityState == EntityState.Modified
                             where info.OriginalValuesMap.ContainsKey("HasAcknowledged")
                             select info).ToList();

                if (infos.Any())
                {
                    foreach (var info in infos)
                    {
                        info.OriginalValuesMap = new Dictionary<string, object>()
                        {{"HasAcknowledged", null}};
                    }

                    saveMap[tStudInGroup] = infos;
                }
                else
                {
                    saveMap.Remove(tStudInGroup);
                }
            }

            if (saveMap.ContainsKey(tStudComment))
            {
                var newComments = saveMap[tStudComment]
                    .Where(info => info.EntityState == EntityState.Added)
                    .Select(info => info.Entity)
                    .OfType<StudSpComment>()
                    .ToList();

                foreach (var comment in newComments)
                {
                    comment.CreatedDate = DateTime.UtcNow;
                }
            }

            saveMap.AuditMap(loggedInUserId);
            saveMap.SoftDeleteMap(loggedInUserId);
            return saveMap;
        }
    }
}
