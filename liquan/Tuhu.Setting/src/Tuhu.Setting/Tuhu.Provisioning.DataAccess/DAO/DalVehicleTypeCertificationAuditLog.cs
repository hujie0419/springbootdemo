using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ActivityBoard;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalVehicleTypeCertificationAuditLog
    {
        public static bool InsertLog(VehicleTypeCertificationAuditLogModel model)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    new SqlCommand(
                        @"INSERT Tuhu_log..VehicleTypeCertificationAuditLog(CarId,Author,OldValue,NewValue,Description,CreateTime) VALUES(@carId,@author,@oldValue,@newValue,@description,GETDATE());")
                )
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@carId", model.CarId);
                    cmd.Parameters.AddWithValue("@author", model.Author);
                    cmd.Parameters.AddWithValue("@oldValue", model.OldValue);
                    cmd.Parameters.AddWithValue("@newValue", model.NewValue);
                    cmd.Parameters.AddWithValue("@description", model.Description);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
        }


        public static List<VehicleTypeCertificationAuditLogModel> SelectLogs(List<Guid> carIds)
        {
            if (!carIds.Any()) return new List<VehicleTypeCertificationAuditLogModel>();
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT * FROM (SELECT ROW_NUMBER() OVER(PARTITION BY CarId ORDER BY CreateTime DESC) AS N,* 
                        FROM Tuhu_log..VehicleTypeCertificationAuditLog WOTH(NOLOCK) WHERE CarId IN({string.Join(",",carIds.Select(_=>$"'{_}'"))})) AS T
                        WHERE T.N=1"))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<VehicleTypeCertificationAuditLogModel>().ToList();
                }
            }
        }

        public static List<VehicleTypeCertificationAuditLogModel> SelectLogsByCarId(Guid carId)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT * 
                        FROM Tuhu_log..VehicleTypeCertificationAuditLog WITH(NOLOCK) WHERE CarId=@carId ORDER BY CreateTime DESC"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@carId", carId);
                    return dbHelper.ExecuteDataTable(cmd).ConvertTo<VehicleTypeCertificationAuditLogModel>().ToList();
                }
            }
        }


        public static ListModel<VehicleAuditInfoModel> SelectVehicleAuditInfo(VehicleAuthAuditRequest request)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<SqlParameter> countSqlParameters = new List<SqlParameter>();
            string whereSql = " AND VTCI.[Status]=@status AND VA.[Status]=@status ";
            sqlParameters.Add(new SqlParameter("@status", request.Status));
            countSqlParameters.Add(new SqlParameter("@status", request.Status));
            if (!string.IsNullOrEmpty(request.Mobile))
            {
                whereSql += " AND U.u_mobile_number=@mobile";
                sqlParameters.Add(new SqlParameter("@mobile", request.Mobile));
                countSqlParameters.Add(new SqlParameter("@mobile", request.Mobile));
            }
            
            if ((!string.IsNullOrWhiteSpace(request.CarNo)||!string.IsNullOrWhiteSpace(request.VinCode))&&request.CarId!=Guid.Empty)
            {
                whereSql += " AND VTCI.CarId!=@CarId AND VTCI.Status=1 ";
                sqlParameters.Add(new SqlParameter("@CarId", request.CarId));
                countSqlParameters.Add(new SqlParameter("@CarId", request.CarId));

                whereSql += " AND ( ";
                List<string> tempSql = new List<string>();
                if (!string.IsNullOrWhiteSpace(request.CarNo))
                {
                    tempSql.Add("CO.u_carno=@CarNo");
                    sqlParameters.Add(new SqlParameter("@CarNo", request.CarNo));
                    countSqlParameters.Add(new SqlParameter("@CarNo", request.CarNo));
                }

                if (!string.IsNullOrWhiteSpace(request.VinCode))
                {
                    tempSql.Add("CO.VinCode=@VinCode");
                    sqlParameters.Add(new SqlParameter("@VinCode", request.VinCode));
                    countSqlParameters.Add(new SqlParameter("@VinCode", request.VinCode));
                }

               

                whereSql += string.Join(" OR ", tempSql);

                whereSql += " )";
            }



           
                //如果是待审核数据，从申诉数据库里关联查询
                string sql = $@"SELECT
                            VTCI.Status,
                            VTCI.LastUpdateDateTime AS LastChangedDate,
                            VA.CreateTime AS CreatedDate,
                            VTCI.certified_time,
                            VTCI.Channel,
                            VA.Vehicle_license_img AS ImageUrl, 
                            {(request.Status==1? "CO.u_carno" : "VA.CarNo")} AS CarNumber,
                            {(request.Status==1? "CO.VinCode" : "VA.ClassNo")} AS VinCode,
                            VA.User_IdCard_img AS IdCardUrl,
                            VA.Reason,
                                CO.CarID,
                            VA.Engineno AS EngineNo,
                            VA.car_Registrationtime,
                            CO.Brand,
                            CO.Vehicle,
                            CO.u_PaiLiang,
                            CO.u_Nian,
                            CO.SalesName,
                            CO.IsDefaultCar,                           
                            U.u_mobile_number AS Mobile
                            FROM tuhu_profiles..VehicleTypeCertificationInfo AS VTCI WITH(NOLOCK)
                            INNER JOIN Tuhu_profiles..CarObject AS CO WITH(NOLOCK) ON VTCI.CarID = CO.CarID
                            INNER JOIN Tuhu_profiles..VehicleAuthAppeal AS VA WITH(NOLOCK) ON VTCI.CarID=VA.CarID AND VA.PKID=(SELECT MAX(PKID) FROM Tuhu_profiles..VehicleAuthAppeal WHERE CarID=VTCI.CarID) 
                            INNER JOIN Tuhu_profiles..UserObject AS U WITH(NOLOCK) ON CO.UserID = U.UserID
                            WHERE CO.IsDeleted = 0 {whereSql} 
                            ORDER BY VTCI.CreateDateTime DESC
                            OFFSET ( @pageSize * ( @pageIndex - 1 ) ) ROWS
                            FETCH NEXT @pageSize ROWS ONLY;";

            string countSql = $@"SELECT COUNT({"DISTINCT VTCI.CarID"}) 
                                FROM tuhu_profiles..VehicleTypeCertificationInfo AS VTCI WITH(NOLOCK)
                                INNER JOIN Tuhu_profiles..CarObject AS CO WITH(NOLOCK) ON VTCI.CarID = CO.CarID
                                INNER JOIN Tuhu_profiles..VehicleAuthAppeal AS VA WITH(NOLOCK) ON VTCI.CarID=VA.CarID
                                INNER JOIN Tuhu_profiles..UserObject AS U WITH(NOLOCK) ON CO.UserID = U.UserID
                                WHERE CO.IsDeleted = 0 {whereSql}";


            sqlParameters.Add(new SqlParameter("@pageIndex", request.PageIndex));
            sqlParameters.Add(new SqlParameter("@pageSize", request.PageSize));
            var model = new ListModel<VehicleAuditInfoModel>();
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddRange(sqlParameters.ToArray());
                var queryResult = DbHelper.ExecuteDataTable(cmd).ConvertTo<VehicleAuditInfoModel>();

                model.Source = queryResult;
            }
            using (var cmd = new SqlCommand(countSql))
            {
                int totalCount = 0;
                cmd.Parameters.AddRange(countSqlParameters.ToArray());
                var countResult = DbHelper.ExecuteScalar(cmd);

                if (!Convert.IsDBNull(countResult))
                {
                    int.TryParse(countResult.ToString(), out totalCount);
                }
                model.Pager = new PagerModel()
                {
                    CurrentPage = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalItem = totalCount
                };
            }
            return model;
        }



        public static bool UpdateVehicleAuditStatus(BaseDbHelper db, Guid carId, int status)
        {

            using (var cmd = new SqlCommand(@"UPDATE Tuhu_profiles..VehicleAuthAppeal WITH(ROWLOCK) SET Status=@Status WHERE CarID=@CarID"))
            {
                cmd.Parameters.Add(new SqlParameter("@Status", status));
                cmd.Parameters.Add(new SqlParameter("@CarID", carId));
                return db.ExecuteNonQuery(cmd)>0;
            }
        }

        public static bool UpdateVehicleTypeCertificationInfoStatus(BaseDbHelper db, Guid carId, int status)
        {
            //如果是认证成功
            string certifiedSql = "";
            if (status == 1)
            {
                certifiedSql = ",certified_time=GETDATE()";
            }
            using (var cmd =
                new SqlCommand(
                    $@"UPDATE Tuhu_Profiles..VehicleTypeCertificationInfo WITH(ROWLOCK) SET Status=@Status{
                            certifiedSql
                        },LastUpdateDateTime=GETDATE() WHERE CarId=@CarId"))
            {
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@CarId", carId);
                cmd.CommandType = CommandType.Text;
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static int GetVehicleTypeCertificationInfStatus(Guid carId)
        {
            using (var cmd =
                new SqlCommand(@"SELECT Status FROM Tuhu_Profiles..VehicleTypeCertificationInfo WITH(NOLOCK) WHERE CarId=@CarId"))
            {
                cmd.Parameters.Add(new SqlParameter("@CarId", carId));
                return (int)DbHelper.ExecuteScalar(cmd);
            }
        }

        public static bool UpdateCarObject(BaseDbHelper db, VehicleAuditInfoModel request)
        {
            using (var cmd =
                new SqlCommand(
                    @"Update Tuhu_Profiles..CarObject WITH(ROWLOCK) SET VinCode=@VinCode,ClassNo=@ClassNo,u_carno=@CarNo,Engineno=@EngineNo where CarId=@CarId")
            )
            {
                cmd.Parameters.AddWithValue("@VinCode", request.VinCode);
                cmd.Parameters.AddWithValue("@CarNo", request.CarNumber);
                cmd.Parameters.AddWithValue("@CarId", request.CarId);
                cmd.Parameters.AddWithValue("@ClassNo", request.VinCode);
                cmd.Parameters.AddWithValue("@EngineNo", request.EngineNo);
                cmd.CommandType = CommandType.Text;
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool UpdateVehicleTypeCertificationInfo(BaseDbHelper db, VehicleAuditInfoModel request)
        {
            using (var cmd =
                new SqlCommand(
                    @"Update Tuhu_Profiles..VehicleTypeCertificationInfo WITH(ROWLOCK) SET Vehicle_license_img=@Vehicle_license_img,User_IdCard_img=@User_IdCard_img where CarId=@CarId")
            )
            {
                cmd.Parameters.AddWithValue("@Vehicle_license_img", request.ImageUrl);
                cmd.Parameters.AddWithValue("@User_IdCard_img", request.IdCardUrl);
                cmd.Parameters.AddWithValue("@CarId", request.CarId);
                cmd.CommandType = CommandType.Text;
                return db.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static VehicleAuditInfoModel GetVehicleAuthAppeal(Guid carId)
        {
            //using (var cmd = new SqlCommand(@"SELECT  Vehicle_license_img AS ImageUrl, CarID,User_IdCard_img AS IdCardUrl,
            //    CarNo AS CarNumber,
            //    ClassNo AS VinCode,Engineno AS EngineNo,car_Registrationtime FROM Tuhu_profiles..VehicleAuthAppeal WITH(NOLOCK) WHERE CarID=@CarID AND Status=0"))
            using (var cmd = new SqlCommand(@" SELECT  vav.Vehicle_license_img AS ImageUrl, vav.CarID,vav.User_IdCard_img AS IdCardUrl,
                vav.CarNo AS CarNumber,
                vav.ClassNo AS VinCode,vav.Engineno AS EngineNo,vav.car_Registrationtime,vtc.Channel FROM Tuhu_profiles..VehicleAuthAppeal vav 
				inner join Tuhu_Profiles..VehicleTypeCertificationInfo vtc
				on vav.CarID=vtc.CarID
				WHERE vav.CarID=@CarID AND vav.Status=0"))

            {
                cmd.Parameters.Add(new SqlParameter("@CarID", carId));
                return DbHelper.ExecuteDataTable(cmd).ConvertTo<VehicleAuditInfoModel>().FirstOrDefault();
            }
        }
    }

}