using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
	/// <summary>
	/// 小程序响应用户事件配置
	/// </summary>
	public class WxAppUserEventConfigModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		public int PKID { get; set; }

		/// <summary>
		/// 小程序原始id
		/// </summary>
		public string OriginId { get; set; }

		/// <summary>
		/// 小程序事件类型（进入客服会话：user_enter_tempsession）
		/// </summary>
		public string EventType { get; set; }

		/// <summary>
		/// 响应消息类型（text/image/link/miniprogrampage）
		/// </summary>
		public string MsgType { get; set; }

		/// <summary>
		/// 响应消息Json
		/// </summary>
		public string ResponseJson { get; set; }

		/// <summary>
		/// 扩展字段
		/// </summary>
		public string UserData { get; set; }

		/// <summary>
		/// 是否启用(1：启用，0：禁用)
		/// </summary>
		public bool IsActive { get; set; }

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