using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business
{
    /// <summary>
    /// 逻辑处理-SE_MDRecommendConfigBLL 
    /// </summary>
    public partial class SE_MDRecommendConfigBLL
    {
        public static IEnumerable<SE_MDRecommendConfigModel> SelectPages(int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            try
            {
                return SE_MDRecommendConfigDAL.SelectPages(ProcessConnection.OpenTuhu_GrouponReadOnly, pageIndex, pageSize, strWhere);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SE_MDRecommendConfigModel GetProducts(int part, int type, int vtype)
        {
            try
            {
                return SE_MDRecommendConfigDAL.GetProducts(ProcessConnection.OpenTuhu_GrouponReadOnly, part, type, vtype);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SE_MDRecommendConfigModel GetEntity(int Id)
        {
            try
            {
                return SE_MDRecommendConfigDAL.GetEntity(ProcessConnection.OpenTuhu_GrouponReadOnly, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(SE_MDRecommendConfigModel model)
        {
            try
            {
                return SE_MDRecommendConfigDAL.Insert(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(SE_MDRecommendConfigModel model)
        {
            try
            {
                return SE_MDRecommendConfigDAL.Update(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Delete(int Id)
        {
            try
            {
                return SE_MDRecommendConfigDAL.Delete(ProcessConnection.OpenTuhu_Groupon, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string IsExistsPId(string pid)
        {
            try
            {
                return SE_MDRecommendConfigDAL.IsExistsPId(ProcessConnection.OpenGungnirReadOnly, pid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
