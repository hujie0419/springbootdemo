using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.LuckyCharm;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Provisioning.Business.LuckyCharm
{
    
    public class LuckyCharmManager
    {
        private static readonly Common.Logging.ILog Logger = Common.Logging.LogManager.GetLogger(typeof(LuckyCharmManager));

        #region 锦鲤活动操作
        /// <summary>
        /// 分页获取活动
        /// </summary>
        /// <returns></returns>
        public PageLuckyCharmActivityModel PageActivtiy(int pageIndex, int pageSize, int pkid)
        {
            var result = new PageLuckyCharmActivityModel() { Activitys = new List<LuckyCharmActivityModel>() };
            try
            {
                using (var client = new LuckyCharmClient())
                {
                    var request = new PageLuckyCharmActivityRequest()
                    {
                        PKID = pkid,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                    };
                    var users = client.PageLuckyCharmActivity(request);

                    if (users.Success)
                    {
                        result.Total = users.Result.Total;
                        foreach (var item in users.Result.Activitys)
                        {
                            result.Activitys.Add(new LuckyCharmActivityModel()
                            {
                                PKID = item.PKID,
                                ActivityDes = item.ActivityDes,
                                ActivityTitle = item.ActivityTitle,
                                ActivitySlug = item.ActivitySlug,
                                StarTime = item.StarTime,
                                EndTime = item.EndTime,
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"PageActivtiy -> {pageIndex}-{pageSize}-{pkid}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 添加活动
        /// </summary>
        /// <returns></returns>
        public bool AddActivtiy(LuckyCharmActivityModel model)
        {
            bool isSuccess = false;
            try
            {
                using (var client = new LuckyCharmClient())
                {
                    var request = new AddLuckyCharmActivityRequest()
                    {
                        ActivityDes = model.ActivityDes,
                        ActivityTitle = model.ActivityTitle,
                        ActivitySlug = model.ActivitySlug,
                        ActivityType = (Tuhu.Service.Activity.Models.ActivityTypeEnum)model.ActivityType,
                        StarTime = model.StarTime,
                        EndTime = model.EndTime,
                    };
                    isSuccess = client.AddLuckyCharmActivity(request).Result;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"AddActivtiy -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return isSuccess;
        }

        /// <summary>
        /// 获取活动详情
        /// </summary>
        /// <returns></returns>
        public LuckyCharmActivityModel GetActivtiy(int pkid)
        {
            var result = new LuckyCharmActivityModel();
            try
            {
                using (var client = new LuckyCharmClient())
                {
                    var model = client.GetLuckyCharmActivity(pkid).Result;
                    result.ActivityDes = model.ActivityDes;
                    result.ActivityTitle = model.ActivityTitle;
                    result.ActivitySlug = model.ActivitySlug;
                    result.StarTime = model.StarTime;
                    result.EndTime = model.EndTime;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetActivtiy -> {pkid}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <returns></returns>
        public bool DelActivtiy(int pkid)
        {
            bool isSuccess = false;
            try
            {
                using (var client = new LuckyCharmClient())
                {
                    isSuccess = client.DelLuckyCharmActivity(pkid).Result;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DelActivtiy -> {pkid}", e);
                throw;
            }
            return isSuccess;
        }

        #endregion

        #region 活动用户操作
        /// <summary>
        /// 分页获取活动用户
        /// </summary>
        /// <returns></returns>
        public PageLuckyCharmUserModel PageActivtiyUser(int pageIndex, int pageSize, string phone, int pkid, string areaName, int checkState)
        {
            var result = new PageLuckyCharmUserModel() { Users = new List<LuckyCharmUserModel>() };
            try
            {
                using (var client = new LuckyCharmClient())
                {
                    var request = new PageLuckyCharmUserRequest()
                    {
                        AreaName = areaName,
                        PKID = pkid,
                        Phone = phone,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        CheckState = checkState
                    };
                    var users = client.PageLuckyCharmUser(request);
                    if (users.Success)
                    {
                        result.Total = users.Result.Total;
                        foreach (var item in users.Result.Users)
                        {
                            result.Users.Add(new LuckyCharmUserModel()
                            {
                                ActivityId = item.ActivityId,
                                AreaId = item.AreaId,
                                AreaName = item.AreaName,
                                Phone = item.Phone,
                                UserId = item.UserId,
                                UserName = item.UserName,
                                PKID = item.PKID,
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"PageActivtiyUser -> {pageIndex}-{pageSize}-{phone}-{pkid}-{areaName}-{checkState}", e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 进行报名
        /// </summary>
        /// <returns></returns>
        public bool AddActivtiyUser(LuckyCharmUserModel model)
        {
            bool isSuccess = false;
            try
            {
                using (var client = new LuckyCharmClient())
                {
                    var request = new AddLuckyCharmUserRequest()
                    {
                        ActivityId = model.ActivityId,
                        AreaId = model.AreaId,
                        AreaName = model.AreaName,
                        Phone = model.Phone,
                        UserId = model.UserId,
                        UserName = model.UserName,
                    };
                    isSuccess = client.AddLuckyCharmUser(request).Result;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"AddActivtiyUser -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return isSuccess;
        }

        /// <summary>
        /// 修改报名信息
        /// </summary>
        /// <returns></returns>
        public bool UpdateActivtiyUser(LuckyCharmUserModel model)
        {
            bool isSuccess = false;
            try
            {
                using (var client = new LuckyCharmClient())
                {
                    var request = new UpdateLuckyCharmUserRequest()
                    {
                        AreaId = model.AreaId,
                        AreaName = model.AreaName,
                        Phone = model.Phone,
                        UserName = model.UserName,
                        PKID = model.PKID,
                        ActivityId = model.ActivityId,
                    };
                    isSuccess = client.UpdateLuckyCharmUser(request).Result;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UpdateActivtiyUser -> {JsonConvert.SerializeObject(model)}", e);
                throw;
            }
            return isSuccess;
        }

        /// <summary>
        /// 删除活动报名用户
        /// </summary>
        /// <returns></returns>
        public bool DelActivtiyUser(int pkid)
        {
            bool isSuccess = false;
            try
            {
                using (var client = new LuckyCharmClient())
                {
                    isSuccess = client.DelLuckyCharmUser(pkid).Result;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DelActivtiyUser -> {pkid}", e);
                throw;
            }
            return isSuccess;
        }

        #endregion

    }
}
