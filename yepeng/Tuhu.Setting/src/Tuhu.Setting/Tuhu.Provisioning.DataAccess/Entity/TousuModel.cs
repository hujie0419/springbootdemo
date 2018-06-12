using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public  class TousuModel
    {
        public string OrderType { get; set; }
        public string DicType { get; set; }

        public string DicValue { get; set; }

        public string DicText { get; set; }

        public string AppDisplayName { get; set; }

        public bool HasThirdLevel { get; set; }
        public bool HasFourthLevel { get; set; }
        
        public bool HasFifthLevel { get; set; }
        
        public bool IsChecked { get; set; }

        public bool IsNeedPhoto { get; set; }

        public string CautionText { get; set; }

        public string GroupName { get; set; }

        public int TypeLevel { get; set; }
    }

    public class OrderTypeTousuConfig
    {
        public int PKID { get; set; }

        public string OrderType { get; set; }

        public string TopLevelTousu { get; set; }

        public string SecondLevelTousu { get; set; }

        public string ThirdLevelTousu { get; set; }

        public string FourthLevelTousu { get; set; }

        public string LastLevelTousu { get; set; }

        public bool IsNeedPhoto { get; set; }

        public bool IsChecked { get; set; }

        public string CautionText { get; set; }

        public string GroupName { get; set; }

    }
}
