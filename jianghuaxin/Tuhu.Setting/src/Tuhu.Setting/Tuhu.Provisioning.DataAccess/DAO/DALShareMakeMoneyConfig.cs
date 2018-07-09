using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu;
using Tuhu.Component.Common;
using System.Data.SqlClient;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALShareMakeMoneyConfig
    {


        public bool Add(SE_ShareMakeMoneyConfig modelConfig,out int ID)
        {
           
            string sql = @"INSERT INTO Configuration.dbo.SE_ShareMakeMoneyConfig
        ( 
          Name ,
          FirstShareNumber ,
          RewardDateTime ,
          CreateDate ,
          UpdateDate ,
          CreateUserName ,
          UpdateUserName,
          RuleInfo
        )
VALUES  (
          @Name , -- Name - nvarchar(50)
          @FirstShareNumber , -- FirstShareNumber - nvarchar(10)
          @RewardDateTime , -- RewardDateTime - int
          GETDATE() , -- CreateDate - datetime
          @UpdateDate , -- UpdateDate - datetime
          @CreateUserName , -- CreateUserName - nvarchar(50)
          @UpdateUserName,  -- UpdateUserName - nvarchar(50)
          @RuleInfo
        )  SELECT   @ResoutValue=@@IDENTITY ";

            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@Name", modelConfig.Name);
                cmd.Parameters.AddWithValue("@FirstShareNumber", modelConfig.FirstShareNumber);
                cmd.Parameters.AddWithValue("@RewardDateTime", modelConfig.RewardDateTime);
                cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreateUserName", DateTime.Now);
                cmd.Parameters.AddWithValue("@UpdateUserName", DateTime.Now);
                cmd.Parameters.AddWithValue("@RuleInfo", modelConfig.RuleInfo);
                cmd.Parameters.Add(new SqlParameter()
                {
                     ParameterName= "@ResoutValue",
                     Direction= ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                });
               
               var result = db.ExecuteNonQuery(cmd) > 0;
                ID = Convert.ToInt32(cmd.Parameters["@ResoutValue"].Value);
                return result;
            }
        }


        public bool Update(SE_ShareMakeMoneyConfig model)
        {
            string sql = @"UPDATE Configuration.dbo.SE_ShareMakeMoneyConfig SET RuleInfo=@RuleInfo, Name=@Name, FirstShareNumber=@FirstShareNumber, RewardDateTime=@RewardDateTime, UpdateDate=@UpdateDate, UpdateUserName=@UpdateUserName   WHERE ID=@ID ";
            try
            {
              
                int i = DbHelper.ExecuteNonQuery(sql,CommandType.Text, new SqlParameter[] {
                new SqlParameter("@Name",model.Name),
                new SqlParameter("@FirstShareNumber",model.FirstShareNumber),
                new SqlParameter("@RewardDateTime",model.RewardDateTime),
                new SqlParameter("@UpdateDate",DateTime.Now),
                new SqlParameter("@UpdateUserName",model.UpdateUserName),
                new SqlParameter("@RuleInfo",model.RuleInfo),
                new SqlParameter("@ID",model.ID)
            });
                return i > 0;
            }
            catch (Exception em)
            {
                return false;
            }
        }


        public IEnumerable<SE_ShareMakeMoneyConfig> GetList()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                var obj = dbHelper.ExecuteDataTable("SELECT *  FROM  Configuration.dbo.SE_ShareMakeMoneyConfig(NOLOCK) ");
                if (obj == null || obj.Rows.Count == 0)
                    return null;
                return obj.ConvertTo<SE_ShareMakeMoneyConfig>();
            }
        }


        public int AddProducts(IEnumerable<SE_ShareMakeImportProducts> models, int FKID)
        { 
            if (FKID == 0)
            {
                Add(new SE_ShareMakeMoneyConfig() {
                     Name="分享赚钱配置"
                },out FKID);
               
                if (FKID == 0)
                    return -1;
            }

            Guid BatchGuid = Guid.NewGuid();
            string sql = @"INSERT INTO Configuration.dbo.SE_ShareMakeImportProducts
        ( FKID ,
          BatchGuid ,
          CreateDate ,
          PID ,
          DisplayName ,
          Times ,
          IsMakeMoney ,
          IsShare ,
          Orderby
        )
VALUES  ( @FKID , -- FKID - int
          @BatchGuid , -- BatchGuid - nvarchar(50)
          GETDATE() , -- CreateDate - datetime
          @PID , -- PID - nvarchar(513)
          @DisplayName , -- DisplayName - nvarchar(128)
          @Times , -- Times - nvarchar(10)
          @IsMakeMoney , -- IsMakeMoney - bit
          @IsShare , -- IsShare - bit
          @Orderby -- Orderby - int
        )";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                db.BeginTransaction();
                foreach (var product in models)
                {
                    SqlCommand cmd = new SqlCommand(sql);
                    cmd.Parameters.AddWithValue("@FKID", FKID);
                    cmd.Parameters.AddWithValue("@BatchGuid", BatchGuid);
                    cmd.Parameters.AddWithValue("@PID", product.PID);
                    cmd.Parameters.AddWithValue("@DisplayName", product.DisplayName);
                    cmd.Parameters.AddWithValue("@Times", product.Times);
                    cmd.Parameters.AddWithValue("@IsMakeMoney", product.IsMakeMoney);
                    cmd.Parameters.AddWithValue("@IsShare", product.IsShare);
                    cmd.Parameters.AddWithValue("@Orderby", product.Orderby);
                    db.ExecuteNonQuery(cmd);
                }
                try
                {
                    db.Commit();
                    return FKID;
                }
                catch (Exception em)
                {
                    db.Rollback();
                    return -1;
                }
            }
        }

        public bool DeleteProduct(int id)
        {
            return DbHelper.ExecuteNonQuery("DELETE FROM Configuration.dbo.SE_ShareMakeImportProducts WHERE ID=@ID", CommandType.Text, new SqlParameter[] {
                 new SqlParameter("@ID",id)
            }) > 0;
        }

        public bool Delete(int id)
        {
            return DbHelper.ExecuteNonQuery("DELETE FROM Configuration.dbo.SE_ShareMakeMoneyConfig WHERE ID=@ID ", CommandType.Text, new SqlParameter()
            {
                 ParameterName="@ID",
                 Value=id
            })>0;
        }


        public SE_ShareMakeMoneyConfig GetEntity(int id)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                string sql = "SELECT TOP 1 * FROM Configuration.dbo.SE_ShareMakeMoneyConfig (NOLOCK) where ID=@ID";
                SE_ShareMakeMoneyConfig model = new SE_ShareMakeMoneyConfig();
                return dbHelper.ExecuteDataTable(sql,CommandType.Text,new SqlParameter() {
                     ParameterName="@ID",
                     Value= id
                }).ConvertTo<SE_ShareMakeMoneyConfig>().FirstOrDefault();
            }
        }


        public IEnumerable<SE_ShareMakeImportProducts> GetProductEntities( int pageIndex, int pageSize,  string whereString,SqlParameter [] parameters, out int total)
        {
           
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                string totalSQL = "SELECT COUNT(1) FROM Configuration.dbo.SE_ShareMakeImportProducts (NOLOCK) where 1=1 "+whereString;
                
                var rows = dbHelper.ExecuteScalar(totalSQL, CommandType.Text, parameters);
                if (rows != null)
                    int.TryParse(rows.ToString(), out total);
                else
                    total = 0;


                string selectSQL = @"
                                    SELECT  *
                                    FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateDate DESC ) AS RowNumber ,
                                                        *
                                              FROM      Configuration.dbo.SE_ShareMakeImportProducts (NOLOCK)
                                              WHERE    1=1 "+whereString+@"
                                            ) AS MT
                                    WHERE   MT.RowNumber > ( @pageIndex - 1 ) * @pageSize
                                            AND MT.RowNumber <= ( @pageIndex * @pageSize );";
                List<SqlParameter> para = new List<SqlParameter>();
                foreach (var p in parameters)
                {
                    SqlParameter item = new SqlParameter(p.ParameterName, p.Value);
                    para.Add(item);
                }

                para.Add(new SqlParameter()
                {
                    ParameterName = "@pageIndex",
                    Value = pageIndex
                });
                para.Add(new SqlParameter()
                {
                    ParameterName = "@pageSize",
                    Value = pageSize
                });
                return dbHelper.ExecuteDataTable(selectSQL, CommandType.Text, para).ConvertTo<SE_ShareMakeImportProducts>();
            }
        }

        public IEnumerable<SE_ShareMakeImportProducts> GetProductEntities(int id)
        {

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                string selectSQL = @"SELECT * FROM Configuration.dbo.SE_ShareMakeImportProducts (NOLOCK) where FKID=@FKID";

                return dbHelper.ExecuteDataTable(selectSQL, CommandType.Text, new SqlParameter()
                {
                    ParameterName = "@FKID",
                    Value = id
                }).ConvertTo<SE_ShareMakeImportProducts>();
            }
        }

        public bool IsExist()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
                return (int)dbHelper.ExecuteScalar("SELECT COUNT(*)  FROM  Configuration.dbo.SE_ShareMakeMoneyConfig(NOLOCK)") > 0;
        }


        public IEnumerable<SE_ShareMakeImportProducts> GetProductEntities(string whereString, SqlParameter[] paramater)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
                return dbHelper.ExecuteDataTable("SELECT * FROM Configuration.dbo.SE_ShareMakeImportProducts (NOLOCK) where 1=1 "+whereString,CommandType.Text,paramater).ConvertTo<SE_ShareMakeImportProducts>();
        }


        public bool UpdateProductIsShare(int id,string isShare)
        {
            return DbHelper.ExecuteNonQuery("UPDATE Configuration.dbo.SE_ShareMakeImportProducts  SET IsShare=@IsShare WHERE ID=@ID", CommandType.Text, new SqlParameter[] {
                new SqlParameter("@IsShare",isShare),
                new SqlParameter("@ID",id)
            })>0;
        }

        public DataTable GetFenxiangzhuanqianProduct()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_BI")))
            {
                var sql = @"SELECT * FROM [TuHu_BI].dbo.[dm_FenxiangzhuanqianProduct] WITH (NOLOCK)";
                return dbHelper.ExecuteDataTable(sql);
            }
        }
    }
}
