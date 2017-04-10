using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using Breeze.Persistence;
using Ecat.Data.Models.Interface;

namespace Ecat.Business.Utilities
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public static class SaveMapExtension
    {
        public static SaveMap RemoveMaps(this SaveMap saveMap, List<KeyValuePair<Type, List<EntityInfo>>> unauthorizedTypes)
        {
            foreach (var uat in unauthorizedTypes)
            {
                saveMap.Remove(uat.Key);
            }

            return saveMap;
        }

        public static SaveMap AuditMap(this SaveMap saveMap, int loggedInUserId)
        {
            var predicate =
                saveMap
                .Where(map => typeof(IAuditable)
                .IsAssignableFrom(map.Key))
                .SelectMany(map => map.Value)
                .Select(info => info);

            foreach (var info in predicate)
            {
                var auditable = info.Entity as IAuditable;

                Contract.Assert(auditable != null);

                auditable.ModifiedById = loggedInUserId;
                auditable.ModifiedDate = DateTime.Now;

                if (info.OriginalValuesMap == null) continue;

                info.OriginalValuesMap["ModifiedById"] = null;
                info.OriginalValuesMap["ModifiedDate"] = null;
            }

            return saveMap;
        }

        public static SaveMap SoftDeleteMap(this SaveMap saveMap, int loggedInUserId)
        {

            var predicate =
                saveMap
                .Where(map => typeof(ISoftDelete)
                .IsAssignableFrom(map.Key))
                .SelectMany(map => map.Value)
                .Select(info => info);

            foreach (var info in predicate)
            {
                var softDeletable = info.Entity as ISoftDelete;

                Contract.Assert(softDeletable != null);

                softDeletable.DeletedById = loggedInUserId;
                softDeletable.DeletedDate = DateTime.Now;
                softDeletable.IsDeleted = true;

                if (info.OriginalValuesMap == null) continue;

                info.OriginalValuesMap["DeletedById"] = null;
                info.OriginalValuesMap["DeletedDate"] = null;
                info.OriginalValuesMap["IsDeleted"] = null;
            }

            return saveMap;
        }

        public static SaveMap MergeMap(this SaveMap saveMap, SaveMap newMap)
        {
            foreach (var map in newMap)
            {
                List<EntityInfo> infos;

                var hasKey = saveMap.TryGetValue(map.Key, out infos);
                if (hasKey)
                {
                    saveMap[map.Key].AddRange(infos);
                }
                else
                {
                    saveMap.Add(map.Key, map.Value);
                }
            }
            return saveMap;
        }

        public static IEnumerable<EntityInfo> MonitorCourseMaps(this SaveMap saveMap)
        {
            //Is there anything to monitored?
            var anyCourseMonitors = saveMap.Keys.Where(key => typeof(ICourseMonitored).IsAssignableFrom(key)).ToList();

            if (!anyCourseMonitors.Any()) return null;

            return saveMap.Where(map => anyCourseMonitors.Contains(map.Key))
                .SelectMany(info => info.Value);
        }


        public static IEnumerable<EntityInfo> MonitorWgMaps(this SaveMap saveMap)
        {
            //Is there anything to monitored?
            var anyWorkGroupMonitors = saveMap.Keys.Where(key => typeof(IWorkGroupMonitored).IsAssignableFrom(key)).ToList();

            if (!anyWorkGroupMonitors.Any()) return null;

            return
                saveMap.Where(map => anyWorkGroupMonitors.Contains(map.Key))
                    .SelectMany(info => info.Value);
        }
    }
}
