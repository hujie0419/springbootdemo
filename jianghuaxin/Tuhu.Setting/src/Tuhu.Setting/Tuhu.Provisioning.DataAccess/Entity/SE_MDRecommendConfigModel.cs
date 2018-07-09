using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{	
	/// <summary>
	/// 实体-SE_MDRecommendConfigModel 
	/// </summary>
	public partial class SE_MDRecommendConfigModel
	{

        /// <summary>
        /// 主键标识
        /// </summary>		
        public int Id { get; set; }

        /// <summary>
        /// 美容栏目ID
        /// </summary>		
        public int PartId { get; set; }

        /// <summary>
        /// 车档类型 1 端车，2中端车，3高端车
        /// </summary>		
        public int Type { get; set; }

        /// <summary>
        /// 车品牌类型 1 轿车，2 SUV，3 MPV，4 SUV/MPV
        /// </summary>		
        public int VehicleType { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>		
        public bool IsDisable { get; set; }

        /// <summary>
        /// 关联产品
        /// </summary>
        public string Products { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public DateTime CreateTime { get; set; }

    }
}
