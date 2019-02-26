using Newtonsoft.Json;
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
        public static List<Tuple<int,string>> GetTousuType(List<int> ids)
        {
            var data = DALCommentReport.GetTousuInfo(ids);
            var tousuTypeItems = DALCommentReport.GetTousuType();
            var result = new List<Tuple<int, string>>();
            for (var i = 0; i < data.Count(); i++)
            {
                var complaint = data[i].Item2;
                string content = "";
                if (!string.IsNullOrEmpty(complaint))
                {
                    var complaintItems = JsonConvert.DeserializeObject<List<TousuContent>>(complaint);
                    foreach (var item in complaintItems)
                    {
                        var typeText = string.Empty;
                        Tuple<int, string, string,string> tousuTypeItem = null;
                        string key = item.key;
                        int level = 0;
                        bool flag = true;
                        while (flag)
                        {
                            if (level == 0)
                            {
                                tousuTypeItem = tousuTypeItems.Where(fd => fd.Item2.Equals(key)).FirstOrDefault();
                            }
                            else
                            {
                                tousuTypeItem = tousuTypeItems.Where(fd => fd.Item2.Equals(key)
                                && fd.Item1.Equals(level - 1)).FirstOrDefault();
                            }
                            if (tousuTypeItem != null)
                            {
                                typeText = tousuTypeItem.Item3 + "-" + typeText;
                                key = tousuTypeItem.Item4;
                                level = tousuTypeItem.Item1;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        if (!string.IsNullOrEmpty(typeText))
                        {
                            typeText = typeText.TrimEnd('-');
                            content += typeText + "|";
                        }
                    }
                }
                result.Add(Tuple.Create(data[i].Item1, content.TrimEnd('|')));
            }
            return result;
        }
    }
    public class TousuContent
    {
        public string key { get; set; }
        public string value { get; set; }
    }

}
