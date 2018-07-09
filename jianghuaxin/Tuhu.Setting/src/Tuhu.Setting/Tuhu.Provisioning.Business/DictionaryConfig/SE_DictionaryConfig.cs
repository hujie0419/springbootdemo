using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business
{
   public static class SE_DictionaryConfigManager
    {


        public static int Save(SE_DictionaryConfigModel entity)
        {
            if (entity.Id <= 0)
            {//添加
                if (DALSE_DictionaryConfig.Exist(entity))
                {
                    return -1;
                }
                else
                {
                    if (DALSE_DictionaryConfig.Add(entity))
                    {
                        return 1;
                    }
                    else
                        return 0;
                }
            }
            else
            {
                if (DALSE_DictionaryConfig.Update(entity))
                    return 1;
                else
                    return 0;
            }
        }


        public static List<SE_DictionaryConfigModel> GetList()
        {
            return DALSE_DictionaryConfig.GetTable().ConvertTo<SE_DictionaryConfigModel>().ToList();
        }

        public static SE_DictionaryConfigModel GetEntity(string id)
        {
            return DALSE_DictionaryConfig.GetEntity(id).ConvertTo<SE_DictionaryConfigModel>().ToList().FirstOrDefault();
        }

        public static bool Delete(string id)
        {
            return DALSE_DictionaryConfig.Delete(id);
        }

    }
}
