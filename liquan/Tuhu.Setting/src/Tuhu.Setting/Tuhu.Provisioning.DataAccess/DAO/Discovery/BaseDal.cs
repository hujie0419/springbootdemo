using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Framework.DbCore;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    public abstract class BaseDal
    {
        protected static readonly DbContextManager<DiscoveryDbContext> DbManager = new DbContextManager<DiscoveryDbContext>();
        protected static readonly int Success = 1;
        protected static readonly int Error = 0;
    }
}
