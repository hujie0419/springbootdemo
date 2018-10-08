USE Tuhu_profiles
GO
IF EXISTS ( SELECT	*
			FROM	dbo.sysobjects
			WHERE	ID = OBJECT_ID(N'[dbo].[JobSchedulerService_SelectSmsWhiteList]')
					AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
   DROP PROCEDURE [dbo].[JobSchedulerService_SelectSmsWhiteList]
GO
/*=============================================================
--FuncDes:	选择新增的手机号
--Author	ModifyDate		Reason
--pengwei	2014-05-14		Create

EXEC [JobSchedulerService_SelectSmsWhiteList] '2014-01-10'
=============================================================*/
CREATE PROCEDURE [dbo].[JobSchedulerService_SelectSmsWhiteList]
	   @LastExecuteDateTime DATETIME
AS
SELECT DISTINCT
		u_mobile_number
FROM	Tuhu_profiles..UserObject
WHERE	(dt_date_created > @LastExecuteDateTime)
		AND ISNUMERIC(u_mobile_number) = 1
		AND SUBSTRING(u_mobile_number, 1, 1) = '1'
		AND (LEN(u_mobile_number) = 11)
ORDER BY u_mobile_number
GO
