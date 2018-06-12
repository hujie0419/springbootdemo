using System;
using System.Collections.Generic;
using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Component.SystemFramework.Extensions;
using Newtonsoft.Json;
using System.Linq;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class CommentModel : BaseModel
    {
        public CommentModel() : base() { }
        public CommentModel(DataRow row) : base(row) { }

        public int CommentId { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户电话号码
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImage { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        public string CommentContent { get; set; }

        /// <summary>
        /// 评价图片
        /// </summary>
        public string CommentImages { get; set; }

        /// <summary>
        /// 评论状态1
        /// </summary>
        public int CommentStatus { get; set; }

        /// <summary>
        /// 评论的产品编号
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string ProductFamilyId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// OrderList中的PKID
        /// </summary>
        public int? OrderListId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int CommentType { get; set; }

        /// <summary>
        /// 评论来源  0表示PC端的评论   1表示手机端的评论
        /// </summary>
        public int? CommentChannel { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string CommentExtAttr { get; set; }

        public int NextP { get; set; }

        public int PrevP { get; set; }

        /// <summary>
        /// 舒适性
        /// </summary>
        public int CommentR1 { get; set; }

        /// <summary>
        /// 静音性
        /// </summary>
        public int? CommentR2 { get; set; }

        /// <summary>
        /// 操控性
        /// </summary>
        public int? CommentR3 { get; set; }

        /// <summary>
        /// 耐磨性
        /// </summary>
        public int? CommentR4 { get; set; }

        /// <summary>
        /// 节油性
        /// </summary>
        public int? CommentR5 { get; set; }

        /// <summary>
        /// 客服满意度
        /// </summary>
        public int? CommentR6 { get; set; }

        /// <summary>
        /// 门店满意度
        /// </summary>
        public int? CommentR7 { get; set; }

        /// <summary>
        /// 晒单的标题
        /// </summary>
        public string SingleTitle { get; set; }


        /// <summary>
        /// 官方回复
        /// </summary>
        public string OfficialReply { get; set; }
        public DateTime UpdateTime { get; set; }
        public int? InstallShopID { get; set; }
    }

    public class ProductCommentModel : CommentModel
    {
        public ProductCommentModel() : base() { }
        public ProductCommentModel(DataRow row) : base(row) { }
        public int? Gender { get; set; }

        public DateTime? RegisteredDateTime { get; set; }

        public int? ParentComment { get; set; }

        public int? TotalPraise { get; set; }

        public int? OrderQuantity { get; set; }

        public CommentExtenstionAttribute CommentExtenstionAttribute { get; set; }

        public IEnumerable<ProductCommentModel> ReplyComments { get; set; }

        protected override void Parse(DataRow row)
        {
            base.Parse(row);
            if (!string.IsNullOrWhiteSpace(this.CommentExtAttr))
                this.CommentExtenstionAttribute = JsonConvert.DeserializeObject<CommentExtenstionAttribute>(this.CommentExtAttr);
        }

        public static IEnumerable<ProductCommentModel> Parse(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return new ProductCommentModel[0];

            return dt.Rows
                .OfType<DataRow>()
                .Select(row => new ProductCommentModel(row))
                .GroupBy(c => c.ParentComment == null ? c.CommentId : c.ParentComment)
                .Select(group =>
                {
                    var parentComment = group.FirstOrDefault(c => c.ParentComment == null);
                    if (parentComment != null)
                    {
                        parentComment.ReplyComments = group.Where(c => c.ParentComment != null).OrderByDescending(c => c.CommentId).ToArray();
                        return parentComment;
                    }
                    return null;
                })
                .Where(c => c != null)
                .OrderByDescending(c => c.CommentId)
                .ToArray();
        }
    }


    public class CommentExtenstionAttribute
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public DateTime? OrderDatetime { get; set; }
        public Guid? CarID { get; set; }
        public string CarTypeDes { get; set; }
        public int? InstallShopID { get; set; }
        public string InstallShop { get; set; }
        public TestUserInfo testUserInfo { get; set; }
        public TestEnvironment testEnvironment { get; set; }
    }

    public class TestUserInfo
    {
        public string Name { get; set; }
        public int Gender { get; set; }
        public int age { get; set; }
        public int DriveAge { get; set; }
        public string DriveDistance { get; set; }
        public string DriveStyle { get; set; }
    }

    public class TestEnvironment
    {
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string DriveSituation { get; set; }
        public string RoadSituation { get; set; }
        public string WeatherSituation { get; set; }
    }

    public class BaiJiaCommentModel : BaseModel
    {
        public BaiJiaCommentModel() : base(){ }

        public BaiJiaCommentModel(DataRow dr) : base(dr)
        {

        }
        public int CommentId { set; get; }
        public string CommentExtAttr { set; get; }
        public string CommentContent { set; get; }
        public string SingleTitle { set; get; }
        public string CommentProductId { set; get; }
        public string CommentType { set; get; }
        public string DisplayName { set; get; }
        public string CommentImages { set; get; }
        public string CP_Brand { set; get; }
        public string CP_Tire_Pattern { set; get; }
        public string TireSize { set; get; }
        public string SpeedRating { set; get; }
        public string[] CommentImagesList
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CommentImages))
                {
                    if (this.CommentImages.Contains(";"))
                        return CommentImages.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);
                    return new[] {CommentImages};
                }

                return null;
            }
        }

        public CommentExtenstionAttribute CommentExtenstionAttribute
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CommentExtAttr))
                    return JsonConvert.DeserializeObject<CommentExtenstionAttribute>(CommentExtAttr);
                return null;
            }
        }
    }
}
