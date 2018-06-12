using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Comment.Models;

namespace Tuhu.Provisioning.Business.Comment
{
    public class CommentReportManager
    {
        public static IEnumerable<CommentReportItem> GetCommentCount(DateTime? startDate, DateTime? endDate)
        {
            var dt = DALCommentReport.GetCommentCount(startDate, endDate);
            if (dt == null || dt.Rows.Count <= 0)
                return new CommentReportItem[0];
            return dt.Rows.Cast<DataRow>().Select(row => new CommentReportItem(row));
        }

        public static IEnumerable<CommentTousuType> GetTousuType(int level)
        {
            var dt = DALCommentReport.GetTousuType(level);
            if (dt == null || dt.Rows.Count <= 0)
                return new CommentTousuType[0];
            return dt.Rows.Cast<DataRow>().Select(row => new CommentTousuType(row));
        }

        public static PagedModel<AdditionProductComment> SelectAdditionProductCommentByOrderId(int PageIndex,
            int PageSize, int Status, int? OrderId, int AutoApproveStatus = 0)
        {
            var result = new PagedModel<AdditionProductComment>()
            {
                Pager = new PagerModel()
                {
                    PageSize = PageSize,
                    CurrentPage = PageIndex
                },
            };
            var Count = DALCommentReport.SelectAdditionProductCommentCountByOrderId(Status, OrderId, AutoApproveStatus);
            result.Pager.Total = Count;
            if (Count > 0)
            {
                var data = DALCommentReport.SelectAdditionProductCommentByOrderId(PageIndex, PageSize, Status, OrderId, AutoApproveStatus);
                result.Source = data.ConvertTo<AdditionProductComment>();
            }
            return result;
        }
        /// <summary>
        /// 根据创建时间查询评论信息
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static DataTable GetCommentByCreateTime(DateTime startDate, DateTime endDate)
        {
            return DALCommentReport.GetCommentByCreateTime(startDate, endDate);
        }
    }
}
