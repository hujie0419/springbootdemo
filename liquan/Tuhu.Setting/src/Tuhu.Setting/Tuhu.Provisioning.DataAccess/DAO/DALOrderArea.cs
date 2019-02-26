using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALOrderArea
    {

        /// <summary>
        /// 根据ID获取配置信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<OrderArea> GetOrderAreaById(SqlConnection sqlconnection, int id)
        {
            const string sql = @"SELECT  *
                                FROM    Gungnir..[OrderFinishPageSetting] WITH (NOLOCK)
                                WHERE   id = @id
                                ORDER BY apptype ,
                                        showorder";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@id", Value = id }).ConvertTo<OrderArea>().ToList();
        }

        /// <summary>
        /// 根据parentid获取配置信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<OrderArea> GetOrderAreaByParentId(SqlConnection sqlconnection, int parentid)
        {
            const string sql = @"SELECT  *
                                FROM Gungnir..[OrderFinishPageSetting] WITH(NOLOCK)
                                WHERE parentid = @parentid
                                ORDER BY apptype ,
                                        showorder";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql, new SqlParameter() { ParameterName = "@parentid", Value = parentid }).ConvertTo<OrderArea>().ToList();
        }

        /// <summary>
        /// 获取全部配置信息
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <returns></returns>
        public static List<OrderArea> GetALLOrderArea(SqlConnection sqlconnection)
        {
            const string sql = "SELECT * FROM Gungnir..[OrderFinishPageSetting] WITH(NOLOCK) ORDER BY  apptype,showorder";
            return SqlHelper.ExecuteDataTable(sqlconnection, CommandType.Text, sql).ConvertTo<OrderArea>().ToList();
        }

        /// <summary>
        /// 添加大渠道
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddOrderArea(SqlConnection sqlconnection, OrderArea model)
        {
            const string sql = @"INSERT  INTO Gungnir..[OrderFinishPageSetting]
                                    ( areatype ,
                                      isparent ,
                                      modelname ,
                                      apptype ,
                                      version ,
                                      showorder ,
                                      showstatic ,
                                      isothercity ,
                                      isproduct
                                    )
                            VALUES  ( @areatype ,
                                      @isparent ,
                                      @modelname ,
                                      @apptype ,
                                      @version ,
                                      @showorder ,
                                      @showstatic ,
                                      @isothercity ,
                                      @isproduct
                                    )";

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

            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0;
        }

        /// <summary>
        /// 修改大渠道
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateOrderArea(SqlConnection sqlconnection, OrderArea model)
        {
            const string sql = @"UPDATE Gungnir..[OrderFinishPageSetting]
                                SET isparent=@isparent,
                                areatype=@areatype,
                                modelname=@modelname,
                                apptype=@apptype,
                                version=@version,
                                showorder=@showorder,
                                showstatic=@showstatic,
                                isothercity=@isothercity,
                                isproduct=@isproduct
                                WHERE id=@id";
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

            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0;
        }

        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddChidAreaData(SqlConnection sqlconnection, OrderArea model)
        {
            const string sql = @"INSERT  INTO Gungnir..[OrderFinishPageSetting]
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
                                  IsReadChild
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
                                  @IsReadChild
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
                 new SqlParameter("@IsReadChild",model.IsReadChild)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0;
        }

        /// <summary>
        /// 修改区域
        /// </summary>
        /// <param name="sqlconnection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateChidAreaData(SqlConnection sqlconnection, OrderArea model)
        {
            const string sql = @" UPDATE  Gungnir..[OrderFinishPageSetting]
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
                                        IsReadChild = @IsReadChild
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
                 new SqlParameter("@IsReadChild",model.IsReadChild)
            };
            return SqlHelper.ExecuteNonQuery(sqlconnection, CommandType.Text, sql, sqlpara) > 0;
        }
    }
}
