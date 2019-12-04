using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Models.Monitor;

namespace Tuhu.C.ActivityJob.Dal.Monitor
{
    public class UserLocationMonitorDal
    {
        public static int GetLocationMonitorCount(DateTime startTime)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                string sql = @"SELECT COUNT(1)
                    FROM Tuhu_log..UserLocationMonitorLog WITH (NOLOCK)
                    WHERE CreateDateTime >= @CreateDateTime;";

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CreateDateTime", startTime);
                    return Convert.ToInt32(dbHelper.ExecuteScalar(cmd));
                }
            }
        }

        public static List<UserLocationMonitor> GetLocationMonitors(int pageSize, DateTime startTime, ref int maxPkid)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand($@"
                    SELECT TOP {pageSize}
                           PKID,
                           Mobile,
                           MobileLocation,
                           Gps,
                           GpsLocation,
                           Ip,
                           IpLocation
                    FROM Tuhu_log..UserLocationMonitorLog WITH (NOLOCK)
                    WHERE PKID > {maxPkid}
                          AND CreateDateTime >= @CreateDateTime
                    ORDER BY PKID;"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CreateDateTime", startTime);
                    var dt = dbHelper.ExecuteQuery(cmd, _ => _);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var result = dt.AsEnumerable().Select(r => new UserLocationMonitor
                        {
                            Mobile = r["Mobile"]?.ToString(),
                            MobileLocation = r["MobileLocation"]?.ToString(),
                            Gps = r["Gps"]?.ToString(),
                            GpsLocation = r["GpsLocation"]?.ToString(),
                            Ip = r["Ip"]?.ToString(),
                            IpLocation = r["IpLocation"]?.ToString(),
                            Pkid = Convert.ToInt32(r["PKID"]?.ToString() ?? "0")
                        });

                        maxPkid = result.Max(x => x.Pkid);
                        return result.ToList();
                    }
                    return new List<UserLocationMonitor>();
                }
            }
        }

        public static bool UpdateLocationMonitor(UserLocationMonitor monitor)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                string sql = @"UPDATE Tuhu_log..UserLocationMonitorLog WITH (ROWLOCK)
                                SET MobileLocation = @MobileLocation,
                                    GpsLocation = @GpsLocation,
                                    IpLocation = @IpLocation,
                                    LastUpdateDateTime = GETDATE()
                                WHERE PKID = @PKID;";

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@MobileLocation", monitor.MobileLocation);
                    cmd.Parameters.AddWithValue("@GpsLocation", monitor.GpsLocation);
                    cmd.Parameters.AddWithValue("@IpLocation", monitor.IpLocation);
                    cmd.Parameters.AddWithValue("@PKID", monitor.Pkid);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
        }

        public static string GetMobileLocation(string mobile)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                string sql = @"SELECT TOP 1
                                       MobileLocation
                                FROM Tuhu_log..UserLocationMonitorLog WITH (NOLOCK)
                                WHERE Mobile = @Mobile;";

                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Mobile", mobile);
                    return dbHelper.ExecuteScalar(cmd)?.ToString();
                }
            }
        }
    }
}
