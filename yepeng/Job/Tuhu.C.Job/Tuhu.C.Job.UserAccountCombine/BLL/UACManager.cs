using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.C.Job.UserAccountCombine.DAL;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.Service.OAuth;
using Tuhu.Service.OAuth.Models;
using Tuhu.Service.OAuth.Request;
using System.Threading.Tasks;
using System.IO;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.MessageQueue;
using Tuhu;

namespace Tuhu.C.Job.UserAccountCombine.BLL
{
    public class UACManager
    {
        public static readonly ILog _sublogger = LogManager.GetLogger(typeof(UACManager));

        public static bool CheckSwitch(string runtimeSwitch)
            => UserAccountCombineDal.CheckSwitch(runtimeSwitch);

        #region CollectNeedCombineUserId
        public static List<YewuThirdpartyInfo> GetNeedCombineYewuUsers()
            => UserAccountCombineDal.GetNeedCombineYewuUsers();

        public static List<UserObjectModel> GetUsersByIds(List<Guid> userIds)
            => UserAccountCombineDal.GetUsersByIds(userIds);

        public static List<NeedCombineUserId> FilterPrimaryUserIds(List<YewuThirdpartyInfo> yewuUsers)
        {
            List<NeedCombineUserId> resultList = new List<NeedCombineUserId>();

            //stepOne 获取唯一的ThirdpartyId和Channel
            var stepOne =
                yewuUsers
                .Where(_item => !string.IsNullOrEmpty(_item.Channel) && !string.IsNullOrEmpty(_item.ThirdpartyId))
                .GroupBy(_item => new { ThirdpartyId = _item.ThirdpartyId.ToLower(), Channel = _item.Channel.ToLower() })
                .Select(_group => new { _group.First().ThirdpartyId, _group.First().Channel })
                .ToList();

            if (stepOne != null)
            {
                foreach (var item in stepOne)
                {
                    //steptwo 获取对应的UserIds
                    //return List<Guid>
                    var stepTwo = yewuUsers.Where(_item => _item.Channel.ToLower() == item.Channel.ToLower()
                    && _item.ThirdpartyId.ToLower() == item.ThirdpartyId.ToLower())
                        .Select(_item => _item.UserId)
                        .ToList();

                    //stepThree 根据UserIds获取User信息
                    //return List<UserObjectModel>
                    var stepThree = GetUsersByIds(stepTwo);

                    //stepFour 分析出Users里主userId
                    //return Guid
                    var stepFour = FilterPrimaryRule(stepThree, resultList);

                    //stepFive 生成主userId和需要合并userid的数据结构
                    //return List<NeedCombineUserId>
                    var result = CombineUserIds(stepThree, stepFour, item.Channel, item.ThirdpartyId);

                    //stepSix 合并结果集
                    resultList.AddRange(result);
                }
            }
            return resultList;
        }

        public static Guid FilterPrimaryRule(List<UserObjectModel> users, List<NeedCombineUserId> resultList)
        {
            Guid primaryGuid = new Guid();
            List<UserObjectModel> tempRemoveUsers = new List<UserObjectModel>();
            List<UserObjectModel> tempUsers = new List<UserObjectModel>();
            tempUsers.AddRange(users);

            //在本次Job执行中已经记录的需要合并数据结果集resultList中
            //判断如果users里有userId等于resultList里的主账号TargetUserId，直接输出
            //判断如果users里有userId等于resultList里的被合并账号SourceUserId，把这个userId从users里删除
            for (int i = tempUsers.Count - 1; i >= 0; i--)
            {
                var checkExistTargetUserId = resultList.Where(_target => _target.TargetUserId == tempUsers[i].UserId.Value).ToList();
                if (checkExistTargetUserId != null && checkExistTargetUserId.Any())
                {
                    return tempUsers[i].UserId.Value;
                }

                var checkExistSourceUserId = resultList.Where(_source => _source.SourceUserId == tempUsers[i].UserId).ToList();
                if (checkExistSourceUserId != null && checkExistSourceUserId.Any())
                {
                    tempUsers.Remove(tempUsers[i]);
                }
            }

            //在之前Job执行中已经记录的合并数据结果集allExistNeedCombineUserIdList中
            //判断如果users里有userId等于allExistNeedCombineUserIdList里的主账号TargetUserId，直接输出
            //判断如果users里有userId等于allExistNeedCombineUserIdList里的被合并账号SourceUserId，把这个userId从users里删除
            List<NeedCombineUserId> allExistNeedCombineUserIdList = UserAccountCombineDal.GetAllExistNeedCombineUserIdList();
            if (allExistNeedCombineUserIdList != null && allExistNeedCombineUserIdList.Any())
            {
                for (int i = tempUsers.Count - 1; i >= 0; i--)
                {
                    var checkExistTargetUserId = allExistNeedCombineUserIdList.
                        Where(_target => _target.TargetUserId == tempUsers[i].UserId.Value).
                        ToList();
                    if (checkExistTargetUserId != null && checkExistTargetUserId.Any())
                    {
                        return tempUsers[i].UserId.Value;
                    }

                    var checkExistSourceUserId = allExistNeedCombineUserIdList.
                        Where(_source => _source.SourceUserId == tempUsers[i].UserId).
                        ToList();
                    if (checkExistSourceUserId != null && checkExistSourceUserId.Any())
                    {
                        tempUsers.Remove(tempUsers[i]);
                    }
                }
            }

            var pickupList = tempUsers.Where(_item => _item.IsActive
                                            && _item.IsMobileVerify.HasValue
                                            && _item.IsMobileVerify == true
                                            && !string.IsNullOrEmpty(_item.MobileNumber))
                                            .ToList();

            if (pickupList == null || !pickupList.Any())  //如果没有验证过手机号
            {
                pickupList = tempUsers.Where(_item => _item.IsActive
                                         && !string.IsNullOrEmpty(_item.MobileNumber))
                                        .ToList();

                if (pickupList == null || !pickupList.Any()) //如果都没有填手机号
                {
                    pickupList = tempUsers;
                }
            }

            var filterList = pickupList.Where(_item => _item.LastLogonTime.HasValue).ToList();
            if (filterList == null || !filterList.Any()) //如果都没有最后登录时间，按照注册时间最近一次的账号为准
            {
                primaryGuid = pickupList.OrderByDescending(_item => _item.CreatedTime).First().UserId.Value;
            }
            else  //有最后登录时间的，按照最近一次的账号为准
            {
                primaryGuid = filterList.OrderByDescending(_item => _item.LastLogonTime).First().UserId.Value;
            }

            return primaryGuid;
        }

        public static List<NeedCombineUserId> CombineUserIds(List<UserObjectModel> users, Guid primaryGuid, string channel, string thirdPartyId)
        {
            List<NeedCombineUserId> resultList = new List<NeedCombineUserId>();
            if (users != null)
            {
                foreach (var item in users)
                {
                    if (item.UserId.Value != primaryGuid)
                    {
                        resultList.Add(new NeedCombineUserId
                        {
                            CreateDataTime = DateTime.Now,
                            SourceUserId = item.UserId.Value,
                            TargetUserId = primaryGuid,
                            Channel = channel,
                            ThirdpartyId = thirdPartyId
                        });
                    }
                }
            }
            return resultList;
        }

        public static bool InsertNeedCombineUserIds(List<NeedCombineUserId> items)
        {
            var result = true;
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        result = UserAccountCombineDal.InsertNeedCombineUserId(item, dbHelper);
                        if (!result)
                        {
                            _sublogger.Info("CollectNeedCombineUserId_Job_InsertNeedCombineUserIds方法出错在SourceUserId:"
                                + item.SourceUserId.ToString()
                                + ", TargetUserId:" + item.TargetUserId.ToString());
                            return result;
                        }
                    }
                }

                dbHelper.Commit();

