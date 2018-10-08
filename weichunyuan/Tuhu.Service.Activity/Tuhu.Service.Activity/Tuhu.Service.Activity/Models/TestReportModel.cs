using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>入选的测试报告和它作者的简要信息</summary>
    public class SelectedTestReport
    {
        /// <summary>commentId</summary>
        public int? CommentId { get; set; }
        /// <summary>userId</summary>
        public Guid UserId { get; set; }
        /// <summary>用户昵称</summary>
        public string UserName { get; set; }
        /// <summary>用户性别</summary>
        public int? Gender { get; set; }
        /// <summary>用户头像</summary>
        public string HeadImage { get; set; }
        /// <summary>众测报告标题</summary>
        public string ReportTitle { get; set; }
        /// <summary>众测报告摘要</summary>
        public string ReportAbstract { get; set; }
        /// <summary>众测报告提交时间</summary>
        public DateTime? ReportCreateTime { get; set; }
        /// <summary> 评价图片</summary>
        public IEnumerable<string> ReportImages { get; set; }
    }

    public class SelectedTestReportDetail : SelectedTestReport
    {
        /// <summary>期数</summary>
        public int Period { get; set; }
        /// <summary>省份ID</summary>
        public int ProvinceID { get; set; }
        /// <summary>城市ID</summary>
        public int CityID { get; set; }
        /// <summary>众测报告内容</summary>
        public string ReportContent { get; set; }
        /// <summary>众测报告更改时间</summary>
        public DateTime? ReportUpdateTime { get; set; }
        /// <summary>众测产品ID</summary>
        public string ProductId { get; set; }
        /// <summary>众测产品大类ID</summary>
        public string ProductFamilyId { get; set; }
        /// <summary>众测订单ID</summary>
        public int? OrderId { get; set; }
        /// <summary>众订单列表ID</summary>
        public int? OrderListId { get; set; }
        /// <summary>众测审核状态</summary>
        public int? CommentStatus { get; set; }
        /// <summary>
        /// 试用报告的状态  0表示还未填写试用报告    1表示仅填写了试用报告   2表示填写了试用报告和试用者信息   3表示试用报告通过可返押金
        /// </summary>
        public int? ReportStatus { get; set; }
        /// <summary> 官方回复</summary>
        public string OfficialReply { get; set; }
        public int? Comfortability { get; set; }
        /// <summary>静音性</summary>
        public int? MutePerformance { get; set; }
        /// <summary>操控性</summary>
        public int? Controllability { get; set; }
        /// <summary>耐磨性</summary>
        public int? AbrasionPerformance { get; set; }
        /// <summary>节油性</summary>
        public int? OilSaving { get; set; }
        /// <summary> 客服满意度</summary>
        public int? CustomServiceSatisfaction { get; set; }
        /// <summary>门店满意度</summary>
        public int? ShopSatisfaction { get; set; }
        public CommentExtenstionAttribute TestReportExtenstionAttribute { get; set; }
    }
    public class CommentExtenstionAttribute
    {
        public DateTime? OrderDatetime { get; set; }
        public Guid? CarID { get; set; }
        public string CarTypeDes { get; set; }
        public int? InstallShopID { get; set; }
        public string InstallShop { get; set; }
        public TestUserInfo TestUserInfo { get; set; }
        public TestEnvironment TestEnvironment { get; set; }
    }

    public class TestUserInfo
    {
        /// <summary>用户姓名</summary>
        public string Name { get; set; }
        /// <summary>用户电话号码</summary>
        public string Cellphone { get; set; }
        /// <summary>用户性别</summary>
        public int Gender { get; set; }
        /// <summary>用户年龄</summary>
        public int Age { get; set; }
        /// <summary>用户驾龄</summary>
        public int DriveAge { get; set; }
        /// <summary>用户驾驶里程</summary>
        public string DriveDistance { get; set; }
        /// <summary>用户驾驶风格</summary>
        public string DriveStyle { get; set; }
    }

    public class TestEnvironment
    {
        /// <summary>温度</summary>
        public string Temperature { get; set; }
        /// <summary>湿度</summary>
        public string Humidity { get; set; }
        /// <summary>行驶环境</summary>
        public string DriveSituation { get; set; }
        /// <summary>路面状况</summary>
        public string RoadSituation { get; set; }
        /// <summary>天气情况</summary>
        public string WeatherSituation { get; set; }
    }
}