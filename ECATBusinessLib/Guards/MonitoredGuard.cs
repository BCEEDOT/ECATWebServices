using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using Breeze.Persistence.EF6;
using Ecat.Data.Contexts;
using Ecat.Data.Models.Interface;
using Ecat.Data.Static;

namespace Ecat.Business.Guards
{
    public class MonitoredGuard
    {
        private EFPersistenceManager<EcatContext> ctxManager;

        public MonitoredGuard(EFPersistenceManager<EcatContext> efCtx)
        {
            ctxManager = efCtx;
        }

        public void ProcessCourseMonitoredMaps(List<EntityInfo> infos)
        {
            var courseMonitorEntities = infos.Select(info => info.Entity).OfType<ICourseMonitored>();
            var courseIds = courseMonitorEntities.Select(cme => cme.CourseId);

            var pubCrseId = ctxManager.Context.Courses
                .Where(crse => courseIds.Contains(crse.Id) && crse.GradReportPublished)
                .Select(crse => crse.Id);

            if (!pubCrseId.Any()) return;

            var errors = from info in infos
                         let crseEntity = info.Entity as ICourseMonitored
                         where crseEntity != null && pubCrseId.Contains(crseEntity.CourseId)
                         select new EFEntityError(info, MpEntityError.CrseNotOpen,
                                     "There was a problem saving the requested items", "Course");

            throw new EntityErrorsException(errors);
        }

        //for workgroups students and faculty are seperate because faculty can still save to Under Review groups, but students can't
        public void ProcessStudentWorkGroupMonitoredMaps(List<EntityInfo> infos)
        {
            var wgMonitorEntities = infos.Select(info => info.Entity).OfType<IWorkGroupMonitored>();
            var wgIds = wgMonitorEntities.Select(wgme => wgme.WorkGroupId);

            var pubWgIds = ctxManager.Context.WorkGroups
                .Where(wg => wgIds.Contains(wg.WorkGroupId) && wg.MpSpStatus != MpSpStatus.Open)
                .Select(wg => wg.WorkGroupId);

            if (!pubWgIds.Any()) return;

            var errors = from info in infos
                         let wgEntity = info.Entity as IWorkGroupMonitored
                         where wgEntity != null && pubWgIds.Contains(wgEntity.WorkGroupId)
                         select new EFEntityError(info, MpEntityError.WgNotOpen,
                                     "There was a problem saving the requested items, the workGroup is in a non-open state!", "WorkGroup");

            throw new EntityErrorsException(errors);
        }

        public void ProcessFacultyWorkGroupMonitoredMaps(List<EntityInfo> infos)
        {
            var wgMonitorEntities = infos.Select(info => info.Entity).OfType<IWorkGroupMonitored>();
            var wgIds = wgMonitorEntities.Select(wgme => wgme.WorkGroupId);

            var pubWgIds = ctxManager.Context.WorkGroups
                .Where(wg => wgIds.Contains(wg.WorkGroupId) && wg.MpSpStatus == MpSpStatus.Published)
                .Select(wg => wg.WorkGroupId);

            if (!pubWgIds.Any()) return;

            var errors = from info in infos
                         let wgEntity = (IWorkGroupMonitored)info.Entity
                         where pubWgIds.Contains(wgEntity.WorkGroupId)
                         select new EFEntityError(info, "WorkGroup Error Validation",
                                     "There was a problem saving the requested items", "WorkGroup");


            throw new EntityErrorsException(errors);
        }
    }
}
