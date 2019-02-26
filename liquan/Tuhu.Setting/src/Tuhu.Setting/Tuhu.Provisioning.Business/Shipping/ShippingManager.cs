using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Shipping
{
    public class ShippingManager
    {
        public static List<GradeDeliveryFeeRule> GradeDeliveryFeeRule(GradeDeliveryFeeRule model)
        {
            return DALShipping.GetGradeDeliveryFeeRule(model);
        }

        public static bool InsertGradeDeliveryFeeRule(GradeDeliveryFeeRule model)
        {
            return DALShipping.InsertGradeDeliveryFeeRule(model);
        }

        public static bool UpdateGradeDeliveryFeeRule(GradeDeliveryFeeRule model)
        {
            return DALShipping.UpdateGradeDeliveryFeeRule(model);
        }
        public static bool DeleteGradeDeliveryFeeRule(GradeDeliveryFeeRule model)
        {
            return DALShipping.DeleteGradeDeliveryFeeRule(model);
        }
        public static IEnumerable<ShippingModel> SelectShippingRule()
        {
            List<ShippingModel> list = new List<ShippingModel>();
            var dt = DALShipping.SelectShippingRule();
            if (dt == null || dt.Rows.Count == 0)
                return new ShippingModel[0];
            var dic = dt.Rows.Cast<DataRow>().Select(row => new ShippingModel(row)).GroupBy(c => c.PKID).ToDictionary(r => r.Key, r => r.ToList());
            foreach (var value in dic.Values)
            {
                var model = new ShippingModel()
                {
                    PKID = value.FirstOrDefault().PKID,
                    Types = value.FirstOrDefault().Types,
                    Value = value.FirstOrDefault().Value,
                    CreateDateTime = value.FirstOrDefault().CreateDateTime,
                    LastUpdateDateTime = value.FirstOrDefault().LastUpdateDateTime,
                    UserType = value.FirstOrDefault().UserType,
                    CityIDs = new List<int>()
                };
                foreach (var item in value)
                {
                    if (item.CityID != null)
                    {
                        model.CityIDs.Add(item.CityID.GetValueOrDefault());
                    }

                }
                if (model.CityIDs.Count() > 0)
                {
                    model.StrCityIDs = string.Join("|", model.CityIDs);
                    model.StrCityNames = SelectCityNameByCityIDs(model.CityIDs);
                }

                list.Add(model);
            }
            return list;
        }

        public static string SelectCityNameByCityIDs(List<int> citys)
        {

            var dt = DALShipping.SelectCityNameByCityIDs(citys);
            if (dt.Rows.Count == 0 || dt == null)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append(dr["RegionName"].ToString() + "、");
            }
            var names = sb.ToString();

            //return names.Length > 50 ? names.Substring(0, 50) + "......" : names.Substring(0, names.Length - 1);
            return names.Substring(0, names.Length - 1);
        }

        public static int InsertAndUpdateaRule(IEnumerable<ShippingModel> list)
        {
            if (list == null || list.Count() == 0)
                return -1;
            return DALShipping.InsertAndUpdateaRule(list);
        }

        public static int DelRule(int pkid)
        {
            return DALShipping.DelRule(pkid);
        }
        public static int InsertOprLog(string ObjectType, int ObjectID, string Operation, string Author, string HostName, string IPAddress)
        {
            OprLogManager log = new OprLogManager();

            OprLog model = new OprLog();
            model.ObjectID = ObjectID;
            model.ObjectType = ObjectType;
            model.Operation = Operation;
            model.Author = Author;
            model.HostName = HostName;
            model.IPAddress = IPAddress;
            try
            {
                log.AddOprLog(model);
                return 1;
            }
            catch
            {
                return 0;
            }
            //return DALShipping.InsertOprLog(ObjectType,ObjectID, Operation, Author, HostName, IPAddress);
        }
    }
}
