using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Requests.Activity;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class TuboAllianceService : ITuboAllianceService
    {
        /// <summary>
        /// 佣金商品列表查询接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<CommissionProductModel>>> GetCommissionProductListAsync(GetCommissionProductListRequest request)
        {
            return OperationResult.FromResult(await TuboAllianceManager.GetCommissionProductListManager(request));
        }


        /// <summary>
        /// 佣金商品详情查询接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<CommissionProductModel>> GetCommissionProductDetatilsAsync(GetCommissionProductDetatilsRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PID) || request.CpsId == null)
            {
                return OperationResult.FromError<CommissionProductModel>
                    (nameof(Resource.ParameterError), Resource.ParameterError);
            }
            else
            {
                return OperationResult.FromResult(await TuboAllianceManager.GetCommissionProductDetatilsManager(request));
            }
        }

        /// <summary>
        /// 佣金订单商品记录创建接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<CreateOrderItemRecordResponse>> CreateOrderItemRecordAsync(CreateOrderItemRecordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId) ||
                request.OrderItem == null || request.OrderItem?.Count <= 0)
            {
                return OperationResult.FromError<CreateOrderItemRecordResponse>
                    (nameof(Resource.ParameterError), Resource.ParameterError);
            }
            else
            {
                return OperationResult.FromResult(await TuboAllianceManager.CreateOrderItemRecordManager(request));
            }
        }


        /// <summary>
        /// 佣金订单商品记录修改接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Obsolete("此方法未实现已弃用;", true)]
        public async Task<OperationResult<UpdateOrderItemRecordResponse>> UpdateOrderItemRecordAsync(UpdateOrderItemRecordRequest request)
        {
            await Task.Yield();
            return null;
        }

        /// <summary>
        /// 订单商品返佣接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<CommodityRebateResponse>> CommodityRebateAsync(CommodityRebateRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId))
            {
                return OperationResult.FromError<CommodityRebateResponse>
                    (nameof(Resource.ParameterError), Resource.ParameterError);
            }
            else
            {
                return OperationResult.FromResult(await TuboAllianceManager.CommodityRebateManager(request));
            }
        }

        /// <summary>
        /// 订单商品扣佣接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<CommodityDeductionResponse>> CommodityDeductionAsync(CommodityDeductionRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId))
            {
                return OperationResult.FromError<CommodityDeductionResponse>
                    (nameof(Resource.ParameterError), Resource.ParameterError);
            }
            else
            {
                return OperationResult.FromResult(await TuboAllianceManager.CommodityDeductionManager(request));
            }
        }

        /// <summary>
        /// CPS支付流水修改状态接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<CpsUpdateRunningResponse>> CpsUpdateRunningAsync(CpsUpdateRunningRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OutBizNo) ||
                string.IsNullOrEmpty(request.Status))
            {
                return OperationResult.FromError<CpsUpdateRunningResponse>
                    (nameof(Resource.ParameterError), Resource.ParameterError);
            }
            else
            {
                return OperationResult.FromResult(await TuboAllianceManager.CpsUpdateRunningManager(request));
            }
        }

    }
}
