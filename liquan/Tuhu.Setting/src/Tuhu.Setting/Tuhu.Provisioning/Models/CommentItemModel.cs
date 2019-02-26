using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MVCControlsToolkit.Linq;
using MVCControlsToolkit.Controller;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Data;
using System.IO;
using Tuhu.Component.ExportImport;
using Tuhu.Provisioning.DataAccess.DAO;
using System.Configuration;

namespace Tuhu.Provisioning.Models
{
    public class CommentItemModel
    {
        public int TotalPages { get; set; }
        public int CurrPage { get; set; }
        public int PrevPage { get; set; }
        public List<KeyValuePair<LambdaExpression, OrderType>> OrderOrder { get; set; }
        public System.Linq.Expressions.Expression<Func<CommentItem, bool>> SoFilter { get; set; }
        public List<Tracker<CommentItem>> CommentItemList { get; set; }

        public List<TousuTypeItem> TousuTypeItems { get; set; }

        public static List<Tracker<CommentItem>> GetCommentAllInfoBySearch(int type, int pageDim, string keyWord, int wordCount,int approveStatus, string InstallShop, string CommentProductId, string CommentTousuType,
        DateTime? commentstartDate, DateTime? commentendDate, DateTime? auditstartDate, DateTime? auditendDate, out int totalPages, out int commentCount, ref List<KeyValuePair<LambdaExpression, OrderType>> order,
        List<int> otherfilter, string orderId, int page = 1, Expression<Func<CommentItem, bool>> filter = null)
        {
            List<Tracker<CommentItem>> result = new List<Tracker<CommentItem>>();
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

            IQueryable<tbl_CommentTousu> commentTousu = dc.tbl_CommentTousu;

            var commentItems = GetAllCommentList(type, pageDim, keyWord, wordCount,approveStatus, InstallShop,
                CommentProductId, CommentTousuType, commentstartDate, commentendDate, auditstartDate,
                auditendDate, ref order, otherfilter, orderId);

            commentCount = commentItems.Count();
            totalPages = 1;

            int rowCount = commentItems.Count();

            if (rowCount == 0)
            {
                totalPages = 0;
                return new List<Tracker<CommentItem>>();
            }
            totalPages = rowCount / pageDim;
            if (rowCount % pageDim > 0) totalPages++;
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;
            int toSkip = (page - 1) * pageDim;

            result = commentItems.Select(viewItem => new Tracker<CommentItem>
            {
                Value = viewItem,
                OldValue = viewItem,
                Changed = false
            }).Skip(toSkip).Take(pageDim).ToList();

            return result;
        }

        public static List<Tracker<CommentItem>> GetAllComments(int pageDim, string keyWord, int wordCount, out int totalPages, ref List<KeyValuePair<LambdaExpression, OrderType>> order, List<int> otherfilter, int page = 1, System.Linq.Expressions.Expression<Func<CommentItem, bool>> filter = null)
        {
            List<Tracker<CommentItem>> result = new List<Tracker<CommentItem>>();
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

            IQueryable<tbl_Comment> items = dc.tbl_Comment.OrderByDescending(p => p.CreateTime);
            if (keyWord == "")
            {
                items = items.Where(p => p.CommentType == 1 && p.CommentStatus == 1 && p.CommentContent.Length <= wordCount);
            }
            else
            {
                items = items.Where(p => p.CommentType == 1 && p.CommentStatus == 1 && p.CommentContent.Contains(keyWord) && p.CommentContent.Length <= wordCount);
            }

            totalPages = 1;

            int rowCount = items.Count();

            if (rowCount == 0)
            {
                totalPages = 0;
                return new List<Tracker<CommentItem>>();
            }
            totalPages = rowCount / pageDim;
            if (rowCount % pageDim > 0) totalPages++;
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;
            int toSkip = (page - 1) * pageDim;

            result = items.Select(view => new CommentItem()
            {
                CommentChannel = view.CommentChannel.GetValueOrDefault(),
                CommentId = view.CommentId,
                CommentUserId = view.CommentUserId,
                SingleTitle = view.SingleTitle,
                CommentUserName = view.CommentUserName,
                CommentContent = view.CommentContent,
                CommentImages = view.CommentImages,
                CommentStatus = view.CommentStatus,
                CommentProductId = view.CommentProductId,
                CommentProductFamilyId = view.CommentProductFamilyId,
                CommentOrderId = view.CommentOrderId,
                CommentOrderListId = view.CommentOrderListId,
                CommentExtAttr = view.CommentExtAttr,
                CreateTime = view.CreateTime,
                CreateTimeDate = view.CreateTime,
                CommentType = view.CommentType,
                UpdateTime = view.UpdateTime,
                NextP = view.NextP,
                PrevP = view.PrevP,
                CommentR1 = view.CommentR1,
                CommentR2 = view.CommentR2,
                CommentR3 = view.CommentR3,
                CommentR4 = view.CommentR4,
                CommentR5 = view.CommentR5,
                CommentR6 = view.CommentR6,
                CommentR7 = view.CommentR7,
                OfficialReply = view.OfficialReply,
                ParentComment = view.ParentComment,
                InstallShopID = view.InstallShopID,
                AutoApproveStatus=view.AutoApproveStatus,
            }).ApplyOrder(order).Select(viewItem => new Tracker<CommentItem>
            {
                Value = viewItem,
                OldValue = viewItem,
                Changed = false
            }).Skip(toSkip).Take(pageDim).ToList();

            return result;
        }

