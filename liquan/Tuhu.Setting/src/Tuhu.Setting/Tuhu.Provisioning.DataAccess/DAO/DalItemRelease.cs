using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalItemRelease
    {
        public static IEnumerable<ReleaseItemModel> SelectList(ReleaseItemModel model, PagerModel pager)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                pager.TotalItem = GetListCount(dbHelper, model, pager);
                var list = dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[Name]
                                                  ,[Content]
                                                  ,[Developer]
                                                  ,[Tester]
                                                  ,[Checker]
                                                  ,[ReleaseTime]
                                                  ,[IsValid]
                                                  ,[Status]
                                                  ,[Reason]
                                              FROM [dbo].[tbl_ReleaseItem](nolock)
                                              where (Name like @Name or @Name is null)
                                              and (Developer like @Developer or @Developer is null)
                                              and (Tester like @Tester or @Tester is null)
                                              and (Checker like @Checker or @Checker is null)
                                              and (datediff(dd,ReleaseTime,@ReleaseTime)=0 or @ReleaseTime is null)
                                             
                                               and (Status=@Status or @Status=-1)
                                              order by PKID desc
                                                       		OFFSET ( @PageIndex - 1 ) * @PageSize ROWS
                                                                                                          FETCH NEXT @PageSize ROWS ONLY;", CommandType.Text,
                                                                                           new SqlParameter[] {
                                                                                               new SqlParameter("@Name", String.IsNullOrWhiteSpace(model.Name)?null:"%"+model.Name.Replace("|","%")+"%"),
                                                                                               new SqlParameter("@Developer", String.IsNullOrWhiteSpace(model.Developer)?null:"%"+model.Developer+"%"),
                                                                                                new SqlParameter("@Tester", String.IsNullOrWhiteSpace(model.Tester)?null:"%"+model.Tester+"%"),
                                                                                                new SqlParameter("@Checker", String.IsNullOrWhiteSpace(model.Checker)?null:"%"+model.Checker+"%"),
                                                                                                new SqlParameter("@ReleaseTime", model.ReleaseTime==null?null:model.ReleaseTime),
                                                                                                   new SqlParameter("@Status", model.Status),
                                                                                               new SqlParameter("@PageIndex", pager.CurrentPage),
                                                                                               new SqlParameter("@PageSize", pager.PageSize),
                                                                                           }).ConvertTo<ReleaseItemModel>();
                return list;
            }
        }

        public static IEnumerable<TuhuReleaseModel> SelectList()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {

                var list = dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[Name]
                                                  ,ISNULL([Person],'') AS Person
                                                  ,ISNULL([Description],'') AS Description                                               
                                                  ,[IsValid]  
                                                  ,[Status]
                                              FROM [dbo].[tbl_TuhuReleaseModel](nolock);", CommandType.Text
                                                                                          ).ConvertTo<TuhuReleaseModel>();
                return list;
            }
        }

        public static int GetListCount(SqlDbHelper dbHelper, ReleaseItemModel model, PagerModel pager)
        {

            var OBJ = dbHelper.ExecuteScalar(@"select count(1) from [dbo].[tbl_ReleaseItem](nolock)   where (Name like @Name or @Name is null)
                                              and (Developer like @Developer or @Developer is null)
                                              and (Tester like @Tester or @Tester is null)
                                              and (Checker like @Checker or @Checker is null)
                                              and (datediff(dd,ReleaseTime,@ReleaseTime)=0 or @ReleaseTime is null)
                                              
                                               and (Status=@Status or @Status=-1)", CommandType.Text,
                new SqlParameter[] {
                      new SqlParameter("@Name", String.IsNullOrWhiteSpace(model.Name)?null:"%"+model.Name.Replace("|","%")+"%"),
                                                                                               new SqlParameter("@Developer", String.IsNullOrWhiteSpace(model.Developer)?null:"%"+model.Developer+"%"),
                                                                                                new SqlParameter("@Tester", String.IsNullOrWhiteSpace(model.Tester)?null:"%"+model.Tester+"%"),
                                                                                                new SqlParameter("@Checker", String.IsNullOrWhiteSpace(model.Checker)?null:"%"+model.Checker+"%"),
                                                                                                new SqlParameter("@ReleaseTime", model.ReleaseTime==null?null:model.ReleaseTime),

                                                                                                    new SqlParameter("@Status", model.Status)
                });

            return Convert.ToInt32(OBJ);
        }

        public static bool AddReleaseItem(ReleaseItemModel model)
        {
            using (var sqlHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                string sql = @"INSERT INTO [dbo].[tbl_ReleaseItem]
									        ( [Name]
                                           ,[Content]
                                           ,[Developer]
                                           ,[Tester]
                                           ,[Checker]
                                           ,[ReleaseTime]
                                           ,[IsValid]
                                           ,[CreateDateTime]
                                           ,[Status]
                                           ,[Reason]
									        )
									VALUES  ( @Name , 
									          @Content , 
									          @Developer ,
									          @Tester ,
									          @Checker , 
									          @ReleaseTime ,
									          @IsValid,
									          GETDATE(),
                                              @Status,
                                              @Reason
									        )";
                var sqlParam = new[]
                {
                    new SqlParameter("@Name", model.Name.Trim(new char[]{'|'})),
                    new SqlParameter("@Content",model.Content),
                    new SqlParameter("@Developer", model.Developer),
                    new SqlParameter("@Tester", model.Tester),
                    new SqlParameter("@Checker",model.Checker),
                    new SqlParameter("@ReleaseTime", model.ReleaseTime),
                    new SqlParameter("@IsValid",1),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Reason",model.Reason)
                };
                return sqlHelper.ExecuteNonQuery(sql, CommandType.Text, sqlParam) > 0 ? true : false;
            }
        }

        public static ReleaseItemModel FetchReleaseItemModel(long pkid)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteDataTable(@"SELECT [PKID]
                                                  ,[Name]
                                                  ,[Content]
                                                  ,[Developer]
                                                  ,[Tester]
                                                  ,[Checker]
                                                  ,[ReleaseTime]
                                                  ,[IsValid]
                                                  ,[CreateDateTime]
                                                  ,[LastUpdatedDateTime]
                                                  ,[Reason]
                                                  ,[Status]
                                              FROM [dbo].[tbl_ReleaseItem] WITH(NOLOCK) WHERE PKID=@PKID
                                              ;", CommandType.Text,
                    new SqlParameter[]
                    {
                        new SqlParameter("@PKID", pkid)
                    }).ConvertTo<ReleaseItemModel>().FirstOrDefault();
            }
        }


        public static int DeleteReleaseItem(int pKID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteNonQuery(@"DELETE dbo.tbl_ReleaseItem  WHERE PKID=@PKID", CommandType.Text, new SqlParameter("@PKID", pKID));
            }
        }

        public static bool UpdateReleaseItemModel(ReleaseItemModel model)
        {

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                return dbHelper.ExecuteNonQuery(@"UPDATE	dbo.tbl_ReleaseItem
                                              SET  [Name]=@Name
                                                  ,[Content]=@Content
                                                  ,[Developer]=@Developer
                                                  ,[Tester]=@Tester
                                                  ,[Checker]=@Checker
                                                  ,[ReleaseTime]=@ReleaseTime                                                
                                                  ,[Status]=@Status
                                                  ,[Reason]=@Reason
                                                  ,[LastUpdatedDateTime]=@LastUpdatedDateTime
                                              WHERE	PKID = @PKID;
                                              ", CommandType.Text, new SqlParameter[] {
                                                new SqlParameter("@PKID",model.PKID),
                                                new SqlParameter("@Name",model.Name),
                                                new SqlParameter("@Content",model.Content),
                                                new SqlParameter("@Developer",model.Developer),
                                                new SqlParameter("@Tester",model.Tester),
                                                new SqlParameter("@Checker",model.Checker),
                                                new SqlParameter("@ReleaseTime",model.ReleaseTime),
                                                 new SqlParameter("@Status",model.Status),
                                                 new SqlParameter("@Reason",model.Reason),
                                                new SqlParameter("@LastUpdatedDateTime",DateTime.Now.ToString("yyyy-MM-dd"))
            }) > 0 ? true : false;
            }
        }
    }
}
