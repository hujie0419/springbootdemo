using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.SalePromotionActivity
{

    /// <summary>
    /// 常量
    /// </summary>
    public static class SalePromotionActivityConst
    {

        //日志 操作类型OperationLogType
        public const string Insert_Activity = "SalePromotion_InsertActivity";
        public const string Update_Activity = "SalePromotion_UpdateActivity";
        public const string PassAudit_Activity = "SalePromotion_PassAuditActivity"; //审核活动
        public const string Reject_Audit = "SalePromotion_RejectAuditActivity"; //拒绝审核活动
        //public const string WaitAudit = "SalePromotion_WaitAudit"; //提交审核活动
        public const string UnShelveActivity = "SalePromotion_UnShelveActivity"; //下架活动
        public const string InsertProduct = "SalePromotion_InsertProduct"; //新增活动商品
        public const string RemoveProduct = "SalePromotion_RemoveProduct"; //撤除商品 
        public const string SetProductStock = "SalePromotion_SetProductStock"; //修改商品库存 
        public const string SetProductImage = "SalePromotion_SetProductImage"; //修改商品图片地址 
        public const string RefreshProductInfo = "SalePromotion_FreshProductInfo"; //同步商品信息  
        public const string ChangeProduct = "SalePromotion_ChangeProduct"; //修改商品信息  
        public const string InsertAiditAuth = "SalePromotion_InsertAiditAuth"; //新增用户审核权限  
        public const string DeleteAiditAuth = "SalePromotion_DeleteAiditAuth"; //删除用户审核权限    


        //日志来源类型
        public const string SalePromotionActivity = "SalePromotionActivity";
    }

}
