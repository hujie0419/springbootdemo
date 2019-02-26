using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
namespace Tuhu.Provisioning.Common
{
    public class RandomHelper
    {
        private RandomHelper()
        {

        }
        private static RandomHelper _Instanse;
        public static RandomHelper Instanse
        {
            get
            {
                if (_Instanse == null)
                {
                    _Instanse = new RandomHelper();
                }
                return _Instanse;
            }
        }

        const string RandomString = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMEOPQRSTUVWXYZ";

        public string GetStringFromRandomString(int length = 8)
        {
            int count = RandomString.Length;
            StringBuilder sb = new StringBuilder();
            var random = new Random();
            while (length > 0)
            {
                int index = random.Next(1, count);
                sb.Append(RandomString[index - 1]);
                length--;
            }
            return sb.ToString();
        }
    }
}