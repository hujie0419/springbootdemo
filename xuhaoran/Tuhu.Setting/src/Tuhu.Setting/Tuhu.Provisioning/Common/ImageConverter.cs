using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Tuhu.Provisioning.Common
{
    public static class ImageConverter
    {
        /// <summary>
        /// 字节流转换成图片
        /// </summary>
        /// <param name="byt">要转换的字节流</param>
        /// <returns>转换得到的Image对象</returns>
        public static Dictionary<string, object> BytToImg(byte[] byt)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(byt))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        var dicToImg = new Dictionary<string, object>
                        {
                            {"Width", img.Width},
                            {"Height", img.Height}
                        };
                        return dicToImg;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

    }
}