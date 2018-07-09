using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SE_Activity
    {
        /// <summary>
        /// ID
        /// </summary>		
        public Guid ID { get; set; }
        /// <summary>
        /// Title
        /// </summary>		
        public string Title { get; set; }
        /// <summary>
        /// CreateDT
        /// </summary>		
        public DateTime CreateDT { get; set; }
        /// <summary>
        /// UpdateDT
        /// </summary>		
        public DateTime UpdateDT { get; set; }
        /// <summary>
        /// BgImageUrl
        /// </summary>		
        public string BgImageUrl { get; set; }
        /// <summary>
        /// BgColor
        /// </summary>		
        public string BgColor { get; set; }


        public IEnumerable<SE_ActivityDeatil> Items { get; set; }


    }


    public class SE_ActivityDeatil
    {

        /// <summary>
        /// ID
        /// </summary>		
        public int ID { get; set; }
        /// <summary>
        /// FK_Activity
        /// </summary>		
        public Guid FK_Activity { get; set; }
        /// <summary>
        /// GROUP
        /// </summary>		
        public string GROUP { get; set; }
        /// <summary>
        /// PID
        /// </summary>		
        public string PID { get; set; }

        public string ProductName { get; set; }

        public decimal ActivityPrice { get; set; }

        /// <summary>
        /// ActivityFlashID
        /// </summary>		
        public string ActivityFlashID { get; set; }
        /// <summary>
        /// CouponID
        /// </summary>		
        public string CouponID { get; set; }
        /// <summary>
        /// SmallImage
        /// </summary>		
        public string SmallImage { get; set; }
        /// <summary>
        /// BigImage
        /// </summary>		
        public string BigImage { get; set; }
        /// <summary>
        /// ColunmNumber
        /// </summary>		
        public int ColunmNumber { get; set; }
        /// <summary>
        /// Type
        /// </summary>		
        public int Type { get; set; }
        /// <summary>
        /// OrderBy
        /// </summary>		
        public int OrderBy { get; set; }
        /// <summary>
        /// AppUrl
        /// </summary>		
        public string AppUrl { get; set; }
        /// <summary>
        /// WapUrl
        /// </summary>		
        public string WapUrl { get; set; }
        /// <summary>
        /// PCUrl
        /// </summary>		
        public string PCUrl { get; set; }
        /// <summary>
        /// HandlerIOS
        /// </summary>		
        public string HandlerIOS { get; set; }
        /// <summary>
        /// HandlerAndroid
        /// </summary>		
        public string HandlerAndroid { get; set; }
        /// <summary>
        /// SOAPIOS
        /// </summary>		
        public string SOAPIOS { get; set; }
        /// <summary>
        /// SOAPAndroid
        /// </summary>		
        public string SOAPAndroid { get; set; }
        /// <summary>
        /// IsImage
        /// </summary>		
        public int IsImage { get; set; }
        /// <summary>
        /// DisplayWay
        /// </summary>		
        public int DisplayWay { get; set; }
        /// <summary>
        /// Description
        /// </summary>		
        public string Description { get; set; }

    }



}
