using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.WebSite.Component.SystemFramework.Models;
using Tuhu.WebSite.Component.SystemFramework.Extensions;
using System.Data;
namespace Tuhu.WebSite.Web.Activity.Models
{
    public class UserObjectModel : BaseModel
    {
        public UserObjectModel() : base() { UserType = UserTypes.RegisteredUser; }
        public UserObjectModel(DataRow row) : base(row) { }

        /// <summary>用户编号</summary>
        public string UserID { get; set; }

        /// <summary>用户类型</summary>
        public UserTypes UserType { get; set; }

        /// <summary>姓名</summary>
        public string UserName { get; set; }

        /// <summary>邮箱地址</summary>
        public string EmailAddress { get; set; }

        /// <summary> 密码，只写</summary>
        public string Password { internal get; set; }

        /// <summary>电话</summary>
        public string TelNumber { get; set; }
        public string TelExtension { get; set; }

        /// <summary>电话</summary>
        public string FaxNumber { get; set; }
        public string FaxExtension { get; set; }

        /// <summary>手机</summary>
        public string MobileNumber { get; set; }

        /// <summary>性别</summary>
        public int? Gender { get; set; }

        /// <summary>默认收货地址</summary>
        public string PreferredAddress { get; set; }

        /// <summary>从哪里知道我们</summary>
        public string Pref3 { get; set; }

        /// <summary>您的地区</summary>
        public string Pref4 { get; set; }

        /// <summary>昵称账户</summary>
        public string Pref5 { get; set; }

        public string CrossFrom { get; set; }

        [Obsolete]
        public DateTime CreateDateTime { get { return RegisteredDateTime; } }
        public DateTime RegisteredDateTime { get; set; }

        /// <summary>所有收货地址 </summary>
        public IEnumerable<string> Addresses { get; set; }

        public string HeadImage { set; get; }

        public string Nickname
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Pref5))
                    return Pref5;
                else if (!string.IsNullOrWhiteSpace(UserName))
                    return UserName;
                else
                    return MobileNumber;
            }
        }

        public string WeiXinID { get; set; }
        public string QQOpenID { get; set; }
        protected override void Parse(DataRow row)
        {
            this.UserID = row.GetValue("UserID");
            if (row.HasValue("UserType"))
                this.UserType = (UserTypes)Convert.ToInt32(row["UserType"]);
            this.UserName = row.GetValue("UserName");
            this.HeadImage = row.GetValue("HeadImage");
            this.EmailAddress = row.GetValue("EmailAddress");
            this.TelNumber = row.GetValue("TelNumber");
            this.TelExtension = row.GetValue("TelExtension");
            this.FaxNumber = row.GetValue("FaxNumber");
            this.FaxExtension = row.GetValue("FaxExtension");
            this.MobileNumber = row.GetValue("MobileNumber");
            this.CrossFrom = row.GetValue("CrossFrom");
            if (row.HasValue("Gender"))
                this.Gender = Convert.ToInt32(row["Gender"]);
            this.PreferredAddress = row.GetValue("PreferredAddress");
            this.Pref5 = row.GetValue("Pref5");

            this.RegisteredDateTime = Convert.ToDateTime(row.HasValue("RegisteredDateTime") ? row["RegisteredDateTime"] : row["CreateDateTime"]);

            this.Addresses = row.HasValue("Addresses") ? row.GetValue("Addresses").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries) : new string[0];

            this.WeiXinID = row.GetValue("WeiXinID");
        }
    }

    public enum UserTypes
    {
        GuestUser = 0,
        RegisteredUser = 1,
        EditorUser = 2,
        AdministratorUser = 3,
    }
}