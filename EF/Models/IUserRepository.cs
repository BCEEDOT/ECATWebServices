using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using Newtonsoft.Json.Linq;

namespace Ecat.Data.Models
{
    public interface IUserRepository
    {
        IEnumerable<UserTest> GetUsers();
        void Add(UserTest user);
        void Add(string username, string password);

        //Breeze specific methods
        string Metadata();
        SaveResult SaveChanges(JObject saveBundle);
    }
}
