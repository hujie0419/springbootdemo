using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
   public  class DiscountManageConfigManage
    {
        public static bool Update(SE_GiftManageConfigModel model)
        {
            try
            {
                return DiscountManageConfigDal.Update(ProcessConnection.OpenConfiguration, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(SE_GiftManageConfigModel model, SE_DictionaryConfigModel log)
        {
            try
            {
                var result = DiscountManageConfigDal.Insert(ProcessConnection.OpenConfiguration, model);
                log.ParentId = result;
                //var reGiftP = SE_GiftManageConfigDAL.DeleteGiftProductConfig(result);
              return SE_GiftManageConfigDAL.InsertLog(ProcessConnection.OpenConfiguration, log) ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
