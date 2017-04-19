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
        private readonly EFPersistenceManager<EcatContext> _efCtx;
        //private readonly Person _loggedInUser;
        private readonly int loggedInUserId;
        private readonly Type _tPerson = typeof(Person);
        //private readonly Type _tProfileExternal = typeof(ProfileExternal);
        private readonly Type _tProfileFaculty = typeof(ProfileFaculty);
        //private readonly Type _tProfileStaff = typeof(ProfileStaff);
        private readonly Type _tProfileStudent = typeof(ProfileStudent);
        private readonly Type _tProfileSecurity = typeof(Security);
        private readonly Type _tCogResponse = typeof(CogResponse);
        private readonly Type _tCogEcpeResult = typeof(CogEcpeResult);
        private readonly Type _tCogEsalbResult = typeof(CogEsalbResult);
        private readonly Type _tCogEtmpreResult = typeof(CogEtmpreResult);
        private readonly Type _tCogEcmspeResult = typeof(CogEcmspeResult);
        private readonly Type _tRoadRunner = typeof(RoadRunner);

        public UserGuard(EFPersistenceManager<EcatContext> efCtx, int userId)
        {
            _efCtx = efCtx;
            //_loggedInUser = loggedInUser;
            loggedInUserId = userId;
        }

        public SaveMap BeforeSaveEntities(SaveMap saveMap)
        {

            var unAuthorizedMaps = saveMap.Where(map => map.Key != _tPerson &&
                                                        //map.Key != _tProfileExternal &&
                                                        map.Key != _tProfileFaculty &&
                                                        map.Key != _tProfileStudent &&
                                                        map.Key != _tProfileSecurity &&
                                                                                      //map.Key != _tProfileStaff &&
                                                                                      map.Key != _tCogResponse &&
                                                                                      map.Key != _tCogEcpeResult &&
                                                                                      map.Key != _tCogEsalbResult &&
                                                                                      map.Key != _tCogEtmpreResult &&
                                                                                      map.Key != _tCogEcmspeResult &&
                                                                                      map.Key != _tRoadRunner)
                                                        .ToList();

            saveMap.RemoveMaps(unAuthorizedMaps);

            saveMap.AuditMap(loggedInUserId);
            saveMap.SoftDeleteMap(loggedInUserId);
            return saveMap;
        }
    }
}
