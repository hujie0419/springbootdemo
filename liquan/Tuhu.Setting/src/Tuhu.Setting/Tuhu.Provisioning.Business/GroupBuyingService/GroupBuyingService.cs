using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.GroupBuying;
using Tuhu.Service.GroupBuying.Models.RedemptionCode;

namespace Tuhu.Provisioning.Business.GroupBuyingService
{
    public class GroupBuyingService
    {
        /// <summary>
        /// 作废兑换码
        /// </summary>
        /// <param name="redeemCodes"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public async Task<InvalidateRedemptionCodeResult> InvalidateRedemptionCode(List<string> redeemCodes, string remark)
        {
            using (var client = new RedemptionCodeClient())
            {
                var result = await client.InvalidateRedemptionCodeAsync(new InvalidateRedemptionCodeRequest
                {
                    Codes = redeemCodes,
                    Remark = remark,
                });
                result.ThrowIfException(true);
                return result.Result;
            }
        }

        public async Task<List<RedemptionCodeModel>> GetBatchRedemptionCodes(string batchCode)
        {
            using (var client = new RedemptionCodeClient())
            {
                var result = await client.GetBatchRedemptionCodesAsync(batchCode);
                result.ThrowIfException(true);
                return result.Result;
            }
        }
    }
}
