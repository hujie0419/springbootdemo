using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
   public  class DalFaqManage
    {
        public static IEnumerable<SimpleProductModel> SelectSimpleProductModels(List<CBrandModel> models,string pids, Pagination pagination = null)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                string sqlall = null;
                if (models != null && models.Any())
                {
                    var sql = string.Join(" union all ", models.Select(p =>
                    {
                        if ("15419"==p.CategoryNodeNO|| "28349"==p.CategoryNodeNO
                        || "4309"==p.CategoryNodeNO|| "28656"==p.CategoryNodeNO
                        || "193649"==p.CategoryNodeNO|| "367713"==p.CategoryNodeNO||"1"==p.CategoryNodeNO)
                        {
                            var sqlrr = $@"
                SELECT  PID ,
                        P.DisplayName AS ProductName
                FROM    Tuhu_productcatalog..vw_Products AS P WITH ( NOLOCK )
                        JOIN Tuhu_productcatalog..[CarPAR_CatalogHierarchy] (NOLOCK)
                        AS cc ON P.oid = cc.child_oid
                WHERE   P.i_ClassType IN (2,4) AND P.CP_Brand IN (N'{string.Join("',N'", p.Brands.Split(','))}') AND  cc.nodeno like N'{p
            .CategoryNodeNO}.%'";
                            return sqlrr;
                        }

                        else
                        {
                            var sqlrr = $@"
                SELECT  PID ,
                        P.DisplayName AS ProductName
                FROM    Tuhu_productcatalog..vw_Products AS P WITH ( NOLOCK )
                        JOIN Tuhu_productcatalog..[CarPAR_CatalogHierarchy] (NOLOCK)
                        AS cc ON P.oid = cc.child_oid
                WHERE   P.i_ClassType IN (2,4) AND P.CP_Brand IN (N'{string.Join("',N'", p.Brands.Split(','))}') AND  cc.nodeno like N'%.{p
            .CategoryNodeNO}.%'";
                            return sqlrr;
                        }
                    }));

                    if (!string.IsNullOrEmpty(pids))
                    {
                        var sqlPid = string.Concat(sql, " union all ", $@" SELECT  PID ,
                        P.DisplayName AS ProductName
                FROM    Tuhu_productcatalog..vw_Products AS P WITH(NOLOCK)
                        JOIN Tuhu_productcatalog..[CarPAR_CatalogHierarchy](NOLOCK)
                        AS cc ON P.oid = cc.child_oid
                WHERE   P.i_ClassType IN(2, 4) AND P.Pid IN(N'{string.Join("', N'", pids.Split(','))}')");
                        sqlall = sqlPid;
                    }

                else
                {
                    sqlall = sql;
                }
                }
                else
                {
                    sqlall = $@" SELECT  PID ,
                        P.DisplayName AS ProductName
                FROM    Tuhu_productcatalog..vw_Products AS P WITH(NOLOCK)
                        JOIN Tuhu_productcatalog..[CarPAR_CatalogHierarchy](NOLOCK)
                        AS cc ON P.oid = cc.child_oid
                WHERE   P.i_ClassType IN(2, 4) AND P.Pid IN(N'{string.Join("', N'", pids.Split(','))}')";
                }
                if (pagination == null)
                {
                    return dbHelper.ExecuteDataTable(sqlall, CommandType.Text).ConvertTo<SimpleProductModel>();
                }
                else
                {
                    var sqlCnt = "select count(1) from (" + sqlall + ") as total";
                    var sqlPage = string.Concat("select * from (", sqlall,
                        ") as tatol order by pid OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY");
                    pagination.records = (int) dbHelper.ExecuteDataRow(sqlCnt)[0];
                    return dbHelper.ExecuteDataTable(sqlPage, CommandType.Text, new SqlParameter[]
                    {
                        new SqlParameter("@PageSize", pagination.rows),
                        new SqlParameter("@PageIndex", pagination.page)
                    }).ConvertTo<SimpleProductModel>();
                }

            }
        }

        public static IEnumerable<ProductFaqConfigModel> SelectProductFaqConfigModelsPagination(Pagination pagination)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql1 =
                    @"SELECT PFC.QuestionDetail,PFC.LastUpdateDateTime,P.Pid,P.DisplayName AS ProductName FROM Configuration..ProductFaqConfig  AS PFC WITH ( NOLOCK)
                join Configuration..ProductFaqPidDetail AS PP on PFC.PKID =PP.FkFaqId
                Join Tuhu_productcatalog..vw_Products AS P WITH(NOLOCK) on PP.PID =P.Pid
                    ORDER BY p.PID
                 OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY";
                var sql2= @"SELECT Count(0) FROM Configuration..ProductFaqConfig  AS PFC WITH ( NOLOCK)
                join Configuration..ProductFaqPidDetail AS PP on PFC.PKID =PP.FkFaqId
                Join Tuhu_productcatalog..vw_Products AS P WITH(NOLOCK) on PP.PID =P.Pid";
                pagination.records = (int)dbHelper.ExecuteScalar(sql2);
                return dbHelper.ExecuteDataTable(sql1, CommandType.Text, new SqlParameter[]
                   {
                    new SqlParameter("@PageSize", pagination.rows),
                    new SqlParameter("@PageIndex", pagination.page)
                   }).ConvertTo<ProductFaqConfigModel>();
            }
        }
        public static IEnumerable<ProductFaqConfigModel> SelectProductFaqConfigModels()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var sql1 =
                    @"SELECT PFC.QuestionDetail,PFC.LastUpdateDateTime,P.Pid,P.DisplayName AS ProductName FROM Configuration..ProductFaqConfig  AS PFC WITH ( NOLOCK)
                join Configuration..ProductFaqPidDetail AS PP on PFC.PKID =PP.FkFaqId
                Join Tuhu_productcatalog..vw_Products AS P WITH(NOLOCK) on PP.PID =P.Pid";
                return dbHelper.ExecuteDataTable(sql1).ConvertTo<ProductFaqConfigModel>();
            }
        }
        public static int SelectPkidByPid(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return (int)dbHelper.ExecuteScalar(@"
                SELECT PKid FROM Configuration..ProductFaqPidDetail  AS PFPD WITH ( NOLOCK)
                Where PFPD.Pid=@Pid;", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@Pid", pid)
                        });
            }
        }
        public static IEnumerable<ProductFaqConfigDetailModel> SelectProductFaqConfigDetailModels(string pid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return dbHelper.ExecuteDataTable(@"
                	SELECT PD.Status,* FROM Configuration..ProductFaqConfig  AS PFC WITH ( NOLOCK) 
						JOIN Configuration..ProductFaqDetailConfig AS PD ON PFC.pkid=PD.FkFaqId
                        JOIN Configuration..ProductFaqPidDetail AS PP ON PFC.pkid=PP.FkFaqId
                        where Pid=@Pid and PD.FkFaqId=PP.FkFaqId;", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@Pid", pid)
                        }).ConvertTo<ProductFaqConfigDetailModel>();
            }
        }

        public static bool InsertProductFaqConfigDetailModels(ProductFaqConfigDetailModel model ,int fkId )
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"
                				INSERT INTO Configuration..ProductFaqDetailConfig(
								  FkFaqId ,
						          Question ,
						          Answer ,
						          Sort ,
						          Status 
						        )
						VALUES  ( @FkFaqId ,
						          @Question , 
						          @Answer , 
						          @Sort , 
						          @Status   
						        );", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@FkFaqId", fkId),
                            new SqlParameter("@Question", model.Question),
                            new SqlParameter("@Answer", model.Answer),
                            new SqlParameter("@Sort", model.Sort),
                            new SqlParameter("@Status", model.Status)
                        })>0;
            }
        }

        public static int DeleteProductFaqConfigDetailModels( int fkId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return dbHelper.ExecuteNonQuery(@"
                				Delete from  Configuration..ProductFaqDetailConfig where FkFaqId=@FkFaqId;", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@FkFaqId", fkId)
                        });
            }
        }

        public static int InsertProductFaqConfigModel(ProductFaqConfigModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return Convert.ToInt32(dbHelper.ExecuteScalar(@"
                				INSERT INTO Configuration..ProductFaqConfig(
								Status,
								QuestionDetail
								)
								VALUES(
                                1,
                                @QuestionDetail
								)SELECT @@IDENTITY;", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@QuestionDetail", model.QuestionDetail),
                        }));
            }
        }

        public static bool InsertProductFaqPidDetail(ProductFaqConfigModel model,  int fkId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"
                				INSERT INTO Configuration..ProductFaqPidDetail (
								Pid,
                                FkFaqId
								)
								VALUES(
                                @Pid,
                                @FkFaqId
								);", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@Pid", model.Pid),
                            new SqlParameter("@FkFaqId", fkId),
                        })>0;
            }
        }
        public static bool DeleteProductFaqPidDetail(ProductFaqConfigModel model)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                return dbHelper.ExecuteNonQuery(@"
                				Delete from  Configuration..ProductFaqPidDetail where Pid=@Pid;", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@Pid", model.Pid)
                        })>=0;
            }
        }

        public static bool DeleteProductFaqPidDetailForPatch(List<string> pids)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                var sql = $@"
                				Delete from  Configuration..ProductFaqPidDetail where Pid IN(N'{string.Join("', N'", pids)}');";
                return dbHelper.ExecuteNonQuery(sql, CommandType.Text,
                        new SqlParameter[]
                        {
                           // new SqlParameter("@Pid", string.Join("',N'",pids) )
                        }) >= 0;
            }
        }
        public static bool InsertProductFaqPidDetailPatch(List<string> pids, int fkId)
        {
            var dt = new DataTable("ProductFaqPidDetail");
            DataColumn dc0 = new DataColumn("Pkid", Type.GetType("System.Int32"));
            var dc1 = new DataColumn("FkFaqId", typeof(int));
            var dc2 = new DataColumn("Pid", typeof(string));

            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            foreach (var pid in pids)
            {
                var dr = dt.NewRow();
                dr["Pkid"] = DBNull.Value;
                dr["Pid"] = pid;
                dr["FkFaqId"] = fkId;
                dt.Rows.Add(dr);
            }
            using (var cmd = new SqlBulkCopy(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                cmd.BatchSize = pids.Count();
                cmd.BulkCopyTimeout = 30;
                cmd.DestinationTableName = "Configuration..ProductFaqPidDetail";
                cmd.WriteToServer(dt);
                return true;
            }
        }

        public static int SelectNotSameFaqCount(List<string> pids,int fkId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                return Convert.ToInt32(dbHelper.ExecuteScalar($@"
                	SELECT count(*) FROM Configuration..ProductFaqPidDetail AS PP 
                        where  Pid IN(N'{string.Join("', N'", pids)}') and PP.FkFaqId!=@fkId;", CommandType.Text,
                        new SqlParameter[]
                        {
                            new SqlParameter("@fkId", fkId)
                        }));
            }
        }
        /// <summary>
        /// 获取后台类目树集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ZTreeModel> SelectProductCategories()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                string sql =
                    @"SELECT  oid AS 'id' ,
        ParentOid AS 'pId' ,
        DisplayName AS 'name' ,
        0 AS 'open' ,
        0 AS 'chkDisabled' ,
        CategoryName ,
        NodeNo
FROM[Tuhu_productcatalog]..vw_ProductCategories WITH(NOLOCK )
WHERE(ParentOid = -1
          AND CategoryName IN('Tires', 'hub', 'AutoProduct', 'Gifts',
                                'BaoYang', 'MR1', 'QCBL')
        )
        OR(ParentOid != -1
             AND(NodeNo LIKE '1.%'
                   OR NodeNo LIKE '15419.%'
                   OR NodeNo LIKE '28349.%'
                   OR NodeNo LIKE '4309.%'
                   OR NodeNo LIKE '28656.%'
                   OR NodeNo LIKE '193649.%'
                   OR NodeNo LIKE '367713.%'
                 )
           ); ";
                return
                    dbHelper.ExecuteDataTable(sql,CommandType.Text).ConvertTo<ZTreeModel>().ToList();
            }
        }
    }
}
