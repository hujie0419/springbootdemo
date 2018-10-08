using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public static class SeckillManager
    {
        public enum OpertionType
        {
            Add = 1,
            EditAdd = 2,
            EditDelete = 3,
            EditEdit = 4,
            ApprovePass=5,
            ApproveBack=6
        }
        public static List<SeckillListModel> SelectSeckillList(DateTime dt)
        {
            var data1 = DalSeckill.SelectQiangGouAndProducts(dt);
            var data2 = DalSeckill.SelectQiangGouTempAndProducts(dt);
            var data2aids = data2.Select(r => r.ActivityID).ToList();
            //var data3 = data2.Except(data1);
            var dbResult = data1.Where(r=>!data2aids.Contains(r.ActivityID)).Union(data2).ToList();
            var scheduleDetails = dbResult.GroupBy(r => new
            {
                r.ActivityID,
                r.StartDateTime,
                r.EndDateTime
            })
                .Select(
                    d =>
                        new ScheduleDetail(DalSeckill.SelectActivityStatusByActivityId(d.Key.ActivityID.ToString()),
                            d.Key.StartDateTime, d.Key.EndDateTime)
                        {
                            ActivityId = d.Key.ActivityID.ToString(),
                            Count = d.Count()
                        });
            var scheduleModels = scheduleDetails.GroupBy(r => new
            {
                r.ShortDate,
                r.Week

            }).Select(d => new ScheduleModel()
            {
                Week = d.Key.Week,
                ShortDate = d.Key.ShortDate,
                Schedule = d.Select(s => new ScheduleDetail()
                {
                    ActivityId = s.ActivityId,
                    Count = s.Count,
                    Schedule = s.Schedule,
                    Status = s.Status,
                    ShortDate = s.ShortDate,
                    StrStatus = s.StrStatus
                }).ToList()
            }).ToList();

            return new List<SeckillListModel>
            {
                new SeckillListModel(dt, "0点场", scheduleModels),
                new SeckillListModel(dt, "10点场", scheduleModels),
                new SeckillListModel(dt, "13点场", scheduleModels),
                new SeckillListModel(dt, "16点场", scheduleModels),
                new SeckillListModel(dt, "20点场", scheduleModels),

            };
        }

        public static QiangGouModel FetchNeedExamQiangGouAndProducts(Guid aid)
        {
            try
            {
                var dt = DalSeckill.FetchNeedExamQiangGouAndProducts(aid);
                if (dt == null || dt.Rows.Count == 0)
                    return null;
                var model = dt.ConvertTo<QiangGouModel>()?.FirstOrDefault() ?? new QiangGouModel();
                var pids = dt.ConvertTo<QiangGouProductModel>().Select(r => r.PID).ToList();
                var costPriceSql = DalSeckill.SelectProductCostPriceByPids(pids);
                var products = from a in dt.ConvertTo<QiangGouProductModel>()
                               join b in costPriceSql on a.PID equals b.PID into temp
                               from b in temp.DefaultIfEmpty()
                               select new QiangGouProductModel
                               {
                                   PKID = a.PKID,
                                   ActivityID = a.ActivityID,
                                   ActivityName = a.ActivityName,
                                   PID = a.PID,
                                   HashKey = a.HashKey,
                                   Price = a.Price,
                                   TotalQuantity = a.TotalQuantity,
                                   MaxQuantity = a.MaxQuantity,
                                   SaleOutQuantity = a.SaleOutQuantity,
                                   InstallAndPay = a.InstallAndPay,
                                   IsUsePCode = a.IsUsePCode,
                                   Channel = a.Channel,
                                   IsJoinPlace = a.IsJoinPlace,
                                   FalseOriginalPrice = a.FalseOriginalPrice,
                                   DisplayName = a.DisplayName,
                                   OriginalPrice = a.OriginalPrice,
                                   ProductName = a.ProductName,
                                   Label = a.Label,
                                   CostPrice = b?.CostPrice,
                                   Position = a.Position,
                                   IsShow = a.IsShow,
                                   Image = a.Image,
                                   InstallService = a.InstallService,
                                   DecreaseDegree = (Math.Round((a.OriginalPrice - a.Price) / a.OriginalPrice, 2) * 100) + "%"
                               };
                model.Products = products;
                return model;
            }
            catch (Exception ex)
            {
                return new QiangGouModel();
            }

        }

        public static int UpdateSeckillToToApprove(string acid)
        {
            return DalSeckill.UpdateSeckillToToApprove(acid);
        }
        public static int DeleteStatusData(string acid)
        {
            return DalSeckill.DeleteStatusData(acid);
        }

        public static int ApproveBack(string acid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                dbHelper.BeginTransaction();
                try
                {
                    //var result1 = DalSeckill.DeleteFlashSaleTempByAcid(acid);
                    //if (result1 <= 0)
                    //{
                    //    dbHelper.Rollback();
                    //    return -1;
                    //}
                    //var result2 = DalSeckill.DeleteFlashSaleProductsTempByAcid(acid);
                    //if (result2 <= 0)
                    //{
                    //    dbHelper.Rollback();
                    //    return -1;
                    //}
                    var result3 = DalSeckill.UpdateSeckillToToApprove(acid,2);
                    if (result3 <= 0)
                    {
                        dbHelper.Rollback();
                        return -1;
                    }
                    dbHelper.Commit();
                    return 1;
                }
                catch (Exception e)
                {
                    dbHelper.Rollback();
                    throw new Exception(e.Message); ;
                }
            }
        }


        public static int SelectQiangGouIsExist(string acid) => DalSeckill.SelectQiangGouIsExist(acid);


        public static bool OpertionLogs(OpertionType type, string beforevalue, string afterValue, string activityId)
        {
            var oprLog = new FlashSaleProductOprLog
            {
                OperateUser = ThreadIdentity.Operator.Name,
                CreateDateTime = DateTime.Now,
                BeforeValue = beforevalue,
                AfterValue = afterValue,
                LogType = "Seckill",
                LogId = activityId,
                Operation = ""
            };
            switch (type)
            {
                case OpertionType.Add:
                    oprLog.Operation = "Add";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    return true;
                case OpertionType.EditAdd:
                    oprLog.Operation = "EditAdd";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    return true;
                case OpertionType.EditDelete:
                    oprLog.Operation = "EditDelete";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    return true;
                case OpertionType.EditEdit:
                    oprLog.Operation = "EditEdit";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    return true;
                case OpertionType.ApprovePass:
                    oprLog.Operation = "ApprovePass";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    return true;
                case OpertionType.ApproveBack:
                    oprLog.Operation = "ApproveBack";
                    LoggerManager.InsertLog("FlashSaleOprLog", oprLog);
                    return true;
                default:
                    return true;
            }
        }

        public static int DeleteFirstCreateActivityApproveBack(string aid)
        {
            var mainDelete = DalSeckill.DeleteFlashSaleTempByAcid(aid);
            if (mainDelete <= 0)
            {
                return -1;
            }
            var subDelete = DalSeckill.DeleteFlashSaleProductsTempByAcid(aid);
            if (subDelete <= 0)
            {
                return -1;
            }
            return 1;
        }

        public static List<string> SelectActivityProductPids(Guid aid)
        {
            return DalSeckill.SelectActivityProductPids(aid);
        }
    }
}
