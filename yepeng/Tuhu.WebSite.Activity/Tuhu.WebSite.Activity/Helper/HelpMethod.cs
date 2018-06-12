using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace Tuhu.WebSite.Web.Activity.Helper
{
    public static class HelpMethod
    {
        //Tuhu.WebSite.Component.SystemFramework.DomainConfig.ResourceSite
        public static string TempSite { get { return WebConfigurationManager.AppSettings["TempSite"]; } }
        public static bool IsPhone(this string phone)
        {
           // Regex phoneRegex = new Regex(@"^1[3|4|5|8][0-9]\d{8}$");
            Regex phoneRegex = new Regex(@"^1\d{10}$");
            if (!phoneRegex.IsMatch(phone))
            {
                return false;
            }
            return true;
        }
        public static bool IsEmail(this string email)
        {
            Regex emailRegex = new Regex(@"^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$");
            if (!emailRegex.IsMatch(email))
            {
                return false;
            }
            return true;
        }
        public static string GetMD5(string myString)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(myString));

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
   
}
