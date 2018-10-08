using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class LogChangeDataManager
    {
        public static Tuple<QiangGouModel, QiangGouModel> GetLogChangeData(QiangGouModel beforeValue,
            QiangGouModel afterValue)
        {
            var returnBeforeValue=new QiangGouModel
            {
                ActivityID= beforeValue.ActivityID,
                ActivityName = beforeValue.ActivityName,
                ActiveType = beforeValue.ActiveType,
                StartDateTime=beforeValue.StartDateTime,
                EndDateTime = beforeValue.EndDateTime,
                PlaceQuantity=beforeValue.PlaceQuantity,
                NeedExam = beforeValue.NeedExam,
                IsNewUserFirstOrder = beforeValue.IsNewUserFirstOrder,
                NeedExamPids = beforeValue.NeedExamPids,
                Products = new List<QiangGouProductModel>()
            };
            var returnAfterValue=new QiangGouModel
            {
                ActivityID = afterValue.ActivityID,
                ActivityName = afterValue.ActivityName,
                ActiveType = afterValue.ActiveType,
                StartDateTime = afterValue.StartDateTime,
                EndDateTime = afterValue.EndDateTime,
                PlaceQuantity = afterValue.PlaceQuantity,
                NeedExam = afterValue.NeedExam,
                IsNewUserFirstOrder = afterValue.IsNewUserFirstOrder,
                NeedExamPids = afterValue.NeedExamPids,
                Products = new List<QiangGouProductModel>()
            };
            var beforeProducts = beforeValue.Products ?? new List<QiangGouProductModel>();
            var afterProducts=afterValue.Products ?? new List<QiangGouProductModel>();
            var afterPids = afterProducts.Select(r => r.PID).ToList();
            var beforPids = beforeProducts.Select(r => r.PID).ToList();
            var returnBeforeProducts = new List<QiangGouProductModel>();
            var returnAfterProducts = new List<QiangGouProductModel>();
            var diffPids1 = beforPids.Except(afterPids);
            var diffpids2 = afterPids.Except(beforPids);
            returnBeforeProducts.AddRange(beforeProducts.Where(r=> diffPids1.Contains(r.PID)));
            returnAfterProducts.AddRange(afterProducts.Where(r => diffpids2.Contains(r.PID)));
            foreach (var item in beforeProducts)
            {
                var afterItem = afterProducts.FirstOrDefault(r => r.PID == item.PID);
                if (afterItem != null)
                {
                    if (item.IsJoinPlace != afterItem.IsJoinPlace
                        || item.IsShow != afterItem.IsShow || item.IsUsePCode != afterItem.IsUsePCode
                        || item.Price != afterItem.Price || (item.InstallAndPay??"") != (afterItem.InstallAndPay??"")
                        || (item.TotalQuantity??0) != (afterItem.TotalQuantity??0) || (item.MaxQuantity??0) != (afterItem.MaxQuantity??0)
                        || (item.Channel??"") != (afterItem.Channel??""))
                    {
                        returnBeforeProducts.Add(item);
                        returnAfterProducts.Add(afterItem);
                    }
                }
            }
            returnBeforeValue.Products = returnBeforeProducts;
            returnAfterValue.Products = returnAfterProducts;
            return new Tuple<QiangGouModel, QiangGouModel>(returnBeforeValue, returnAfterValue);
        }
    }
}
