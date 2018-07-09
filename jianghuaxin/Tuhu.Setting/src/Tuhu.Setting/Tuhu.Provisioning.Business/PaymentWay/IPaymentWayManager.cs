using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.PaymentWay
{
    public interface IPaymentWayManager
    {
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<PaymentWayModel> GetAllPaymentWay();

        /// <summary>
        /// 修改/添加
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <returns></returns>
        bool UpdatePaymentWay(List<PaymentWayModel> pwModelList);
    }
}
