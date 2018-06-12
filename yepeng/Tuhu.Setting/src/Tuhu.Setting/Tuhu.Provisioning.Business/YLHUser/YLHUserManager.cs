using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess;
using System.Data;

namespace Tuhu.Provisioning.Business.YLHUser
{
    public class YLHUserManager
    {
        public static string GetUserId(string mobile)
        {
            DalYLHUser dal = new DalYLHUser();
            return dal.GetUserId(mobile);
        }

        public static int GetYLHUserInfoPKID(string userid)
        {
            DalYLHUser dal = new DalYLHUser();
            return dal.GetYLHUserInfoPKID(userid);
        }

        public static int InsertUserToObjectTable(tbl_UserObjectModel user)
        {
            DalYLHUser dal = new DalYLHUser();
            return dal.InsertUserToObjectTable(user);
        }

        public static int InsertUserObject(tbl_UserObjectModel user)
        {
            DalYLHUser dal = new DalYLHUser();
            return dal.InsertUserObject(user);
        }

        public static int InsertYLHUserInfo(YLHUserInfoModel info)
        {
            DalYLHUser dal = new DalYLHUser();
            return dal.InsertYLHUserInfo(info);
        }

        public static int InsertYLHVipCardInfo(YLHUserVipCardInfoModel info)
        {
            DalYLHUser dal = new DalYLHUser();
            return dal.InsertYLHVipCardInfo(info);
        }

        public static int CreatePromotionCode(PromotionCode model)
        {
            DalYLHUser dal = new DalYLHUser();
            return dal.CreatePromotionCode(model);
        }

        public static void ExportDataTableToHtml(DataTable dt, string path)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");
            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' bgcolor='lightyellow' style='font-family:Garamond; font-size:smaller'>");

            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<td >");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</td>");

            }
            strHTMLBuilder.Append("</tr>");


            foreach (DataRow myRow in dt.Rows)
            {

                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags. 
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();
            System.IO.File.WriteAllText(path, Htmltext);
        }
    }
}
