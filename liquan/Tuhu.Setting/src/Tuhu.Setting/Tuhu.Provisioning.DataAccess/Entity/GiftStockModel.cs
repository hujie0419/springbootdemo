
namespace Tuhu.Provisioning.DataAccess.Entity
{
   public class GiftStockModel2
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Stock { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? IsRetrieve { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RuleId { get; set; }

        /// <summary>
        /// 是否买三送一
        /// </summary>
        public bool IsGiveAway { get; set; }
    }
}
