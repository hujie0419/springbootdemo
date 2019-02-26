using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ServiceTag
{
    public class ServiceTagManager
    {
        public static IEnumerable<ServiceTagModel> SelectServiceTag()
        {
            List<ServiceTagModel> list = new List<ServiceTagModel>();
            var dt = DALServiceTag.SelectServiceTag();
            if (dt == null || dt.Rows.Count == 0)
                return new ServiceTagModel[0];

            var dic = dt.Rows.Cast<DataRow>().Select(row => new ServiceTagModel(row)).GroupBy(c => c.PKID).ToDictionary(r => r.Key, r => r.ToList());
            foreach (var value in dic.Values)
            {
                var model = new ServiceTagModel()
                {
                    PKID = value.FirstOrDefault().PKID,
                    Type = value.FirstOrDefault().Type,
                    ServiceTag = value.FirstOrDefault().ServiceTag,
                    ServiceDescribe=value.FirstOrDefault().ServiceDescribe,
                    ProductTag=value.FirstOrDefault().ProductTag,
                    ProductDescribe=value.FirstOrDefault().ProductDescribe,
                    ServiceDescribeTID = value.FirstOrDefault().ServiceDescribeTID,
                    ServiceIDs = new List<string>(),
                    DisplayNames = new List<string>()
                };
                foreach (var item in value)
                {
                    if (item.PID != "" && item.PID != null)
                    {
                        model.ServiceIDs.Add(item.PID);
                    }
                    else if (item.Category != "" && item.Category != null)
                    {
                        model.ServiceIDs.Add(item.Category);
                        model.DisplayNames.Add(item.DisplayName);
                    }
                    else { }

                }
                if (model.ServiceIDs.Count() > 0)
                {
                    model.StrServiceIDs = string.Join(";", model.ServiceIDs);
                    model.StrDisplayNames = string.Join(";", model.DisplayNames);
                }

                list.Add(model);
            }
            return list;
        }

        public static int DeleteTag(int PKID)
        {
            return DALServiceTag.DeleteTag(PKID);
        }

        public static int InsertAndUpdateTag(IEnumerable<ServiceTagModel> list)
        {
            if (list == null || list.Count() == 0)
                return -1;
            return DALServiceTag.InsertAndUpdateTag(list);
        }

        public static int VaildatePID(string strPIDs, ref string falsePids)
        {
            var dt = DALServiceTag.VaildatePID(strPIDs);
            if (dt == null || dt.Rows.Count == 0)
                return -1;
            List<string> listAll = strPIDs.Split(';').Distinct().ToList();
            foreach (DataRow dr in dt.Rows)
            {
                listAll.Remove(dr["PID"].ToString());
            }
            listAll.Remove("");
            if (listAll.Count() == 0)
                return 99;
            else
            {
                falsePids = string.Join(";", listAll);
                return -2;
            }
        }

    }
}
