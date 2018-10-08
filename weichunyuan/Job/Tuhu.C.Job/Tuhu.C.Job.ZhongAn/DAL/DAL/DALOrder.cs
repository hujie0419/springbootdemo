using K.DLL.Common;
using K.Domain;
using System.Collections.Generic;
using System.Linq;

namespace K.DLL.DAL
{
    public static class DALOrder
    {
        public static List<DOrder> SelectFirstSendOrders()
        {
            return DataOp.GetDataSet(@"SELECT PKID ,
                                              OrderDate ,
                                              UserName ,
                                              UserTel ,
                                              CarPlate ,
                                              StoreAddress ,
                                              StoreName,
                                              OrderNo
                                       FROM ZhonganDeliveryData WITH(NOLOCK)", null).ToIList<DOrder>().ToList();
        }

        public static List<DOrder> SelectSecondSendOrders()
        {
            return DataOp.GetDataSet(@"SELECT PKID ,
                                              OrderDate ,
                                              UserName ,
                                              UserTel ,
                                              CarPlate ,
                                              StoreAddress ,
                                              StoreName,
                                              OrderNo
                                       FROM ZhonganInstallData WITH(NOLOCK)", null).ToIList<DOrder>().ToList();
        }
    }
}
