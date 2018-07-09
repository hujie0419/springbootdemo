using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalFlashSales
    {
        #region FlashSales
        /// <summary>
        /// 获取所有FlashSales
        /// </summary>
        public static List<FlashSales> GetAllFlashSales(SqlConnection connection)
        {
            var sql = "SELECT * FROM Marketing.dbo.tbl_FlashSales WITH (NOLOCK) ORDER BY Position,StartTime DESC";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<FlashSales>().ToList();
        }
        /// <summary>
        /// 删除FlashSales
        /// </summary>
        /// <param name="id">PKID</param>
        public static void DeleteFlashSales(SqlConnection connection, int id)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",id)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Marketing.dbo.tbl_FlashSales WHERE PKID=@PKID;DELETE FROM Marketing.dbo.tbl_FlashSalesProduct WHERE FlashSalesID=@PKID", sqlParamters);
        }
        /// <summary>
        /// 添加FlashSales
        /// </summary>
        /// <param name="advertise">FlashSales对象</param>
        public static void AddFlashSales(SqlConnection connection, FlashSales flashSales)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@EnID",flashSales.EnID),
                new SqlParameter("@StartTime",flashSales.StartTime),
                new SqlParameter("@CountDown",flashSales.CountDown),
                new SqlParameter("@Name",flashSales.Name??string.Empty),
                new SqlParameter("@PictureUrl",flashSales.PictureUrl??string.Empty),
                new SqlParameter("@Position",flashSales.Position),
                new SqlParameter("@DisplayColumn",flashSales.DisplayColumn),
                new SqlParameter("@AppValue",flashSales.AppValue??string.Empty),
                new SqlParameter("@IsBanner",flashSales.IsBanner),
                new SqlParameter("@BannerUrl",flashSales.BannerUrl??string.Empty),
                new SqlParameter("@BackgoundColor",flashSales.BackgoundColor??string.Empty),
                new SqlParameter("@TomorrowText",flashSales.TomorrowText??string.Empty),
                new SqlParameter("@Status",flashSales.Status),
                new SqlParameter("@IsTomorrowTextActive",flashSales.IsTomorrowTextActive),
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"insert into Marketing.dbo.tbl_FlashSales(EnID,StartTime,CountDown,Name,PictureUrl,Position,DisplayColumn,AppValue,IsBanner,BannerUrl,BackgoundColor,TomorrowText,Status,IsTomorrowTextActive) values
                 (@EnID,@StartTime,@CountDown,@Name,@PictureUrl,@Position,@DisplayColumn,@AppValue,@IsBanner,@BannerUrl,@BackgoundColor,@TomorrowText,@Status,@IsTomorrowTextActive)"
                , sqlParamters);
        }
        /// <summary>
        /// 修改FlashSales
        /// </summary>
        /// <param name="advertise">FlashSales对象</param>
        public static void UpdateFlashSales(SqlConnection connection, FlashSales flashSales)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",flashSales.PKID),
                new SqlParameter("@EnID",flashSales.EnID),
                new SqlParameter("@StartTime",flashSales.StartTime),
                new SqlParameter("@CountDown",flashSales.CountDown),
                new SqlParameter("@Name",flashSales.Name??string.Empty),
                new SqlParameter("@PictureUrl",flashSales.PictureUrl??string.Empty),
                new SqlParameter("@Position",flashSales.Position),
                new SqlParameter("@DisplayColumn",flashSales.DisplayColumn),
                new SqlParameter("@AppValue",flashSales.AppValue??string.Empty),
                new SqlParameter("@IsBanner",flashSales.IsBanner),
                new SqlParameter("@BannerUrl",flashSales.BannerUrl??string.Empty),
                new SqlParameter("@BackgoundColor",flashSales.BackgoundColor??string.Empty),
                new SqlParameter("@TomorrowText",flashSales.TomorrowText??string.Empty),
                new SqlParameter("@Status",flashSales.Status),
                new SqlParameter("@IsTomorrowTextActive",flashSales.IsTomorrowTextActive),
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"update Marketing.dbo.tbl_FlashSales set EnID=@EnID,StartTime=@StartTime,CountDown=@CountDown,Name=@Name
                    ,PictureUrl=@PictureUrl,Position=@Position,DisplayColumn=@DisplayColumn,AppValue=@AppValue,IsBanner=@IsBanner
                    ,BannerUrl=@BannerUrl,BackgoundColor=@BackgoundColor,TomorrowText=@TomorrowText,Status=@Status,IsTomorrowTextActive=@IsTomorrowTextActive where PKID=@PKID", sqlParamters);
        }
        /// <summary>
        /// 根据id获取FlashSales对象
        /// </summary>
        /// <param name="advertise">FlashSales对象</param>
        public static FlashSales GetFlashSalesByID(SqlConnection connection, int id)
        {
            FlashSales _FlashSales = null;
            var parameters = new[]
            {
                new SqlParameter("@PKID", id)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT TOP 1 
        PKID
      ,EnID
      ,StartTime
      ,CountDown
      ,Name
      ,PictureUrl
      ,Position
      ,DisplayColumn
      ,AppValue
      ,IsBanner
      ,BannerUrl
      ,BackgoundColor
      ,TomorrowText
      ,Status
      ,IsTomorrowTextActive
FROM Marketing.dbo.tbl_FlashSales WITH (NOLOCK) WHERE PKID=@PKID", parameters))
            {
                if (_DR.Read())
                {
                    _FlashSales = new FlashSales();
                    _FlashSales.PKID = _DR.GetTuhuValue<int>(0);
                    _FlashSales.EnID = _DR.GetTuhuString(1);
                    _FlashSales.StartTime = _DR.GetTuhuValue<System.DateTime>(2);
                    _FlashSales.CountDown = _DR.GetTuhuValue<int>(3);
                    _FlashSales.Name = _DR.GetTuhuString(4);
                    _FlashSales.PictureUrl = _DR.GetTuhuString(5);
                    _FlashSales.Position = _DR.GetTuhuValue<int>(6);
                    _FlashSales.DisplayColumn = _DR.GetTuhuValue<int>(7);
                    _FlashSales.AppValue = _DR.GetTuhuString(8);
                    _FlashSales.IsBanner = _DR.GetTuhuValue<bool>(9);
                    _FlashSales.BannerUrl = _DR.GetTuhuString(10);
                    _FlashSales.BackgoundColor = _DR.GetTuhuString(11);
                    _FlashSales.TomorrowText = _DR.GetTuhuString(12);
                    _FlashSales.Status = _DR.GetTuhuValue<byte>(13);
                    _FlashSales.IsTomorrowTextActive = _DR.GetTuhuValue<bool>(14);
                }
            }
            return _FlashSales;
        }
        /// <summary>
        /// 删除用户购买记录并重置产品数量
        /// </summary>
        public static void ResetFlashSales(SqlConnection connection, int id)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",id)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                "DELETE FROM Marketing.dbo.tbl_FlashSalesUserLog WHERE FlashSalesID=@PKID;UPDATE Marketing.dbo.tbl_FlashSalesProduct SET NumLeft=PromotionNum WHERE FlashSalesID=@PKID", sqlParamters);

        }
        #endregion
        #region FlashSalesProduct
        /// <summary>
        /// 获取模块下面的所有产品
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<FlashSalesProduct> GetProListByFlashSalesID(SqlConnection connection, int FlashSalesID)
        {
            var parameters = new[]
            {
                new SqlParameter("@FlashSalesID", FlashSalesID)
            };
            var sql = "SELECT * FROM Marketing.dbo.tbl_FlashSalesProduct WITH (NOLOCK) where FlashSalesID=@FlashSalesID ORDER BY Status desc,Position";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, parameters).ConvertTo<FlashSalesProduct>().ToList();
            //return DbHelper.ExecuteDataTable(sql, CommandType.Text, parameters).ConvertTo<FlashSalesProduct>().ToList();
        }
        public static string GetCountByFlashSalesID(SqlConnection connection, int FlashSalesID)
        {
            var sqlParameter = new SqlParameter("@FlashSalesID", FlashSalesID);
            var _Count = SqlHelper.ExecuteScalar(connection, CommandType.Text, "SELECT COUNT(1) FROM Marketing.dbo.tbl_FlashSalesProduct WITH (NOLOCK) where FlashSalesID=@FlashSalesID", sqlParameter);
            return _Count == null ? "0" : _Count.ToString();
        }
        public static void DeleteFlashSalesProduct(SqlConnection connection, int PKID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",PKID)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM Marketing.dbo.tbl_FlashSalesProduct WHERE PKID=@PKID", sqlParamters);
        }
        public static void ChangeStatus(SqlConnection connection, int PKID, byte Status)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",PKID),
                new SqlParameter("@Status",Status==0?1:0)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "update Marketing.dbo.tbl_FlashSalesProduct set Status=@Status where PKID=@PKID", sqlParamters);
        }
        public static void ChangeIsHotSale(SqlConnection connection, int PKID, bool IsHotSale)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",PKID),
                new SqlParameter("@IsHotSale",IsHotSale?false:true)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "update Marketing.dbo.tbl_FlashSalesProduct set IsHotSale=@IsHotSale where PKID=@PKID", sqlParamters);
        }
        public static void UpdateFlashSalesProduct(SqlConnection connection, FlashSalesProduct flashSalesProduct)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",flashSalesProduct.PKID),
                new SqlParameter("@PID",flashSalesProduct.PID),
                new SqlParameter("@Position",flashSalesProduct.Position),
                new SqlParameter("@PromotionPrice",flashSalesProduct.PromotionPrice),
                new SqlParameter("@MarketPrice",flashSalesProduct.MarketPrice),
                new SqlParameter("@PromotionNum",flashSalesProduct.PromotionNum),
                new SqlParameter("@MaxNum",flashSalesProduct.MaxNum),
                new SqlParameter("@NumLeft",flashSalesProduct.NumLeft),
                new SqlParameter("@ProductID",flashSalesProduct.FlashSalesProductPara.ProductID),
                new SqlParameter("@VariantID",flashSalesProduct.FlashSalesProductPara.VariantID),
                new SqlParameter("@DisplayName",flashSalesProduct.FlashSalesProductPara.DisplayName),
                new SqlParameter("@CP_Vehicle",flashSalesProduct.FlashSalesProductPara.CP_Vehicle),
                new SqlParameter("@Price",flashSalesProduct.FlashSalesProductPara.Price),
                new SqlParameter("@Description",flashSalesProduct.FlashSalesProductPara.Description),
                new SqlParameter("@DefinitionName",flashSalesProduct.FlashSalesProductPara.DefinitionName),
                new SqlParameter("@Image_filename",flashSalesProduct.FlashSalesProductPara.Image_filename),
                new SqlParameter("@Image_filename_2",flashSalesProduct.FlashSalesProductPara.Image_filename_2),
                new SqlParameter("@Image_filename_3",flashSalesProduct.FlashSalesProductPara.Image_filename_3),
                new SqlParameter("@Image_filename_4",flashSalesProduct.FlashSalesProductPara.Image_filename_4),
                new SqlParameter("@Image_filename_5",flashSalesProduct.FlashSalesProductPara.Image_filename_5),
                new SqlParameter("@Variant_Image_filename_1",flashSalesProduct.FlashSalesProductPara.Variant_Image_filename_1),
                new SqlParameter("@Variant_Image_filename_2",flashSalesProduct.FlashSalesProductPara.Variant_Image_filename_2),
                new SqlParameter("@Variant_Image_filename_3",flashSalesProduct.FlashSalesProductPara.Variant_Image_filename_3),
                new SqlParameter("@Variant_Image_filename_4",flashSalesProduct.FlashSalesProductPara.Variant_Image_filename_4)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, @"UPDATE Marketing.dbo.tbl_FlashSalesProduct SET PID=@PID,Position=@Position,PromotionPrice=@PromotionPrice,MarketPrice=@MarketPrice,PromotionNum=@PromotionNum,MaxNum=@MaxNum,NumLeft=@NumLeft,
