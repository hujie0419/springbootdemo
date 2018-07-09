using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Component.Common.Models;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class Advertise
    {
        public int PKID { get; set; }
        public string AdColumnID { get; set; }
        public string Name { get; set; }
        public byte Position { get; set; }
        public DateTime? BeginDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public int ShowType { get; set; }
        public byte State { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public int Platform { get; set; }
        public string FunctionID { get; set; }
        public string TopPicture { get; set; }
        public int AdType { get; set; }
        public string ProductID { get; set; }
        public string ActivityID { get; set; } //活动ID
        /// <summary>暂用以控制电瓶Banner点击是否直接领券开关 </summary>
        public bool IsSendStamps { get; set; }
        /// <summary>用以控制电瓶Banner点击直接领券活动ID </summary>
        public string ActivityKey { get; set; }
    }
    public class AdProductModel : BaseModel
    {
        public AdProductModel() : base() { }
        public AdProductModel(DataRow row) : base(row) { }
        public string AdColumnID { get; set; }
        public int AdvertiseID { get; set; }
        public string PID { get; set; }
        public int Position { get; set; }
        public int state { get; set; }
    }
    public class AdvertiseModel : BaseModel
    {
        public AdvertiseModel() : base() { }
        public AdvertiseModel(DataRow row) : base(row) { }
        /// <summary>
        /// 广告唯一标识ID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 广告位ID
        /// </summary>
        public string AdColumnID { get; set; }
        /// <summary>
        /// 广告名称
        /// </summary>
        public string AdName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDateTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? ShowType { get; set; }
        /// <summary>
        /// 状态（开启或是关闭）
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AdCreateDateTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime AdUpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Platform { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FunctionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TopPicture { get; set; }
        /// <summary>
        /// 广告位类型
        /// </summary>
        public string AdType { get; set; }

        public string ProductID { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string bgColor { get; set; }

    }
    public class AdColumnModel : BaseModel
    {
        public AdColumnModel() : base() { }
        public AdColumnModel(DataRow row) : base(row) { }
        /// <summary>
        /// 广告位ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 广告位名称
        /// </summary>
        public string ADCName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity { get; set; }
        /// <summary>
        /// 切换时间
        /// </summary>
        public string SwitchMillisecond { get; set; }
        /// <summary>
        /// 默认图片
        /// </summary>
        public string DefaultImage { get; set; }
        /// <summary>
        /// 默认链接
        /// </summary>
        public string DefaultUrl { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AdcCreateDateTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime AdcUpdateTime { get; set; }
        public IEnumerable<AdvertiseModel> Advertises { get; set; }
        public IEnumerable<AdProductModel> Products { get; set; }

        /// <summary>
        /// 默认背景色
        /// </summary>
        public string DefaultbgColor { get; set; }

        public IEnumerable<AdColumnModel> Parse(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
                return new AdColumnModel[0];
            return dt.Rows
                .Cast<DataRow>()
                .GroupBy(row => row["ID"], row => row)
                .Select(group =>
                {
                    var Adcolum = new AdColumnModel(group.FirstOrDefault());
                    Adcolum.Advertises = group.Where(g => !String.IsNullOrWhiteSpace(g["AdColumnID"].ToString())).Count() == 0 ? null : group.Where(g => !String.IsNullOrWhiteSpace(g["AdColumnID"].ToString())).Select(g => new AdvertiseModel(g));
                    return Adcolum;
                }
                );
        }

    }
}
