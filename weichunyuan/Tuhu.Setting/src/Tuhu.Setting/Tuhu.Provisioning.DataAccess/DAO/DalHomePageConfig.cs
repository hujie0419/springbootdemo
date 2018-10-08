using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalHomePageConfig
    {
        /// <summary>
        /// 根据ID获取配置信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<tal_newappsetdata_v2> GetTal_newappsetdata_v2ById(SqlConnection sqlconnection, int id)
        {
            string sql = "SELECT * FROM Gungnir..[tal_newappsetdata_V2] with(nolock) where id = @id order by apptype,showorder";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@id", Value = id }).ConvertTo<tal_newappsetdata_v2>().ToList();
        }

        /// <summary>
        /// 根据parentid获取配置信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<tal_newappsetdata_v2> GetTal_newappsetdata_v2ByParentId(SqlConnection sqlconnection, int parentid)
        {
            string sql = "SELECT * FROM Gungnir..[tal_newappsetdata_V2] with(nolock) where parentid = @parentid order by apptype,showorder";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@parentid", Value = parentid }).ConvertTo<tal_newappsetdata_v2>().ToList();
        }

        /// <summary>
        /// 获取全部配置信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <returns></returns>
        public static List<tal_newappsetdata_v2> GetALLTal_newappsetdata_v2(SqlConnection sqlconnection)
        {
            string sql = "SELECT * FROM Gungnir..[tal_newappsetdata_V2] with(nolock) order by apptype,showorder";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql).ConvertTo<tal_newappsetdata_v2>().ToList();
        }

        /// <summary>
        /// 添加大渠道
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddTal_newappsetdata_v2(SqlConnection sqlconnection, tal_newappsetdata_v2 model)
        {
            string sql = @"insert into Gungnir..[tal_newappsetdata_V2](areatype,isparent,modelname,apptype,version,showorder,showstatic,isothercity,isproduct)
                           values(@areatype,@isparent,@modelname,@apptype,@version,@showorder,@showstatic,@isothercity,@isproduct)";

            var sqlpara = new SqlParameter[]{
                new SqlParameter("@areatype",model.areatype),
                new SqlParameter("@isparent",model.isparent),
                new SqlParameter("@modelname",model.modelname),
                new SqlParameter("@apptype",model.apptype),
                new SqlParameter("@version",model.version),
                new SqlParameter("@showorder",model.showorder),
                new SqlParameter("@showstatic",model.showstatic),
                new SqlParameter("@isothercity",model.isothercity),
                new SqlParameter("@isproduct",model.isproduct)
            };

            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0 ? true : false;
        }

        /// <summary>
        /// 修改大渠道
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateTal_newappsetdata_v2(SqlConnection sqlconnection, tal_newappsetdata_v2 model)
        {
            string sql = @"
            update Gungnir..[tal_newappsetdata_V2]
            set isparent=@isparent,
            areatype=@areatype,
            modelname=@modelname,
            apptype=@apptype,
            version=@version,
            showorder=@showorder,
            showstatic=@showstatic,
            isothercity=@isothercity,
            isproduct=@isproduct
            where id=@id";

            var sqlpara = new SqlParameter[]{
                new SqlParameter("@isparent",model.isparent),
                new SqlParameter("@areatype",model.areatype),
                new SqlParameter("@modelname",model.modelname),
                new SqlParameter("@apptype",model.apptype),
                new SqlParameter("@version",model.version),
                new SqlParameter("@showorder",model.showorder),
                new SqlParameter("@showstatic",model.showstatic),
                new SqlParameter("@isothercity",model.isothercity),
                new SqlParameter("@isproduct",model.isproduct),
                new SqlParameter("@id",model.id)
            };

            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0 ? true : false;
        }

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddChidAreaData(SqlConnection sqlconnection, tal_newappsetdata_v2 model)
        {
            string sql = @"INSERT  INTO Gungnir..[tal_newappsetdata_v2]
                                ( parentid ,
                                  isparent ,
                                  version ,
                                  showorder ,
                                  modelname ,
                                  bigtitle ,
                                  smalltitle ,
                                  apptype ,
                                  areatype ,
                                  showstyle ,
                                  citycode ,
                                  districtcode ,
                                  jumph5url ,
                                  icoimgurl ,
                                  cpshowbanner ,
                                  appoperateval ,
                                  productNum ,
                                  keyvaluelenth ,
                                  youmen ,
                                  showstatic ,
                                  isothercity ,
                                  isproduct ,
                                  starttime ,
                                  overtime ,
                                  createtime ,
                                  updatetime ,
                                  IsReadChild,
		                          StartVersion,
		                          EndVersion
                                )
                        VALUES  ( @parentid ,
                                  @isparent ,
                                  @version ,
                                  @showorder ,
                                  @modelname ,
                                  @bigtitle ,
                                  @smalltitle ,
                                  @apptype ,
                                  @areatype ,
                                  @showstyle ,
                                  @citycode ,
                                  @districtcode ,
                                  @jumph5url ,
                                  @icoimgurl ,
                                  @cpshowbanner ,
                                  @appoperateval ,
                                  @productNum ,
                                  @keyvaluelenth ,
                                  @youmen ,
                                  @showstatic ,
                                  @isothercity ,
                                  @isproduct ,
                                  @starttime ,
                                  @overtime ,
                                  @createtime ,
                                  @updatetime ,
                                  @IsReadChild,
		                          @StartVersion,
		                          @EndVersion
                                )";

            var sqlpara = new SqlParameter[]{
                 new SqlParameter("@parentid",model.parentid),
                 new SqlParameter("@isparent",model.isparent),
                 new SqlParameter("@version",model.version),
                 new SqlParameter("@showorder",model.showorder),
                 new SqlParameter("@modelname",model.modelname),
                 new SqlParameter("@bigtitle",model.bigtitle),
                 new SqlParameter("@smalltitle",model.smalltitle),
                 new SqlParameter("@apptype",model.apptype),
                 new SqlParameter("@areatype",model.areatype),
                 new SqlParameter("@showstyle",model.showstyle),
                 new SqlParameter("@citycode",model.citycode),
                 new SqlParameter("@districtcode",model.districtcode),
                 new SqlParameter("@jumph5url",model.jumph5url),
                 new SqlParameter("@icoimgurl",model.icoimgurl),
                 new SqlParameter("@cpshowbanner",model.cpshowbanner),
                 new SqlParameter("@appoperateval",model.appoperateval),
                 new SqlParameter("@productNum",model.productNum),
                 new SqlParameter("@keyvaluelenth",model.keyvaluelenth),
                 new SqlParameter("@youmen",model.youmen),
                 new SqlParameter("@showstatic",model.showstatic),
                 new SqlParameter("@isothercity",model.isothercity),
                 new SqlParameter("@isproduct",model.isproduct),
                 new SqlParameter("@starttime",model.starttime),
                 new SqlParameter("@overtime",model.overtime),
                 new SqlParameter("@createtime",model.createtime),
                 new SqlParameter("@updatetime",model.updatetime),
                 new SqlParameter("@IsReadChild",model.IsReadChild),
                 new SqlParameter("@StartVersion",model.StartVersion??string.Empty),
                 new SqlParameter("@EndVersion",model.EndVersion??string.Empty)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0 ? true : false;
        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateChidAreaData(SqlConnection sqlconnection, tal_newappsetdata_v2 model)
        {
            string sql = @"UPDATE  Gungnir..[tal_newappsetdata_v2]
                            SET     parentid = @parentid ,
                                    isparent = @isparent ,
                                    version = @version ,
                                    showorder = @showorder ,
                                    modelname = @modelname ,
                                    bigtitle = @bigtitle ,
                                    smalltitle = @smalltitle ,
                                    apptype = @apptype ,
                                    areatype = @areatype ,
                                    showstyle = @showstyle ,
                                    citycode = @citycode ,
                                    districtcode = @districtcode ,
                                    jumph5url = @jumph5url ,
                                    icoimgurl = @icoimgurl ,
                                    cpshowbanner = @cpshowbanner ,
                                    appoperateval = @appoperateval ,
                                    productNum = @productNum ,
                                    keyvaluelenth = @keyvaluelenth ,
                                    youmen = @youmen ,
                                    showstatic = @showstatic ,
                                    isothercity = @isothercity ,
                                    isproduct = @isproduct ,
                                    starttime = @starttime ,
                                    overtime = @overtime ,
                                    createtime = @createtime ,
                                    updatetime = @updatetime ,
                                    IsReadChild = @IsReadChild ,
                                    StartVersion = @StartVersion ,
                                    EndVersion = @EndVersion
                            WHERE   id = @id";

            var sqlpara = new SqlParameter[]{
                 new SqlParameter("@id",model.id),
                 new SqlParameter("@parentid",model.parentid),
                 new SqlParameter("@isparent",model.isparent),
                 new SqlParameter("@version",model.version),
                 new SqlParameter("@showorder",model.showorder),
                 new SqlParameter("@modelname",model.modelname),
                 new SqlParameter("@bigtitle",model.bigtitle),
                 new SqlParameter("@smalltitle",model.smalltitle),
                 new SqlParameter("@apptype",model.apptype),
                 new SqlParameter("@areatype",model.areatype),
                 new SqlParameter("@showstyle",model.showstyle),
                 new SqlParameter("@citycode",model.citycode),
                 new SqlParameter("@districtcode",model.districtcode),
                 new SqlParameter("@jumph5url",model.jumph5url),
                 new SqlParameter("@icoimgurl",model.icoimgurl),
                 new SqlParameter("@cpshowbanner",model.cpshowbanner),
                 new SqlParameter("@appoperateval",model.appoperateval),
                 new SqlParameter("@productNum",model.productNum),
                 new SqlParameter("@keyvaluelenth",model.keyvaluelenth),
                 new SqlParameter("@youmen",model.youmen),
                 new SqlParameter("@showstatic",model.showstatic),
                 new SqlParameter("@isothercity",model.isothercity),
                 new SqlParameter("@isproduct",model.isproduct),
                 new SqlParameter("@starttime",model.starttime),
                 new SqlParameter("@overtime",model.overtime),
                 new SqlParameter("@createtime",model.createtime),
                 new SqlParameter("@updatetime",model.updatetime),
                 new SqlParameter("@IsReadChild",model.IsReadChild),
                 new SqlParameter("@StartVersion",model.StartVersion??string.Empty),
                 new SqlParameter("@EndVersion",model.EndVersion??string.Empty)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0 ? true : false;
        }

        public static int CopyNewAppSetData(SqlConnection conn, int id, int newid)
        {
            var sqlParameters = new SqlParameter[]{
                 new SqlParameter("@id",SqlDbType.Int),
                 new SqlParameter("@newid",SqlDbType.Int),
                 new SqlParameter("@ReturnValue",SqlDbType.Int)
             };
            sqlParameters[0].Value = id;
            sqlParameters[1].Value = newid;
            sqlParameters[2].Direction = ParameterDirection.ReturnValue;
            //sqlParamters[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "CopyNewAppSetData", sqlParameters);
            return (int)sqlParameters[2].Value;
        }
        /// <summary>
        /// 保存动画
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AnimationSave(SqlConnection sqlconnection, HomePagePopupAnimation model)
        {

            string sql = @"insert into Configuration..[HomePagePopupAnimationConfig](PopupConfigId,ImageUrl,MovementType,ImageWidth,ImageHeight,LeftMargin,TopMargin,ZIndex,LinkUrl,CreateDateTime,UpdateDateTime,Creator,MiniGramId,MGPageUrl)
                           values(@PopupConfigId,@ImageUrl,@MovementType,@ImageWidth,@ImageHeight,@LeftMargin,@TopMargin,@ZIndex,@LinkUrl,GETDATE(),GETDATE(),@Creator,@MiniGramId,@MGPageUrl);Select SCOPE_IDENTITY();";

            var sqlpara = new SqlParameter[]{
                new SqlParameter("@PopupConfigId",model.PopupConfigId),
                new SqlParameter("@ImageUrl",model.ImageUrl),
                new SqlParameter("@MovementType",model.MovementType),
                new SqlParameter("@ImageWidth",model.ImageWidth),
                new SqlParameter("@ImageHeight",model.ImageHeight),
                new SqlParameter("@LeftMargin",model.LeftMargin),
                new SqlParameter("@TopMargin",model.TopMargin),
                new SqlParameter("@LinkUrl",(string.IsNullOrWhiteSpace(model.LinkUrl) || string.Equals(model.LinkUrl, "null", StringComparison.InvariantCultureIgnoreCase))? null : model.LinkUrl),
                new SqlParameter("@ZIndex",model.ZIndex),
                new SqlParameter("@Creator",model.Creator),
                new SqlParameter("@MiniGramId",(string.IsNullOrWhiteSpace(model.MiniGramId) || string.Equals(model.MiniGramId, "null", StringComparison.InvariantCultureIgnoreCase))? null : model.MiniGramId),
                new SqlParameter("@MGPageUrl", (string.IsNullOrWhiteSpace(model.MGPageUrl) || string.Equals(model.MGPageUrl, "null", StringComparison.InvariantCultureIgnoreCase)) ? null : model.MGPageUrl)
            };
            int result = int.Parse(SqlHelper.ExecuteScalar(sqlconnection, CommandType.Text, sql, sqlpara).ToString());
            return result;
        }
        /// <summary>
        /// 删除动画
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AnimationDelete(SqlConnection sqlconnection, int PKID)
        {
            string sql = @"delete from Configuration..[HomePagePopupAnimationConfig] where PKId = " + PKID;
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql);
        }
        /// <summary>
        /// 更换图片
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AnimationUpdate(SqlConnection sqlconnection, HomePagePopupAnimation model)
        {
            string sql = @"UPDATE  Configuration..[HomePagePopupAnimationConfig] 
                           SET ImageUrl=@ImageUrl,
                               ImageWidth=@ImageWidth,
                               ImageHeight=@ImageHeight,
                               LeftMargin=@LeftMargin,
                               TopMargin=@TopMargin,
                               ZIndex=@ZIndex,
                               MovementType=@MovementType,
                               LinkUrl=@LinkUrl,   
                               MGPageUrl=@MGPageUrl,    
                               MiniGramId=@MiniGramId   
                               WHERE PKId = @pkid";

            var sqlpara = new SqlParameter[]{
                new SqlParameter("@ImageUrl",model.ImageUrl??string.Empty),
                new SqlParameter("@ImageWidth",model.ImageWidth),
                new SqlParameter("@ImageHeight",model.ImageHeight),
                new SqlParameter("@LeftMargin",model.LeftMargin),
                new SqlParameter("@TopMargin",model.TopMargin),
                new SqlParameter("@ZIndex",model.ZIndex),
                new SqlParameter("@MovementType",model.MovementType),
                new SqlParameter("@LinkUrl",(string.IsNullOrWhiteSpace(model.LinkUrl) || string.Equals(model.LinkUrl, "null", StringComparison.InvariantCultureIgnoreCase)) ? null : model.LinkUrl),
                new SqlParameter("@MGPageUrl",(string.IsNullOrWhiteSpace(model.MGPageUrl) || string.Equals(model.MGPageUrl, "null", StringComparison.InvariantCultureIgnoreCase)) ? null : model.MGPageUrl),
                new SqlParameter("@MiniGramId", (string.IsNullOrWhiteSpace(model.MiniGramId) || string.Equals(model.MiniGramId, "null", StringComparison.InvariantCultureIgnoreCase))? null : model.MiniGramId),
                new SqlParameter("@pkid",model.PKId)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0 ? true : false;
        }

        /// <summary>
        /// 根据animaId获取弹窗中可领券信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="animaId"></param>
        /// <returns></returns>
        public static List<CouponsInPopup> SelectCouponsOnAnimaId(SqlConnection sqlconnection, int animaId)
        {
            string sql = "SELECT * FROM Configuration.dbo.CouponInPopupConfig with(nolock) where PopupAnimationId = @animaId order by CreateDateTime";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, new SqlParameter("@animaId", animaId)).ConvertTo<CouponsInPopup>().ToList();
        }

        /// <summary>
        /// 插入弹窗中的优惠券
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertCouponInPopup(SqlConnection sqlconnection, CouponsInPopup model)
        {

            string sql = @"INSERT INTO Configuration..[CouponInPopupConfig](CouponId,PopupAnimationId,CreateDateTime,UpdateDateTime,Creator)
                           values(@CouponId,@PopupAnimationId,GETDATE(),GETDATE(), @Creator);Select SCOPE_IDENTITY();";

            var sqlpara = new SqlParameter[]{
                new SqlParameter("@CouponId",model.CouponId),
                new SqlParameter("@PopupAnimationId",model.PopupAnimationId),
                new SqlParameter("@Creator",model.Creator)
            };
            int result = int.Parse(SqlHelper.ExecuteScalar(sqlconnection, CommandType.Text, sql, sqlpara).ToString());
            return result;
        }

        /// <summary>
        /// 更换弹窗中的优惠券
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int UpdateCouponInPopup(SqlConnection sqlconnection, CouponsInPopup model)
        {
            string sql = @"UPDATE Configuration..[CouponInPopupConfig] 
                           SET CouponId=@CouponId,
                               UpdateDateTime=GETDATE()                      
                               WHERE PKId = @pkid";

            var sqlpara = new SqlParameter[]{
                new SqlParameter("@CouponId",model.CouponId??string.Empty),
                new SqlParameter("@pkid",model.PKId)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara);
        }

        /// <summary>
        /// 删除优惠券配置
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="PKId"></param>
        /// <returns></returns>
        public static int DeleteCouponInPopup(SqlConnection sqlconnection, int PKId)
        {
            string sql = @"DELETE FROM Configuration..[CouponInPopupConfig] WHERE PKId = @PKId";
            var sqlpara = new SqlParameter[]{
                new SqlParameter("@PKId", PKId)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara);
        }
    }
}
