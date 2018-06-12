using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ThBiz.Common.Entity;
using Tuhu.Component.Common;
using Tuhu.Component.ExportImport;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Monitor;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.OprLog;

namespace Tuhu.Provisioning.Business.PromotionCodeManagerment
{
	public class PromotionCodeManager : IPromotionCodeManager
	{

		#region Private Fields
		private static readonly IConnectionManager connectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);

        private static readonly IConnectionManager alwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly IDBScopeManager AlwaysOnReadDbScopeManager = new DBScopeManager(alwaysOnReadConnectionManager);

        private static readonly ILog Logger = LoggerFactory.GetLogger("PromotionCodeManager");

		private PromotionCodeHandler handler = null;
        private PromotionCodeHandler handlerReadOnly = null;
        #endregion
        public PromotionCodeManager()
        {
            handler = new PromotionCodeHandler(DbScopeManager);
            handlerReadOnly = new PromotionCodeHandler(AlwaysOnReadDbScopeManager);
        }

		#region Public Method

		public List<BizPromotionCode> SelectPromotionCodesByUserId(string userId)
		{
			//var handler = new PromotionCodeHandler(ConnectionManager);

			return handler.SelectPromotionCodesByUserId(userId);
		}

        /// <summary>
        /// 根据订单号查询优惠券
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
		public List<BizPromotionCode> SelectPromotionByOrderId(int OrderId)
		{
			return handler.SelectPromotionByOrderId(OrderId);
		}
		#endregion

