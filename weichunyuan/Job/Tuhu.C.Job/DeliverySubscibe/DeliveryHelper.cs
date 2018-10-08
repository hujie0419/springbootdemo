using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Tuhu.Component.Order.BusinessData;
using Tuhu.Component.Order.BusinessFacade;

namespace Tuhu.Yewu.WinService.JobSchedulerService.DeliverySubscibe
{
   public static class DeliveryHelper
   {
       private static readonly string Wms = ConfigurationManager.ConnectionStrings["WMS"].ConnectionString;

       public static List<SubscibeDeliveryModel> SelectSubscibeDeliveryModels()
       {
           var deliveryModels = new List<SubscibeDeliveryModel>();

           using (var connection = new SqlConnection(Wms))
           {
               connection.Open();

               var command = new SqlCommand("Delivery_SelectNeedSubscibeDelivery", connection);
               command.CommandType = CommandType.StoredProcedure;

               using (var reader = command.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       var deliveryModel = new SubscibeDeliveryModel();
                       deliveryModel.OrderID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                       deliveryModel.DeliveryCompany = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                       deliveryModel.DeliveryCode = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                       deliveryModel.DeliveryAddress = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);

                       deliveryModels.Add(deliveryModel);
                   }
               }
           }

           return deliveryModels;
       }

       public static void SubscribeDelivery(string deliveryCompany, string deliveryCode)
       {
           var salt =  DeliverySystem.GetKuaidi100Salt(deliveryCompany, deliveryCode).ToString();
           using (var connection = new SqlConnection(Wms))
           {
               connection.Open();

               var command = new SqlCommand("Delivery_SubscibeDeliveryByDeliveryCompanyAndDeliveryCode", connection);
               command.CommandType = CommandType.StoredProcedure;
               command.Parameters.Add(new SqlParameter("@DeliveryCompany", deliveryCompany ?? string.Empty));
               command.Parameters.Add(new SqlParameter("@DeliveryCode", deliveryCode ?? string.Empty));
               command.Parameters.Add(new SqlParameter("@Salt", salt));

               command.ExecuteNonQuery();
           }
       }
    }
}
