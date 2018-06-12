using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    public enum CoverMode
    {
        NoPicMode = 0,
        OnePicSmallMode = 1,
        OnePicBigMode = 2,
        ThreePicMode = 3,
        TopBigPicMode = 4,
        BigPicLeftMode = 5,
    }

    public enum CustomCategoryMode
    {
        [Description("无自定义标签")]
        NoCustom = 0,
        [Description("置顶")]
        TopMost = 1
    }
    public static class EnumDescription
    {
        /// <summary>
        /// 获取枚举Description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDesString(this CustomCategoryMode value)
        {
            Type enumType = value.GetType();
            // 获取枚举常数名称。
            string name = Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                {
                    // 获取描述的属性。
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo,
                        typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
