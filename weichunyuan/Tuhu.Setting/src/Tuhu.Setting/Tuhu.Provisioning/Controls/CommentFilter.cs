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
using Tuhu.Provisioning.Models;


namespace Tuhu.Provisioning.Controls
{
	public class CommentFilter :
		IFilterDescription<CommentItem>
	{
		public string CommentStatus { get; set; }
		static public List<string> istatus = new List<string>() { "全部", "未审核", "已审核" };
		public CommentFilter()
		{

		}
		public System.Linq.Expressions.Expression<Func<CommentItem, bool>> GetExpression()
		{
			int CommentStatus1 = 0;
			switch(CommentStatus)
			{
				case "全部":
					CommentStatus1 = 0;
					break;
				case "未审核":
					CommentStatus1 = 1;
					break;
				default:
					CommentStatus1 = 2;
					break;
			}

			FilterBuilder<CommentItem> fb = new FilterBuilder<CommentItem>();
			fb = fb.Add(CommentStatus1 == 0, m => m.CommentStatus.Equals(1) || m.CommentStatus.Equals(2));
			fb = fb.Add(CommentStatus1 > 0, m => m.CommentStatus.Equals(CommentStatus1));
			System.Linq.Expressions.Expression<Func<CommentItem, bool>> res = new FilterBuilder<CommentItem>().Add(true, fb.Get())
				.Get();
			return res;


		}

	}
}
