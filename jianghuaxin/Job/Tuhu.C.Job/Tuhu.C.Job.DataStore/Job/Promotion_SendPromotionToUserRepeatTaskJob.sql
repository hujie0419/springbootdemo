USE [Gungnir];
GO

IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[Promotion_SendPromotionToUserRepeatTaskJob]')
                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[Promotion_SendPromotionToUserRepeatTaskJob];
GO
/*
Author:   dapeng
CREATE date: 2016-02-03 11:19:30.997
Description:  
塞券重复任务JOB
当SelectUserType=1（上传文件），用户从表UserCellPhone中获取
ModifiedHistory
LYY:
	CREATE date: 2016-02-03 11:19:30.997
	Description:  
	修改定义=》执行塞券任务 

*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE PROC [dbo].[Promotion_SendPromotionToUserRepeatTaskJob]
	--任务编号 null:不关联任务将待发送列表中有效的任务进行塞券操作
    @PromotionTaskId INT
AS
    BEGIN

        DECLARE @UserPhoneFilter TABLE
            (
              userid UNIQUEIDENTIFIER ,
              userphone NVARCHAR(32) ,
              orderNo NVARCHAR(10)
            );
        DECLARE @PromotionIdlist TABLE
            (
              PromotionSingleTaskUsersId INT ,
              userphone NVARCHAR(32)
            );
        DECLARE @PromotionIdlistHas TABLE
            (
              PromotionSingleTaskUsersId INT ,
              userphone NVARCHAR(32)
            );
        DECLARE @PromotionTask TABLE
            (
              id INT IDENTITY(1, 1) ,
              promotiontaskid INT ,
              tasktype INT ,
              taskstarttime DATETIME ,
              taskendtime DATETIME ,
              execperiod INT ,
              selectusertype INT ,
              filterstarttime DATETIME ,
              filterendtime DATETIME ,
              brand NVARCHAR(200) ,
              category NVARCHAR(200) ,
              pid NVARCHAR(200) ,
              spendmoney FLOAT ,
              purchasenum INT ,
              area NVARCHAR(200) ,
              channel NVARCHAR(200) ,
              installtype INT ,
              orderstatus NVARCHAR(200) ,
              vehicle NVARCHAR(200) ,
              creater NVARCHAR(50)
            );
			  
			/*
			TaskType 过滤可注销，目前Job分为单次任务执行job与触发任务执行Job
			单次任务执行job：@PromotionTaskId=传入任务Id
			触发任务执行Job: @PromotionTaskId=null
			此处TaskType为区分两种Job, 注销不影响塞券逻辑
			*/
        INSERT  INTO @PromotionTask
                SELECT  PromotionTaskId ,
                        TaskType ,
                        TaskStartTime ,
                        TaskEndTime ,
                        ExecPeriod ,
                        SelectUserType ,
                        FilterStartTime ,
                        FilterEndTime ,
                        Brand ,
                        Category ,
                        Pid ,
                        SpendMoney ,
                        PurchaseNum ,
                        Area ,
                        Channel ,
                        InstallType ,
                        OrderStatus ,
                        Vehicle ,
                        Creater
                FROM    dbo.tbl_PromotionTask (NOLOCK)
                WHERE   ( @PromotionTaskId IS NULL
                          OR PromotionTaskId = @PromotionTaskId
                        )
                        AND TaskStartTime <= GETDATE()
                        AND ( ( TaskType = 2
                                AND TaskEndTime >= GETDATE()
                              )
                              OR TaskType = 1
                            )
                        AND TaskStatus = 1; --已审核
			
		  
        DECLARE @rows INT;
        DECLARE @index INT;
        DECLARE @CurTime DATETIME= GETDATE();

        SELECT  @rows = COUNT(1)
        FROM    @PromotionTask;

        SET @index = 0;

        DECLARE @Id INT;
        DECLARE @TaskType INT ,
            @TaskStartTime DATETIME ,
            @TaskEndTime DATETIME ,
            @ExecPeriod INT ,
            @SelectUserType INT ,
            @FilterStartTime DATETIME ,
            @FilterEndTime DATETIME ,
            @Brand NVARCHAR(200) ,
            @Category NVARCHAR(200) ,
            @Pid NVARCHAR(200) ,
            @SpendMoney FLOAT ,
            @PurchaseNum INT ,
            @Area NVARCHAR(200) ,
            @Channel NVARCHAR(200) ,
            @InstallType INT ,
            @OrderStatus NVARCHAR(200) ,
            @Vehicle NVARCHAR(200);
        DECLARE @Creater NVARCHAR(200);
        DECLARE @Cellphones Cellphone; --要发送的手机列表
        DECLARE @TaskPromotionListId INT;
        DECLARE @CouponRulesId INT;
        DECLARE @PromotionDescription NVARCHAR(50);
        DECLARE @StartTime DATETIME;
        DECLARE @EndTime DATETIME;
        DECLARE @MinMoney FLOAT;
        DECLARE @DiscountMoney FLOAT;
        DECLARE @Type INT = NULL;
			--DECLARE @Channels NVARCHAR(200)
        DECLARE @IsEnableSend INT;

        BEGIN TRY
            BEGIN TRAN;

            WHILE ( @index < @rows )
                BEGIN
				  -- 一个任务对应多张券，遍历多张券并执行塞券动作

				  --设置任务信息
                    SELECT  @PromotionTaskId = promotiontaskid ,
                            @TaskType = tasktype ,
                            @TaskStartTime = taskstarttime ,
                            @TaskEndTime = taskendtime ,
                            @ExecPeriod = execperiod ,
                            @SelectUserType = selectusertype ,
                            @FilterStartTime = filterstarttime ,
                            @FilterEndTime = filterendtime ,
                            @Brand = brand ,
                            @Category = category ,
                            @Pid = pid ,
                            @SpendMoney = spendmoney ,
                            @PurchaseNum = purchasenum ,
                            @Area = area ,
                            @Channel = channel ,
                            @InstallType = installtype ,
                            @OrderStatus = orderstatus ,
                            @Vehicle = vehicle ,
                            @Creater = creater
                    FROM    @PromotionTask
                    WHERE   id = ( @index + 1 );

                    DECLARE @PromotionTaskPromotionList TABLE
                        (
                          id INT IDENTITY(1, 1) ,
                          taskpromotionlistid INT ,
                          promotiontaskid INT ,
                          couponrulesid INT ,
                          promotiondescription NVARCHAR(200) ,
                          starttime DATETIME ,
                          endtime DATETIME ,
                          minmoney FLOAT ,
                          discountmoney FLOAT
                        );
				  
				  --插入优惠券列表
                    INSERT  INTO @PromotionTaskPromotionList
                            ( taskpromotionlistid ,
                              promotiontaskid ,
                              couponrulesid ,
                              promotiondescription ,
                              starttime ,
                              endtime ,
                              minmoney ,
                              discountmoney
                            )
                            SELECT  TaskPromotionListId ,
                                    PromotionTaskId ,
                                    CouponRulesId ,
                                    PromotionDescription ,
                                    StartTime ,
                                    EndTime ,
                                    MinMoney ,
                                    DiscountMoney
                            FROM    dbo.tbl_PromotionTaskPromotionList (NOLOCK)
                            WHERE   PromotionTaskId = @PromotionTaskId;

				  --单次条件选择任务 即时筛选用户 其他情况下对待塞券列表中的用户进行塞券
				  --【插入待待发送用户表】
                    IF ( @TaskType = 1
                         AND @SelectUserType = 2
                       )
                        BEGIN
                            INSERT  INTO @UserPhoneFilter
                                    EXEC [dbo].[Promotion_FilterUserByCondition] NULL,
                                        NULL, NULL, NULL, NULL, NULL, NULL,
                                        NULL, NULL, NULL, NULL, NULL,
                                        @PromotionTaskId,
                                        @FilterOrderNo = NULL;

                            INSERT  INTO [dbo].[tbl_PromotionSingleTaskUsers]
                                    SELECT  T.* ,
                                            @CurTime
                                    FROM    ( SELECT    @PromotionTaskId ID ,
                                                        userphone
                                              FROM      @UserPhoneFilter
                                            ) T;
                        END;
			
                    DELETE  @PromotionIdlist;
				  --插入控制表
                    INSERT  INTO @PromotionIdlist
                            SELECT  PromotionSingleTaskUsersId ,
                                    UserCellPhone
                            FROM    dbo.tbl_PromotionSingleTaskUsers (NOLOCK)
                            WHERE   PromotionTaskId = @PromotionTaskId;
				  
				  --单用户多优惠券插入情况
                    NOEMPTY:
				  
                    DELETE  @Cellphones;

				  --插入待发送临时表 每个电话的第一条记录
                    INSERT  INTO @Cellphones
                            SELECT  T.userphone
                            FROM    ( SELECT    PromotionSingleTaskUsersId ,
                                                userphone ,
                                                ROW_NUMBER() OVER ( PARTITION BY userphone ORDER BY PromotionSingleTaskUsersId ) rn
                                      FROM      @PromotionIdlist
                                      WHERE     PromotionSingleTaskUsersId NOT IN (
                                                SELECT  PromotionSingleTaskUsersId
                                                FROM    @PromotionIdlistHas )
                                    ) T
                            WHERE   T.rn = 1;

				  --没有记录跳出塞券操作
                    IF NOT EXISTS ( SELECT  *
                                    FROM    @Cellphones )
                        GOTO EMPTYEND;

				  --加入到已发送控制表
                    INSERT  INTO @PromotionIdlistHas
                            SELECT  T.PromotionSingleTaskUsersId ,
                                    T.userphone
                            FROM    ( SELECT    PromotionSingleTaskUsersId ,
                                                userphone ,
                                                ROW_NUMBER() OVER ( PARTITION BY userphone ORDER BY PromotionSingleTaskUsersId ) rn
                                      FROM      @PromotionIdlist
                                      WHERE     PromotionSingleTaskUsersId NOT IN (
                                                SELECT  PromotionSingleTaskUsersId
                                                FROM    @PromotionIdlistHas )
                                    ) T
                            WHERE   T.rn = 1;

                    DECLARE @rows_rule INT;
                    DECLARE @index_rule INT;

                    SELECT  @rows_rule = COUNT(1)
                    FROM    @PromotionTaskPromotionList;

                    SET @index_rule = 0;
				 
                    WHILE ( @index_rule < @rows_rule )
                        BEGIN
						--设置当前要派发的优惠券信息
                            SELECT  @TaskPromotionListId = taskpromotionlistid ,
                                    @CouponRulesId = couponrulesid ,
                                    @PromotionDescription = promotiondescription ,
                                    @StartTime = starttime ,
                                    @EndTime = endtime ,
                                    @MinMoney = minmoney ,
                                    @DiscountMoney = discountmoney
                            FROM    @PromotionTaskPromotionList
                            WHERE   id = ( @index_rule + 1 );
						
						--设置Type
                            SELECT  @Type = [Type]
                            FROM    Activity..tbl_CouponRules
                            WHERE   PKID = @CouponRulesId
                                    AND ParentID = 0;

						--【调用塞券PROC】 有记录并且自动发送开关为打开状态
                            DECLARE @return_value INT;
                            SELECT  @return_value = Value
                            FROM    RuntimeSwitch
                            WHERE   SwitchName = 'IsAutoSendPromotion';
                            IF ( EXISTS ( SELECT    *
                                          FROM      @Cellphones )
                                 AND ( @return_value = 1 )
                               )
                                BEGIN
						
                                    EXEC @return_value = [dbo].[PromotionCode_CreatePromotionCode_Cellphones] @Cellphones = @Cellphones,
                                        @StartDate = @StartTime,
                                        @EndDate = @EndTime, @Type = @Type,
                                        @RuleId = @CouponRulesId,
                                        @Description = @PromotionDescription,
                                        @Discount = @DiscountMoney,
                                        @MinMoney = @MinMoney,
                                        @CreateUser = @Creater,
                                        @Channels = N'JOB塞券';
							--塞券失败 回滚本次任务
                                    IF @return_value = -1
                                        BEGIN
                                            ROLLBACK TRAN;
                                            RETURN -1;
                                        END; 
                                END;
	
                            SET @index_rule = @index_rule + 1;
                        END;

                    GOTO NOEMPTY;

                    EMPTYEND:

				  --【插入历史表】
                    INSERT  INTO dbo.tbl_PromotionSingleTaskUsersHistory
                            ( PromotionTaskId ,
                              UserCellPhone
                            )
                            SELECT  @PromotionTaskId ,
                                    userphone
                            FROM    @PromotionIdlist;

				  --【删除待发送表】
                    DELETE  FROM dbo.tbl_PromotionSingleTaskUsers
                    WHERE   PromotionSingleTaskUsersId IN (
                            SELECT  PromotionSingleTaskUsersId
                            FROM    @PromotionIdlist );

				  --【设置为关闭 如果为单次任务】
                    IF ( @TaskType = 1 )
                        BEGIN
                            UPDATE  dbo.tbl_PromotionTask WITH (ROWLOCK)
                            SET     TaskStatus = 2 ,--已关闭
                                    ExecuteTime = GETDATE() , --执行时间
                                    CloseTime = GETDATE()--关闭时间
                            WHERE   PromotionTaskId = @PromotionTaskId;
                        END;
				  
                    SET @index = @index + 1;
                END;  
			   
            COMMIT TRAN;
        END TRY
        BEGIN CATCH 
            SELECT  'There was an error! ' + ERROR_MESSAGE();  
            ROLLBACK TRAN;
            RETURN -1;
        END CATCH; 
    END;


