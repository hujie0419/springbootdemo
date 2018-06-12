using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.DAO.Discovery
{
    internal class DBContextFactory
    {
        public  DbContext GetDbContext()
        {
            //从当前线程中获取EF上下文
            DbContext dbContext = CallContext.GetData(typeof(DBContextFactory).Name) as DbContext;
            if (dbContext == null)
            {
                dbContext = new  DiscoveryDbContext();
                CallContext.SetData(typeof(DBContextFactory).Name, dbContext);
            }
            return dbContext;
        }
    }
}