		public IEnumerable<ExchangeCodeDetail> SelectExchangeCodeDetailByPage(int PageNumber, int PageSize, out int TotalCount)
		{
			DataTable dt = handler.SelectExchangeCodeDetailByPage(PageNumber, PageSize, out TotalCount);
			if (dt == null || dt.Rows.Count == 0)
				return null;
			return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));

		}

		public IEnumerable<ExchangeCodeDetail> SelectGiftBag(int PageNumber, int PageSize, out int TotalCount)
		{
			DataTable dt = handler.SelectGiftBag(PageNumber, PageSize, out TotalCount);
			if (dt == null || dt.Rows.Count == 0)
				return null;
			return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
		}

		public IEnumerable<ExchangeCodeDetail> SelectGiftBagByPKID(int pkid)
		{
			DataTable dt = handler.SelectGiftBagByPKID(pkid);
			if (dt == null || dt.Rows.Count == 0)
				return new List<ExchangeCodeDetail>();
			return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
		}

		public IEnumerable<ExchangeCodeDetail> GetEdit(int pkid)
		{
			DataTable dt = handler.GetEdit(pkid);
			if (dt == null || dt.Rows.Count == 0)
				return new List<ExchangeCodeDetail>();
			return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
		}

		public IEnumerable<ExchangeCodeDetail> UpdateOEM(int pkid)
		{
			DataTable dt = handler.UpdateOEM(pkid);
			if (dt == null || dt.Rows.Count == 0)
				return new List<ExchangeCodeDetail>();
			return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
		}
		public int UpdateGift(ExchangeCodeDetail ecd)
		{
			return handler.UpdateGift(ecd);
		}

		public int DoUpdateOEM(ExchangeCodeDetail ecd)
		{
			return handler.DoUpdateOEM(ecd);
		}

		public int AddGift(ExchangeCodeDetail ecd)
		{
			return handler.AddGift(ecd);
		}

		public object SelectCodeChannelByAddGift(int id)
		{
			return handler.SelectCodeChannelByAddGift(id);
		}

		public int AddOEM(ExchangeCodeDetail ecd)
		{
			return handler.AddOEM(ecd);
		}

		public string GenerateCoupon(int Number, int DetailsID)
		{
			return handler.GenerateCoupon(Number, DetailsID);

		}

		public DataTable SelectGiftByDonwLoad(int pkid)
		{
			DataTable dt = handler.SelectGiftByDonwLoad(pkid);
			return dt;
		}

		public int SelectCountByDownLoad(int pkid)
		{
			return handler.SelectDownloadByPKID(pkid);
		}

        public DataTable CreateExcel(int pkid)
        {
            return handler.CreateExcel(pkid);
        }

        public int DeletePromoCode(int pkid)
		{
			return handler.DeletePromoCode(pkid);
		}

		public int SelectPromoCodeCount(int pkid)
		{
			return handler.SelectPromoCodeCount(pkid);
		}

		public int DeleteGift(int pkid)
		{
			return handler.DeleteGift(pkid);
		}

		public DataTable SelectByPhoneNumk(string PhoneNum, int TabIndex)
		{

			var dst = new DataSet();

			var ds = new DataSet();
			ds = handler.SelectByPhoneNum(PhoneNum);
			if (ds == null || ds.Tables == null || ds.Tables.Count == 0)
			{
				return null;
			}

			var dt = ds.Tables[TabIndex];
			return dt;

		}

		public int GetOrderUsedPromtionCodeNumByOrderId(int orderId)
		{
			return handler.GetOrderUsedPromtionCodeNumByOrderId(orderId);
		}

		public static int InsertPromotionCode(SqlDbHelper dbhelper, PromotionCode PC)
		{
			return DalPromotionCode.InsertPromotionCode(dbhelper, PC);
		}

		/// <summary>
		/// 查询优惠券下拉列表
		/// </summary>
		/// <returns></returns>
		public DataTable SelectDropDownList()
		{
			return handler.SelectDropDownList();
		}

		/// <summary>
		/// 创建优惠券
		/// </summary>
		/// <param name="ecd"></param>
		/// <returns></returns>
		public int CreeatePromotion(ExchangeCodeDetail ecd)
		{
			return handler.CreeatePromotion(ecd);
		}

		/// <summary>
		/// 优惠券列表详情
		/// </summary>
		/// <param name="pkid"></param>
		/// <returns></returns>
		public IEnumerable<ExchangeCodeDetail> SelectPromotionDetails(int pkid)
		{
			DataTable dt = handler.SelectPromotionDetails(pkid);
			if (dt == null || dt.Rows.Count == 0)
				return new List<ExchangeCodeDetail>();
			return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
		}

		/// <summary>
		/// 查询优惠券详情-->修改
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IEnumerable<ExchangeCodeDetail> SelectPromotionDetailsByEdit(int id)
		{
			DataTable dt = handler.SelectPromotionDetailsByEdit(id);
			if (dt == null || dt.Rows.Count == 0)
				return new List<ExchangeCodeDetail>();
			return dt.Rows.OfType<DataRow>().Select(x => new ExchangeCodeDetail(x));
		}

		/// <summary>
		/// 修改优惠券
		/// </summary>
		/// <param name="ecd"></param>
		/// <returns></returns>
		public int UpdatePromotionDetailsByOK(ExchangeCodeDetail ecd)
		{
			return handler.UpdatePromotionDetailsByOK(ecd);
		}

		/// <summary>
		/// 创建优惠券任务
		/// </summary>
		/// <param name="promotionTask">优惠券任务对象</param>
		/// <param name="cellPhones">需要发送优惠券的用户列表</param>
		/// <returns></returns>
		public int CreatePromotionTask(PromotionTask promotionTask, List<string> TaskPromotionListIds = null, List<string> cellPhones = null)
		{
			try
			{
			    var oper = ThreadIdentity.Operator.Name;
			    var task = DalPromotionJob.GetPromotionTaskById(promotionTask.PromotionTaskId ?? 0);
			    if (task != null && task.TaskStatus > 0) //已经审核或者关闭的任务不能修改
			        return 0;
                if (promotionTask.PromotionTaskId!=null&&promotionTask.PromotionTaskId > 0)
			    {
			       
                    //如果是修改了数据源，则要释放原来的数据源
			        if (promotionTask.SelectUserType == 3 && task.PromotionTaskActivityId !=
			            promotionTask.PromotionTaskActivityId)
			        {
			            DalPromotionJob.ResetPromotionTaskActivity(task.PromotionTaskActivityId,
			                task.PromotionTaskId);
			        }
			    }
			    
			    var resultId = handler.CreatePromotionTask(promotionTask, oper, TaskPromotionListIds, cellPhones);
			    if (promotionTask.SelectUserType == 3)
			    {
                    //如果是从BI库里获取数据，则要同步taskid
			        DalPromotionJob.SetPromotionTaskActivity(promotionTask.PromotionTaskActivityId.Value, resultId);
			    }
			    
                using(var log = new ConfigLogClient())
                {
                    log.InsertDefaultLogQueue("PromotionConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = resultId,
                        ObjectType = "PromTask",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(promotionTask),
                        Operate = promotionTask?.PromotionTaskId == null ? "新增优惠券任务" : "修改优惠券任务",
                        Author = oper
                    }));
                }
				return resultId;
			}
			catch (Exception ex)
			{
				throw new Exception(" 优惠券任务对象出错");
			}

		}

		/// <summary>
		/// 根据优惠券ID查询优惠券名称
		/// </summary>
		/// <param name="promotionRuleId"></param>
		/// <returns></returns>
		public string GetPromotionRuleNameById(int promotionRuleId)
		{
			try
			{
				return handler.GetPromotionRuleNameById(promotionRuleId);
			}
			catch (Exception ex)
			{
				throw new Exception(" 根据优惠券ID查询优惠券名称出错");
			}
		}

        public static int CreatePromotionCodeNew(PromotionCode model)
        {
            return DALPromotion.CreatePromotionCodeNew(model);
        }
    }
}
