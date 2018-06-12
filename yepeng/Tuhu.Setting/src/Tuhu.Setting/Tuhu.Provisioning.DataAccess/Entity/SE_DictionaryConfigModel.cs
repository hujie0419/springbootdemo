using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 实体-SE_DictionaryConfigModel 
    /// </summary>
    public class SE_DictionaryConfigModel
    {
        /// <summary>
        /// 主键标识
        /// </summary>		
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int? ParentId { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Describe { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public int? Sort { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>		
        public int? State { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>		
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Images { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Extend1 { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Extend2 { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Extend3 { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Extend4 { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        public string Extend5 { get; set; }

    }
}