using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ProductDescriptionModel : BaseModel
    {
        public ProductDescriptionModel() { }
        public ProductDescriptionModel(DataRow row) : base(row) { }
        public string PID { get; set; }
        public int AddOrDel { get; set; }
        public string ModuleName { get; set; }
        public int ModuleID { get; set; }
        public int ModuleOrder { get; set; }
        public string Brand { get; set; }
        public string Categories { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string CategoryName { get; set; }
        public bool IsAdvert { get; set; }
        public string ModuleContent { get; set; }
        public int ShowPlatform { get; set; }
        public string BigImageUrl { get; set; }
        public string SmallImageUrl { get; set; }
        public string URL { get; set; }
        public string AppHandleKey { get; set; }
        public string AppSpecialKey { get; set; }

    }
}
