using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SeckillListModel
    {
        public SeckillListModel(DateTime dt, string schedule, List<ScheduleModel> scheduleModels)
        {
            var week = dt.DayOfWeek.ToString();
            ShortDate = dt.ToShortDateString();
            Week = week;

            switch (week)
            {
                case "Monday":
                    break;
                case "Tuesday":
                    dt = dt.AddDays(-1);
                    break;
                case "Wednesday":
                    dt = dt.AddDays(-2);
                    break;
                case "Thursday":
                    dt = dt.AddDays(-3);
                    break;
                case "Friday":
                    dt = dt.AddDays(-4);
                    break;
                case "Saturday":
                    dt = dt.AddDays(-5);
                    break;
                case "Sunday":
                    dt = dt.AddDays(-6);
                    break;
                default:
                    break;
            }
            Schedule = schedule;
            var scheduleDetail = new ScheduleDetail();
            Day1 = scheduleDetail.GenerateScheduleDetail(0, dt, schedule, scheduleModels, scheduleDetail);
            Day2 = scheduleDetail.GenerateScheduleDetail(1, dt, schedule, scheduleModels, scheduleDetail);
            Day3 = scheduleDetail.GenerateScheduleDetail(2, dt, schedule, scheduleModels, scheduleDetail);
            Day4 = scheduleDetail.GenerateScheduleDetail(3, dt, schedule, scheduleModels, scheduleDetail);
            Day5 = scheduleDetail.GenerateScheduleDetail(4, dt, schedule, scheduleModels, scheduleDetail);
            Day6 = scheduleDetail.GenerateScheduleDetail(5, dt, schedule, scheduleModels, scheduleDetail);
            Day7 = scheduleDetail.GenerateScheduleDetail(6, dt, schedule, scheduleModels, scheduleDetail);
            Default = scheduleDetail.DefaultGenerateDefaultActivity(schedule, Schedule);
        }
        public string Schedule { get; set; }
        public string Week { get; set; }
        public string ShortDate { get; set; }
        public ScheduleDetail Day1 { get; set; }
        public ScheduleDetail Day2 { get; set; }
        public ScheduleDetail Day3 { get; set; }
        public ScheduleDetail Day4 { get; set; }
        public ScheduleDetail Day5 { get; set; }
        public ScheduleDetail Day6 { get; set; }
        public ScheduleDetail Day7 { get; set; }
        public ScheduleDetail Default { get; set; }
    }
    public class ScheduleModel
    {
        public List<ScheduleDetail> Schedule { get; set; }
        public string Week { get; set; }

        public string ShortDate { get; set; }
    }
    public class ScheduleDetail
    {
        public ScheduleDetail()
        {

        }
        public ScheduleDetail(int status, DateTime sDate, DateTime eDate)
        {

            if (eDate < DateTime.Now)
            {
                this.Status = 6;
                this.StrStatus = "已结束";
            }
            if (sDate > DateTime.Now)
            {
                switch (status)
                {
                    case 0:
                        this.Status = 3;
                        this.StrStatus = "已发布";
                        break;

                    case 1:

                        this.Status = 2;
                        this.StrStatus = "待审核";
                        break;
                    case 2:
                        this.Status = 4;
                        this.StrStatus = "已驳回";
                        break;
                }
            }
            if (sDate < DateTime.Now && eDate > DateTime.Now)
            {
                switch (status)
                {
                    case 0:
                        this.Status = 5;
                        this.StrStatus = "售卖中";
                        break;
                    case 1:

                        this.Status = 2;
                        this.StrStatus = "待审核";
                        break;
                    case 2:
                        this.Status = 4;
                        this.StrStatus = "已驳回";
                        break;
                }

            }
            this.Week = sDate.DayOfWeek.ToString();
            this.ShortDate = sDate.ToString("yyyy-MM-dd");
            var hour = sDate.Hour;
            Schedule = ConvertToSchedule(hour);
        }
        public string StrStatus { get; set; }

        public int Status { get; set; }

        public string ActivityId { get; set; }

        public int Count { get; set; }

        public string Schedule { get; set; }
        public string Week { get; set; }

        public string ShortDate { get; set; }

        public bool IsDefault { get; set; }

        public string ConvertToSchedule(int hour)
        {
            var s = "";
            if (hour >= 20 && hour <= 24)
            {
                s = "20点场";
            }
            if (hour >= 0 && hour <= 9)
            {
                s = "0点场";
            }
            if (hour >= 10 && hour <= 12)
            {
                s = "10点场";
            }
            if (hour >= 13 && hour <= 15)
            {
                s = "13点场";
            }
            if (hour >= 16 && hour <= 19)
            {
                s = "16点场";
            }
            return s;
        }

        public Tuple<int, string> CompareSchedule(string sSchedule, string tSchedule, DateTime dt)
        {
            if (String.CompareOrdinal(dt.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd")) < 0)
            {
                return new Tuple<int, string>(6, "已结束");
            }
            if (String.CompareOrdinal(dt.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd")) > 0)
            {
                return new Tuple<int, string>(1, "新增");
            }
            switch (sSchedule)
            {
                case "0点场":
                    if (tSchedule == "0点场")
                        return new Tuple<int, string>(5, "售卖中");
                    else
                    {
                        return new Tuple<int, string>(6, "已结束");
                    }
                case "10点场":

                    if (tSchedule == "0点场")
                        return new Tuple<int, string>(1, "新增");
                    else
                    {
                        if (tSchedule == "10点场")
                            return new Tuple<int, string>(5, "售卖中");
                        else
                        {
                            return new Tuple<int, string>(6, "已结束");
                        }
                    }
                case "13点场":
                    if (tSchedule == "0点场" || tSchedule == "10点场")
                        return new Tuple<int, string>(1, "新增");
                    else
                    {
                        if (tSchedule == "13点场")
                            return new Tuple<int, string>(5, "售卖中");
                        else
                        {
                            return new Tuple<int, string>(6, "已结束");
                        }
                    }
                case "16点场":
                    if (tSchedule == "0点场" || tSchedule == "10点场" || tSchedule == "13点场")
                        return new Tuple<int, string>(1, "新增");
                    else
                    {
                        if (tSchedule == "16点场")
                            return new Tuple<int, string>(5, "售卖中");
                        else
                        {
                            return new Tuple<int, string>(6, "已结束");
                        }
                    }
                case "20点场":
                    if (tSchedule != "20点场")
                        return new Tuple<int, string>(1, "新增");
                    else
                    {
                        return new Tuple<int, string>(5, "售卖中");
                    }
                default:
                    return new Tuple<int, string>(1, "新增");
            }
        }

        public ScheduleDetail GenerateDefaultActivity(DateTime dt, string schedule, string shortDate)
        {
            var status = CompareSchedule(schedule, ConvertToSchedule(DateTime.Now.Hour), dt);
            var models = new List<QiangGouProductModel>();
            if (status.Item1 != 1)
            {
                models = DalSeckill.SelectDefultActivityBySchedule(schedule)?.ToList() ?? new List<QiangGouProductModel>(); ;
            }
            var first = models.FirstOrDefault();
            if (first == null || first.ActivityID == new Guid())
            {
                return new ScheduleDetail
                {
                    Status = 1,
                    StrStatus = "新建",
                    Schedule = schedule,
                    IsDefault = false,
                    ShortDate = shortDate
                };
            }
            return new ScheduleDetail
            {
                Status = status.Item1,
                StrStatus = status.Item2,
                ActivityId = models.Select(r => r.ActivityID).FirstOrDefault().ToString(),
                Count = models.Count,
                Schedule = schedule,
                ShortDate = shortDate,
                IsDefault = true
            };
        }

        public ScheduleDetail DefaultGenerateDefaultActivity(string schedule, string shortDate)
        {
            var models = DalSeckill.SelectDefultActivityBySchedule(schedule)?.ToList() ?? new List<QiangGouProductModel>(); ;
            var first = models.FirstOrDefault();
            if (first == null || first.ActivityID == new Guid())
            {
                models = DalSeckill.SelectDefultActivityTempBySchedule(schedule)?.ToList() ?? new List<QiangGouProductModel>();
                 first = models.FirstOrDefault();
                if (first == null || first.ActivityID == new Guid())
                {
                    return new ScheduleDetail
                    {
                        Status = 1,
                        StrStatus = "新建",
                        ShortDate = shortDate,
                        Schedule = schedule,
                        IsDefault = true,
                    };
                }
            }
            var status = DalSeckill.SelectActivityStatusByActivityId(first.ActivityID.ToString());
            var strStatus = "已发布";
            var count = models.Count;
            if (status != 0)
            {
                status = status == 1 ? 2 : 4;
                   count = DalSeckill.SelectActivityProductsByActivityId(first.ActivityID.ToString());
                strStatus = status == 2
                    ? "待审核"
                    : "已驳回";
            }
            else
            {
                status = 3;
            }
            return new ScheduleDetail
            {
                Status = status,
                StrStatus = strStatus,
                ActivityId = models.Select(r => r.ActivityID).FirstOrDefault().ToString(),
                Count = count,
                Schedule = schedule,
                IsDefault = true,
                ShortDate = shortDate
            };
        }

        public ScheduleDetail GenerateScheduleDetail(int addDays, DateTime dt, string schedule, List<ScheduleModel> scheduleModels, ScheduleDetail scheduleDetail)
        {
            return scheduleModels.Where(r => r.ShortDate == dt.AddDays(addDays).ToString("yyyy-MM-dd"))
                        ?.Select(r => r.Schedule.FirstOrDefault(s => s.Schedule == schedule)).FirstOrDefault()
                        ?? scheduleDetail.GenerateDefaultActivity(dt.AddDays(addDays), schedule, dt.AddDays(addDays).ToShortDateString());
        }
    }

    //public class ActivityModel
    //{
    //    public string ActivityId { get; set; }

    //    /// <summary>
    //    /// 活动名称
    //    /// </summary>
    //    public string ActivityName { get; set; }
    //    /// <summary>
    //    /// 活动开始时间
    //    /// </summary>
    //    public DateTime StartDateTime { get; set; }
    //    /// <summary>
    //    /// 活动结束时间
    //    /// </summary>
    //    public DateTime EndDateTime { get; set; }
    //    /// <summary>
    //    /// 活动类型 0限时抢购 1天天秒杀
    //    /// </summary>
    //    public int ActiveType { get; set; }

    //    public int IsDefault { get; set; }

    //    public bool NeedExam { get; set; }
    //}

    //public class ActivityProductModel
    //{        
    //    /// <summary>
    //         /// 主键
    //         /// </summary>
    //    public int Pkid { get; set; }
    //    /// <summary>
    //    /// 活动ID
    //    /// </summary>
    //    public Guid ActivityId { get; set; }
    //    /// <summary>
    //    /// 产品ID
    //    /// </summary> 
    //    /// 
    //    public string Pid { get; set; }

    //    /// <summary>
    //    /// 促销价格
    //    /// </summary>
    //    public decimal Price { get; set; }
    //    /// <summary>
    //    /// 总限购
    //    /// </summary>
    //    public int? TotalQuantity { get; set; }

    //    /// <summary>
    //    /// 个人限购
    //    /// </summary>
    //    public int? MaxQuantity { get; set; }
    //    /// <summary>
    //    /// 已售出数量
    //    /// </summary>
    //    public int SaleOutQuantity { get; set; }

    //    /// <summary>
    //    /// 是否可以使用优惠券
    //    /// </summary>
    //    public bool IsUsePCode { get; set; }

    //    /// <summary>
    //    /// 伪原价
    //    /// </summary>
    //    public decimal? FalseOriginalPrice { get; set; }

    //    /// <summary>
    //    /// 原价
    //    /// </summary>
    //    public decimal OriginalPrice { get; set; }

    //    /// <summary>
    //    /// 配置的活动产品名称
    //    /// </summary>
    //    public string ProductName { get; set; }

    //    /// <summary>
    //    /// 排序
    //    /// </summary>
    //    public int? Position { get; set; }

    //    public bool IsShow { get; set; }

    //    public decimal? CostPrice { get; set; }

    //    /// <summary>
    //    /// 产品名称
    //    /// </summary>
    //    public string DisplayName { get; set; }

    //}
}
