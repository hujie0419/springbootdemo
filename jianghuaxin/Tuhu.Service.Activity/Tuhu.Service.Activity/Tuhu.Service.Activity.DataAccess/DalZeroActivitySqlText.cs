using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess
{
    public partial class DalZeroActivity
    {
        private static string SelectNumOfApplications => @"SELECT COUNT(0) AS NumOfApplications FROM Activity..tbl_ZeroActivity_Apply WITH(NOLOCK) WHERE Period = @Period";
        private static string SelectUnfinishedZeroActivitiesForHomepage => @"
                                        SELECT  Period,
                                                Quantity, 
                                                SucceedQuota, 
                                                StartDateTime, 
                                                EndDateTime, 
                                                Description, 
                                                ImgUrl,
                                                ProductID + '|' + VariantID AS PID
                                        FROM Activity..tbl_ZeroActivity WITH (NOLOCK)
                                        WHERE GETDATE() < EndDateTime";
        private static string SelectFinishedZeroActivitiesForHomepage => @"
                                        SELECT  Period, 
                                                Quantity, 
                                                SucceedQuota, 
                                                StartDateTime, 
                                                EndDateTime,
                                                CASE WHEN GETDATE() >= DATEADD(DAY,30,EndDateTime) THEN 4 ELSE 3 END AS StatusOfActivity, 
                                                Description, 
                                                ImgUrl,
                                                ProductID + '|' + VariantID AS PID
                                        FROM Activity..tbl_ZeroActivity WITH (NOLOCK)
                                        WHERE GETDATE() >= EndDateTime 
                                        ORDER BY StatusOfActivity, Period DESC
                                        OFFSET (@PageNumber - 1) * 5 ROWS FETCH NEXT 5 ROWS ONLY";
        private static string FetchZeroActivityDetail => @"
                                        SELECT  Period, 
                                                Quantity,
                                                SucceedQuota, 
                                                StartDateTime, 
                                                EndDateTime, 
                                                Description, 
                                                ImgUrl,
                                                ProductID + '|' + VariantID AS PID
                                        FROM Activity..tbl_ZeroActivity WITH (NOLOCK)
                                        WHERE Period = @Period";
        private static string NumOfZeroActivityApplications => @"SELECT COUNT(0) FROM Activity..tbl_ZeroActivity_Apply with(nolock) WHERE UserID = @UserID AND Period = @Period";

        private static string NumOfZeroActivityReminders => @"SELECT COUNT(0) FROM Tuhu_log.dbo.ZeroActivityReminder with(nolock) WHERE UserID = @UserID AND Period = @Period";

        private static string SelectChosenUserReports => @"SELECT   TC.CommentId ,
                                                                    ZAA.UserID ,
                                                                    TC.SingleTitle ,
                                                                    TC.CommentContent ,
                                                                    TC.CreateTime ,
                                                                    TC.CommentImages ,
                                                                    TC.CommentStatus ,
                                                                    TC.CommentType
                                                            FROM    Activity..tbl_ZeroActivity_Apply AS ZAA WITH ( NOLOCK )
                                                                    LEFT JOIN Gungnir..tbl_Comment AS TC WITH ( NOLOCK ) ON TC.CommentUserId = ZAA.UserID
                                                                                                                          AND TC.CommentOrderId = ZAA.OrderId
                                                            WHERE   ZAA.Status = 1
                                                                    AND ZAA.Succeed = 1
                                                                    AND ZAA.OrderId IS NOT NULL
                                                                    AND ZAA.Period = @Period";
        private static string FetchTestReportDetail => @"SELECT    TC.CommentId, 
                                                                    ZAA.UserID, 
                                                                    TC.SingleTitle, 
                                                                    TC.CommentContent, 
                                                                    TC.CreateTime, 
                                                                    ZAA.Period, 
                                                                    ZAA.ProvinceID, 
                                                                    ZAA.CityID, 
                                                                    TC.UpdateTime, 
                                                                    TC.CommentProductId, 
                                                                    TC.CommentProductFamilyId, 
                                                                    TC.CommentOrderId, 
                                                                    TC.CommentOrderListId, 
                                                                    TC.CommentImages, 
                                                                    TC.CommentStatus, 
                                                                    ZAA.ReportStatus, 
                                                                    TC.OfficialReply, 
                                                                    TC.CommentR1, 
                                                                    TC.CommentR2, 
                                                                    TC.CommentR3, 
                                                                    TC.CommentR4, 
                                                                    TC.CommentR5, 
                                                                    TC.CommentR6, 
                                                                    TC.CommentR7, 
                                                                    TC.CommentExtAttr
                                                        FROM Gungnir..tbl_Comment AS TC WITH (NOLOCK)
                                                        JOIN Activity..tbl_ZeroActivity_Apply AS ZAA WITH (NOLOCK) ON TC.CommentUserId = ZAA.UserID AND TC.CommentOrderId = ZAA.OrderId 
                                                        WHERE ZAA.status = 1 AND ZAA.Succeed = 1 AND ZAA.OrderId IS NOT NULL AND TC.CommentType = 3 AND TC.CommentStatus = 2 AND TC.CommentID = @CommentId";
        private static string SelectMyApplications => @"SELECT  Period, 
                                                                ApplyDateTime, 
                                                                OrderID,
                                                                PID
                                                        FROM Activity..tbl_ZeroActivity_Apply WITH (NOLOCK)
                                                        WHERE UserID = @UserID AND Status = @ApplicationStatus ORDER BY ApplyDateTime DESC";
        private static string InsertZeroActivityApplication => @"INSERT INTO Activity..tbl_ZeroActivity_Apply
                                                                         ( Period ,
                                                                           UserID ,
                                                                           UserName ,
                                                                           PID ,
                                                                           ProductName ,
                                                                           Quantity ,
                                                                           ProvinceID ,
                                                                           CityID ,
                                                                           ApplyDateTime ,
                                                                           SupportNumber ,
                                                                           LastUpdateDateTime ,
                                                                           ApplyReason ,
                                                                           ReportStatus, 
                                                                           ApplyChannel,
                                                                           CarID,
                                                                           Status,
																		   UserMobileNumber
                                                                         )
                                                                 VALUES  ( @Period , -- Period - int
                                                                           @UserID , -- UserID - uniqueidentifier
                                                                           @UserName , -- UserName - nvarchar(50)
                                                                           @PID , -- PID - varchar(200)
                                                                           @ProductName , -- ProductName - nvarchar(500)
                                                                           @Quantity , -- Quantity - tinyint
                                                                           @ProvinceID , -- ProvinceID - int
                                                                           @CityID, -- CityID - int
                                                                           GETDATE() , -- ApplyDateTime - datetime
                                                                           0 , -- SupportNumber - int
                                                                           GETDATE() , -- LastUpdateDateTime - datetime
                                                                           @ApplyReason , -- ApplyReason - nvarchar(2000)
                                                                           0,
                                                                           1,
                                                                           @CarID, -- CarID
                                                                           0,  -- Status - int
                                                                           @Mobile --usermobilenumber - string
                                                                         );
                                                                SELECT @@ROWCOUNT";
        private static string FetchStartTimeOfZeroActivity = @"SELECT StartDateTime FROM Activity..tbl_ZeroActivity WITH (NOLOCK) WHERE Period = @Period";
        private static string InsertZeroActivityReminder = @"   INSERT INTO Tuhu_log.dbo.ZeroActivityReminder 
                                                                    (Period, -- 期数 - int
                                                                    UserID, -- UserID - uniqueidentifier
                                                                    CreateTime --创建时间1 - DateTime
                                                                    ) 
                                                                    VALUES (@Period, 
                                                                    @UserID, 
                                                                    GETDATE()
                                                                    );
                                                                    SELECT @@ROWCOUNT";
    }
}