ProductID=@ProductID,VariantID=@VariantID,DisplayName=@DisplayName,CP_Vehicle=@CP_Vehicle,Price=@Price,Description=@Description,Image_filename=@Image_filename,Image_filename_2=@Image_filename_2,Image_filename_3=@Image_filename_3,Image_filename_4=@Image_filename_4,
Image_filename_5=@Image_filename_5,Variant_Image_filename_1=@Variant_Image_filename_1,Variant_Image_filename_2=@Variant_Image_filename_2,Variant_Image_filename_3=@Variant_Image_filename_3,Variant_Image_filename_4=@Variant_Image_filename_4
WHERE PKID=@PKID", sqlParamters);
        }

        public static void AddFlashSalesProduct(SqlConnection connection, FlashSalesProduct flashSalesProduct)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",flashSalesProduct.PKID),
                new SqlParameter("@FlashSalesID",flashSalesProduct.FlashSalesID),
                new SqlParameter("@PID",flashSalesProduct.PID),
                new SqlParameter("@Position",flashSalesProduct.Position),
                new SqlParameter("@PromotionPrice",flashSalesProduct.PromotionPrice),
                new SqlParameter("@MarketPrice",flashSalesProduct.MarketPrice),
                new SqlParameter("@PromotionNum",flashSalesProduct.PromotionNum),
                new SqlParameter("@MaxNum",flashSalesProduct.MaxNum),
                new SqlParameter("@NumLeft",flashSalesProduct.NumLeft),
                new SqlParameter("@Status",flashSalesProduct.Status),
                new SqlParameter("@IsHotSale",flashSalesProduct.IsHotSale),
                new SqlParameter("@ProductID",flashSalesProduct.FlashSalesProductPara.ProductID),
                new SqlParameter("@VariantID",flashSalesProduct.FlashSalesProductPara.VariantID),
                new SqlParameter("@DisplayName",flashSalesProduct.FlashSalesProductPara.DisplayName),
                new SqlParameter("@CP_Vehicle",flashSalesProduct.FlashSalesProductPara.CP_Vehicle),
                new SqlParameter("@Price",flashSalesProduct.FlashSalesProductPara.Price),
                new SqlParameter("@Description",flashSalesProduct.FlashSalesProductPara.Description),
                new SqlParameter("@DefinitionName",flashSalesProduct.FlashSalesProductPara.DefinitionName),
                new SqlParameter("@Image_filename",flashSalesProduct.FlashSalesProductPara.Image_filename),
                new SqlParameter("@Image_filename_2",flashSalesProduct.FlashSalesProductPara.Image_filename_2),
                new SqlParameter("@Image_filename_3",flashSalesProduct.FlashSalesProductPara.Image_filename_3),
                new SqlParameter("@Image_filename_4",flashSalesProduct.FlashSalesProductPara.Image_filename_4),
                new SqlParameter("@Image_filename_5",flashSalesProduct.FlashSalesProductPara.Image_filename_5),
                new SqlParameter("@Variant_Image_filename_1",flashSalesProduct.FlashSalesProductPara.Variant_Image_filename_1),
                new SqlParameter("@Variant_Image_filename_2",flashSalesProduct.FlashSalesProductPara.Variant_Image_filename_2),
                new SqlParameter("@Variant_Image_filename_3",flashSalesProduct.FlashSalesProductPara.Variant_Image_filename_3),
                new SqlParameter("@Variant_Image_filename_4",flashSalesProduct.FlashSalesProductPara.Variant_Image_filename_4)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text,
                @"insert into Marketing.dbo.tbl_FlashSalesProduct(FlashSalesID,PID,Position,PromotionPrice,MarketPrice,PromotionNum,MaxNum,NumLeft,Status,IsHotSale,
