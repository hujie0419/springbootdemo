using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary> 
    /// tbl_OperationCategory:实体类 
    /// </summary>  
    [Serializable]
    public class OperationCategoryProductsModel
    {
        #region tbl_OperationCategory_Products

        public int Id { get; set; }

        public int OId { get; set; }

        public int CorrelId { get; set; }

        public string CategoryCode { get; set; }

        public string DefinitionCode { get; set; }

        public int Sort { get; set; }

        public int IsShow { get; set; }

        public int Type { get; set; }

        public DateTime CreateTime { get; set; }

        #endregion
    }
}
