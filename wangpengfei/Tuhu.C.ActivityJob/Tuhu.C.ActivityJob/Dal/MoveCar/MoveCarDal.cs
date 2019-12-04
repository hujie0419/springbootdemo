using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Dal.MoveCar
{
    public class MoveCarDal
    {
        /// <summary>
        /// 获取扫码挪车二维码表的数据个数
        /// </summary>
        /// <returns></returns>
        public static int GetMoveCarQRCodeCount()
        {
            string sql = @"
                        SELECT COUNT(*)
                          FROM Tuhu_profiles..MoveCarQRCode WITH (NOLOCK)
                         WHERE (   UniqueQRCode IS NULL
                              OR   UniqueQRCode    = '')
                           AND QRCodeImageUrl      != ''
                           AND QRCodeImageUrl IS NOT NULL
                           AND LEN(QRCodeImageUrl) >= 42;";
            using(var cmd=new SqlCommand(sql))
            {
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        /// <summary>
        /// 更新业务主键
        /// </summary>
        /// <param name="updateNum"></param>
        /// <returns></returns>
        public static int UpdateUniqueQRCode(int updateNum)
        {
            string sql = @"
                        UPDATE TOP ({0}) [Tuhu_profiles].[dbo].MoveCarQRCode WITH (ROWLOCK)
                           SET           LastUpdateDateTime = GETDATE(),
                                         UniqueQRCode = SUBSTRING(QRCodeImageUrl, 42, 28)
                         WHERE           ( UniqueQRCode IS NULL
                                          OR  UniqueQRCode = '')
                                           AND QRCodeImageUrl IS NOT NULL
                                           AND QRCodeImageUrl  != ''
                                           AND LEN(QRCodeImageUrl) >= 42;";
            sql = string.Format(sql, updateNum);
            using (var cmd=new SqlCommand(sql))
            {
                return DbHelper.ExecuteNonQuery(false, cmd);
            }
        }
    }
}