        public static List<TousuTypeItem> GetAllTousuTypeItems()
        {
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            List<TousuTypeItem> result = new List<TousuTypeItem>();

            result = dc.TousuType.Select(view => new TousuTypeItem()
            {
                DicType = view.DicType,
                DicText = view.DicText,
                DicValue = view.DicValue,
                TypeLevel = view.TypeLevel,
                IsDelete = view.IsDelete ?? false
            }).Where(x => x.IsDelete != true).ToList();

            return result;

        }

        public static List<CommentItem> GetAllCommentList(int type, int pageDim, string keyWord, int wordCount, int approveStatus,string InstallShop, string CommentProductId, string CommentTousuType,
            DateTime? commentstartDate, DateTime? commentendDate, DateTime? auditstartDate, DateTime? auditendDate, ref List<KeyValuePair<LambdaExpression, OrderType>> order,
            List<int> otherfilter, string orderId)
        {
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

            IQueryable<tbl_CommentTousu> commentTousu = dc.tbl_CommentTousu;

            IQueryable<tbl_Comment> items = dc.tbl_Comment.OrderByDescending(p => p.CommentId);
            var orderid = 0;
            if (!string.IsNullOrEmpty(orderId))
            {
                orderid = Convert.ToInt32(orderId.Trim());
            }
            if (type == 1)
                items = items.Where(p => p.CommentType == 0 || p.CommentType == 1);
            else if (type == 2)
                items = items.Where(p => p.CommentType == 2 || p.CommentType == 4);
            else if (type == 3)
                items = items.Where(p => p.CommentType == 3);

            #region 筛选自动审核
            //0 =》除审核中评论的所有评论
            //1 =》自动审核通过
            //2 =》自动审核不通过
            //3 =》未审核
            //-1 =》审核中
            if (approveStatus == 0)
                items = items.Where(g => g.AutoApproveStatus != -1);
            else if (approveStatus == 1)
                items = items.Where(g => g.AutoApproveStatus == 1);
            else if (approveStatus == 2)
                items = items.Where(g => g.AutoApproveStatus == 2);
            else if (approveStatus == 3)
                items = items.Where(g => g.AutoApproveStatus == 0);
            else if (approveStatus == -1)
                items = items.Where(g => g.AutoApproveStatus == -1);
            #endregion

            if (otherfilter[0] == 0)
            {
                items = items.Where(p => p.CommentStatus == 0 || p.CommentStatus == 5);
            }
            else if (otherfilter[0] == 1)
            {
                items = items.Where(p => p.CommentStatus == 1 || p.CommentStatus == 4);
            }
            else if (otherfilter[0] == 2)
            {
                items = items.Where(p => p.CommentStatus == 2);
            }
            else if (otherfilter[0] == 3)
            {
                items = items.Where(p => p.CommentStatus == 3);
            }
            if (otherfilter[1] == 1)
            {
                items = items.Where(p => p.InstallShopID != null);
            }
            else if (otherfilter[1] == 2)
            {
                items = items.Where(p => p.InstallShopID == null);
            }
            if (otherfilter[2] == 1)
            {
                items = items.Where(p => p.CommentChannel == 0);
            }
            else if (otherfilter[2] == 2)
            {
                items = items.Where(p => p.CommentChannel == 1);
            }

            if (orderid != 0)
            {
                items = items.Where(p => p.CommentOrderId == orderid);
            }
            if (!string.IsNullOrEmpty(keyWord))
            {
                items = items.Where(p => p.CommentContent.Contains(keyWord));
            }
            if (wordCount != 0)
            {
                items = items.Where(p => p.CommentContent.Length <= wordCount && p.CommentContent != null);
            }
            if (!string.IsNullOrEmpty(InstallShop))
            {
                items = items.Where(p => p.CommentExtAttr.Contains(InstallShop));
            }
            if (!string.IsNullOrEmpty(CommentProductId))
            {
                items = items.Where(p => p.CommentProductId.Contains(CommentProductId));
            }
            if (!string.IsNullOrEmpty(commentstartDate.ToString()))
            {
                items = items.Where(p => Convert.ToDateTime(p.CreateTime) >= commentstartDate);
            }
            if (!string.IsNullOrEmpty(commentendDate.ToString()))
            {
                items = items.Where(p => Convert.ToDateTime(p.CreateTime) <= commentendDate);
            }
            if (!string.IsNullOrEmpty(auditstartDate.ToString()))
            {
                items = items.Where(p => Convert.ToDateTime(p.UpdateTime) >= auditstartDate && p.CommentStatus != 1);
            }
            if (!string.IsNullOrEmpty(auditendDate.ToString()))
            {
                items = items.Where(p => Convert.ToDateTime(p.UpdateTime) <= auditendDate && p.CommentStatus != 1);
            }

            List<CommentItem> commentItems = null;

            commentItems = items.Select(view => new CommentItem()
            {
                CommentChannel = view.CommentChannel.GetValueOrDefault(),
                CommentSource = view.CommentChannel == null ? string.Empty : view.CommentChannel.ToString(),
                CommentId = view.CommentId,
                CommentUserId = view.CommentUserId,
                SingleTitle = view.SingleTitle,
                CommentUserName = view.CommentUserName,
                CommentContent = view.CommentContent,
                CommentImages = view.CommentImages,
                CommentStatus = view.CommentStatus,
                CommentStatusName = view.CommentStatus == null ? string.Empty : view.CommentStatus.ToString(),
                CommentProductId = view.CommentProductId,
                CommentProductFamilyId = view.CommentProductFamilyId,
                CommentOrderId = view.CommentOrderId,
                CommentOrderListId = view.CommentOrderListId,
                CommentExtAttr = view.CommentExtAttr,
                CreateTime = view.CreateTime,
                CreateTimeDate = view.CreateTime,
                CommentType = view.CommentType,
                UpdateTime = view.UpdateTime,
                NextP = view.NextP,
                PrevP = view.PrevP,
                CommentR1 = view.CommentR1,
                CommentR2 = view.CommentR2,
                CommentR3 = view.CommentR3,
                CommentR4 = view.CommentR4,
                CommentR5 = view.CommentR5,
                CommentR6 = view.CommentR6,
                CommentR7 = view.CommentR7,
                OfficialReply = view.OfficialReply,
                ParentComment = view.ParentComment,
                InstallShopID = view.InstallShopID,
                AutoApproveStatus=view.AutoApproveStatus,
                Complaint = commentTousu.Where(p => p.CommentId == view.CommentId).FirstOrDefault() == null
                ? string.Empty : commentTousu.Where(p => p.CommentId == view.CommentId).FirstOrDefault().ComplaintContent,
                FirstTousuType = commentTousu.Where(p => p.CommentId == view.CommentId).FirstOrDefault() == null
                ? string.Empty : commentTousu.Where(p => p.CommentId == view.CommentId).FirstOrDefault().FirstTousuType
            }).ApplyOrder(order).ToList();

            if (CommentTousuType == "0")
            {
                commentItems = commentItems.Where(p => !string.IsNullOrEmpty(p.Complaint)).ToList();
            }
            else
            {
                commentItems = commentItems.Where(p => p.FirstTousuType.Contains(CommentTousuType)).ToList();
            }

            return commentItems;

        }

