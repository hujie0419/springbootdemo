using Tuhu.Service.Activity.Server.Utils;

namespace Tuhu.Service.Activity.Server.Model
{
    public enum RewardApplicationCheckStatus
    {
        /// <summary>
        /// 验证通过
        /// </summary>
        [EnumExtension.Remark("验证通过")]
        Succeed = 1,
        /// <summary>
        /// 申请人姓名为空
        /// </summary>
        [EnumExtension.Remark("申请人姓名为空")]
        NameEmpty = -1,

        /// <summary>
        /// 输入超过20个字
        /// </summary>
        [EnumExtension.Remark("名字超长,请输入您的真实姓名")]
        NameTooLong = -2,

        /// <summary>
        /// 途虎注册手机号为空
        /// </summary>
        [EnumExtension.Remark("途虎注册手机号不能为空")]
        PhoneEmpty = -3,

        /// <summary>
        ///不是11位手机号
        /// </summary>
        [EnumExtension.Remark("请输入正确的手机号")]
        PhoneWrong = -6,

        /// <summary>
        /// 不是途虎注册账户
        /// </summary>
        [EnumExtension.Remark("该手机号还没有注册途虎,请输入正确的途虎账户")]
        AccountWrong = -7,

        /// <summary>
        /// 没有上传截图
        /// </summary>
        [EnumExtension.Remark("至少上传一张图片")]
        ImageEmpty = -8,
        /// <summary>
        /// 已有已经通过的申请
        /// </summary>
        [EnumExtension.Remark("您已申请成功，不必再次提交")]
        Passed = -9,
        /// <summary>
        /// 已有已经审核中的申请
        /// </summary>
        [EnumExtension.Remark("您的申请正在审核中，请勿重复提交")]
        UnderReview = -10,

        /// <summary>
        /// 申请时间在3月26日之后
        /// </summary>
        [EnumExtension.Remark("很抱歉,活动申请已结束")]

        Expired = -11,

        /// <summary>
        /// 服务器错误
        /// </summary>
        [EnumExtension.Remark("申请失败，请咨询客服 400-111-8868")]
        OtherFailed = -12,

    }

    public class RewardApplicationCheckModel
    {
        public RewardApplicationCheckModel(RewardApplicationCheckStatus status)
        {
            Status = status;

        }
        public RewardApplicationCheckStatus Status { get; set; }
    }
}