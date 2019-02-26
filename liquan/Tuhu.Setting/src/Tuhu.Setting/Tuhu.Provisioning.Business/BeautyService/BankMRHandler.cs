using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.BankMRActivityDal;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;

namespace Tuhu.Provisioning.Business.BeautyService
{
    internal class BankMRHandler
    {
        /// <summary>
        /// 生成银行美容活动场次
        /// </summary>
        /// <returns></returns>
        public bool GenerateBankMRActivityRound(SqlConnection conn, BankMRActivityConfig config)
        {
            var result = false;
            var startTime = config.StartTime;
            var endTime = config.StartTime;
            switch (config.RoundCycleType)
            {
                case "year":
                    while (endTime <= config.EndTime)
                    {
                        endTime = endTime.AddYears(1);
                        if (endTime > config.EndTime)
                        {
                            endTime = config.EndTime;
                        }
                        var roundConfig = new BankMRActivityRoundConfig()
                        {
                            ActivityId = config.ActivityId,
                            StartTime = startTime,
                            EndTime = endTime,
                            IsActive = true
                        };
                        result = BankMRActivityDal.InsertBankMRActivityRoundConfig(conn, roundConfig);
                        if (!result)
                            return result;
                        startTime = endTime = endTime.AddDays(1);
                    }

                    break;
                case "quarter":
                    while (endTime <= config.EndTime)
                    {
                        switch (startTime.Month)
                        {
                            case 1:
                            case 2:
                            case 3:
                                endTime = new DateTime(startTime.Year, 3, 31);
                                if (endTime > config.EndTime)
                                    endTime = config.EndTime;
                                result = BankMRActivityDal.InsertBankMRActivityRoundConfig(conn, new BankMRActivityRoundConfig()
                                {
                                    ActivityId = config.ActivityId,
                                    StartTime = startTime,
                                    EndTime = endTime,
                                    IsActive = true
                                });
                                if (!result)
                                    return result;
                                startTime = endTime = endTime.AddDays(1);
                                break;
                            case 4:
                            case 5:
                            case 6:
                                endTime = new DateTime(startTime.Year, 7, 1);
                                endTime = endTime.AddDays(-1);
                                if (endTime > config.EndTime)
                                    endTime = config.EndTime;
                                result = BankMRActivityDal.InsertBankMRActivityRoundConfig(conn, new BankMRActivityRoundConfig()
                                {
                                    ActivityId = config.ActivityId,
                                    StartTime = startTime,
                                    EndTime = endTime,
                                    IsActive = true
                                });
                                if (!result)
                                    return result;
                                startTime = endTime = endTime.AddDays(1);
                                break;
                            case 7:
                            case 8:
                            case 9:
                                endTime = new DateTime(startTime.Year, 10, 1);
                                endTime = endTime.AddDays(-1);
                                if (endTime > config.EndTime)
                                    endTime = config.EndTime;
                                result = BankMRActivityDal.InsertBankMRActivityRoundConfig(conn, new BankMRActivityRoundConfig()
                                {
                                    ActivityId = config.ActivityId,
                                    StartTime = startTime,
                                    EndTime = endTime,
                                    IsActive = true
                                });
                                if (!result)
                                    return result;
                                startTime = endTime = endTime.AddDays(1);
                                break;
                            case 10:
                            case 11:
                            case 12:
                                endTime = new DateTime(startTime.Year, 12, 31);
                                if (endTime > config.EndTime)
                                    endTime = config.EndTime;
                                result = BankMRActivityDal.InsertBankMRActivityRoundConfig(conn, new BankMRActivityRoundConfig()
                                {
                                    ActivityId = config.ActivityId,
                                    StartTime = startTime,
                                    EndTime = endTime,
                                    IsActive = true
                                });
                                if (!result)
                                    return result;
                                startTime = endTime = endTime.AddDays(1);
                                break;
                        }
                    }
                    break;
                case "month":
                    while (endTime <= config.EndTime)
                    {
                        var monthIndex = endTime.Month;
                        while (monthIndex == endTime.Month)
                        {
                            endTime = endTime.AddDays(1);
                            if (endTime > config.EndTime)
                                break;
                        };
                        var roundConfig = new BankMRActivityRoundConfig()
                        {
                            ActivityId = config.ActivityId,
                            StartTime = startTime,
                            EndTime = endTime.AddDays(-1),
                            IsActive = true
                        };
                        result = BankMRActivityDal.InsertBankMRActivityRoundConfig(conn, roundConfig);
                        if (!result)
                            return result;
                        startTime = endTime;
                    }
                    break;
                case "week":
                    while (endTime <= config.EndTime)
                    {
                        while (((int)endTime.DayOfWeek) > 0)
                        {
                            endTime = endTime.AddDays(1);
                        };
                        if (endTime > config.EndTime)
                            endTime = config.EndTime;
                        var roundConfig = new BankMRActivityRoundConfig()
                        {
                            ActivityId = config.ActivityId,
                            StartTime = startTime,
                            EndTime = endTime,
                            IsActive = true
                        };
                        result = BankMRActivityDal.InsertBankMRActivityRoundConfig(conn, roundConfig);
                        if (!result)
                            return result;
                        startTime = endTime = endTime.AddDays(1);
                    }

                    break;
            }

            return result;
        }
        /// <summary>
        /// 批量导入银行美容活动用户规则
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        internal bool BatchImportBankMRActivityUsers(SqlConnection conn, IEnumerable<BankMRActivityUser> list)
        {
            var dt = new DataTable { TableName = "Tuhu_groupon.dbo.BankMRActivityUsers" };
            dt.Columns.Add(new DataColumn("ActivityRoundId", typeof(int)));
            dt.Columns.Add(new DataColumn("BankCardNum", typeof(string)));
            dt.Columns.Add(new DataColumn("Mobile", typeof(string)));
            dt.Columns.Add(new DataColumn("OtherNum", typeof(string)));
            dt.Columns.Add(new DataColumn("LimitCount", typeof(int)));
            dt.Columns.Add(new DataColumn("DayLimit", typeof(int)));
            dt.Columns.Add(new DataColumn("BatchCode", typeof(string)));
            list?.ToList().ForEach(item =>
            {
                var row = dt.NewRow();
                row["ActivityRoundId"] = item.ActivityRoundId;
                row["BankCardNum"] = item.BankCardNum;
                row["Mobile"] = item.Mobile;
                row["OtherNum"] = item.OtherNum;
                row["LimitCount"] = item.LimitCount;
                row["DayLimit"] = item.DayLimit;
                row["BatchCode"] = item.BatchCode;
                dt.Rows.Add(row);
            });
            return BankMRActivityDal.BatchImportBankMRActivityUsers(conn, dt);
        }

        /// <summary>
        /// 批量导入银行活动白名单
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        internal bool BatchImportBankActivityWhiteUsers(SqlConnection conn, IEnumerable<BankActivityWhiteUsers> list)
        {
            var dt = new DataTable { TableName = "Tuhu_groupon.dbo.BankActivityWhiteUsers" };
            dt.Columns.Add(new DataColumn("GroupConfigId", typeof(int)));
            dt.Columns.Add(new DataColumn("CardNum", typeof(string)));
            dt.Columns.Add(new DataColumn("Mobile", typeof(string)));
            list?.ToList().ForEach(item =>
            {
                var row = dt.NewRow();
                row["GroupConfigId"] = item.GroupConfigId;
                row["CardNum"] = item.CardNum;
                row["Mobile"] = item.Mobile;
                dt.Rows.Add(row);
            });
            return BankMRActivityDal.BatchImportBankMRActivityUsers(conn, dt);
        }
    }
}
