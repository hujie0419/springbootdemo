using System;
using System.Linq;

namespace Tuhu.Service.Activity.Server.Utils
{
    public static class EnumExtension
    {
        public class RemarkAttribute : Attribute
        {
            public RemarkAttribute(string remark)
            {
                Remark = remark;
            }
            /// <summary>
            /// 备注
            /// </summary>
            public string Remark { get; set; }   
        }
        /// <summary>
        /// 获取当前枚举值的注释
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetRemark(this System.Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attrs = fieldInfo.GetCustomAttributes(typeof(RemarkAttribute), false);
            var attr = (RemarkAttribute)attrs.FirstOrDefault(a => a is RemarkAttribute);
            var remark = attr == null ? fieldInfo.Name : attr.Remark;
            return remark;
        }
    }
}

