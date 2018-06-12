using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Tuhu.C.Job.PushProduct.Model;

namespace Tuhu.C.Job.PushProduct.DAL
{
    public class BaseDataDAL
    {
        public static IEnumerable<string> GetPIDList()
        {
            string sqltext = @"WITH TOPFive AS (
    SELECT PS.SalesQuantity, PC.displayName AS EngLishDisplayName, PC.categoryName,VP.cp_brand AS Brand, ROW_NUMBER()
    over(
        PARTITION BY PC.categoryName, VP.cp_brand
        order by PS.SalesQuantity DESC
    ) AS RowNo, VP.*
     FROM Tuhu_productcatalog..vw_Products AS VP

        INNER JOIN Tuhu_productcatalog.[dbo].[vw_ProductCategories] PC        

        ON VP.category = PC.CategoryName AND((PC.ParentOid IN(1, 15419))


         OR(LEN(PC.NodeNo) - LEN(REPLACE(PC.NodeNo, '.', '')) = 2 AND PC.NodeNo LIKE '28349.%')

         OR(LEN(PC.NodeNo) - LEN(REPLACE(PC.NodeNo, '.', '')) = 2 AND PC.NodeNo LIKE '28656.%'))
		 AND VP.onSale = 1 AND VP.cp_brand IS NOT NULL

        INNER JOIN Tuhu_productcatalog..tbl_ProductStatistics PS
        ON VP.pid = ISNULL(PS.ProductID, '') + '|' + ISNULL(PS.variantID, '')
       
)
SELECT PID FROM TOPFive WHERE RowNo <= 5";


            using (
               var dbhelper =
                   DbHelper.CreateDbHelper())
            {
                using (var cmd = new SqlCommand(sqltext) { CommandTimeout = 10 * 60 })
                {
                    return dbhelper.ExecuteQuery(cmd, dt => dt.ToList<string>("PID"));
                }
                //return dbhelper.ExecuteSelect<string>(sqltext);
            }
        }

        public static IEnumerable<ProductCategory> GetProductCategoryByOid(string oids)
        {
            //string str= "SELECT oid,parentOid,CategoryName,DisplayName,DescendantProductCount FROM Tuhu_productcatalog.[dbo].[vw_ProductCategories] WHERE oid in ()"

            using (
             var dbhelper =
                 DbHelper.CreateDbHelper())
            {
                using (var cmd = new SqlCommand("SELECT oid,parentOid,CategoryName,DisplayName,DescendantProductCount FROM Tuhu_productcatalog.[dbo].[vw_ProductCategories] WHERE oid in ("+ oids+")") { CommandTimeout = 10 * 60 })
                {
                    cmd.CommandType = CommandType.Text;
                   
                    return dbhelper.ExecuteSelect<ProductCategory>(cmd);
                }                
            }
        }

    }
}
