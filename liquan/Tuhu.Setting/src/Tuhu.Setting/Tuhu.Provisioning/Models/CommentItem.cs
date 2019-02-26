using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCControlsToolkit.Controls;
using MVCControlsToolkit.DataAnnotations;
using MVCControlsToolkit.Controls.DataFilter;
using MVCControlsToolkit.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using NPOI.Util;
using Tuhu.Component.ExportImport;

namespace Tuhu.Provisioning.Models
{
    [TableExportImport(ExportImportName = "评论列表")]
    public class CommentItem
	{
        [Display(Name = "评论来源")]
        public int CommentChannel { get; set; }
        [ColumnExportImport(ExportImportName = "评论来源")]
        public string CommentSource { get; set; }
		[Display(Name = "评论ID")]
        public int CommentId { get; set; }
		[Display(Name = "用户ID")]
        //[ColumnExportImport(ExportImportName = "用户ID")]
        public Guid? CommentUserId { get; set; }

        [Display(Name = "晒单标题")]
        public string SingleTitle { get; set; }
        [ColumnExportImport(ExportImportName = "用户名称")]
        [Display(Name = "用户名称")]
		public string CommentUserName { get; set; }
		[Display(Name = "评论内容")]
        [ColumnExportImport(ExportImportName = "评论内容")]
        public string CommentContent { get; set; }
		[Display(Name = "评论图片")]
		public string CommentImages { get; set; }
		[Display(Name = "评论状态")]
        public int? CommentStatus { get; set; }
        [ColumnExportImport(ExportImportName = "审核状态")]
        public string CommentStatusName { get; set; }
		[Display(Name = "评论的产品ID")]
        [ColumnExportImport(ExportImportName = "评论的产品ID")]
        public string CommentProductId { get; set; }
		[Display(Name = "评论的产品父ID")]
		public string CommentProductFamilyId { get; set; }
		[Display(Name = "订单ID")]
        [ColumnExportImport(ExportImportName = "订单ID", Order = 2)]
        public int? CommentOrderId { get; set; }
		public int? CommentOrderListId { get; set; }
		[Display(Name = "评论类型")]
		public int? CommentType { get; set; }
        [ColumnExportImport(ExportImportName = "评论的门店", Order = 2)]
        public string CommentExtAttr { get; set; }
		[Display(Name = "创建时间")]
        [ColumnExportImport(ExportImportName = "创建时间", Order = 2)]
        public DateTime? CreateTimeDate { get; set; }
		[Format(DataFormatString = "{0:hh:mm:ss}")]
		public DateTime? CreateTime { get; set; }
		[Display(Name = "更新时间")]
		public DateTime? UpdateTime { get; set; }
		public int? NextP { get; set; }
		public int? PrevP { get; set; }
		[Display(Name = "R1")]
        [ColumnExportImport(ExportImportName = "R1", Order = 2)]
        public int? CommentR1 { get; set; }
		[Display(Name = "R2")]
        [ColumnExportImport(ExportImportName = "R2", Order = 2)]
        public int? CommentR2 { get; set; }
		[Display(Name = "R3")]
        [ColumnExportImport(ExportImportName = "R3", Order = 2)]
        public int? CommentR3 { get; set; }
		[Display(Name = "R4")]
        [ColumnExportImport(ExportImportName = "R4", Order = 2)]
        public int? CommentR4 { get; set; }
		[Display(Name = "R5")]
        [ColumnExportImport(ExportImportName = "R5", Order = 2)]
        public int? CommentR5 { get; set; }
		[Display(Name = "R6")]
        [ColumnExportImport(ExportImportName = "R6", Order = 2)]
        public int? CommentR6 { get; set; }
		[Display(Name = "R7")]
        [ColumnExportImport(ExportImportName = "R7", Order = 2)]
        public int? CommentR7 { get; set; }

		[Display(Name = "父评论")]
		public int? ParentComment { get; set; }
		[Display(Name = "官方回复")]
		public string OfficialReply { get; set; }
		public string getProductLink(string productId)
		{
			string[] pids = productId.Split('|');
			string link = "http://www.tuhu.cn/Products/";
			if(pids.Length > 1)
			{
				if(!string.IsNullOrEmpty(pids[1]))
				{
					link = link + "PID-" + pids[0] + "/" + "VID-" + pids[1] + ".aspx";
				}
				else
				{
					link = link + "PID-" + pids[0] + ".aspx";

				}

			}
			else
			{
				link = link + "PID-" + pids[0] + ".aspx";
			}
			return link;
		}

		[Display(Name = "评论的门店")]
        public int? InstallShopID { get; set; }
        public string Complaint { get; set; }

        public string FirstTousuType { get; set; }

        public List<Complaint> ComplaintItem { get; set; }
        [ColumnExportImport(ExportImportName = "投诉类型", Order = 2)]
        public string ComplaintItems { get; set; }
        [Display(Name = "自动审核状态")]
        public int AutoApproveStatus { get; set; }
    }
    public class Complaint
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class CommentItemWithProductName: CommentItem
    {
        [Display(Name = "评论的产品名称")]
        [ColumnExportImport(ExportImportName = "评论的产品名称", Order = 1)]
        public string CommentProductName { get; set; }
    }
}