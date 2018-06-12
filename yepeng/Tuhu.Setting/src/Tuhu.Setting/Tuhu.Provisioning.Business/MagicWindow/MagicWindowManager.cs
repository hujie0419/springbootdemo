using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.MagicWindow
{
    public class MagicWindowManager
    {

        /// <summary>
        /// 获取魔窗列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MagicWindowModel> SelectMagicWindowList(int pageIndex = 1, int pageSize = 10)
        {
            return DALMagicWindow.fetchMagicWindow(pageIndex, pageSize);
        }

        public static int InsertMagicWindow(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return 0;
            return DALMagicWindow.InsertMagicWindow(url);
        }

        public static int UpdateMagicWindow(string url, int pkid)
        {
            if (string.IsNullOrWhiteSpace(url) || pkid <= 0) return 0;
            return DALMagicWindow.UpdateMagicWindow(url, pkid);
        }
        public static int DeleteMagicWindow(int pkid)
        {
            if (pkid <= 0) return 0;
            return DALMagicWindow.DeleteMagicWindow(pkid);
        }
    }
}
