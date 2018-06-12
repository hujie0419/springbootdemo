using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{

    public class HomePageModuleType
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }

    public class HomePageConfiguation
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string KeyValue { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
   public class WechatHomeList
    {
        public int ID { get; set; }

        public string TypeName { get; set; }        
        public string Title { get; set; }

        public int IsEnabled { get; set; }

        public string SVersion { get; set; }


        public string EVersion { get; set; }

        public DateTime CDateTime { get; set; }


        public DateTime UDateTime { get; set; }

        public int? OrderBy { get; set; }

        /// <summary>
        /// 是否新人
        /// </summary>
        public bool? IsNewUser { get; set; }

        public bool? IsShownButtom { get; set; }

        public int HomePageConfigID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Headings { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }

        public string ImageUrl { get; set; }

        public string Uri { get; set; }

      
    }


    public class WechatHomeContent
    {
        public int ID { get; set; }

        public string AppID { get; set; }

        public int FKID { get; set; }

        public string Title { get; set; }


        public string ImageUrl { get; set; }

        public string Uri { get; set; }


        public int IsEnabled { get; set; }

        public DateTime CDateTime { get; set; }

        public DateTime UDateTime { get; set; }

        public int OrderBy { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Headings { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// 跳转类型
        /// </summary>
        public bool? UriType { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string PathImage { get; set; }
    
        public string BuriedPointParam { get; set; }

        public string UriTypeText { get; set; }
    }

    public class WechatHomeAreaContent
    {
        public int ID { get; set; }

        public string AppID { get; set; }

        public int FKID { get; set; }

    
        public string CityList { get; set; }

        public string CityIDs { get; set; }

        public string ImageUrl { get; set; }

        public string Uri { get; set; }


        public int IsEnabled { get; set; }

        public DateTime CDateTime { get; set; }

        public DateTime UDateTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Headings { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }


        public string UriTypeText { get; set; }
    }


    public class WechatHomeProductContent
    {
        public int ID { get; set; }

        

        public int FKID { get; set; }

        public string PID { get; set; }

        public string GroupId { get; set; }

        public string ProductName { get; set; }


        
        public string ImageUrl { get; set; }


        public int OrderBy { get; set; }

        public int IsEnabled { get; set; }

        public DateTime CDateTime { get; set; }

        public DateTime UDateTime { get; set; }

        public string BuriedPointParam { get; set; }
    }
}
