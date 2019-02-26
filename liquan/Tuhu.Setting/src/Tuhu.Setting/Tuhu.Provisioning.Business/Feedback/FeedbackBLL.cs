using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO.Feedback;
using Tuhu.Provisioning.DataAccess.Entity.Feedback;

namespace Tuhu.Provisioning.Business.Feedback
{
    public class FeedbackBLL
    {
        private readonly FeedbackDAL _feedbackDal;

        public FeedbackBLL()
        {
            _feedbackDal = new FeedbackDAL();
        }
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public List<string> GetVersionNumList()
        {
            return _feedbackDal.GetVersionNumList();
        }

        /// <summary>
        /// 获取手机型号
        /// </summary>
        /// <returns></returns>
        public List<string> GetPhoneModelList()
        {
            return _feedbackDal.GetPhoneModelList();
        }

        /// <summary>
        /// 获取网络环境
        /// </summary>
        /// <returns></returns>
        public List<string> GetNetworkEnvironmentList()
        {
            return _feedbackDal.GetNetworkEnvironmentList();
        }

        /// <summary>
        /// 获取问题反馈数据集合（包括搜索）
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="typeIds">问题类型Id,用，隔开</param>
        /// <param name="flag">时间标识，0全部，1一周内，2一个月内，3具体时间段</param>
        /// <param name="versionNum">版本号</param>
        /// <param name="phoneModel">手机型号</param>
        /// <param name="networkEnvir">网络环境</param>
        /// <param name="startTime">日期：开始时间</param>
        /// <param name="endTime">日期：结束时间</param>
        /// <param name="isDown">是否是下载数据：1为是，0为查询</param>
        /// <returns></returns>
        public PageResult GetFeedbackListByCondition(int pageIndex, int pageSize, string typeIds, int flag, string versionNum, string phoneModel, string networkEnvir, DateTime? startTime, DateTime? endTime, int isDown = 0)
        {
            var result = new PageResult();
            int count = 0;
            var source = _feedbackDal.GetFeedbackListByCondition(pageIndex, pageSize, typeIds, flag, versionNum, phoneModel, networkEnvir, startTime, endTime, out count, isDown).ConvertTo<FeedbackBLLModel>();
            result.total = count;
            result.FeedbackList = source;
            return result;
        }

        /// <summary>
        /// 添加意见反馈信息
        /// </summary>
        /// <param name="typeId">问题类型</param>
        /// <param name="userPhone">手机号</param>
        /// <param name="feedbackContent">反馈内容</param>
        /// <param name="versionNum">版本号</param>
        /// <param name="phoneModels">手机型号</param>
        /// <param name="networkEnvironment">网络环境</param>
        /// <param name="images">图片，>=0张</param>
        /// <returns></returns>
        public async Task<int> AddFeedbackInfo(int typeId, string userPhone, string feedbackContent, string versionNum, string phoneModels, string networkEnvironment, List<string> images)
        {
            return await _feedbackDal.AddFeedbackInfo(typeId, userPhone, feedbackContent, versionNum, phoneModels, networkEnvironment, images);
        }

        /// <summary>
        /// 按问题类型和用户手机号查询问题反馈信息
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="userPhone"></param>
        /// <returns></returns>
        public FeedbackBLLModel GetFeedbackEntityByTypeIdAndUser(int typeId, string userPhone)
        {
            var entity = new FeedbackBLLModel();
            var table = _feedbackDal.GetFeedbackEntityByTypeIdAndUser(typeId, userPhone);
            if (table != null && table.Rows.Count > 0)
            {
                entity.Id = Convert.ToInt32(table.Rows[0]["Id"].ToString());
                entity.TypeName = table.Rows[0]["TypeName"].ToString();
                entity.UserPhone = table.Rows[0]["UserPhone"].ToString();
                entity.VersionNum = table.Rows[0]["VersionNum"].ToString();
                entity.FeedbackContent = table.Rows[0]["FeedbackContent"].ToString();
                entity.PhoneModels = table.Rows[0]["PhoneModels"].ToString();
                entity.NetworkEnvironment = table.Rows[0]["NetworkEnvironment"].ToString();
                entity.FeedbackImgs = new List<string>();
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    if (entity.Id == Convert.ToInt32(table.Rows[i]["Id"].ToString()))
                    {
                        entity.FeedbackImgs.Add(table.Rows[i]["ImgUrl"].ToString());
                    }
                }
            }
            return entity;
        }

        public async Task<int> UpdateIsCustomerServer(int id)
        {
            return await _feedbackDal.UpdateIsCustomerServer(id);
        }
    }
}
