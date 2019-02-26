using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Service.Activity;
using Tuhu.Service.PinTuan;

namespace Tuhu.Provisioning.Business.MGMT
{
    public class MGMToolsManager
    {
        public static string ChangeGroupBuyingStatus(int orderId)
        {
            var cmt = DalMGMTools.CheckExistPinTuanOrder(orderId);
            if (cmt)
            {
                var dat = DalMGMTools.FetchUserInfoByOrderId(orderId);
                using (var client = new PinTuanClient())
                {
                    if (dat.UserStatus == 0)
                    {
                        if (dat.Code == 1)
                        {
                            var result = client.ChangeGroupBuyingStatus(dat.GroupId, dat.OrderId);
                            if (!result.Success || !result.Result)
                            {
                                return ($"订单{dat.OrderId}付款,且用户为团{dat.GroupId}团长,对应团状态修改出现异常，{result.Exception?.Message}");
                            }
                        }
                        var val = client.ChangeUserStatus(dat.GroupId, dat.UserId, dat.OrderId);
                        if (!val.Success || !val.Result)
                        {
                            return ($"订单{dat.OrderId}付款,且用户为团{dat.GroupId}成员,对应用户状态修改出现异常，{val.Exception?.Message}");
                        }
                        return "设置成功";
                    }
                }
                return "用户状态不为‘待付款’";
            }
            return "未找到该拼团订单";
        }
    }
}