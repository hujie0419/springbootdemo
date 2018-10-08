using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Tuhu.Provisioning.Models.SalePromotionActivity
{
    public class EnumTool
    {

        private static string GetName(System.Type t, object v)
        {
            try
            {
                return Enum.GetName(t, v);
            }
            catch (Exception e)
            {
                return "未知";//UNKNOWN
            }
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="t"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string GetDescription(System.Type t, object v)
        {
            try
            {
                if (!Enum.IsDefined(t, v))
                    return "未知";

                FieldInfo fi = t.GetField(GetName(t, v));
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : GetName(t, v);
            }
            catch
            {
                return "未知";
            }
        }
    }


   
}