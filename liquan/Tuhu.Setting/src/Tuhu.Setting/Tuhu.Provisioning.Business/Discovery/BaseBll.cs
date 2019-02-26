using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Framework.DbCore;
using Tuhu.Provisioning.DataAccess;

namespace Tuhu.Provisioning.Business.Discovery
{
    public abstract class BaseBll
    {
        protected static readonly DbContextManager<DiscoveryDbContext> DbManager
   = new DbContextManager<DiscoveryDbContext>();
        protected static readonly DiscoveryDbContext DiscoveryDbContext = new DiscoveryDbContext();
    }
}
