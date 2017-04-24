using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Designer;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;

namespace Ecat.Business.Repositories.Interface
{
    public interface IUserRepo
    {
        string MetaData();
        SaveResult ClientSave(JObject saveBundle);

        int loggedInUserId { get; set; }
        //Person User { get; set; }
        Task<List<object>> GetProfile();
        //Task<Person> LoginUser(string userName, string password);
        //Task<Person> ProcessLtiUser(ILtiRequest parsedRequest);
        Task<bool> UniqueEmailCheck(string email);
        Task<CogInstrument> GetCogInst(string type);
        Task<List<object>> GetCogResults(bool? all);
        Task<List<RoadRunner>> GetRoadRunnerInfo();
        IEnumerable<Person> GetUsers();
    }
}
