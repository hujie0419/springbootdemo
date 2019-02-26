using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalNewAppSet
    {
        public static void Add(SqlConnection connection, NewAppSet newAppSet)
        {
            var parameters = new[]
            {
                 new SqlParameter("@apptype", newAppSet.Apptype.HasValue? (object)newAppSet.Apptype.Value : DBNull.Value),
                 new SqlParameter("@Version", newAppSet.Version.HasValue? (object)newAppSet.Version.Value : DBNull.Value),
                 new SqlParameter("@modelname", newAppSet.Modelname?? string.Empty),
                 new SqlParameter("@modelfloor", newAppSet.Modelfloor.HasValue? (object)newAppSet.Modelfloor.Value : DBNull.Value),
                 new SqlParameter("@showorder", newAppSet.Showorder.HasValue? (object)newAppSet.Showorder.Value : DBNull.Value),
                 new SqlParameter("@icoimgurl", newAppSet.Icoimgurl?? string.Empty),
                 new SqlParameter("@jumph5url", newAppSet.Jumph5url?? string.Empty),
                 new SqlParameter("@showstatic", newAppSet.Showstatic.HasValue? (object)newAppSet.Showstatic.Value : DBNull.Value),
                 new SqlParameter("@starttime", newAppSet.Starttime.HasValue? (object)newAppSet.Starttime.Value : DBNull.Value),
                 new SqlParameter("@overtime", newAppSet.Overtime.HasValue? (object)newAppSet.Overtime.Value : DBNull.Value),
                 new SqlParameter("@cpshowtype", newAppSet.Cpshowtype.HasValue? (object)newAppSet.Cpshowtype.Value : DBNull.Value),
                 new SqlParameter("@cpshowbanner", newAppSet.Cpshowbanner?? string.Empty),
                 new SqlParameter("@appoperateval", newAppSet.Appoperateval?? string.Empty),
                 new SqlParameter("@operatetypeval", newAppSet.Operatetypeval?? string.Empty),
                 new SqlParameter("@pronumberval", newAppSet.Pronumberval?? string.Empty),
                 new SqlParameter("@keyvaluelenth", newAppSet.Keyvaluelenth?? string.Empty),
                 new SqlParameter("@umengtongji", newAppSet.Umengtongji?? string.Empty),
                 new SqlParameter("@createtime", newAppSet.Createtime.HasValue? (object)newAppSet.Createtime.Value : DBNull.Value),
                 new SqlParameter("@updatetime", newAppSet.Updatetime.HasValue? (object)newAppSet.Updatetime.Value : DBNull.Value),
                 new SqlParameter("@ModelType", newAppSet.ModelType.HasValue? (object)newAppSet.ModelType.Value : DBNull.Value),
                 new SqlParameter("@ActivityID", newAppSet.ActivityID.HasValue? (object)newAppSet.ActivityID.Value : DBNull.Value)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "procInsertNewAppSet", parameters);
        }

        public static NewAppSet GetNewAppSet(SqlConnection connection, long id)
        {
            NewAppSet newAppSet = null;

            var parameters = new[]
            {
                new SqlParameter("@id", id)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "procSelectNewAppSet", parameters))
            {
                if (dataReader.Read())
                {
                    newAppSet = new NewAppSet();

                    newAppSet.Id = dataReader.GetTuhuValue<long>(0);
                    newAppSet.Apptype = dataReader.GetTuhuNullableValue<short>(1);
                    newAppSet.Version = dataReader.GetTuhuNullableValue<int>(2);
                    newAppSet.Modelname = dataReader.GetTuhuString(3);
                    newAppSet.Modelfloor = dataReader.GetTuhuNullableValue<short>(4);
                    newAppSet.Showorder = dataReader.GetTuhuNullableValue<int>(5);
                    newAppSet.Icoimgurl = dataReader.GetTuhuString(6);
                    newAppSet.Jumph5url = dataReader.GetTuhuString(7);
                    newAppSet.Showstatic = dataReader.GetTuhuNullableValue<short>(8);
                    newAppSet.Starttime = dataReader.GetTuhuNullableValue<System.DateTime>(9);
                    newAppSet.Overtime = dataReader.GetTuhuNullableValue<System.DateTime>(10);
                    newAppSet.Cpshowtype = dataReader.GetTuhuNullableValue<short>(11);
                    newAppSet.Cpshowbanner = dataReader.GetTuhuString(12);
                    newAppSet.Appoperateval = dataReader.GetTuhuString(13);
                    newAppSet.Operatetypeval = dataReader.GetTuhuString(14);
                    newAppSet.Pronumberval = dataReader.GetTuhuString(15);
                    newAppSet.Keyvaluelenth = dataReader.GetTuhuString(16);
                    newAppSet.Umengtongji = dataReader.GetTuhuString(17);
                    newAppSet.Createtime = dataReader.GetTuhuNullableValue<System.DateTime>(18);
                    newAppSet.Updatetime = dataReader.GetTuhuNullableValue<System.DateTime>(19);
                    newAppSet.ModelType = dataReader.GetTuhuNullableValue<int>(20);
                    newAppSet.ActivityID = dataReader.GetTuhuNullableValue<System.Guid>(21);
                }
            }

            return newAppSet;
        }
        public static List<NewAppSet> SelectNewAppSet(SqlConnection connection)
        {
            var newAppSetlist = new List<NewAppSet>();

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, "SELECT Id,Apptype,Modelname,Modelfloor,Jumph5url,Showstatic,Starttime,Overtime FROM [Gungnir].[dbo].[tal_newappsetdata] where apptype in(1,2) and modelfloor=2 order by apptype,modelfloor,showorder,modelname", null))
            {
                while (dataReader.Read())
                {
                    var newAppSet = new NewAppSet
                    {
                        Id = dataReader.GetTuhuValue<long>(0),
                        Apptype = dataReader.GetTuhuNullableValue<short>(1),
                        Modelname = dataReader.GetTuhuString(2),
                        Modelfloor = dataReader.GetTuhuNullableValue<short>(3),
                        Jumph5url = dataReader.GetTuhuString(4),
                        Showstatic = dataReader.GetTuhuNullableValue<short>(5),
                        Starttime = dataReader.GetTuhuNullableValue<System.DateTime>(6),
                        Overtime = dataReader.GetTuhuNullableValue<System.DateTime>(7)
                    };

                    newAppSetlist.Add(newAppSet);
                }
            }

            return newAppSetlist;
        }
    }

}
