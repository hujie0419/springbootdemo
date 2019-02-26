using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalFlashSalesTwo
    {
        /// <summary>
        /// 获取所有FlashSales
        /// </summary>
        public static List<FlashSalesTwo> GetAllFlashSales(SqlConnection connection)
        {
            var sql = "SELECT * FROM Activity..tbl_FlashSale WITH (NOLOCK) ORDER BY Position,StartDateTime DESC";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<FlashSalesTwo>().ToList();
        }

        public static List<FlashSalesTwo> GetAllFlashSalesV1(SqlConnection conn, string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @" 
                            SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY UpdateDateTime DESC ) AS ROWNUMBER ,
                                                *
                                      FROM      Activity..tbl_FlashSale WITH ( NOLOCK ) 
                                      WHERE    ActiveType=0 AND  1 = 1" + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                             ";
            string sqlCount = @"SELECT COUNT(1) FROM Activity..tbl_FlashSale WITH (NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlPrams = new SqlParameter[]
            {             
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPrams).ConvertTo<FlashSalesTwo>().ToList();
        }
        /// <summary>
        /// 获取模块下面的所有产品
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<FlashSalesProductTwo> GetProListByFlashSalesID(SqlConnection connection, string activityid)
        {
            var parameters = new[]
            {
                new SqlParameter("@ActivityID", activityid)
            };
            string sql = @"
                    SELECT  A.* ,
                            B.DisplayName ,
                            B.cy_list_price
                    FROM    Activity..tbl_FlashSaleProducts A  
                            JOIN Tuhu_productcatalog..[CarPAR_zh-CN] B ON A.PID = B.PID
				            AND A.Channel IN ( 'all', 'app' )
                    WHERE   ActivityID = @ActivityID
                    ORDER BY Position";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, parameters).ConvertTo<FlashSalesProductTwo>().ToList();

        }

        //模块下面的产品更改保存
        public static int SaveProductTwo(SqlConnection connection, FlashSalesProductTwo flashsalesproducttwo)
        {
            var sqlParamters = new[]{
               new SqlParameter("@ProductName",flashsalesproducttwo.ProductName),
               new SqlParameter("@Label",flashsalesproducttwo.Label??string.Empty),
               new SqlParameter("@PKID",flashsalesproducttwo.PKID),
            };
            string sql = @" UPDATE  Activity..tbl_FlashSaleProducts
                            SET     ProductName = @ProductName ,
                                    Label = @Label
                            WHERE   PKID = @PKID";
            return SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParamters);
        }

        public static string GetCountByFlashSalesID(SqlConnection connection, string ActivityID)
        {
            var sqlParameter = new SqlParameter("@ActivityID", ActivityID);
            var _Count = SqlHelper.ExecuteScalar(connection, CommandType.Text, "SELECT COUNT(1) FROM Activity..tbl_FlashSaleProducts WITH (NOLOCK) where ActivityID=@ActivityID", sqlParameter);
            return _Count == null ? "0" : _Count.ToString();
        }

        /// <summary>
        /// 根据id获取FlashSales对象
        /// </summary>
        /// <param name="advertise">FlashSales对象</param>
        public static FlashSalesTwo GetFlashSalesByID(SqlConnection connection, string activityid)
        {
            FlashSalesTwo _FlashSales = null;
            var parameters = new[]
            {
                new SqlParameter("@ActivityID", activityid)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1* FROM Activity..tbl_FlashSale WITH (NOLOCK) WHERE ActivityID=@ActivityID", parameters))
            {
                if (_DR.Read())
                {
                    _FlashSales = new FlashSalesTwo();
                    _FlashSales.ActivityID = Convert.ToString(_DR.GetTuhuValue<System.Guid>(0));
                    _FlashSales.ActivityName = _DR.GetTuhuString(1);
                    _FlashSales.StartDateTime = _DR.GetTuhuValue<System.DateTime>(2);
                    _FlashSales.EndDateTime = _DR.GetTuhuValue<System.DateTime>(3);
                    _FlashSales.CreateDateTime = _DR.GetTuhuValue<System.DateTime>(4);
                    _FlashSales.UpdateDateTime = _DR.GetTuhuValue<System.DateTime>(5);
                    _FlashSales.Area = _DR.GetTuhuString(6);
                    _FlashSales.BannerUrlAndroid = _DR.GetTuhuString(7);
                    _FlashSales.BannerUrlIOS = _DR.GetTuhuString(8);
                    _FlashSales.AppVlueAndroid = _DR.GetTuhuString(9);
                    _FlashSales.AppVlueIOS = _DR.GetTuhuString(10);
                    _FlashSales.BackgoundColor = _DR.GetTuhuString(11);
                    _FlashSales.Position = _DR.GetTuhuValue<int>(12);
                    _FlashSales.TomorrowText = _DR.GetTuhuString(13);
                    _FlashSales.IsBannerIOS = _DR.GetTuhuValue<int>(14);
                    _FlashSales.IsBannerAndroid = _DR.GetTuhuValue<int>(15);
                    _FlashSales.ShowType = _DR.GetTuhuValue<int>(16);
                    _FlashSales.ShippType = _DR.GetTuhuValue<int>(17);
                    _FlashSales.IsTomorrowTextActive = _DR.GetTuhuValue<int>(18);
                    _FlashSales.CountDown = _DR.GetTuhuValue<int>(19);
                    _FlashSales.Status = _DR.GetTuhuValue<int>(20);
                    _FlashSales.IsNoActiveTime = _DR.GetTuhuValue<int>(24);
                    _FlashSales.EndImage = _DR.GetTuhuString(25);
                    _FlashSales.IsEndImage = _DR.GetTuhuValue<bool>(26);
                    _FlashSales.ShoppingCart = _DR.GetTuhuValue<bool>(30);
                    _FlashSales.H5Url = _DR.GetTuhuString(31);

                }
            }
            return _FlashSales;
        }

        /// <summary>
        /// 修改FlashSales
        /// </summary>
        /// <param name="advertise">FlashSales对象</param>
        public static void UpdateFlashSalesTwoV1(SqlConnection connection, FlashSalesTwo flashSales)
        {
            SqlTransaction trans = connection.BeginTransaction();
            var sqlParamters = new[]
            {
                new SqlParameter("@ActivityID",flashSales.ActivityID),
                new SqlParameter("@ActivityName",flashSales.ActivityName),
                new SqlParameter("@UpdateDateTime",System.DateTime.Now),
                new SqlParameter("@BannerUrlAndroid",flashSales.BannerUrlAndroid??string.Empty),
                new SqlParameter("@BannerUrlIOS",flashSales.BannerUrlIOS??string.Empty),
                new SqlParameter("@AppVlueAndroid",flashSales.AppVlueAndroid??string.Empty),
                new SqlParameter("@AppVlueIOS",flashSales.AppVlueIOS??string.Empty),
                new SqlParameter("@BackgoundColor",flashSales.BackgoundColor??string.Empty),
                new SqlParameter("@Position",flashSales.Position),
                new SqlParameter("@TomorrowText",flashSales.TomorrowText??string.Empty),
                new SqlParameter("@IsBannerIOS",flashSales.IsBannerIOS),
                new SqlParameter("@IsBannerAndroid",flashSales.IsBannerAndroid),
                new SqlParameter("@ShowType",flashSales.ShowType),
                new SqlParameter("@ShippType",flashSales.ShippType),
                new SqlParameter("@IsTomorrowTextActive",flashSales.IsTomorrowTextActive),
                new SqlParameter("@Status",flashSales.Status),
                new SqlParameter("@IsNoActiveTime",flashSales.IsNoActiveTime),
                new SqlParameter("@IsEndImage",flashSales.IsEndImage),
                new SqlParameter("@EndImage",flashSales.EndImage??string.Empty),
                new SqlParameter("@ShoppingCart",flashSales.ShoppingCart),
                new SqlParameter("@H5Url",flashSales.H5Url??string.Empty)
            };

            const string sqlStr = @"UPDATE  Activity.dbo.tbl_FlashSale
                                SET     ActivityName = @ActivityName ,
                                        UpdateDateTime = @UpdateDateTime ,                                   
                                        BannerUrlAndroid = @BannerUrlAndroid ,
                                        BannerUrlIOS = @BannerUrlIOS ,
                                        AppVlueAndroid = @AppVlueAndroid ,
                                        AppVlueIOS = @AppVlueIOS ,
                                        BackgoundColor = @BackgoundColor ,
                                        Position = @Position ,
                                        TomorrowText = @TomorrowText ,
                                        IsBannerIOS = @IsBannerIOS ,
                                        IsBannerAndroid = @IsBannerAndroid ,
                                        ShowType = @ShowType ,
                                        ShippType = @ShippType ,
                                        IsTomorrowTextActive = @IsTomorrowTextActive ,
                                        Status = @Status ,
                                        IsNoActiveTime = @IsNoActiveTime,
                                        IsEndImage = @IsEndImage ,
                                        EndImage = @EndImage ,
                                        ShoppingCart = @ShoppingCart ,
                                        H5Url = @H5Url   
                                WHERE   ActivityID = @ActivityID";

            try
            {
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sqlStr, sqlParamters);

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
            }
        }

        /// <summary>
        /// 修改FlashSales
        /// </summary>
        /// <param name="advertise">FlashSales对象</param>
        public static void UpdateFlashSalesTwo(SqlConnection connection, FlashSalesTwo flashSales)
        {
            SqlTransaction trans = connection.BeginTransaction();
            var sqlParamters = new[]
            {
                new SqlParameter("@ActivityID",flashSales.ActivityID),
                new SqlParameter("@ActivityName",flashSales.ActivityName),
                new SqlParameter("@UpdateDateTime",System.DateTime.Now),
                new SqlParameter("@Area",flashSales.Area??string.Empty),
                new SqlParameter("@BannerUrlAndroid",flashSales.BannerUrlAndroid??string.Empty),
                new SqlParameter("@BannerUrlIOS",flashSales.BannerUrlIOS??string.Empty),
                new SqlParameter("@AppVlueAndroid",flashSales.AppVlueAndroid??string.Empty),
                new SqlParameter("@AppVlueIOS",flashSales.AppVlueIOS??string.Empty),
                new SqlParameter("@BackgoundColor",flashSales.BackgoundColor??string.Empty),
                new SqlParameter("@Position",flashSales.Position),
                new SqlParameter("@TomorrowText",flashSales.TomorrowText??string.Empty),
                new SqlParameter("@IsBannerIOS",flashSales.IsBannerIOS),
                new SqlParameter("@IsBannerAndroid",flashSales.IsBannerAndroid),
                new SqlParameter("@ShowType",flashSales.ShowType),
                new SqlParameter("@ShippType",flashSales.ShippType),
                new SqlParameter("@IsTomorrowTextActive",flashSales.IsTomorrowTextActive),
                new SqlParameter("@Status",flashSales.Status),
                new SqlParameter("@IsNoActiveTime",flashSales.IsNoActiveTime),
                new SqlParameter("@IsEndImage",flashSales.IsEndImage),
                new SqlParameter("@EndImage",flashSales.EndImage??string.Empty)
            };
            try
            {
                //先把这个区域下面的所有状态关闭
                if (flashSales.Status == 1 && !string.IsNullOrEmpty(flashSales.Area))//说明有开启的操作  0:关闭1的:开启
                {
                    var sqlParamters2 = new[] {
                      new SqlParameter("@Area",flashSales.Area),
                      new SqlParameter("@Status",Convert.ToInt32(0)),
                    };
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, @"update Activity.dbo.tbl_FlashSale set Status=@Status where Area=@Area", sqlParamters2);//则关闭此区域下面的所有的模块
                }

                string sqlStr = @"UPDATE  Activity.dbo.tbl_FlashSale
                                SET     ActivityName = @ActivityName ,
                                        UpdateDateTime = @UpdateDateTime ,
                                        Area = @Area ,
                                        BannerUrlAndroid = @BannerUrlAndroid ,
                                        BannerUrlIOS = @BannerUrlIOS ,
                                        AppVlueAndroid = @AppVlueAndroid ,
                                        AppVlueIOS = @AppVlueIOS ,
                                        BackgoundColor = @BackgoundColor ,
                                        Position = @Position ,
                                        TomorrowText = @TomorrowText ,
                                        IsBannerIOS = @IsBannerIOS ,
                                        IsBannerAndroid = @IsBannerAndroid ,
                                        ShowType = @ShowType ,
                                        ShippType = @ShippType ,
                                        IsTomorrowTextActive = @IsTomorrowTextActive ,
                                        Status = @Status ,
                                        IsNoActiveTime = @IsNoActiveTime,
                                        IsEndImage = @IsEndImage ,
                                        EndImage = @EndImage  
                                WHERE   ActivityID = @ActivityID";
                //给限时抢购的活动表加，app用的参数
                SqlHelper.ExecuteNonQuery(trans, CommandType.Text, sqlStr, sqlParamters);

                //给手机客户端表，插入这个活动的开始时间结束时间
                if (flashSales.Area == "B")//只有B区域可以改时间
                {
                    string ioskeyvaluelenth = string.Empty;
                    string androidkeyvaluelenth = string.Empty;
                    if (flashSales.Status == 1 && flashSales.IsNoActiveTime == 1)//活动开启的时候和此活动的时间选择显示的时候
                    {
                        ioskeyvaluelenth = "{\"startdatetime\":\"" + flashSales.StartDateTime + "\",\"enddatetime\":\"" + flashSales.EndDateTime + "\" }";
                        androidkeyvaluelenth = "[{'startdatetime':\'" + flashSales.StartDateTime + "'},{'enddatetime':'" + flashSales.EndDateTime + "' }]";
                    }
                    //  int androidid = 102;
                    // int iosid = 109;
                    //if (Debugger.IsAttached)//如果是在调试模式下
                    //{
                    //    androidid = 80;
                    //    iosid = 88;
                    //}

                    //通过下面几个条件定位 出，新的限时抢购的这个记录
                    var sqlParamtersios = new[] {
                        new SqlParameter("@ActivityID", flashSales.ActivityID),
                        new SqlParameter("@keyvaluelenth", ioskeyvaluelenth),
                        new SqlParameter("@modelfloor", 2),//2楼
                        new SqlParameter("@modelname", "限时抢购专区"),//这里的“限时抢购专区”不可以改，改了，就不能定位到这条记录了
                        new SqlParameter("@Version", 1),//是新版本的
                        new SqlParameter("@apptype", 2)//ios
                    };
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, @" update  [Gungnir].[dbo].[tal_newappsetdata] set ActivityID=@ActivityID,keyvaluelenth=@keyvaluelenth where  modelfloor=@modelfloor AND modelname=@modelname AND Version=@Version AND apptype=@apptype", sqlParamtersios);

                    var sqlParamtersandroid = new[] {
                        new SqlParameter("@ActivityID", flashSales.ActivityID),
                        new SqlParameter("@keyvaluelenth", androidkeyvaluelenth),
                        new SqlParameter("@modelfloor", 2),//2楼
                        new SqlParameter("@modelname", "限时抢购专区"),
                        new SqlParameter("@Version", 1),//是新版本的
                        new SqlParameter("@apptype", 1)//安卓
                    };
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, @" update  [Gungnir].[dbo].[tal_newappsetdata] set ActivityID=@ActivityID,keyvaluelenth=@keyvaluelenth where  modelfloor=@modelfloor AND modelname=@modelname AND Version=@Version AND apptype=@apptype", sqlParamtersandroid);
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
            }
        }

        //根据楼层去查，限时抢购的这个区对应的唯一一个开启的活动，1-A,2-B,3-C,,,,,,7-G
        public static FlashSalesTwo GetActivityByModelfloor(SqlConnection connection, int showorder)
        {
            string area = string.Empty;
            //using (var table = SqlHelper.ExecuteDataTable(connection, CommandType.Text, @" SELECT TOP 7 showorder   FROM [Gungnir].[dbo].[tal_newappsetdata] where modelfloor=2   order by apptype,modelfloor,showorder,modelname"))//查询2楼的所有的排序
            //{
            //    if(table!=null)
            //    {

            //        int[] intarray = new int[table.Rows.Count ];
            //        for (int i = 0; i < table.Rows.Count; i++)
            //        {
            //            if(showorder==Convert.ToInt32( table.Rows[i][0]))
            //            {
            //                if (i == 0)
            //                    area = "A";
            //                if (i == 1)
            //                    area = "B";
            //                if (i == 2)
            //                    area = "C";
            //                if (i == 3)
            //                    area = "D";
            //                if (i == 4)
            //                    area = "E";
            //                if (i == 5)
            //                    area = "F";
            //                if (i == 6)
            //                    area = "G";
            //            }
            //        }
            //    }
            //}

            #region 数字对照区域
            if (showorder == 8)
            {
                area = "A";
            }
            if (showorder == 9)
            {
                area = "B";
            }
            if (showorder == 10)
            {
                area = "C";
            }
            if (showorder == 11)
            {
                area = "D";
            }
            if (showorder == 12)
            {
                area = "E";
            }
            if (showorder == 13)
            {
                area = "F";
            }
            if (showorder == 14)
            {
                area = "G";
            }
            if (showorder == 15)
            {
                area = "H";
            }
            if (showorder == 16)
            {
                area = "I";
            }
            if (showorder == 17)
            {
                area = "J";
            }
            if (showorder == 18)
            {
                area = "K";
            }
            if (showorder == 19)
            {
                area = "L";
            }
            if (showorder == 20)
            {
                area = "M";
            }
            if (showorder == 21)
            {
                area = "N";
            }
            #endregion
            var sqlParamters = new[]{
             new SqlParameter("@Area",area)
           };
            FlashSalesTwo flashSalesTwo = new FlashSalesTwo();
            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"select top 1* from [Activity].[dbo].[tbl_FlashSale] with(nolock) where Area=@Area and Status=1", sqlParamters))
            {
                if (_DR.Read())
                {
                    flashSalesTwo.ActivityID = Convert.ToString(_DR.GetTuhuValue<System.Guid>(0));
                    flashSalesTwo.Area = _DR.GetTuhuString(6);
                }
            }
            return flashSalesTwo;
        }

    }
}
