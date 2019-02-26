using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ProductInfo
    {
        /// <summary>
        /// 产品展示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 产品唯一标识
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 产品所属品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 产品关联车型配置，如果为空则没有配置车型
        /// </summary>
        public string CP_Remark { get; set; }

        /// <summary>
        /// 实际适配的车型级数
        /// </summary>
        public string VehicleLevel { get; set; }
        /// <summary>
        /// 产品配置车型时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// 产品价格
        /// </summary>
        public string ListPrice { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public string MarketingPrice { get; set; }
        /// <summary>
        /// 广告语
        /// </summary>
        public string CP_ShuXing5 { get; set; }
        /// <summary>
        /// 相关excel附件
        /// </summary>
        public List<ProductVehicleTypeFileInfoDb> ExcelInfoList { get; set; }
        /// <summary>
        /// 是否自动配置
        /// </summary>
        public bool IsAutoAssociate { get; set; }
        /// <summary>
        /// 是否上架  1 是  0 否
        /// </summary>
        public int OnSale { get; set; }
        /// <summary>
        /// 是否缺货 1 是  0 否
        /// </summary>
        public int Stockout { get; set; }

        public int Total { get; set; }
    }

    public class ProductVehicleTypeFileInfoDb
    {
        public string PID { get; set; }

        public string Operator { get; set; }

        public string FilePath { get; set; }

        public DateTime CreatedTime { get; set; }
    }


    [Serializable]
    public class VehicleTypeInfoDb
    {
        /// <summary>
        /// 车型唯一标识
        /// </summary>
        public string TID { get; set; }
        /// <summary>
        /// 车型ID
        /// </summary>
        public string VehicleID { get; set; }
        /// <summary>
        /// 车型类型
        /// </summary>
        public string VehicleType { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 车型系列
        /// </summary>
        public string VehicleSeries { get; set; }
        /// <summary>
        /// 子系列
        /// </summary>
        public string Vehicle { get; set; }
        /// <summary>
        /// 排量
        /// </summary>
        public string PaiLiang { get; set; }
        /// <summary>
        /// 生产年份
        /// </summary>
        public string ListedYear { get; set; }
        /// <summary>
        /// 停止生产年份
        /// </summary>
        public string StopProductionYear { get; set; }
        /// <summary>
        /// 年款
        /// </summary>
        public string Nian { get; set; }
        /// <summary>
        /// 完整卖点宣传
        /// </summary>
        public string SalesName { get; set; }
        /// <summary>
        /// 均价
        /// </summary>
        public double AvgPrice { get; set; }
        /// <summary>
        /// 是否国产
        /// </summary>
        public string JointVenture { get; set; }
        /// <summary>
        /// 车系（德美日中）
        /// </summary>
        public string BrandCategory { get; set; }

        public string GetFieldValues(string[] fields)
        {
            var strValues = new List<string>();
            foreach (var field in fields)
            {
                switch (field)
                {
                    case "TID":
                        strValues.Add(this.TID); break;
                    case "VehicleID":
                        strValues.Add(this.VehicleID); break;
                    case "VehicleType":
                        strValues.Add(this.VehicleType); break;
                    case "Brand":
                        strValues.Add(this.Brand); break;
                    case "VehicleSeries":
                        strValues.Add(this.VehicleSeries); break;
                    case "Vehicle":
                        strValues.Add(this.Vehicle); break;
                    case "PaiLiang":
                        strValues.Add(this.PaiLiang); break;
                    case "ListedYear":
                        strValues.Add(this.ListedYear); break;
                    case "Nian":
                        strValues.Add(this.Nian); break;
                    case "SalesName":
                        strValues.Add(this.SalesName); break;//不用补一个年款
                    case "AvgPrice":
                        strValues.Add(this.AvgPrice.ToString()); break;
                    case "JointVenture":
                        strValues.Add(this.JointVenture); break;
                    case "BrandCategory":
                        strValues.Add(this.BrandCategory); break;
                }
            }
            return string.Join("$", strValues);
        }


    }

    public class VehicleBrandCategoryAndType
    {
        public List<string> BrandCategoryList { get; set; }

        public List<string> VehicleTypeList { get; set; }
    }

    public class ProductVehicleTypeConfigDb
    {
        //public int Id { get; set; }
        public long PKID { get; set; }
        public string PID { get; set; }

        public string TID { get; set; }

        public string VehicleID { get; set; }

        public string Nian { get; set; }

        public string PaiLiang { get; set; }

        public string SalesName { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public int ConfigLevel { get; set; }
    }

    public class ProductVehicleTypeConfigOpLog
    {
        public long? Id { get; set; }

        public string PID { get; set; }

        public string Operator { get; set; }

        public string OperateContent { get; set; }

        public DateTime OperateTime { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    public class VehicleInfoExDb
    {

        public string PID { get; set; }

        public string TID { get; set; }

        public string VehicleID { get; set; }

        public string Nian { get; set; }

        public string PaiLiang { get; set; }

        public string SalesName { get; set; }

        public int ConfigLevel { get; set; }

        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public string ListedYear { get; set; }

        public string StopProductionYear { get; set; }
    }
}
