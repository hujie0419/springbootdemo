using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.MDBeauty
{
    /// <summary>
    /// 逻辑处理-SE_MDBeautyPartConfigBLL 
    /// </summary>
    public partial class SE_MDBeautyPartConfigBLL
    {
        public static IEnumerable<SE_MDBeautyPartConfigModel> SelectPages(int pageIndex = 1, int pageSize = 20, string strWhere = "")
        {
            try
            {
                return SE_MDBeautyPartConfigDAL.SelectPages(ProcessConnection.OpenTuhu_GrouponReadOnly, pageIndex, pageSize, strWhere);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SE_MDBeautyPartConfigModel Select(int Id)
        {
            try
            {
                return SE_MDBeautyPartConfigDAL.Select(ProcessConnection.OpenTuhu_GrouponReadOnly, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Insert(SE_MDBeautyPartConfigModel model)
        {
            try
            {
                return SE_MDBeautyPartConfigDAL.Insert(ProcessConnection.OpenTuhu_Groupon, model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Update(SE_MDBeautyPartConfigModel model)
        {
            try
            {
                return SE_MDBeautyPartConfigDAL.Update(ProcessConnection.OpenTuhu_Groupon, model);
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
                return SE_MDBeautyPartConfigDAL.Delete(ProcessConnection.OpenTuhu_Groupon, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
