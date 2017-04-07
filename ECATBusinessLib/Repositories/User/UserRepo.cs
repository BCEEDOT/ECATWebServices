using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECATBusinessLib.Repositories.Interface;
using ECATDataLib.Models.User;
using Breeze.Persistence;
using Breeze.Persistence.EF6;
using Newtonsoft.Json.Linq;
using ECATDataLib.Contexts;

namespace ECATBusinessLib.Repositories.User
{
    public class UserRepo: IUserRepo
    {
        public Person User { get; set; }
        private readonly EFPersistenceManager<EcatContext> _efCtx;

        public UserRepo(EcatContext ecatCtx)
        {
            _efCtx = new EFPersistenceManager<EcatContext>(ecatCtx);
        }

        public string MetaData()
        {
            return _efCtx.Metadata();
        }
    }
}