                return result;
            }
        }
        #endregion

        #region UserCombineAndRecordLog
        public static List<NeedCombineUserId> GetNeedCombineUserIdList()
            => UserAccountCombineDal.GetNeedCombineUserIdList();

        public static string CombineAndRecordAction(List<NeedCombineUserId> needCombineList)
        {
            string result = "一共执行" + needCombineList.Count() + "行记录,";
            int successCount = 0;
            int failCount = 0;
            if (needCombineList != null)
            {
                foreach (var needCombineItem in needCombineList)
                {
                    List<RelatedTableCombineLog> actionList = new List<RelatedTableCombineLog>();

                    #region 用户账号
                    //更新( 用户账号主表)      更新IsActive, 合并u_addresses, 更新u_preferred_address
                    var result_Update_UserObjectTable = Update_UserObjectTable(needCombineItem);

                    //更新( 三方用户业务表)    更新UserId
                    var result_Update_YewuThirdpartyInfo = Update_YewuThirdpartyInfo(needCombineItem);

                    //更新( 永隆行用户信息表)  更新u_user_id
                    var result_Update_YLH_UserInfo = Update_YLH_UserInfo(needCombineItem);

                    //更新( 永隆行会员卡车辆表)更新u_user_id
                    var result_Update_YLH_UserVipCardInfo = Update_YLH_UserVipCardInfo(needCombineItem);
                    #endregion

                    #region 门店
                    //更新( 门店预约记录表 )   更新UserID
                    var result_Update_ShopReserve = Update_ShopReserve(needCombineItem);

                    //更新( 门店质检表 )       更新UserID
                    var result_Update_ShopReceiveCheckThird = Update_ShopReceiveCheckThird(needCombineItem);

                    //更新( 门店上线检测表 )   更新UserID
                    var result_Update_ShopReceiveCheckSecond = Update_ShopReceiveCheckSecond(needCombineItem);

                    //更新( 门店预检表 )       更新UserID
                    var result_Update_ShopReceiveCheckFirst = Update_ShopReceiveCheckFirst(needCombineItem);

                    //更新( 门店接待订单表 )   更新UserID
                    var result_Update_ShopReceiveOrder = Update_ShopReceiveOrder(needCombineItem);

                    //更新( 门店到店记录表 )   更新UserID
                    var result_Update_ShopReceive = Update_ShopReceive(needCombineItem);

                    //更新( 门店到店记录表 )   更新UserID
                    var result_Update_ShopReceiveNew = Update_ShopReceiveNew(needCombineItem);
                    #endregion

                    #region 订单
                    //更新( 订单关联地址表 )   更新UserId,只设置一个IsDefaultAddress
                    var result_Update_Addresses = Update_Addresses(needCombineItem);

                    //更新( 订单关联车型表 )   更新u_user_id
                    var result_Update_CarObject = Update_CarObject(needCombineItem);

                    //更新（ 订单表 ）          更新UserID
                    var result_Update_tbl_Order = Update_tbl_Order(needCombineItem);
                    #endregion

                    #region CRM
                    //更新( 老CRMCase表 )      更新EndUserGuid
                    var result_Update_tbl_EndUserCase = Update_tbl_EndUserCase(needCombineItem);

                    //更新( 老CRM重要提醒表 )  更新EndUserGuid
                    var result_Update_tbl_CRM_Requisition = Update_tbl_CRM_Requisition(needCombineItem);

                    //更新（ 新CRM预约提醒表 ） 更新UserID
                    var result_Update_CRMAppointment = Update_CRMAppointment(needCombineItem);

                    //更新（ 新CRM联系记录表）  更新UserID
                    var result_Update_CRMContactLog = Update_CRMContactLog(needCombineItem);

                    //更新( CRM插旗记录表)　　 更新UserId
                    var result_Update_CRMFlagInfo = Update_CRMFlagInfo(needCombineItem);
                    #endregion

                    #region 优惠券、积分
                    //更新( 优惠券表)       更新UserId
                    var result_Update_tbl_PromotionCode = Update_tbl_PromotionCode(needCombineItem);

                    //更新( 积分表)         合并积分Integral字段，Status设0
                    //更新( 积分详情表)     更新IntegralID为主UserID对应的IntegralID
                    var result_Update_tbl_UserIntegral = Update_tbl_UserIntegral(needCombineItem);
                    #endregion

                    if (result_Update_UserObjectTable
                        && result_Update_YewuThirdpartyInfo
                        && result_Update_YLH_UserInfo
                        && result_Update_YLH_UserVipCardInfo
                        && result_Update_ShopReserve
                        && result_Update_ShopReceiveCheckThird
                        && result_Update_ShopReceiveCheckSecond
                        && result_Update_ShopReceiveCheckFirst
                        && result_Update_ShopReceiveOrder
                        && result_Update_ShopReceive
                        && result_Update_ShopReceiveNew
                        && result_Update_Addresses
                        && result_Update_CarObject
                        && result_Update_tbl_Order
                        && result_Update_tbl_EndUserCase
                        && result_Update_tbl_CRM_Requisition
                        && result_Update_CRMAppointment
                        && result_Update_CRMContactLog
                        && result_Update_CRMFlagInfo
                        && result_Update_tbl_PromotionCode
                        && result_Update_tbl_UserIntegral)
                    {
                        //更新NeedCombineUserId里的IsOperateSuccess为1
                        try
                        {
                            var update_NeedCombineUserId_IsOperateSuccess =
                                UserAccountCombineDal.Update_NeedCombineUserId_IsOperateSuccess(needCombineItem);
                            if (!update_NeedCombineUserId_IsOperateSuccess)
                            {
                                _sublogger.Info("UserCombineAndRecordLog_Job_Update_NeedCombineUserId_IsOperateSuccess方法出错在SourceUserId:"
                                    + needCombineItem.SourceUserId.ToString()
                                    + ", TargetUserId:" + needCombineItem.TargetUserId.ToString()
                                    + ", PKID:" + needCombineItem.PKID);
                            }
                        }
                        catch (Exception ex)
                        {
                            _sublogger.Info("UserCombineAndRecordLog_Job_Update_NeedCombineUserId_IsOperateSuccess方法出错在SourceUserId:"
                                    + needCombineItem.SourceUserId.ToString()
                                    + ", TargetUserId:" + needCombineItem.TargetUserId.ToString()
                                    + ", PKID:" + needCombineItem.PKID + ex.Message);
                        }

                        successCount++; // 累计成功
                    }
                    else
                        failCount++; // 累计失败
                }
            }
            result = result + "成功" + successCount + "行记录, 失败" + failCount + "行记录.";
            return result;
        }

        #region 更新逻辑
        //更新( 用户账号主表)      
        //更新IsActive
        //更新userObject表u_preferred_address
        //更新addresses表IsDefaultAddress
        //合并u_addresses
        //插入成功条数，结果return true
        //插入失败条数，结果return false
        public static bool Update_UserObjectTable(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            //更新IsActive
            try
            {
                var update_UserObject_IsActive = UserAccountCombineDal.Update_UserObject_IsActive(needCombineItem.SourceUserId);
                if (update_UserObject_IsActive)
                {
                    logList.Add(new RelatedTableCombineLog
                    {
                        SourceUserId = needCombineItem.SourceUserId,
                        TargetUserId = needCombineItem.TargetUserId,
                        RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                        RelatedTablePK = "UserID='" + needCombineItem.SourceUserId.ToString("b") + "'",
                        UpdatedParameter = "IsActive",
                        ActionName = "Update",
                        SourceValue = "1",
                        TargetValue = "0"
                    });
                }
                else
                {
                    faillogList.Add(new RelatedTableCombineFailLog
                    {
                        SourceUserId = needCombineItem.SourceUserId,
                        TargetUserId = needCombineItem.TargetUserId,
                        RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                        RelatedTablePK = "UserID='" + needCombineItem.SourceUserId.ToString("b") + "'",
                        UpdatedParameter = "IsActive",
                        ActionName = "Update",
                        SourceValue = "1",
                        TargetValue = "0",
                        FailReason = "Update 0 row, cannot found sourceUserId:" + needCombineItem.SourceUserId.ToString("b")
                    });
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                faillogList.Add(new RelatedTableCombineFailLog
                {
                    SourceUserId = needCombineItem.SourceUserId,
                    TargetUserId = needCombineItem.TargetUserId,
                    RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                    RelatedTablePK = "UserID='" + needCombineItem.SourceUserId.ToString("b") + "'",
                    UpdatedParameter = "IsActive",
                    ActionName = "Update",
                    SourceValue = "1",
                    TargetValue = "0",
                    FailReason = ex.Message
                });
                isSuccess = false;
            }

            //获取原user和目标user
            UserObjectModel sUser = new UserObjectModel();
            UserObjectModel tUser = new UserObjectModel();
            sUser = UserAccountCombineDal.GetUserById(needCombineItem.SourceUserId);
            tUser = UserAccountCombineDal.GetUserById(needCombineItem.TargetUserId);

            //获取地址信息
            List<string> sUserAddressList = new List<string>();
            List<string> tUserAddressList = new List<string>();

            //只有辅账号有地址信息才执行
            if (!string.IsNullOrWhiteSpace(sUser.Addresses))
            {
                List<string> sUserAddressMiddleItem = new List<string>();
                sUserAddressMiddleItem = sUser.Addresses.Split(';').ToList();

                if (sUserAddressMiddleItem.Count < 2)  //如果辅User的地址值有问题
                {
                    faillogList.Add(new RelatedTableCombineFailLog
                    {
                        SourceUserId = needCombineItem.SourceUserId,
                        TargetUserId = needCombineItem.TargetUserId,
                        RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                        RelatedTablePK = "UserID='" + needCombineItem.SourceUserId.ToString("b") + "'",
                        UpdatedParameter = "u_addresses",
                        ActionName = "Update",
                        SourceValue = sUser.Addresses,
                        TargetValue = "",
                        FailReason = "SourceUserId:" + needCombineItem.SourceUserId.ToString("b") + " has an address data error."
                    });
                    isSuccess = false;
                }
                else
                {
                    sUserAddressList.AddRange(sUserAddressMiddleItem.GetRange(1, sUserAddressMiddleItem.Count - 1));
                    //更新userObject表u_preferred_address
                    if (string.IsNullOrWhiteSpace(sUser.PreferredAddress)) //如果辅账号没有默认地址，顺序取一个
                    {
                        sUser.PreferredAddress = sUserAddressList[0];
                    }
                    //主账号的默认地址只有在没有默认地址，且主账号地址信息等会也会被更新的情况下更新                    
                    if (string.IsNullOrWhiteSpace(tUser.PreferredAddress))
                    {
                        //更新结果主账号的默认地址
                        try
                        {
                            var update_UserObject_PreferredAddress =
                                UserAccountCombineDal.Update_UserObject_PreferredAddress(
                                    needCombineItem.TargetUserId, sUser.PreferredAddress);
                            if (update_UserObject_PreferredAddress)
                            {
                                logList.Add(new RelatedTableCombineLog
                                {
                                    SourceUserId = needCombineItem.SourceUserId,
                                    TargetUserId = needCombineItem.TargetUserId,
                                    RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                    RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                    UpdatedParameter = "u_preferred_address",
                                    ActionName = "Assignment",
                                    SourceValue = "",
                                    TargetValue = sUser.PreferredAddress
                                });
                            }
                            else
                            {
                                faillogList.Add(new RelatedTableCombineFailLog
                                {
                                    SourceUserId = needCombineItem.SourceUserId,
                                    TargetUserId = needCombineItem.TargetUserId,
                                    RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                    RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                    UpdatedParameter = "u_preferred_address",
                                    ActionName = "Assignment",
                                    SourceValue = "",
                                    TargetValue = sUser.PreferredAddress,
                                    FailReason = "Update 0 row, cannot found targetUserId" + needCombineItem.TargetUserId.ToString("b")
                                });
                                isSuccess = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                UpdatedParameter = "u_preferred_address",
                                ActionName = "Assignment",
                                SourceValue = "",
                                TargetValue = sUser.PreferredAddress,
                                FailReason = ex.Message
                            });
                            isSuccess = false;
                        }

                        //获取sUser.PreferredAddress的地址表信息，如果IsDefaultAddress=1就不更新
                        var addressInfo = UserAccountCombineDal.GetAddressById(sUser.PreferredAddress);
                        if (addressInfo != null)
                        {
                            if (!addressInfo.IsDefaultAddress.HasValue || addressInfo.IsDefaultAddress.Value == false)
                            {
                                try
                                {
                                    var update_Addresses_IsDefaultAddress = UserAccountCombineDal.Update_Addresses_IsDefaultAddress(sUser.PreferredAddress);
                                    if (update_Addresses_IsDefaultAddress)
                                    {
                                        logList.Add(new RelatedTableCombineLog
                                        {
                                            SourceUserId = needCombineItem.SourceUserId,
                                            TargetUserId = needCombineItem.TargetUserId,
                                            RelatedTableName = "[Tuhu_profiles].[dbo].[Addresses]",
                                            RelatedTablePK = "u_address_id='" + sUser.PreferredAddress + "'",
                                            UpdatedParameter = "IsDefaultAddress",
                                            ActionName = "Update",
                                            SourceValue = addressInfo.IsDefaultAddress.HasValue ? "False" : "null",
                                            TargetValue = "True"
                                        });
                                    }
                                    else
                                    {
                                        faillogList.Add(new RelatedTableCombineFailLog
                                        {
                                            SourceUserId = needCombineItem.SourceUserId,
                                            TargetUserId = needCombineItem.TargetUserId,
                                            RelatedTableName = "[Tuhu_profiles].[dbo].[Addresses]",
                                            RelatedTablePK = "u_address_id='" + sUser.PreferredAddress + "'",
                                            UpdatedParameter = "IsDefaultAddress",
                                            ActionName = "Update",
                                            SourceValue = addressInfo.IsDefaultAddress.HasValue ? "False" : "null",
                                            TargetValue = "True",
                                            FailReason = "Update 0 row, cannot found u_address_id=" + sUser.PreferredAddress + " data in Addresses table."
                                        });
                                        isSuccess = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    faillogList.Add(new RelatedTableCombineFailLog
                                    {
                                        SourceUserId = needCombineItem.SourceUserId,
                                        TargetUserId = needCombineItem.TargetUserId,
                                        RelatedTableName = "[Tuhu_profiles].[dbo].[Addresses]",
                                        RelatedTablePK = "u_address_id='" + sUser.PreferredAddress + "'",
                                        UpdatedParameter = "IsDefaultAddress",
                                        ActionName = "Update",
                                        SourceValue = addressInfo.IsDefaultAddress.HasValue ? "False" : "null",
                                        TargetValue = "True",
                                        FailReason = ex.Message
                                    });
                                    isSuccess = false;
                                }
                            }
                        }
                        else  //找不到sUser.PreferredAddress对应的地址信息
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[Addresses]",
                                RelatedTablePK = "u_address_id='" + sUser.PreferredAddress + "'",
                                UpdatedParameter = "IsDefaultAddress",
                                ActionName = "Update",
                                SourceValue = "",
                                TargetValue = "True",
                                FailReason = "用户表主地址里数据有误,请检查. Cannot found u_address_id="
                                + sUser.PreferredAddress
                                + " data in Addresses table. SourceUserId:"
                                + needCombineItem.SourceUserId.ToString("b")
                            });
                            //不需要认为出错，只记录信息方便以后检查
                            //isSuccess = false;
                        }
                    }

                    //合并u_addresses                    
                    //结果主账号有地址信息，需要合并去重，再更新到主账号
                    if (!string.IsNullOrWhiteSpace(tUser.Addresses))
                    {
                        List<string> tUserAddressMiddleItem = new List<string>();
                        tUserAddressMiddleItem = tUser.Addresses.Split(';').ToList();
                        if (tUserAddressMiddleItem.Count < 2) //如果主User的地址有问题
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                UpdatedParameter = "u_addresses",
                                ActionName = "Combine",
                                SourceValue = "",
                                TargetValue = tUser.Addresses,
                                FailReason = "TargetUserId:" + needCombineItem.TargetUserId.ToString("b") + " has an address data error."
                            });
                            isSuccess = false;
                        }
                        else
                        {
                            tUserAddressList.AddRange(tUserAddressMiddleItem.GetRange(1, tUserAddressMiddleItem.Count - 1));
                            tUserAddressList.AddRange(sUserAddressList);  //合并
                            tUserAddressList = tUserAddressList.Distinct().ToList(); //去重
                            //生成结果集合
                            string combineAddresses = tUserAddressList.Count().ToString();
                            if (tUserAddressList != null)
                            {
                                foreach (var item in tUserAddressList) combineAddresses += ";" + item;
                            }

                            try
                            {
                                var update_UserObject_Addresses =
                                    UserAccountCombineDal.Update_UserObject_Addresses(
                                        needCombineItem.TargetUserId, combineAddresses);
                                if (update_UserObject_Addresses)
                                {
                                    logList.Add(new RelatedTableCombineLog
                                    {
                                        SourceUserId = needCombineItem.SourceUserId,
                                        TargetUserId = needCombineItem.TargetUserId,
                                        RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                        RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                        UpdatedParameter = "u_addresses",
                                        ActionName = "Combine",
                                        SourceValue = tUser.Addresses,
                                        TargetValue = combineAddresses
                                    });
                                }
                                else
                                {
                                    faillogList.Add(new RelatedTableCombineFailLog
                                    {
                                        SourceUserId = needCombineItem.SourceUserId,
                                        TargetUserId = needCombineItem.TargetUserId,
                                        RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                        RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                        UpdatedParameter = "u_addresses",
                                        ActionName = "Combine",
                                        SourceValue = tUser.Addresses,
                                        TargetValue = combineAddresses,
                                        FailReason = "Update 0 row, cannot found targetUserId" + needCombineItem.TargetUserId.ToString("b")
                                    });
                                    isSuccess = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                faillogList.Add(new RelatedTableCombineFailLog
                                {
                                    SourceUserId = needCombineItem.SourceUserId,
                                    TargetUserId = needCombineItem.TargetUserId,
                                    RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                    RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                    UpdatedParameter = "u_addresses",
                                    ActionName = "Combine",
                                    SourceValue = tUser.Addresses,
                                    TargetValue = combineAddresses,
                                    FailReason = ex.Message
                                });
                                isSuccess = false;
                            }
                        }
                    }
                    else //如果主账号没有地址信息，直接取辅账号地址信息更新进去
                    {
                        try
                        {
                            var update_UserObject_Addresses =
                                UserAccountCombineDal.Update_UserObject_Addresses(
                                    needCombineItem.TargetUserId, sUser.Addresses);
                            if (update_UserObject_Addresses)
                            {
                                logList.Add(new RelatedTableCombineLog
                                {
                                    SourceUserId = needCombineItem.SourceUserId,
                                    TargetUserId = needCombineItem.TargetUserId,
                                    RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                    RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                    UpdatedParameter = "u_addresses",
                                    ActionName = "Assignment",
                                    SourceValue = "",
                                    TargetValue = sUser.Addresses
                                });
                            }
                            else
                            {
                                faillogList.Add(new RelatedTableCombineFailLog
                                {
                                    SourceUserId = needCombineItem.SourceUserId,
                                    TargetUserId = needCombineItem.TargetUserId,
                                    RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                    RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                    UpdatedParameter = "u_addresses",
                                    ActionName = "Assignment",
                                    SourceValue = "",
                                    TargetValue = sUser.Addresses,
                                    FailReason = "Update 0 row, cannot found targetUserId" + needCombineItem.TargetUserId.ToString("b")
                                });
                                isSuccess = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[UserObject]",
                                RelatedTablePK = "UserID='" + needCombineItem.TargetUserId.ToString("b") + "'",
                                UpdatedParameter = "u_addresses",
                                ActionName = "Assignment",
                                SourceValue = "",
                                TargetValue = sUser.Addresses,
                                FailReason = ex.Message
                            });
                            isSuccess = false;
                        }
                    }
                }
            }

            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新 用户授权表
        //更新UserId
        public static bool Update_UserAuthTable(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            //更新UserId
            List<UserAuth> sUserAuths = new List<UserAuth>();
            sUserAuths = UserAccountCombineDal.GetUserAuthById(needCombineItem.SourceUserId);
            if (sUserAuths != null && sUserAuths.Any())
            {
                foreach (var sUserAuthItem in sUserAuths)
                {
                    try
                    {
                        var update_UserAuth_UserId = UserAccountCombineDal.Update_UserAuth_UserId(sUserAuthItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_UserAuth_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_profiles.dbo.UserAuth",
                                RelatedTablePK = "PKID=" + sUserAuthItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sUserAuthItem.UserId.ToString("d"),
                                TargetValue = needCombineItem.TargetUserId.ToString("d")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_profiles.dbo.UserAuth",
                                RelatedTablePK = "PKID=" + sUserAuthItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sUserAuthItem.UserId.ToString("d"),
                                TargetValue = needCombineItem.TargetUserId.ToString("d"),
                                FailReason = "Update 0 row, cannot found PKID=" + sUserAuthItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_profiles.dbo.UserAuth",
                            RelatedTablePK = "PKID=" + sUserAuthItem.PKID.ToString(),
                            UpdatedParameter = "UserId",
                            ActionName = "Update",
                            SourceValue = sUserAuthItem.UserId.ToString("d"),
                            TargetValue = needCombineItem.TargetUserId.ToString("d"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }

            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 三方用户业务表)    
        //更新UserId
        public static bool Update_YewuThirdpartyInfo(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<YewuThirdpartyInfo> sYewuUsers = new List<YewuThirdpartyInfo>();
            sYewuUsers = UserAccountCombineDal.GetYewuThirdpartyInfoById(needCombineItem.SourceUserId,
                needCombineItem.Channel, needCombineItem.ThirdpartyId);
            if (sYewuUsers != null && sYewuUsers.Any())
            {
                foreach (var sYewuItem in sYewuUsers)
                {
                    try
                    {
                        var Update_YewuThirdpartyInfo_UserId =
                            UserAccountCombineDal.Update_YewuThirdpartyInfo_UserId(sYewuItem.PKID,
                            needCombineItem.TargetUserId);
                        if (Update_YewuThirdpartyInfo_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[YewuThirdpartyInfo]",
                                RelatedTablePK = "PKID=" + sYewuItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sYewuItem.UserId.ToString("b"),
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[YewuThirdpartyInfo]",
                                RelatedTablePK = "PKID=" + sYewuItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sYewuItem.UserId.ToString("b"),
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sYewuItem.PKID.ToString()
                                + " its channel=" + sYewuItem.Channel
                                + " its 3rdPartyId=" + sYewuItem.ThirdpartyId
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[YewuThirdpartyInfo]",
                            RelatedTablePK = "PKID=" + sYewuItem.PKID.ToString(),
                            UpdatedParameter = "UserId",
                            ActionName = "Update",
                            SourceValue = sYewuItem.UserId.ToString("b"),
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }

            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);
            return isSuccess;
        }

        //更新（三方用户业务表）只根据UserId查询，并更新
        //更新UserId
        public static bool Update_YewuThirdpartyInfoByUserId(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<YewuThirdpartyInfo> sYewuUsers = new List<YewuThirdpartyInfo>();
            sYewuUsers = UserAccountCombineDal.GetYewuThirdpartyInfoByIdOnly(needCombineItem.SourceUserId);
            if (sYewuUsers != null && sYewuUsers.Any())
            {
                foreach (var sYewuItem in sYewuUsers)
                {
                    try
                    {
                        var Update_YewuThirdpartyInfo_UserId =
                            UserAccountCombineDal.Update_YewuThirdpartyInfo_UserId(sYewuItem.PKID,
                            needCombineItem.TargetUserId);
                        if (Update_YewuThirdpartyInfo_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[YewuThirdpartyInfo]",
                                RelatedTablePK = "PKID=" + sYewuItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sYewuItem.UserId.ToString("b"),
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[YewuThirdpartyInfo]",
                                RelatedTablePK = "PKID=" + sYewuItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sYewuItem.UserId.ToString("b"),
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sYewuItem.PKID.ToString()
                                + " its userId=" + sYewuItem.UserId.ToString("b")
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[YewuThirdpartyInfo]",
                            RelatedTablePK = "PKID=" + sYewuItem.PKID.ToString(),
                            UpdatedParameter = "UserId",
                            ActionName = "Update",
                            SourceValue = sYewuItem.UserId.ToString("b"),
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }

            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);
            return isSuccess;
        }

        //更新( 永隆行用户信息表)  
        //更新u_user_id
        public static bool Update_YLH_UserInfo(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<YLHUserInfo> sYLHUsers = new List<YLHUserInfo>();
            sYLHUsers = UserAccountCombineDal.GetYLHUserInfoById(needCombineItem.SourceUserId.ToString("b"));
            if (sYLHUsers != null && sYLHUsers.Any())
            {
                foreach (var sYLHItem in sYLHUsers)
                {
                    try
                    {
                        var update_YLHUserInfo_UserId = UserAccountCombineDal.Update_YLHUserInfo_UserId(sYLHItem.PKID,
                            needCombineItem.TargetUserId.ToString("b"));
                        if (update_YLHUserInfo_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[YLH_UserInfo]",
                                RelatedTablePK = "PKID=" + sYLHItem.PKID.ToString(),
                                UpdatedParameter = "u_user_id",
                                ActionName = "Update",
                                SourceValue = sYLHItem.UserId,
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[YLH_UserInfo]",
                                RelatedTablePK = "PKID=" + sYLHItem.PKID.ToString(),
                                UpdatedParameter = "u_user_id",
                                ActionName = "Update",
                                SourceValue = sYLHItem.UserId,
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sYLHItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[YLH_UserInfo]",
                            RelatedTablePK = "PKID=" + sYLHItem.PKID.ToString(),
                            UpdatedParameter = "u_user_id",
                            ActionName = "Update",
                            SourceValue = sYLHItem.UserId,
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }

            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 永隆行会员卡车辆表)
        //更新u_user_id
        public static bool Update_YLH_UserVipCardInfo(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<YLHUserVipCardInfo> sYLHUserVipCardInfos = new List<YLHUserVipCardInfo>();
            sYLHUserVipCardInfos = UserAccountCombineDal.GetYLHUserVipCardInfoById(needCombineItem.SourceUserId.ToString("b"));
            if (sYLHUserVipCardInfos != null && sYLHUserVipCardInfos.Any())
            {
                foreach (var sYLHItem in sYLHUserVipCardInfos)
                {
                    try
                    {
                        var update_YLHUserVipCardInfo_UserId = UserAccountCombineDal.Update_YLHUserVipCardInfo_UserId(sYLHItem.PKID,
                            needCombineItem.TargetUserId.ToString("b"));
                        if (update_YLHUserVipCardInfo_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[YLH_UserVipCardInfo]",
                                RelatedTablePK = "PKID=" + sYLHItem.PKID.ToString(),
                                UpdatedParameter = "u_user_id",
                                ActionName = "Update",
                                SourceValue = sYLHItem.UserId,
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[YLH_UserVipCardInfo]",
                                RelatedTablePK = "PKID=" + sYLHItem.PKID.ToString(),
                                UpdatedParameter = "u_user_id",
                                ActionName = "Update",
                                SourceValue = sYLHItem.UserId,
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sYLHItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[YLH_UserVipCardInfo]",
                            RelatedTablePK = "PKID=" + sYLHItem.PKID.ToString(),
                            UpdatedParameter = "u_user_id",
                            ActionName = "Update",
                            SourceValue = sYLHItem.UserId,
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }

            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 门店预约记录表 )   
        //更新UserID
        public static bool Update_ShopReserve(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<ShopReserve> sResults = new List<ShopReserve>();
            sResults = UserAccountCombineDal.GetShopReserveById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_ShopReserve_UserID = UserAccountCombineDal.Update_ShopReserve_UserID(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_ShopReserve_UserID)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReserve]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReserve]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_shop.[dbo].[ShopReserve]",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }

            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 门店质检表 )       
        //更新UserID
        public static bool Update_ShopReceiveCheckThird(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<ShopReceiveCheckThird> sResults = new List<ShopReceiveCheckThird>();
            sResults = UserAccountCombineDal.GetShopReceiveCheckThirdById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_ShopReceiveCheckThird_UserID = UserAccountCombineDal.Update_ShopReceiveCheckThird_UserID(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_ShopReceiveCheckThird_UserID)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckThird]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckThird]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckThird]",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 门店上线检测表 )   
        //更新UserID
        public static bool Update_ShopReceiveCheckSecond(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<ShopReceiveCheckSecond> sResults = new List<ShopReceiveCheckSecond>();
            sResults = UserAccountCombineDal.GetShopReceiveCheckSecondById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_ShopReceiveCheckSecond_UserID = UserAccountCombineDal.Update_ShopReceiveCheckSecond_UserID(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_ShopReceiveCheckSecond_UserID)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckSecond]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckSecond]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckSecond]",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 门店预检表 )       
        //更新UserID
        public static bool Update_ShopReceiveCheckFirst(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<ShopReceiveCheckFirst> sResults = new List<ShopReceiveCheckFirst>();
            sResults = UserAccountCombineDal.GetShopReceiveCheckFirstById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_YLHUserInfo_UserId = UserAccountCombineDal.Update_ShopReceiveCheckFirst_UserID(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_YLHUserInfo_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckFirst]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckFirst]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveCheckFirst]",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 门店接待订单表 )   
        //更新UserID
        public static bool Update_ShopReceiveOrder(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<ShopReceiveOrder> sResults = new List<ShopReceiveOrder>();
            sResults = UserAccountCombineDal.GetShopShopReceiveOrderById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_ShopReceiveOrder_UserID = UserAccountCombineDal.Update_ShopReceiveOrder_UserID(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_ShopReceiveOrder_UserID)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveOrder]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveOrder]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveOrder]",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 门店到店记录表 )   
        //更新UserID
        public static bool Update_ShopReceive(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<ShopReceive> sResults = new List<ShopReceive>();
            sResults = UserAccountCombineDal.GetShopReceiveById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_ShopReceive_UserID = UserAccountCombineDal.Update_ShopReceive_UserID(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_ShopReceive_UserID)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceive]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceive]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_shop.[dbo].[ShopReceive]",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 门店到店记录表 )   
        //更新UserID
        public static bool Update_ShopReceiveNew(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<ShopReceiveNew> sResults = new List<ShopReceiveNew>();
            sResults = UserAccountCombineDal.GetShopReceiveNewById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_YLHUserInfo_UserId = UserAccountCombineDal.Update_ShopReceiveNew_UserID(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_YLHUserInfo_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveNew]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveNew]",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_shop.[dbo].[ShopReceiveNew]",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 订单关联地址表 )   
        //更新UserId
        public static bool Update_Addresses(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<Addresses> sResults = new List<Addresses>();
            sResults = UserAccountCombineDal.GetAddressesById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_Addresses_UserId = UserAccountCombineDal.Update_Addresses_UserId(
                            new Guid(sItem.AddreesId).ToString("B"),
                            needCombineItem.TargetUserId);
                        if (update_Addresses_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[Addresses]",
                                RelatedTablePK = "u_address_id=" + sItem.AddreesId,
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[Addresses]",
                                RelatedTablePK = "u_address_id=" + new Guid(sItem.AddreesId).ToString("B"),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found u_address_id=" + sItem.AddreesId
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[Addresses]",
                            RelatedTablePK = "u_address_id=" + sItem.AddreesId,
                            UpdatedParameter = "UserId",
                            ActionName = "Update",
                            SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 订单关联车型表 )   
        //更新u_user_id
        public static bool Update_CarObject(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<CarObject> sResults = new List<CarObject>();
            sResults = UserAccountCombineDal.GetCarObjectById(needCombineItem.SourceUserId.ToString("b"));
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_CarObject_UserId = UserAccountCombineDal.Update_CarObject_UserId(
                            new Guid(sItem.CarId).ToString("b"),
                            needCombineItem.TargetUserId.ToString("b"));
                        if (update_CarObject_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].dbo.CarObject",
                                RelatedTablePK = "u_car_id=" + sItem.CarId,
                                UpdatedParameter = "u_user_id",
                                ActionName = "Update",
                                SourceValue = sItem.UserId,
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].dbo.CarObject",
                                RelatedTablePK = "u_car_id=" + sItem.CarId,
                                UpdatedParameter = "u_user_id",
                                ActionName = "Update",
                                SourceValue = sItem.UserId,
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found u_car_id=" + sItem.CarId
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].dbo.CarObject",
                            RelatedTablePK = "u_car_id=" + sItem.CarId,
                            UpdatedParameter = "u_user_id",
                            ActionName = "Update",
                            SourceValue = sItem.UserId,
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新（ 订单表 ）          
        //更新UserID
        public static bool Update_tbl_Order(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<Order> sResults = new List<Order>();
            sResults = UserAccountCombineDal.GetOrderById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_Order_UserID = UserAccountCombineDal.Update_Order_UserID(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_Order_UserID)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Gungnir.dbo.tbl_Order",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Gungnir.dbo.tbl_Order",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Gungnir.dbo.tbl_Order",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 老CRMCase表 )      
        //更新EndUserGuid
        public static bool Update_tbl_EndUserCase(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            _sublogger.Info("UserCombineAndRecordLog_Job_Update_tbl_EndUserCase开始执行SourceUserId:"
                                + needCombineItem.SourceUserId.ToString()
                                + ", TargetUserId:" + needCombineItem.TargetUserId.ToString());
            List<EndUserCase> sResults = new List<EndUserCase>();
            sResults = UserAccountCombineDal.GetEndUserCaseById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_EndUserCase_EndUserGuid = UserAccountCombineDal.Update_EndUserCase_EndUserGuid(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_EndUserCase_EndUserGuid)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Gungnir.dbo.tbl_EndUserCase",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "EndUserGuid",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Gungnir.dbo.tbl_EndUserCase",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "EndUserGuid",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Gungnir.dbo.tbl_EndUserCase",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "EndUserGuid",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.HasValue ? sItem.UserID.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);
            _sublogger.Info("UserCombineAndRecordLog_Job_Update_tbl_EndUserCase执行完成SourceUserId:"
                                + needCombineItem.SourceUserId.ToString()
                                + ", TargetUserId:" + needCombineItem.TargetUserId.ToString());
            return isSuccess;
        }

        //更新( 老CRM重要提醒表 )  
        //更新EndUserGuid
        public static bool Update_tbl_CRM_Requisition(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<CRMRequisition> sResults = new List<CRMRequisition>();
            sResults = UserAccountCombineDal.GetCRMRequisitionById(needCombineItem.SourceUserId.ToString("b"));
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_CRMRequisition_EndUserGuid = UserAccountCombineDal.Update_CRMRequisition_EndUserGuid(sItem.ID,
                            needCombineItem.TargetUserId);
                        if (update_CRMRequisition_EndUserGuid)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Gungnir.dbo.tbl_CRM_Requisition",
                                RelatedTablePK = "ID=" + sItem.ID.ToString(),
                                UpdatedParameter = "EndUserGuid",
                                ActionName = "Update",
                                SourceValue = sItem.EndUserGuid,
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Gungnir.dbo.tbl_CRM_Requisition",
                                RelatedTablePK = "ID=" + sItem.ID.ToString(),
                                UpdatedParameter = "EndUserGuid",
                                ActionName = "Update",
                                SourceValue = sItem.EndUserGuid,
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found ID=" + sItem.ID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Gungnir.dbo.tbl_CRM_Requisition",
                            RelatedTablePK = "ID=" + sItem.ID.ToString(),
                            UpdatedParameter = "EndUserGuid",
                            ActionName = "Update",
                            SourceValue = sItem.EndUserGuid,
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新（ 新CRM预约提醒表 ） 
        //更新UserID
        public static bool Update_CRMAppointment(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<CRMAppointment> sResults = new List<CRMAppointment>();
            sResults = UserAccountCombineDal.GetCRMAppointmentById(needCombineItem.SourceUserId.ToString("b"));
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_CRMAppointment_UserID = UserAccountCombineDal.Update_CRMAppointment_UserID(sItem.PKID,
                            needCombineItem.TargetUserId.ToString("b"));
                        if (update_CRMAppointment_UserID)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_crm.dbo.CRMAppointment",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID,
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_crm.dbo.CRMAppointment",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID,
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_crm.dbo.CRMAppointment",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID,
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新（ 新CRM联系记录表）  
        //更新UserID
        public static bool Update_CRMContactLog(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<CRMContactLog> sResults = new List<CRMContactLog>();
            sResults = UserAccountCombineDal.GetCRMContactLogById(needCombineItem.SourceUserId.ToString("b"));
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_CRMContactLog_UserID = UserAccountCombineDal.Update_CRMContactLog_UserID(sItem.PKID,
                            needCombineItem.TargetUserId.ToString("b"));
                        if (update_CRMContactLog_UserID)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_crm.dbo.CRMContactLog",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID,
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_crm.dbo.CRMContactLog",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID,
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_crm.dbo.CRMContactLog",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID,
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( CRM插旗记录表)　　 
        //更新UserId
        public static bool Update_CRMFlagInfo(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<CRMFlagInfo> sResults = new List<CRMFlagInfo>();
            sResults = UserAccountCombineDal.GetCRMFlagInfoById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_CRMFlagInfo_UserId = UserAccountCombineDal.Update_CRMFlagInfo_UserId(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_CRMFlagInfo_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_crm.dbo.CRMFlagInfo",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_crm.dbo.CRMFlagInfo",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_crm.dbo.CRMFlagInfo",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserId",
                            ActionName = "Update",
                            SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 优惠券表)       
        //更新UserId
        public static bool Update_tbl_PromotionCode(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<PromotionCode> sResults = new List<PromotionCode>();
            sResults = UserAccountCombineDal.GetPromotionCodeById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_PromotionCode_UserId = UserAccountCombineDal.Update_PromotionCode_UserId(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_PromotionCode_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Gungnir.dbo.tbl_PromotionCode",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Gungnir.dbo.tbl_PromotionCode",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserId",
                                ActionName = "Update",
                                SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                                TargetValue = needCombineItem.TargetUserId.ToString("b"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Gungnir.dbo.tbl_PromotionCode",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserId",
                            ActionName = "Update",
                            SourceValue = sItem.UserId.HasValue ? sItem.UserId.Value.ToString("b") : "",
                            TargetValue = needCombineItem.TargetUserId.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新( 积分表)         
        //合并积分Integral字段，Status设0
        //更新( 积分详情表)     
        //更新IntegralID为主UserID对应的IntegralID
        public static bool Update_tbl_UserIntegral(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            //根据SourceUserId,TargetUserId获取用户积分信息
            var sUserIntegral = UserAccountCombineDal.GetUserIntegralById(needCombineItem.SourceUserId);
            var tUserIntegral = UserAccountCombineDal.GetUserIntegralById(needCombineItem.TargetUserId);

            //只有SourceUserId有积分数据才执行以下更新
            if (sUserIntegral != null)
            {
                List<UserIntegralDetail> sUserIntegralDetail = new List<UserIntegralDetail>();
                sUserIntegralDetail = UserAccountCombineDal.GetUserIntegralDetailById(sUserIntegral.IntegralID);

                if (tUserIntegral == null) //TargetUserId没有积分数据，全部新建
                {
                    //新建一个T用户的积分信息，然后插入数据库
                    tUserIntegral = new UserIntegral
                    {
                        IntegralID = Guid.NewGuid(),
                        UserID = needCombineItem.TargetUserId,
                        Status = 1,
                        Integral = sUserIntegral.Integral //用S用户的积分赋值
                    };
                    try
                    {
                        var insert_UserIntegral = UserAccountCombineDal.Insert_UserIntegral(tUserIntegral);
                        if (insert_UserIntegral)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                                RelatedTablePK = "IntegralID=" + tUserIntegral.IntegralID.ToString("b"),
                                UpdatedParameter = "IntegralID",
                                ActionName = "Insert",
                                SourceValue = "",
                                TargetValue = tUserIntegral.IntegralID.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                                RelatedTablePK = "IntegralID=" + tUserIntegral.IntegralID.ToString(),
                                UpdatedParameter = "IntegralID",
                                ActionName = "Insert",
                                SourceValue = "",
                                TargetValue = tUserIntegral.IntegralID.ToString("b"),
                                FailReason = "Insert 0 row, cannot insert IntegralID=" + tUserIntegral.IntegralID.ToString("b")
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                            RelatedTablePK = "IntegralID=" + tUserIntegral.IntegralID.ToString(),
                            UpdatedParameter = "IntegralID",
                            ActionName = "Insert",
                            SourceValue = "",
                            TargetValue = tUserIntegral.IntegralID.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
                else
                {
                    //T用户积分加上S用户的积分，更新表
                    tUserIntegral.Integral += sUserIntegral.Integral;
                    try
                    {
                        var update_UserIntegral_Integral = UserAccountCombineDal.Update_UserIntegral_Integral(
                            tUserIntegral.IntegralID, tUserIntegral.Integral);
                        if (update_UserIntegral_Integral)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                                RelatedTablePK = "IntegralID=" + tUserIntegral.IntegralID.ToString("b"),
                                UpdatedParameter = "Integral",
                                ActionName = "Update",
                                SourceValue = "",
                                TargetValue = tUserIntegral.IntegralID.ToString("b")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                                RelatedTablePK = "IntegralID=" + tUserIntegral.IntegralID.ToString("b"),
                                UpdatedParameter = "Integral",
                                ActionName = "Update",
                                SourceValue = "",
                                TargetValue = tUserIntegral.IntegralID.ToString("b"),
                                FailReason = "Update 0 row, cannot found IntegralID=" + tUserIntegral.IntegralID.ToString("b")
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                            RelatedTablePK = "IntegralID=" + tUserIntegral.IntegralID.ToString("b"),
                            UpdatedParameter = "Integral",
                            ActionName = "Update",
                            SourceValue = "",
                            TargetValue = tUserIntegral.IntegralID.ToString("b"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }

                //如果前面更新T用户积分出问题
                //就不执行下面的更新S用户Status为0; 也不行置空S用户的积分; 也不更新S积分详情表的IntegralID
                if (isSuccess)
                {
                    //更新S用户Status为0
                    try
                    {
                        var update_SourceUserIntegral_Status = UserAccountCombineDal.Update_UserIntegral_Status(
                            sUserIntegral.IntegralID, 0);
                        if (update_SourceUserIntegral_Status)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                                RelatedTablePK = "IntegralID=" + sUserIntegral.IntegralID.ToString("b"),
                                UpdatedParameter = "Status",
                                ActionName = "Update",
                                SourceValue = "1",
                                TargetValue = "0"
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                                RelatedTablePK = "IntegralID=" + sUserIntegral.IntegralID.ToString("b"),
                                UpdatedParameter = "Status",
                                ActionName = "Update",
                                SourceValue = "1",
                                TargetValue = "0",
                                FailReason = "Update 0 row, cannot found IntegralID=" + tUserIntegral.IntegralID.ToString("b")
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                            RelatedTablePK = "IntegralID=" + sUserIntegral.IntegralID.ToString("b"),
                            UpdatedParameter = "Status",
                            ActionName = "Update",
                            SourceValue = "1",
                            TargetValue = "0",
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }

                    //置空S用户的积分
                    try
                    {
                        var update_SourceUserIntegral_Integral = UserAccountCombineDal.Update_UserIntegral_Integral(
                            sUserIntegral.IntegralID, 0);
                        if (update_SourceUserIntegral_Integral)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                                RelatedTablePK = "IntegralID=" + sUserIntegral.IntegralID.ToString("b"),
                                UpdatedParameter = "Integral",
                                ActionName = "Update",
                                SourceValue = sUserIntegral.Integral.ToString(),
                                TargetValue = "0"
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                                RelatedTablePK = "IntegralID=" + sUserIntegral.IntegralID.ToString("b"),
                                UpdatedParameter = "Integral",
                                ActionName = "Update",
                                SourceValue = sUserIntegral.Integral.ToString(),
                                TargetValue = "0",
                                FailReason = "Update 0 row, cannot found IntegralID=" + tUserIntegral.IntegralID.ToString("b")
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                            RelatedTablePK = "IntegralID=" + sUserIntegral.IntegralID.ToString("b"),
                            UpdatedParameter = "Integral",
                            ActionName = "Update",
                            SourceValue = sUserIntegral.Integral.ToString(),
                            TargetValue = "0",
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }

                    //更新S积分详情表的IntegralID
                    if (sUserIntegralDetail != null)
                    {
                        foreach (var sItem in sUserIntegralDetail)
                        {
                            try
                            {
                                var update_UserIntegralDetail_IntegralID = UserAccountCombineDal.Update_UserIntegralDetail_IntegralID(
                                    sItem.IntegralDetailID,
                                    tUserIntegral.IntegralID);
                                if (update_UserIntegralDetail_IntegralID)
                                {
                                    logList.Add(new RelatedTableCombineLog
                                    {
                                        SourceUserId = needCombineItem.SourceUserId,
                                        TargetUserId = needCombineItem.TargetUserId,
                                        RelatedTableName = "Tuhu_profiles.dbo.tbl_UserIntegralDetail",
                                        RelatedTablePK = "IntegralDetailID=" + sItem.IntegralDetailID.ToString(),
                                        UpdatedParameter = "IntegralID",
                                        ActionName = "Update",
                                        SourceValue = sItem.IntegralID.ToString("b"),
                                        TargetValue = tUserIntegral.IntegralID.ToString("b")
                                    });
                                }
                                else
                                {
                                    faillogList.Add(new RelatedTableCombineFailLog
                                    {
                                        SourceUserId = needCombineItem.SourceUserId,
                                        TargetUserId = needCombineItem.TargetUserId,
                                        RelatedTableName = "Tuhu_profiles.dbo.tbl_UserIntegralDetail",
                                        RelatedTablePK = "IntegralDetailID=" + sItem.IntegralDetailID.ToString(),
                                        UpdatedParameter = "IntegralID",
                                        ActionName = "Update",
                                        SourceValue = sItem.IntegralID.ToString("b"),
                                        TargetValue = tUserIntegral.IntegralID.ToString("b"),
                                        FailReason = "Update 0 row, cannot found IntegralDetailID=" + sItem.IntegralDetailID.ToString()
                                    });
                                    isSuccess = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                faillogList.Add(new RelatedTableCombineFailLog
                                {
                                    SourceUserId = needCombineItem.SourceUserId,
                                    TargetUserId = needCombineItem.TargetUserId,
                                    RelatedTableName = "Tuhu_profiles.dbo.tbl_UserIntegralDetail",
                                    RelatedTablePK = "IntegralDetailID=" + sItem.IntegralDetailID.ToString(),
                                    UpdatedParameter = "IntegralID",
                                    ActionName = "Update",
                                    SourceValue = sItem.IntegralID.ToString("b"),
                                    TargetValue = tUserIntegral.IntegralID.ToString("b"),
                                    FailReason = ex.Message
                                });
                                isSuccess = false;
                            }
                        }
                    }
                }
                else
                {
                    faillogList.Add(new RelatedTableCombineFailLog
                    {
                        SourceUserId = needCombineItem.SourceUserId,
                        TargetUserId = needCombineItem.TargetUserId,
                        RelatedTableName = "[Tuhu_profiles].[dbo].[tbl_UserIntegral]",
                        RelatedTablePK = "IntegralID=" + tUserIntegral.IntegralID.ToString("b"),
                        UpdatedParameter = "IntegralID",
                        ActionName = "Update",
                        SourceValue = "",
                        TargetValue = tUserIntegral.IntegralID.ToString("b"),
                        FailReason = "操作TargetUser积分时出错，后续更新SourceUser积分和积分详情操作均不执行，请检查TargetUser的数据，IntegralID=" + tUserIntegral.IntegralID.ToString("b")
                    });
                }
            }

            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }

        //更新（用户成长值表）
        //更新UserID
        public static bool Update_tbl_UserGradeStatisticsDetail(NeedCombineUserId needCombineItem)
        {
            List<RelatedTableCombineLog> logList = new List<RelatedTableCombineLog>();
            List<RelatedTableCombineFailLog> faillogList = new List<RelatedTableCombineFailLog>();
            bool isSuccess = true;

            List<UserGradeStatisticsDetail> sResults = new List<UserGradeStatisticsDetail>();
            sResults = UserAccountCombineDal.GetUserGradeStatisticsDetailById(needCombineItem.SourceUserId);
            if (sResults != null && sResults.Any())
            {
                foreach (var sItem in sResults)
                {
                    try
                    {
                        var update_UserGradeStatisticsDetail_UserId = UserAccountCombineDal.Update_UserGradeStatisticsDetail_UserId(sItem.PKID,
                            needCombineItem.TargetUserId);
                        if (update_UserGradeStatisticsDetail_UserId)
                        {
                            logList.Add(new RelatedTableCombineLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_profiles.dbo.UserGradeStatisticsDetail",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.ToString("d"),
                                TargetValue = needCombineItem.TargetUserId.ToString("d")
                            });
                        }
                        else
                        {
                            faillogList.Add(new RelatedTableCombineFailLog
                            {
                                SourceUserId = needCombineItem.SourceUserId,
                                TargetUserId = needCombineItem.TargetUserId,
                                RelatedTableName = "Tuhu_profiles.dbo.UserGradeStatisticsDetail",
                                RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                                UpdatedParameter = "UserID",
                                ActionName = "Update",
                                SourceValue = sItem.UserID.ToString("d"),
                                TargetValue = needCombineItem.TargetUserId.ToString("d"),
                                FailReason = "Update 0 row, cannot found PKID=" + sItem.PKID.ToString()
                            });
                            isSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        faillogList.Add(new RelatedTableCombineFailLog
                        {
                            SourceUserId = needCombineItem.SourceUserId,
                            TargetUserId = needCombineItem.TargetUserId,
                            RelatedTableName = "Tuhu_profiles.dbo.UserGradeStatisticsDetail",
                            RelatedTablePK = "PKID=" + sItem.PKID.ToString(),
                            UpdatedParameter = "UserID",
                            ActionName = "Update",
                            SourceValue = sItem.UserID.ToString("d"),
                            TargetValue = needCombineItem.TargetUserId.ToString("d"),
                            FailReason = ex.Message
                        });
                        isSuccess = false;
                    }
                }
            }
            if (logList.Count() > 0) InsertRelatedTableSuccessLog(logList);
            if (faillogList.Count() > 0) InsertRelatedTableFailLog(faillogList);

            return isSuccess;
        }
        #endregion

        public static bool InsertRelatedTableSuccessLog(List<RelatedTableCombineLog> items)
        {
            var result = true;
            var middleResult = true;
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        result = UserAccountCombineDal.InsertRelatedTableCombineSuccessLog(item, dbHelper);
                        if (!result)
                        {
                            _sublogger.Info("UserCombineAndRecordLog_Job_InsertRelatedTableSuccessLog方法出错在SourceUserId:"
                                + item.SourceUserId.ToString()
                                + ", TargetUserId:" + item.TargetUserId.ToString());
                            middleResult = false;
                        }
                    }
                }

                dbHelper.Commit();
                if (!middleResult) return false;
                return true;
            }
        }

        public static bool InsertRelatedTableFailLog(List<RelatedTableCombineFailLog> items)
        {
            var result = true;
            var middleResult = true;
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        result = UserAccountCombineDal.InsertRelatedTableCombineFailLog(item, dbHelper);
                        if (!result)
                        {
                            _sublogger.Info("UserCombineAndRecordLog_Job_InsertRelatedTableFailLog方法出错在SourceUserId:"
                                + item.SourceUserId.ToString()
                                + ", TargetUserId:" + item.TargetUserId.ToString());
                            middleResult = false;
                        }
                    }
                }

                dbHelper.Commit();

                if (!middleResult) return false;
                return true;
            }
        }
        #endregion

        #region CleanDupYewuUserJob
        public static List<YewuThirdpartyInfo> GetDupYewuUsers()
            => UserAccountCombineDal.GetDupYewuUsers();

        public static List<YewuThirdpartyInfo> CollectNeedDeleteDupYewuUserList(List<YewuThirdpartyInfo> dupList)
        {
            List<YewuThirdpartyInfo> resultList = new List<YewuThirdpartyInfo>();

            //stepOne 获取唯一的UserId+ThirdpartyId+Channel
            var stepOne =
                dupList
                .GroupBy(_item => new { UserId = _item.UserId, ThirdpartyId = _item.ThirdpartyId.ToLower(), Channel = _item.Channel.ToLower() })
                .Select(_group => new { _group.First().UserId, _group.First().ThirdpartyId, _group.First().Channel })
                .ToList();

            if (stepOne != null)
            {
                foreach (var item in stepOne)
                {
                    //stepTwo 获取相同三参数数据中pkid最小的记录
                    //return List<int>
                    var stepTwo = dupList
                        .OrderBy(_item => _item.PKID)
                        .Where(_item => _item.Channel.ToLower() == item.Channel.ToLower()
                      && _item.ThirdpartyId.ToLower() == item.ThirdpartyId.ToLower()
                      && _item.UserId == item.UserId)
                    .FirstOrDefault();

                    //stepThree 筛选除stepTwo里一条记录外，同三参数数据的所有数据
                    var stepThree = dupList
                        .Where(_item => _item.PKID != stepTwo.PKID
                        && _item.ThirdpartyId.ToLower() == stepTwo.ThirdpartyId.ToLower()
                        && _item.UserId == stepTwo.UserId
                        && _item.Channel.ToLower() == stepTwo.Channel.ToLower()).ToList();

                    //stepFour 收集结果集
                    resultList.AddRange(stepThree);
                }
            }
            return resultList;
        }

        public static bool DeleteAndRecordDupYewuUsers(List<YewuThirdpartyInfo> items)
        {
            var result = true;
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var result_delete = UserAccountCombineDal.DeleteDupYewuThirdpartyInfo(item.PKID, dbHelper);
                        var result_record = UserAccountCombineDal.RecordDeletedDupYewuThirdpartyInfo(item, dbHelper);

                        if (!result_delete || !result_record)
                        {
                            _sublogger.Info("CleanDupYewuUserJob_DeleteDupYewuUsers方法出错在PKID:"
                                + item.PKID);
                            return false;
                        }
                    }
                }
                dbHelper.Commit();

                return result;
            }
        }
        #endregion

        #region CollectNeedCombineInUserTableJob
        public static List<UserObjectModel> GetNeedCombineInUserTable()
            => UserAccountCombineDal.GetNeedCombineInUserTable();

        public static List<NeedCombineUserIdViaPhone> FilterPrimaryUserIdsViaPhone(List<UserObjectModel> Users)
        {
            List<NeedCombineUserIdViaPhone> resultList = new List<NeedCombineUserIdViaPhone>();
            int countNum = 0;
            //stepOne 获取唯一的MobileNumber
            try
            {
                var stepOne =
                Users
                .Where(_item => !string.IsNullOrWhiteSpace(_item.MobileNumber))
                .GroupBy(_item => new { MobileNumber = _item.MobileNumber })
                .Select(_group => new { _group.First().MobileNumber })
                .ToList();

                _sublogger.Info("CollectNeedCombineInUserTable,StepOne获取唯一Mobile,Done." + DateTime.Now.ToString());

                List<NeedCombineUserIdViaPhone> allExistNeedCombineUserIdViaPhoneList = UserAccountCombineDal.GetAllExistNeedCombineUserIdViaPhoneList();

                if (stepOne != null)
                {
                    foreach (var item in stepOne)
                    {
                        //steptwo 获取对应的UserIds
                        //return List<Guid>
                        var stepTwo = Users.Where(_item => _item.MobileNumber == item.MobileNumber)
                            .Select(_item => _item.UserId.Value)
                            .ToList();

                        //stepThree 根据UserIds获取User信息
                        //return List<UserObjectModel>
                        var stepThree = GetUsersByIds(stepTwo);

                        //stepFour 分析出Users里主userId
                        //return Guid
                        var stepFour = FilterUserObjectPrimaryRule(stepThree, resultList, allExistNeedCombineUserIdViaPhoneList);

                        //stepFive 生成主userId和需要合并userid的数据结构
                        //return List<NeedCombineUserIdViaPhone>
                        var result = CombineUserObjectIds(stepThree, stepFour, item.MobileNumber);

                        //stepSix 合并结果集
                        resultList.AddRange(result);

                        countNum++;
                        if (countNum % 100 == 0)
                        {
                            _sublogger.Info("CollectNeedCombineInUserTable,StepTwo整理出" + countNum + "个手机号,当前手机号是:" + item.MobileNumber + DateTime.Now.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _sublogger.Info("CollectNeedCombineInUserTable,FilterPrimaryUserId环节出错." + ex);
            }

            return resultList;
        }

        public static Guid FilterUserObjectPrimaryRule(List<UserObjectModel> users, List<NeedCombineUserIdViaPhone> resultList, List<NeedCombineUserIdViaPhone> existList)
        {
            Guid primaryGuid = new Guid();
            List<UserObjectModel> tempRemoveUsers = new List<UserObjectModel>();
            List<UserObjectModel> tempUsers = new List<UserObjectModel>();
            tempUsers.AddRange(users);

            //在本次Job执行中已经记录的需要合并数据结果集resultList中
            //判断如果users里有userId等于resultList里的主账号TargetUserId，直接输出
            //判断如果users里有userId等于resultList里的被合并账号SourceUserId，把这个userId从users里删除
            for (int i = tempUsers.Count - 1; i >= 0; i--)
            {
                var checkExistTargetUserId = resultList.Where(_target => _target.TargetUserId == tempUsers[i].UserId.Value).ToList();
                if (checkExistTargetUserId != null && checkExistTargetUserId.Any())
                {
                    return tempUsers[i].UserId.Value;
                }

                var checkExistSourceUserId = resultList.Where(_source => _source.SourceUserId == tempUsers[i].UserId).ToList();
                if (checkExistSourceUserId != null && checkExistSourceUserId.Any())
                {
                    tempUsers.Remove(tempUsers[i]);
                }
            }

            //在之前Job执行中已经记录的合并数据结果集allExistNeedCombineUserIdList中
            //判断如果users里有userId等于allExistNeedCombineUserIdList里的主账号TargetUserId，直接输出
            //判断如果users里有userId等于allExistNeedCombineUserIdList里的被合并账号SourceUserId，把这个userId从users里删除
            //List<NeedCombineUserIdViaPhone> allExistNeedCombineUserIdViaPhoneList = UserAccountCombineDal.GetAllExistNeedCombineUserIdViaPhoneList();
            if (existList != null && existList.Any())
            {
                for (int i = tempUsers.Count - 1; i >= 0; i--)
                {
                    var checkExistTargetUserId = existList.
                        Where(_target => _target.TargetUserId == tempUsers[i].UserId.Value).
                        ToList();
                    if (checkExistTargetUserId != null && checkExistTargetUserId.Any())
                    {
                        return tempUsers[i].UserId.Value;
                    }

                    var checkExistSourceUserId = existList.
                        Where(_source => _source.SourceUserId == tempUsers[i].UserId).
                        ToList();
                    if (checkExistSourceUserId != null && checkExistSourceUserId.Any())
                    {
                        tempUsers.Remove(tempUsers[i]);
                    }
                }
            }

            //首先挑选已经验证过手机号
            var pickupList = tempUsers.Where(_item => _item.IsActive
              && _item.IsMobileVerify.HasValue
              && _item.IsMobileVerify == true)
            .ToList();

            if (pickupList == null || !pickupList.Any())  //如果都没有验证过手机号
            {
                //筛选出状态是active的
                pickupList = tempUsers.Where(_item => _item.IsActive)
                                        .ToList();

                if (pickupList == null || !pickupList.Any()) //如果都是非active
                {
                    pickupList = tempUsers;
                }
            }

            //获取pickupList中用户的Auths信息
            var hasAuthList = UserAccountCombineDal.GetUserAuthsByIds(pickupList);
            if (hasAuthList != null && hasAuthList.Any())
            {
                var authIds = hasAuthList.Select(_id => _id.UserId).ToList();

                pickupList = (from p in pickupList
                              where authIds.Contains(p.UserId.Value)
                              select p)
                              .ToList();
            }

            //获取pickupList中用户YLH信息
            var hasYLHList = UserAccountCombineDal.GetUserYLHInfosByIds(pickupList);
            if (hasYLHList != null && hasYLHList.Any())
            {
                var ylhIds = hasYLHList.Select(_id => new Guid(_id.UserId)).ToList();

                pickupList = (from p in pickupList
                              where ylhIds.Contains(p.UserId.Value)
                              select p)
                              .ToList();
            }

            var filterList = pickupList.Where(_item => _item.LastLogonTime.HasValue).ToList();
            if (filterList == null || !filterList.Any()) //如果都没有最后登录时间，按照注册时间最近一次的账号为准
            {
                primaryGuid = pickupList.OrderByDescending(_item => _item.CreatedTime).First().UserId.Value;
            }
            else  //有最后登录时间的，按照最近一次的账号为准
            {
                primaryGuid = filterList.OrderByDescending(_item => _item.LastLogonTime).First().UserId.Value;
            }

            return primaryGuid;
        }

        public static List<NeedCombineUserIdViaPhone> CombineUserObjectIds(List<UserObjectModel> users, Guid primaryGuid, string mobileNumber)
        {
            List<NeedCombineUserIdViaPhone> resultList = new List<NeedCombineUserIdViaPhone>();
            if (users != null)
            {
                foreach (var item in users)
                {
                    if (item.UserId.Value != primaryGuid)
                    {
                        resultList.Add(new NeedCombineUserIdViaPhone
                        {
                            CreateDataTime = DateTime.Now,
                            SourceUserId = item.UserId.Value,
                            TargetUserId = primaryGuid,
                            MobileNumber = mobileNumber
                        });
                    }
                }
            }
            return resultList;
        }

        public static bool InsertNeedCombineUserIdsViaPhone(List<NeedCombineUserIdViaPhone> items)
        {
            var result = true;
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        result = UserAccountCombineDal.InsertNeedCombineUserIdViaPhone(item, dbHelper);
                        if (!result)
                        {
                            _sublogger.Info("CollectNeedCombineInUserTable_Job_InsertNeedCombineUserIdsViaPhone方法出错在SourceUserId:"
                                + item.SourceUserId.ToString()
                                + ", TargetUserId:" + item.TargetUserId.ToString());
                            return result;
                        }
                    }
                }

                dbHelper.Commit();

                return result;
            }
        }
        #endregion

        #region UserCombineViaPhoneAndRecordLogJob
        public static List<NeedCombineUserIdViaPhone> GetNeedCombineUserIdViaPhoneList()
            => UserAccountCombineDal.GetNeedCombineUserIdViaPhoneList();

        public static string CombineAndRecordActionForPhoneCombile(List<NeedCombineUserIdViaPhone> needCombineViaPhoneList
            , string runtimeSwitch)
        {
            string result = "一共执行" + needCombineViaPhoneList.Count() + "行记录,";
            int successCount = 0;
            int failCount = 0;

            if (needCombineViaPhoneList != null)
            {
                foreach (var needCombineItemViaPhone in needCombineViaPhoneList)
                {
                    if (!CheckSwitch(runtimeSwitch))
                    {
                        break;
                    }

                    NeedCombineUserId needCombineItem = new NeedCombineUserId
                    {
                        SourceUserId = needCombineItemViaPhone.SourceUserId,
                        TargetUserId = needCombineItemViaPhone.TargetUserId,
                        PKID = needCombineItemViaPhone.PKID
                    };

                    #region 用户账号
                    //更新( 用户账号主表)      更新IsActive, 合并u_addresses, 更新u_preferred_address
                    var result_Update_UserObjectTable = Update_UserObjectTable(needCombineItem);

                    //更新（用户授权表）  更新UserId字段
                    var result_Update_UserAuthTable = Update_UserAuthTable(needCombineItem);

                    //更新( 三方用户业务表)    更新UserId
                    var result_Update_YewuThirdpartyInfoByUserId = Update_YewuThirdpartyInfoByUserId(needCombineItem);

                    //更新( 永隆行用户信息表)  更新u_user_id
                    var result_Update_YLH_UserInfo = Update_YLH_UserInfo(needCombineItem);

                    //更新( 永隆行会员卡车辆表)更新u_user_id
                    var result_Update_YLH_UserVipCardInfo = Update_YLH_UserVipCardInfo(needCombineItem);
                    #endregion

                    #region 门店
                    //更新( 门店预约记录表 )   更新UserID
                    var result_Update_ShopReserve = Update_ShopReserve(needCombineItem);

                    //更新( 门店质检表 )       更新UserID
                    var result_Update_ShopReceiveCheckThird = Update_ShopReceiveCheckThird(needCombineItem);

                    //更新( 门店上线检测表 )   更新UserID
                    var result_Update_ShopReceiveCheckSecond = Update_ShopReceiveCheckSecond(needCombineItem);

                    //更新( 门店预检表 )       更新UserID
                    var result_Update_ShopReceiveCheckFirst = Update_ShopReceiveCheckFirst(needCombineItem);

                    //更新( 门店接待订单表 )   更新UserID
                    var result_Update_ShopReceiveOrder = Update_ShopReceiveOrder(needCombineItem);

                    //更新( 门店到店记录表 )   更新UserID
                    var result_Update_ShopReceive = Update_ShopReceive(needCombineItem);

                    //更新( 门店到店记录表 )   更新UserID
                    var result_Update_ShopReceiveNew = Update_ShopReceiveNew(needCombineItem);
                    #endregion

                    #region 订单
                    //更新( 订单关联地址表 )   更新UserId,只设置一个IsDefaultAddress
                    var result_Update_Addresses = Update_Addresses(needCombineItem);

                    //更新( 订单关联车型表 )   更新u_user_id
                    var result_Update_CarObject = Update_CarObject(needCombineItem);

                    //更新（ 订单表 ）          更新UserID
                    var result_Update_tbl_Order = Update_tbl_Order(needCombineItem);
                    #endregion

                    #region CRM
                    //更新( 老CRMCase表 )      更新EndUserGuid
                    var result_Update_tbl_EndUserCase = Update_tbl_EndUserCase(needCombineItem);

                    //更新( 老CRM重要提醒表 )  更新EndUserGuid
                    var result_Update_tbl_CRM_Requisition = Update_tbl_CRM_Requisition(needCombineItem);

                    //更新（ 新CRM预约提醒表 ） 更新UserID
                    var result_Update_CRMAppointment = Update_CRMAppointment(needCombineItem);

                    //更新（ 新CRM联系记录表）  更新UserID
                    var result_Update_CRMContactLog = Update_CRMContactLog(needCombineItem);

                    //更新( CRM插旗记录表)　　 更新UserId
                    var result_Update_CRMFlagInfo = Update_CRMFlagInfo(needCombineItem);
                    #endregion

                    #region 优惠券、积分
                    //更新( 优惠券表)       更新UserId
                    var result_Update_tbl_PromotionCode = Update_tbl_PromotionCode(needCombineItem);

                    //更新( 积分表)         合并积分Integral字段，Status设0
                    //更新( 积分详情表)     更新IntegralID为主UserID对应的IntegralID
                    var result_Update_tbl_UserIntegral = Update_tbl_UserIntegral(needCombineItem);

                    //更新（用户成长值表）
                    //更新UserID
                    var result_Update_tbl_UserGradeStatisticsDetail = Update_tbl_UserGradeStatisticsDetail(needCombineItem);
                    #endregion

                    if (result_Update_UserObjectTable
                        && result_Update_UserAuthTable
                        && result_Update_YewuThirdpartyInfoByUserId
                        && result_Update_YLH_UserInfo
                        && result_Update_YLH_UserVipCardInfo
                        && result_Update_ShopReserve
                        && result_Update_ShopReceiveCheckThird
                        && result_Update_ShopReceiveCheckSecond
                        && result_Update_ShopReceiveCheckFirst
                        && result_Update_ShopReceiveOrder
                        && result_Update_ShopReceive
                        && result_Update_ShopReceiveNew
                        && result_Update_Addresses
                        && result_Update_CarObject
                        && result_Update_tbl_Order
                        && result_Update_tbl_EndUserCase
                        && result_Update_tbl_CRM_Requisition
                        && result_Update_CRMAppointment
                        && result_Update_CRMContactLog
                        && result_Update_CRMFlagInfo
                        && result_Update_tbl_PromotionCode
                        && result_Update_tbl_UserIntegral
                        && result_Update_tbl_UserGradeStatisticsDetail)
                    {
                        //更新NeedCombineUserId里的IsOperateSuccess为1
                        try
                        {
                            var update_NeedCombineUserIdViaPhone_IsOperateSuccess =
                                UserAccountCombineDal.Update_NeedCombineUserIdViaPhone_IsOperateSuccess(needCombineItemViaPhone);
                            if (!update_NeedCombineUserIdViaPhone_IsOperateSuccess)
                            {
                                _sublogger.Info("UserCombineViaPhoneAndRecordLog_Job_Update_NeedCombineUserIdViaPhone_IsOperateSuccess方法出错在SourceUserId:"
                                    + needCombineItemViaPhone.SourceUserId.ToString()
                                    + ", TargetUserId:" + needCombineItemViaPhone.TargetUserId.ToString()
                                    + ", PKID:" + needCombineItemViaPhone.PKID);
                            }

                            //如果操作成功后，执行登出用户操作
                            var result_Target = LogoutUser(needCombineItemViaPhone.TargetUserId);
                            if (!result_Target.Result)
                            {
                                _sublogger.Info("UserCombineViaPhoneAndRecordLog_Job_Update_NeedCombineUserIdViaPhone登出方法出错在TargetUserId:" + needCombineItemViaPhone.TargetUserId.ToString()
                                    + ", PKID:" + needCombineItemViaPhone.PKID);
                            }
                            var result_Source = LogoutUser(needCombineItemViaPhone.SourceUserId);
                            if (!result_Source.Result)
                            {
                                _sublogger.Info("UserCombineViaPhoneAndRecordLog_Job_Update_NeedCombineUserIdViaPhone登出方法出错在SourceUserId:" + needCombineItemViaPhone.SourceUserId.ToString()
                                    + ", PKID:" + needCombineItemViaPhone.PKID);
                            }
                        }
                        catch (Exception ex)
                        {
                            _sublogger.Info("UserCombineViaPhoneAndRecordLog_Job_Update_NeedCombineUserIdViaPhone_IsOperateSuccess方法出错在SourceUserId:"
                                    + needCombineItemViaPhone.SourceUserId.ToString()
                                    + ", TargetUserId:" + needCombineItemViaPhone.TargetUserId.ToString()
                                    + ", PKID:" + needCombineItemViaPhone.PKID + ex.Message);
                        }

                        successCount++; // 累计成功
                    }
                    else
                        failCount++; // 累计失败
                }
            }

            result = result + "成功" + successCount + "行记录, 失败" + failCount + "行记录.";
            return result;
        }

        public async static Task<bool> LogoutUser(Guid userId)
        {
            using (var client = new AccessTokenClient())
            {
                var result = await client.RemoveAllAsync(userId, "合并账号登出");
                result.ThrowIfException(true);
                return result.Result > 0;
            }
        }

        public static void Task_CombineAndRecordActionForPhoneCombile(List<NeedCombineUserIdViaPhone> needCombineViaPhoneList
            , string runtimeSwitch, string onceRunLastWord)
        {
            try
            {
                List<NeedCombineUserIdViaPhone> list = new List<NeedCombineUserIdViaPhone>();
                if (!string.IsNullOrWhiteSpace(onceRunLastWord) && onceRunLastWord != "")
                {
                    list = needCombineViaPhoneList
                        .Where(_item => _item.MobileNumber.EndsWith(onceRunLastWord))
                        .ToList();
                }
                if (null == list || !list.Any())
                {
                    _sublogger.Info("UserCombineViaPhoneAndRecordLogJob没有手机号字段以" + onceRunLastWord + "为结尾的记录需要合并：" + DateTime.Now.ToString());
                }
                else
                {
                    var result = CombineAndRecordActionForPhoneCombile(list, runtimeSwitch);
                }
            }
            catch (Exception ex)
            {
                _sublogger.Info($"UserCombineViaPhoneAndRecordLogJob：运行异常=》{ex}");
            }
        }
        #endregion

        #region BatchImportUserJob
        public static bool BatchImportUser()
        {
            var result = false;
            string[] txtData = File.ReadAllLines("BatchImportMobileUsers.txt", System.Text.Encoding.Default);
            List<string> mobileList = txtData.Distinct().ToList();
            int importSuccess = 0;
            int count = 0;

            if (mobileList.Any())
            {
                foreach (var item in mobileList)
                {
                    count++;
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        try
                        {
                            using (var client = new UserAccountClient())
                            {
                                //去除前后的空格
                                var mobile = item.Replace(" ", "");

                                if (!string.IsNullOrWhiteSpace(mobile))
                                {
                                    var exist = client.GetUserByMobile(item);
                                    exist.ThrowIfException(true);

                                    if (exist.Result != null && exist.Result.UserId != Guid.Empty)
                                        importSuccess++;
                                    else
                                    {
                                        Tuhu.Service.UserAccount.Models.CreateUserRequest request = new Service.UserAccount.Models.CreateUserRequest()
                                        {
                                            ChannelIn = nameof(ChannelIn.Import),
                                            UserCategoryIn = nameof(UserCategoryIn.Tuhu),
                                            Profile = new Service.UserAccount.Models.UserProfile(),
                                            MobileNumber = mobile,
                                            CRMInfo = new Service.UserAccount.Models.UserCRMInfo(),
                                            IsMobileVerified = false
                                        };
                                        var import = client.CreateUserRequest(request);
                                        import.ThrowIfException(true);

                                        if (import.Success && import.Result != null && import.Result.UserId != Guid.Empty)
                                            importSuccess++;
                                        else
                                            _sublogger.Info("BatchImportUser插入手机号用户:" + mobile + "失败");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _sublogger.Info("BatchImportUser异常:" + ex.ToString());
                        }
                    }

                    if (count % 100 == 0)
                    {
                        _sublogger.Info("BatchImportUser已经执行" + count + "条数据，当前手机号" + item + ",一共需要执行" + mobileList.Count + "条数据.");
                    }
                }
            }
            else
            {
                _sublogger.Info("BatchImportUser需要插入用户数量0，因为文本空.");
            }

            _sublogger.Info("BatchImportUser一共插入了" + importSuccess + "条数据，文本内需要执行" + mobileList.Count + "条数据.");
            return result;
        }
        #endregion

        #region BatchLogOffUserJob
        public static bool BatchLogOffUser()
        {
            var result = false;
            int count = 0;
            List<Guid> userIds = UserAccountCombineDal.GetNeedLogOffUserIdsForChangeBind();

            if (userIds.Any())
            {
                foreach (var id in userIds)
                {
                    count++;
                    if (id != null && id != Guid.Empty)
                    {
                        try
                        {
                            using (var client = new UserAccountClient())
                            {
                                var logoffResult = client.LogOffUser(id);

                                if (!logoffResult.Success)
                                {
                                    using (var accessClient = new AccessTokenClient())
                                    {
                                        var backupResult = accessClient.RemoveAll(id, "换绑登出");
                                        backupResult.ThrowIfException(true);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _sublogger.Info("BatchLogOffUser异常:" + ex.ToString());
                        }
                    }

                    if (count % 100 == 0)
                    {
                        _sublogger.Info("BatchLogOffUser已经执行" + count + "条数据，一共需要执行" + userIds.Count + "条数据.");
                    }
                }
            }
            return result;
        }
        #endregion

        #region CollectUserDailyIncJob
        public static void CollectUserTableDailyInc(string date)
        {
            var totalExistResult = UserAccountCombineDal.CheckDailyTotalDataExist(date);
            if (totalExistResult)
            {
                //存在数据
                var totalResult = UserAccountCombineDal.GetDailyTotalNum(date);
                var countResult = UserAccountCombineDal.GetDailyIncCountNum(date);
                if (totalResult != countResult)
                {
                    //数据存在差异，各渠道数据总和不等于Total数据
                    //先清理掉当天数据，再重新计算
                    var clearResult = UserAccountCombineDal.ClearDailyIncData(date);
                    CalculateAndInsertUserDailyInc(date);
                }
                else
                {
                    //不需要做任何操作，数据完整
                }
            }
            else
            {
                //不存在数据
                //计算数据
                CalculateAndInsertUserDailyInc(date);
            }
        }

        public static bool CalculateAndInsertUserDailyInc(string date)
        {
            List<UserGrowthSchedule> growthList = new List<UserGrowthSchedule>();

            var totalInc = UserAccountCombineDal.GetDailyIncTotalInfo(date);

            if (totalInc == null || string.IsNullOrWhiteSpace(totalInc.Channel))
            {
                growthList.Add(new UserGrowthSchedule
                {
                    Channel = "Total",
                    Group = "Total",
                    DayString = DateTime.Parse(date),
                    IncNum = 0
                });
            }
            else
            {
                growthList.Add(new UserGrowthSchedule
                {
                    Channel = totalInc.Channel,
                    Group = totalInc.Channel,
                    DayString = DateTime.Parse(totalInc.CreateTime),
                    IncNum = totalInc.CountNum
                });

                var detailIncs = UserAccountCombineDal.GetDailyIncInfos(date);
                if (detailIncs == null || !detailIncs.Any() || detailIncs.Count < 1)
                {
                    Tuhu.TuhuMessage.SendEmail("【用户日增长Warning】日期" + date + "各项增长人数缺失EOM",
                        "zhangchen3@tuhu.cn;liuchao1@tuhu.cn",
                        "请检查数据,稍后重试.");
                    return false;
                }
                else
                {
                    int checkSum = detailIncs.Sum(_ => _.CountNum);

                    //判断数据不一致
                    if (checkSum != totalInc.CountNum)
                    {
                        Tuhu.TuhuMessage.SendEmail("【用户日增长Warning】日期" + date + "总增长人数和各项增长人数总和，数据对不上EOM",
                                "zhangchen3@tuhu.cn;liuchao1@tuhu.cn",
                                "总增长人数：" + totalInc.CountNum + ", 各项增长人数总和：" + checkSum + ", 请检查数据.");
                        return false;
                    }
                    else
                    {
                        foreach (var inc in detailIncs)
                        {
                            var dictionary = SetDailyChannelAndGroup();
                            if (string.IsNullOrWhiteSpace(inc.Channel) || !dictionary.ContainsKey(inc.Channel))
                            {
                                Tuhu.TuhuMessage.SendEmail("【用户日增长Warning】日期" + date + "出现未收录的数据类型EOM",
                                "zhangchen3@tuhu.cn;liuchao1@tuhu.cn",
                                "未收录的数据类型：" + inc.Channel + ", 请检查数据.");
                                return false;
                            }
                            else
                            {
                                if (inc.Channel == "H5")
                                {
                                    var h5Inc = UserAccountCombineDal.GetSpecifiedDailyIncInfo(date, "H5");
                                    if (h5Inc == null || string.IsNullOrWhiteSpace(h5Inc.Channel))
                                    {
                                        growthList.Add(new UserGrowthSchedule
                                        {
                                            Channel = inc.Channel,
                                            Group = "Unauthenticated",
                                            DayString = DateTime.Parse(inc.CreateTime),
                                            IncNum = inc.CountNum
                                        });
                                    }
                                    else if (h5Inc.CountNum == inc.CountNum)
                                    {
                                        growthList.Add(new UserGrowthSchedule
                                        {
                                            Channel = inc.Channel,
                                            Group = "Authenticate",
                                            DayString = DateTime.Parse(inc.CreateTime),
                                            IncNum = inc.CountNum
                                        });
                                    }
                                    else
                                    {
                                        growthList.Add(new UserGrowthSchedule
                                        {
                                            Channel = inc.Channel,
                                            Group = "Authenticate",
                                            DayString = DateTime.Parse(inc.CreateTime),
                                            IncNum = h5Inc.CountNum
                                        });
                                        growthList.Add(new UserGrowthSchedule
                                        {
                                            Channel = inc.Channel,
                                            Group = "Unauthenticated",
                                            DayString = DateTime.Parse(inc.CreateTime),
                                            IncNum = inc.CountNum - h5Inc.CountNum
                                        });
                                    }
                                }
                                else
                                {
                                    growthList.Add(new UserGrowthSchedule
                                    {
                                        Channel = inc.Channel,
                                        Group = dictionary[inc.Channel],
                                        DayString = DateTime.Parse(inc.CreateTime),
                                        IncNum = inc.CountNum
                                    });
                                }
                            }
                        }
                    }
                }
            }

            //数据插入
            var insert = UserAccountCombineDal.InsertDailyIncInfos(growthList);
            if (!insert)
            {
                Tuhu.TuhuMessage.SendEmail("【用户日增长Warning】日期" + date + "插入日增长数据失败EOM",
                        "zhangchen3@tuhu.cn;liuchao1@tuhu.cn",
                        "请检查数据,稍后重试.");
                return false;
            }

            return true;
        }

        public static Dictionary<string, string> SetDailyChannelAndGroup()
        {
            Dictionary<string, string> channelInGroup = new Dictionary<string, string>();
            var list = UserAccountCombineDal.GetUserChannelGroupV1();

            foreach (var item in list)
            {
                channelInGroup.Add(item.Channel, item.Group);
            }

            return channelInGroup;
        }
        #endregion

        #region Monitor
        public static int GetCount_UnionIdBindMultiUserIds()
            => UserAccountCombineDal.GetCount_UnionIdBindMultiUserIds();

        public static int GetCount_UserIdBindMultiUnionIds()
            => UserAccountCombineDal.GetCount_UserIdBindMultiUnionIds();
        #endregion
    }
}
