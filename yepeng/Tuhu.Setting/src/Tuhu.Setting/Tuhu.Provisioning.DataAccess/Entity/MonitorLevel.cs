using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public enum MonitorLevel
    {
        [EnumDescription("正常")]
        Info = 1,
        [EnumDescription("一般错误")]
        Error = 2,
        [EnumDescription("严重错误")]
        Critial = 3
    }

    public class EnumDescriptionAttribute : Attribute
    {
        private readonly string _value = "";
        public string Text
        {
            get { return this._value; }
        }
        public EnumDescriptionAttribute(string text)
        {
            _value = text;
        }

    }

    public class EnumStringHelper
    {
        public static string GetEnumDescription(object e)
        {
            var t = e.GetType();
            string description = e.ToString();
            var os = (EnumDescriptionAttribute[])t.GetField(description).GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if (os.Length == 1)
            {
                return os[0].Text;
            }
            return description;
        }
    }
}
