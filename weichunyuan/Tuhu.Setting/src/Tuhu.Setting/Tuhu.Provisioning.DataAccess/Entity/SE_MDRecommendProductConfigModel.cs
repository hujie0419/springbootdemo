using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{	
	/// <summary>
	/// 实体-SE_MDRecommendProductConfigModel 
	/// </summary>
	public partial class SE_MDRecommendProductConfigModel
	{
        /// <summary>
        /// 主键标识
        /// </summary>		
        public int Id { get; set; }

        /// <summary>
        /// 推荐栏目ID
        /// </summary>		
        public int RecommendId { get; set; }

        /// <summary>
        /// 产品Code
        /// </summary>		
        public string ProductCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public DateTime CreateTime { get; set; }

    }
}