ProductID,VariantID,DisplayName,CP_Vehicle,Price,Description,DefinitionName,Image_filename,Image_filename_2,Image_filename_3,Image_filename_4,Image_filename_5,Variant_Image_filename_1,Variant_Image_filename_2,Variant_Image_filename_3,Variant_Image_filename_4) values
                 (@FlashSalesID,@PID,@Position,@PromotionPrice,@MarketPrice,@PromotionNum,@MaxNum,@NumLeft,@Status,@IsHotSale,
@ProductID,@VariantID,@DisplayName,@CP_Vehicle,@Price,@Description,@DefinitionName,@Image_filename,@Image_filename_2,@Image_filename_3,@Image_filename_4,@Image_filename_5,@Variant_Image_filename_1,@Variant_Image_filename_2,@Variant_Image_filename_3,@Variant_Image_filename_4)"
                , sqlParamters);
        }

        public static FlashSalesProductPara GetFlashSalesProductParaByPID(SqlConnection connection, string PID)
        {
            FlashSalesProductPara _FSPP = null;
            var parameters = new[]
            {
                new SqlParameter("@PID", PID)
            };

            using (var _DR = SqlHelper.ExecuteReader(connection, CommandType.Text, @"SELECT 
        CP.ProductID ,
        CP.VariantID ,
        C.DisplayName ,
        CP.CP_Vehicle ,
        ISNULL(CP.cy_list_price, 0) AS Price ,
        C.Description ,
        CP.DefinitionName ,
        CP.Image_filename ,
        CP.Image_filename_2 ,
        CP.Image_filename_3 ,
        CP.Image_filename_4 ,
        CP.Image_filename_5 ,
        CP.Variant_Image_filename_1 ,
        CP.Variant_Image_filename_2 ,
        CP.Variant_Image_filename_3 ,
        CP.Variant_Image_filename_4
FROM    Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH ( NOLOCK )
        JOIN Tuhu_productcatalog..[CarPAR_zh-CN_Catalog] AS C WITH ( NOLOCK ) ON CP.oid = C.#Catalog_Lang_Oid
WHERE   ( CP.ProductID + '|' + CP.VariantID ) = @PID", parameters))
            {
                if (_DR.Read())
                {
                    _FSPP = new FlashSalesProductPara();
                    _FSPP.ProductID = _DR.GetTuhuString(0);
                    _FSPP.VariantID = _DR.GetTuhuString(1);
                    _FSPP.DisplayName = _DR.GetTuhuString(2);
                    _FSPP.CP_Vehicle = _DR.GetTuhuString(3);
                    _FSPP.Price = _DR.GetTuhuValue<decimal>(4);
                    _FSPP.Description = _DR.GetTuhuString(5);
                    _FSPP.DefinitionName = _DR.GetTuhuString(6);
                    _FSPP.Image_filename = _DR.GetTuhuString(7);
                    _FSPP.Image_filename_2 = _DR.GetTuhuString(8);
                    _FSPP.Image_filename_3 = _DR.GetTuhuString(9);
                    _FSPP.Image_filename_4 = _DR.GetTuhuString(10);
                    _FSPP.Image_filename_5 = _DR.GetTuhuString(11);
                    _FSPP.Variant_Image_filename_1 = _DR.GetTuhuString(12);
                    _FSPP.Variant_Image_filename_2 = _DR.GetTuhuString(13);
                    _FSPP.Variant_Image_filename_3 = _DR.GetTuhuString(14);
                    _FSPP.Variant_Image_filename_4 = _DR.GetTuhuString(15);
                }
            }
            return _FSPP;
        }
        #endregion
    }
}


