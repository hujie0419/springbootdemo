using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business
{
   public class CarTagManager
    {

      
        public CarTagManager()
        {
          
        }


        public bool Add(SE_CarTagCouponConfig model)
        {
            return new DALCarTag().Add(model);
        }


        public bool UpdateStatus(int id, int status) => new DALCarTag().UpdateStatus(id, status);


        public IEnumerable<SE_CarTagCouponConfig> GetList() => new DALCarTag().GetList();

        public bool UpdateDateTime(DateTime startDateTime, DateTime endDateTime,string name) => new DALCarTag().UpdateDateTime(startDateTime, endDateTime,name);


        public bool UpdateImageUrl(int id, string url) => new DALCarTag().UpdateImageUrl(id, url);


    }
}