        public static List<CommentItemWithProductName> GetAllCommentListWithProductName(int type, int pageDim, string keyWord, int wordCount, string InstallShop, string CommentProductId, string CommentTousuType,
            DateTime? commentstartDate, DateTime? commentendDate, DateTime? auditstartDate, DateTime? auditendDate, ref List<KeyValuePair<LambdaExpression, OrderType>> order,
            List<int> otherfilter, string orderId)
        {
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);


            IQueryable<tbl_CommentTousu> commentTousu = dc.tbl_CommentTousu;

            IQueryable<tbl_Comment> items = dc.tbl_Comment.OrderByDescending(p => p.CommentId);
            var orderid = 0;
            if (!string.IsNullOrEmpty(orderId))
            {
                orderid = Convert.ToInt32(orderId.Trim());
            }
            if (type == 1)
                items = items.Where(p => p.CommentType == 0 || p.CommentType == 1);
            else if (type == 2)
                items = items.Where(p => p.CommentType == 2 || p.CommentType == 4);
            else if (type == 3)
                items = items.Where(p => p.CommentType == 3);


            if (otherfilter[0] == 0)
            {
                items = items.Where(p => p.CommentStatus == 0 || p.CommentStatus == 5);
            }
            else if (otherfilter[0] == 1)
            {
                items = items.Where(p => p.CommentStatus == 1 || p.CommentStatus == 4);
            }
            else if (otherfilter[0] == 2)
            {
                items = items.Where(p => p.CommentStatus == 2);
            }
            else if (otherfilter[0] == 3)
            {
                items = items.Where(p => p.CommentStatus == 3);
            }
            if (otherfilter[1] == 1)
            {
                items = items.Where(p => p.InstallShopID != null);
            }
            else if (otherfilter[1] == 2)
            {
                items = items.Where(p => p.InstallShopID == null);
            }
            if (otherfilter[2] == 1)
            {
                items = items.Where(p => p.CommentChannel == 0);
            }
            else if (otherfilter[2] == 2)
            {
                items = items.Where(p => p.CommentChannel == 1);
            }

