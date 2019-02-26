using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Provisioning.DataAccess.DAO;
using Newtonsoft.Json;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business
{
   public static class SE_BrandFlagshipManager
    {
        private static SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Aliyun"].ToString());

        public static string SelectArticle(string id)
        {
        DataTable dt =   DALSE_BrandFlagship.SelectArticle(id, conn);

            return JsonConvert.SerializeObject(dt);
        }


        public static int Save(SE_BrandFlagship model)
        {
            if (model.ID == 0)
            {
                return DALSE_BrandFlagship.Add(model);
            }
            else
            {
                return DALSE_BrandFlagship.Update(model);
            }
        }

        public static IEnumerable<SE_BrandFlagship> GetList()
        {
            var dt = DALSE_BrandFlagship.GetList();
            if (dt == null || dt.Rows.Count <= 0)
                return null;
            return dt.ConvertTo<SE_BrandFlagship>();
        }


        public static SE_BrandFlagship GetEntity(string id)
        {
            SE_BrandFlagship model = DALSE_BrandFlagship.GetBrandFlagship(id).ConvertTo<SE_BrandFlagship>().FirstOrDefault();
            if (model == null)
            {
                return null;
            }
            else
            {
                IEnumerable<SE_BrandFlagshipDetail> information = DALSE_BrandFlagship.GetBrandFlagshipDeatil(id,0).ConvertTo<SE_BrandFlagshipDetail>();
                IEnumerable<SE_BrandFlagshipDetail> testing = DALSE_BrandFlagship.GetBrandFlagshipDeatil(id,1).ConvertTo<SE_BrandFlagshipDetail>();
                model.Information = information;
                model.Testing = testing;
                return model;
            }
        }


        public static bool Delete(string id)
        {
            return DALSE_BrandFlagship.Delete(id);
        }


    }
}
