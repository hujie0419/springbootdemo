using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.PaymentWay
{
    public class PaymentWayHandler
    {
        private readonly IDBScopeManager dbManager;

        public PaymentWayHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        /// <summary>
        /// 获取支付方式
        /// </summary>
        /// <returns></returns>
        public List<PaymentWayModel> GetAllPaymentWay()
        {
            Func<SqlConnection, List<PaymentWayModel>> action = (connection) => DalPaymentWay.GetAllPaymentWay(connection);
            return dbManager.Execute(action);
        }
        /// <summary>
        /// 修改/添加
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public bool UpdatePaymentWay(List<PaymentWayModel> paymentWayModelList)
        {
            StringBuilder strSql = new StringBuilder();
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            for (int i = 0; i < paymentWayModelList.Count; i++)
            {
                var data = paymentWayModelList[i];
                //判断ID是否为空（为空则Add 反之则Update）
                if (!string.IsNullOrEmpty(data.Id.ToString()) && data.Id > 0)
                {
                    string sqlSet = string.Format(@"Payment_way_name = @Payment_way_name_{0},
                                                    Payment_way_order = @Payment_way_order_{0},
                                                    Describe = @Describe_{0},
                                                    ColorValue_1 = @ColorValue_1_{0},
                                                    ActivityInformation = @ActivityInformation_{0},
                                                    ColorValue_2 = @ColorValue_2_{0},
                                                    [State] = @State_{0},
                                                    MoreAndMore = @MoreAndMore_{0},
                                                    Field_1 = @Field_1_{0},
                                                    Field_2 = @Field_2_{0},
                                                    IOSStartVersion = @IOSStartVersion_{0},
                                                    IOSEndVersion = @IOSEndVersion_{0},
                                                    AndroidStartVersion = @AndroidStartVersion_{0},
                                                    AndroidEndVersion = @AndroidEndVersion_{0}"
                                                    , i);
                    string sqlWhere = string.Format(" Id = @Id_{0} ", i);
                    strSql.AppendFormat(" UPDATE [Gungnir].[dbo].[Payment_way_2] WITH(rowlock) SET {0} WHERE {1} ", sqlSet, sqlWhere);

                    dicParams.Add("@Payment_way_name_" + i, data.Payment_way_name);
                    dicParams.Add("@Payment_way_order_" + i, Convert.ToInt32(data.Payment_way_order));
                    dicParams.Add("@Describe_" + i, data.Describe);
                    dicParams.Add("@ColorValue_1_" + i, data.ColorValue_1);
                    dicParams.Add("@ActivityInformation_" + i, data.ActivityInformation);
                    dicParams.Add("@ColorValue_2_" + i, data.ColorValue_2);
                    dicParams.Add("@State_" + i, data.State);
                    dicParams.Add("@MoreAndMore_" + i, data.MoreAndMore);
                    dicParams.Add("@Field_1_" + i, data.Field_1);
                    dicParams.Add("@Field_2_" + i, data.Field_2);
                    dicParams.Add("@Id_" + i, data.Id);
                    dicParams.Add("@IOSStartVersion_" + i, data.IOSStartVersion);
                    dicParams.Add("@IOSEndVersion_" + i, data.IOSEndVersion);
                    dicParams.Add("@AndroidStartVersion_" + i, data.AndroidStartVersion);
                    dicParams.Add("@AndroidEndVersion_" + i, data.AndroidEndVersion);
                }
                else
                {
                    string sqlSet = string.Format("Payment_way_name,Payment_way_order,Describe,ColorValue_1,ActivityInformation,ColorValue_2,[State],MoreAndMore,Field_1,Field_2,IOSStartVersion,IOSEndVersion,AndroidStartVersion,AndroidEndVersion");
                    string sqlWhere = string.Format("@Payment_way_name_{0},@Payment_way_order_{0},@Describe_{0},@ColorValue_1_{0},@ActivityInformation_{0},@ColorValue_2_{0},@State_{0},@MoreAndMore_{0},@Field_1_{0},@Field_2_{0},@IOSStartVersion_{0},@IOSEndVersion_{0},@AndroidStartVersion_{0},@AndroidEndVersion_{0}", i);
                    strSql.AppendFormat(" INSERT into [Gungnir].[dbo].[Payment_way_2]({0}) VALUES({1}) ", sqlSet, sqlWhere);

                    dicParams.Add("@Payment_way_name_" + i, data.Payment_way_name);
                    dicParams.Add("@Payment_way_order_" + i, Convert.ToInt32(data.Payment_way_order));
                    dicParams.Add("@Describe_" + i, data.Describe);
                    dicParams.Add("@ColorValue_1_" + i, data.ColorValue_1);
                    dicParams.Add("@ActivityInformation_" + i, data.ActivityInformation);
                    dicParams.Add("@ColorValue_2_" + i, data.ColorValue_2);
                    dicParams.Add("@State_" + i, data.State);
                    dicParams.Add("@MoreAndMore_" + i, data.MoreAndMore);
                    dicParams.Add("@Field_1_" + i, data.Field_1);
                    dicParams.Add("@Field_2_" + i, data.Field_2);

                    dicParams.Add("@IOSStartVersion_" + i, data.IOSStartVersion);
                    dicParams.Add("@IOSEndVersion_" + i, data.IOSEndVersion);
                    dicParams.Add("@AndroidStartVersion_" + i, data.AndroidStartVersion);
                    dicParams.Add("@AndroidEndVersion_" + i, data.AndroidEndVersion);
                }
            }

            SqlParameter[] sqlParams = new SqlParameter[dicParams.Count];//参数值
            //修改
            for (int i = 0; i < dicParams.Count; i++)
            {
                var dicKey = dicParams.ElementAt(i).Key;
                var dicValue = dicParams.ElementAt(i).Value;
                sqlParams[i] = new SqlParameter(dicKey, dicValue);
            }
            Func<SqlConnection, bool> action = (connection) => DalPaymentWay.UpdatePaymentWay(connection, strSql.ToString(), sqlParams);
            return dbManager.Execute(action);
        }
    }
}
