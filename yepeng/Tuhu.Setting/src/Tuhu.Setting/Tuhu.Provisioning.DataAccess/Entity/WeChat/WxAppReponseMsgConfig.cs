using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
	/// <summary>
	/// 小程序响应消息配置
	/// </summary>
	public class WxAppReponseMsgConfigModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		public int PKID { get; set; }

		/// <summary>
		/// 响应消息Json
		/// </summary>
		public string ResponseJson { get; set; }

		/// <summary>
		/// 备留字段扩展
		/// </summary>
		public string UserData { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		public string CreateBy { get; set; }

		/// <summary>
		/// 更新人
		/// </summary>
		public string UpdateBy { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateDateTime { get; set; }

		/// <summary>
		/// 最后更新时间
		/// </summary>
		public DateTime LastUpdateDateTime { get; set; }
	}
}