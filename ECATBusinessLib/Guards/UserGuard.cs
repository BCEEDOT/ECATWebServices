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
using Ecat.Business.Utilities;

namespace Ecat.Business.Guards
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public class UserGuard
    {
        //TODO: Update as more profiles are implemented
        private readonly EFPersistenceManager<EcatContext> ctxManager;
        //private readonly Person _loggedInUser;
        private readonly int loggedInUserId;
        private readonly Type tPerson = typeof(Person);
        //private readonly Type _tProfileExternal = typeof(ProfileExternal);
        private readonly Type tProfileFaculty = typeof(ProfileFaculty);
        //private readonly Type _tProfileStaff = typeof(ProfileStaff);
        private readonly Type tProfileStudent = typeof(ProfileStudent);
        private readonly Type tProfileSecurity = typeof(Security);
        private readonly Type tCogResponse = typeof(CogResponse);
        private readonly Type tCogEcpeResult = typeof(CogEcpeResult);
        private readonly Type tCogEsalbResult = typeof(CogEsalbResult);
        private readonly Type tCogEtmpreResult = typeof(CogEtmpreResult);
        private readonly Type tCogEcmspeResult = typeof(CogEcmspeResult);
        private readonly Type tRoadRunner = typeof(RoadRunner);

        public UserGuard(EFPersistenceManager<EcatContext> efCtx, int userId)
        {
            ctxManager = efCtx;
            //_loggedInUser = loggedInUser;
            loggedInUserId = userId;
        }

        public SaveMap BeforeSaveEntities(SaveMap saveMap)
        {

            var unAuthorizedMaps = saveMap.Where(map => map.Key != tPerson &&
                                                        //map.Key != _tProfileExternal &&
                                                        map.Key != tProfileFaculty &&
                                                        map.Key != tProfileStudent &&
                                                        map.Key != tProfileSecurity &&
                                                                                      //map.Key != _tProfileStaff &&
                                                                                      map.Key != tCogResponse &&
                                                                                      map.Key != tCogEcpeResult &&
                                                                                      map.Key != tCogEsalbResult &&
                                                                                      map.Key != tCogEtmpreResult &&
                                                                                      map.Key != tCogEcmspeResult &&
                                                                                      map.Key != tRoadRunner)
                                                        .ToList();

            saveMap.RemoveMaps(unAuthorizedMaps);

            saveMap.AuditMap(loggedInUserId);
            saveMap.SoftDeleteMap(loggedInUserId);
            return saveMap;
        }
    }
}
