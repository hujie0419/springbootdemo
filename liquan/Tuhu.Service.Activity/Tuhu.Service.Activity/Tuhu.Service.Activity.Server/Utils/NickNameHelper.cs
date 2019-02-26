using System.Linq;

namespace Tuhu.Service.Activity.Server.Utils
{
    /// <summary>
    ///     昵称帮助类
    /// </summary>
    public class NickNameHelper
    {
        /// <summary>
        ///     数据 脱敏
        /// </summary>
        public static string NickNameDataMasking(string nickName)
        {
            nickName = nickName?.Trim() ?? "";
            //加密 如果11位 那么隐藏中间 四位
            if (nickName.Length == 11)
            {
                nickName = nickName.Substring(0, 3) + "****" + nickName.Substring(7, 4);
            }

            //写死 临时需求 不想在排行榜显示这些词汇
            var keyNames = new[] {"京东", "淘宝", "测试", "途虎"};
            keyNames.ForEach(name => { nickName = nickName.Replace(name, "*"); });
            return nickName;
        }
    }
}
