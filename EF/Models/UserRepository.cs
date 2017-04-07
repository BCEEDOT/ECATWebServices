using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.Persistence;
using ECATDataLib.Models;
using Newtonsoft.Json.Linq;
using ECATDataLib.Contexts;
using Breeze.Persistence.EF6;

namespace ECATDataLib.Models
{
    public class UserRepository : IUserRepository
    {

        //breeze manager
        private readonly EFPersistenceManager<ProductContext> _persistenceManager;

        //inject the context
        public UserRepository(ProductContext productContext)
        {
            //new up the breeze manager using the context
            _persistenceManager = new EFPersistenceManager<ProductContext>(productContext);
        }

        public void Add(UserTest user)
        {
            _persistenceManager.Context.Users.Add(user);
            _persistenceManager.Context.SaveChanges();
        }

        public void Add(string username, string password)
        {
            _persistenceManager.Context.Users.Add(new UserTest { Username = username, Password = password });
            _persistenceManager.Context.SaveChanges();
        }

        public IEnumerable<UserTest> GetUsers()
        {
            return _persistenceManager.Context.Users;
        }

        public string Metadata()
        {
            return _persistenceManager.Metadata();
        }

        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _persistenceManager.SaveChanges(saveBundle);
        }
    }
}
