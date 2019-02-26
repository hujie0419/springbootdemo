using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public enum SearchWordConfigType
    {
        Config,
        NewWord,
        VehicleType
    }
    public class SearchWordConvertMapDb
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        /// 目标词
        /// </summary>
        public string TargetWord { get; set; }

        /// <summary>
        /// 源词
        /// </summary>
        public string SourceWord { get; set; }

        /// <summary>
        /// 客户端搜索时搜索栏下方搜索词是否转换标识
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 二级车型ID
        /// </summary>
        public string VehicleID { get; set; }

        /// <summary>
        /// 排序（升序）
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string TireSize { get; set; }

        /// <summary>
        /// 特殊规格
        /// </summary>
        public string SpecialTireSize { get; set; }

        /// <summary>
        /// 客户端展示二级车型名称（Brand+Vehicle）
        /// </summary>
        public string VehicleName { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public string UpdateBy { get; set; }

    }
}