            if (orderid != 0)
            {
                items = items.Where(p => p.CommentOrderId == orderid);
            }
            if (!string.IsNullOrEmpty(keyWord))
            {
                items = items.Where(p => p.CommentContent.Contains(keyWord));
            }
            if (wordCount != 0)
            {
                items = items.Where(p => p.CommentContent.Length <= wordCount && p.CommentContent != null);
            }
            if (!string.IsNullOrEmpty(InstallShop))
            {
                items = items.Where(p => p.CommentExtAttr.Contains(InstallShop));
            }
            if (!string.IsNullOrEmpty(CommentProductId))
            {
                items = items.Where(p => p.CommentProductId.Contains(CommentProductId));
            }
            if (!string.IsNullOrEmpty(commentstartDate.ToString()))
            {
                items = items.Where(p => Convert.ToDateTime(p.CreateTime) >= commentstartDate);
            }
            if (!string.IsNullOrEmpty(commentendDate.ToString()))
            {
                items = items.Where(p => Convert.ToDateTime(p.CreateTime) <= commentendDate);
            }
            if (!string.IsNullOrEmpty(auditstartDate.ToString()))
            {
                items = items.Where(p => Convert.ToDateTime(p.UpdateTime) >= auditstartDate && p.CommentStatus != 1);
            }
            if (!string.IsNullOrEmpty(auditendDate.ToString()))
            {
                items = items.Where(p => Convert.ToDateTime(p.UpdateTime) <= auditendDate && p.CommentStatus != 1);
            }

            List<CommentItemWithProductName> commentItems = null;

            commentItems = items.Select(view => new CommentItemWithProductName()
            {
                CommentChannel = view.CommentChannel.GetValueOrDefault(),
                CommentSource = view.CommentChannel == null ? string.Empty : view.CommentChannel.ToString(),
                CommentId = view.CommentId,
                CommentUserId = view.CommentUserId,
                SingleTitle = view.SingleTitle,
                CommentUserName = view.CommentUserName,
                CommentContent = view.CommentContent,
                CommentImages = view.CommentImages,
                CommentStatus = view.CommentStatus,
                CommentStatusName = view.CommentStatus == null ? string.Empty : view.CommentStatus.ToString(),
                CommentProductId = view.CommentProductId,
                CommentProductFamilyId = view.CommentProductFamilyId,
                CommentOrderId = view.CommentOrderId,
                CommentOrderListId = view.CommentOrderListId,
                CommentExtAttr = view.CommentExtAttr,
                CreateTime = view.CreateTime,
                CreateTimeDate = view.CreateTime,
                CommentType = view.CommentType,
                UpdateTime = view.UpdateTime,
                NextP = view.NextP,
                PrevP = view.PrevP,
                CommentR1 = view.CommentR1,
                CommentR2 = view.CommentR2,
                CommentR3 = view.CommentR3,
                CommentR4 = view.CommentR4,
                CommentR5 = view.CommentR5,
                CommentR6 = view.CommentR6,
                CommentR7 = view.CommentR7,
                OfficialReply = view.OfficialReply,
                ParentComment = view.ParentComment,
                InstallShopID = view.InstallShopID,
                Complaint = commentTousu.Where(p => p.CommentId == view.CommentId).FirstOrDefault() == null
                ? string.Empty : commentTousu.Where(p => p.CommentId == view.CommentId).FirstOrDefault().ComplaintContent,
                FirstTousuType = commentTousu.Where(p => p.CommentId == view.CommentId).FirstOrDefault() == null
                ? string.Empty : commentTousu.Where(p => p.CommentId == view.CommentId).FirstOrDefault().FirstTousuType
            }).ApplyOrder(order).ToList();

