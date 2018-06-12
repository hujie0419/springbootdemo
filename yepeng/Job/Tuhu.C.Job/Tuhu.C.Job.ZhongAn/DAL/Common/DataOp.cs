using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace K.DLL.Common
{
    public class DataOp
    {
        public static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        public static DataSet GetDataSet(string strSQL, params SqlParameter[] parametes)
        {
            DataHelper ksql = new DataHelper(connString);
            return ksql.GetDataSet(strSQL, parametes);
        }
        public static int SqlCom(string strSQL, params SqlParameter[] parametes)
        {
            DataHelper ksql = new DataHelper(connString);
            return ksql.ExecuteNonQuery(strSQL, parametes);
        }
        public static string GetPara(string strSQL, params SqlParameter[] parametes)
        {
            DataHelper ksql = new DataHelper(connString);
            object kobj = ksql.ExecuteScalar(strSQL, parametes);
            if (kobj == null)
            {
                return "";
            }
            else
            {
                return kobj.ToString();
            }
        }

    }

    public class WMSDataOp
    {
        public static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["WMS"].ConnectionString;
        public static DataSet GetDataSet(string strSQL, params SqlParameter[] parametes)
        {
            DataHelper ksql = new DataHelper(connString);
            return ksql.GetDataSet(strSQL, parametes);
        }
        public static int SqlCom(string strSQL, params SqlParameter[] parametes)
        {
            DataHelper ksql = new DataHelper(connString);
            return ksql.ExecuteNonQuery(strSQL, parametes);
        }
        public static string GetPara(string strSQL, params SqlParameter[] parametes)
        {
            DataHelper ksql = new DataHelper(connString);
            object kobj = ksql.ExecuteScalar(strSQL, parametes);
            if (kobj == null)
            {
                return "";
            }
            else
            {
                return kobj.ToString();
            }
        }
    }

    public class TuhuLogDataOp
    {
        public static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["OprLogConnection"].ConnectionString;
        public static string GetPara(string strSQL, params SqlParameter[] parametes)
        {
            DataHelper ksql = new DataHelper(connString);
            object kobj = ksql.ExecuteScalar(strSQL, parametes);
            if (kobj == null)
            {
                return "";
            }
            else
            {
                return kobj.ToString();
            }
        }
    }
}