            if (CommentTousuType == "0")
            {
                commentItems = commentItems.Where(p => !string.IsNullOrEmpty(p.Complaint)).ToList();
            }
            else
            {
                commentItems = commentItems.Where(p => p.FirstTousuType.Contains(CommentTousuType)).ToList();
            }
            return commentItems;

        }

        public static List<CommentItemWithProductName> GetAllCommentListByExportExcel(int type, int pageDim, string keyWord, int wordCount, string InstallShop, string CommentProductId, string CommentTousuType,
            DateTime? commentstartDate, DateTime? commentendDate, DateTime? auditstartDate, DateTime? auditendDate, ref List<KeyValuePair<LambdaExpression, OrderType>> order, List<int> otherfilter, string orderId)
        {
            GungnirDataContext dc = new GungnirDataContext(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            IQueryable<tbl_CommentTousu> commentTousu = dc.tbl_CommentTousu;

            var result = GetAllCommentListWithProductName(type, pageDim, keyWord, wordCount, InstallShop, CommentProductId, CommentTousuType,
                commentstartDate, commentendDate, auditstartDate, auditendDate, ref order, otherfilter, orderId);

            if (result != null && result.Any())
            {
                var pids = result.Select(cl => "'" + cl.CommentProductId + "'").ToList();

                var productModels = OperationCategoryDAL.FetchPrProductsModelByProductIds(pids);
                if (productModels != null && productModels.Any())
                {
                    result.ForEach(cl => cl.CommentProductName = productModels.Where(pm => pm.PID == cl.CommentProductId).FirstOrDefault() == null ? null : productModels.Where(pm => pm.PID == cl.CommentProductId).FirstOrDefault().DisplayName);
                }
            }
            var tousuTypeItems = dc.TousuType.Select(view => new TousuTypeItem()
            {
                DicType = view.DicType,
                DicText = view.DicText,
                DicValue = view.DicValue,
                TypeLevel = view.TypeLevel
            }).ToList();

            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.CommentExtAttr))
                {
                    item.CommentExtAttr = Regex.Match(item.CommentExtAttr, @"InstallShop[^ID][\s|\S]*[-]*.*testUserInfo").ToString()
                        .Replace("InstallShop", "").Replace("testUserInfo", "").Replace(":", "")
                        .Replace(",", "").Replace("\"", "");
                }
                if (!string.IsNullOrEmpty(item.CommentSource))
                {
                    if (item.CommentSource == "0")
                    {
                        item.CommentSource = "PC端";
                    }
                    else if (item.CommentSource == "1")
                    {
                        item.CommentSource = "手机端";
                    }
                }
                if (!string.IsNullOrEmpty(item.CommentStatusName))
                {
                    if (item.CommentStatusName == "0")
                    {
                        item.CommentStatusName = "审核不通过";
                    }
                    else if (item.CommentStatusName == "1")
                    {
                        item.CommentStatusName = "未审核";
                    }
                    else if (item.CommentStatusName == "2")
                    {
                        item.CommentStatusName = "审核通过";
                    }
                    else if (item.CommentStatusName == "3")
                    {
                        item.CommentStatusName = "仅打分";
                    }
                }
            }

            for (var i = 0; i < result.Count(); i++)
            {
                var complaint = result[i].Complaint;
                if (!string.IsNullOrEmpty(complaint))
                {
                    result[i].ComplaintItem = JsonConvert.DeserializeObject<List<Complaint>>(complaint);
                    var complaintItems = result[i].ComplaintItem;
                    foreach (var item in complaintItems)
                    {
                        var typeText = string.Empty;
                        TousuTypeItem tousuTypeItem = null;
                        string key = item.key;
                        int level = 0;
                        bool flag = true;
                        while (flag)
                        {
                            if (level == 0)
                            {
                                tousuTypeItem = tousuTypeItems.Where(fd => fd.DicValue.Equals(key)).FirstOrDefault();
                            }
                            else
                            {
                                tousuTypeItem = tousuTypeItems.Where(fd => fd.DicValue.Equals(key)
                                && fd.TypeLevel.Equals(level - 1)).FirstOrDefault();
                            }
                            if (tousuTypeItem != null)
                            {
                                typeText = tousuTypeItem.DicText + "-" + typeText;
                                key = tousuTypeItem.DicType;
                                level = tousuTypeItem.TypeLevel;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        if (!string.IsNullOrEmpty(typeText))
                        {
                            typeText = typeText.TrimEnd('-');
                            result[i].ComplaintItems += typeText + "|";
                        }
                    }
                }
            }
            return result;
        }
    }
}