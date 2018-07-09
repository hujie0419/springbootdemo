using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public  class DALPayment
    {
        public static dynamic GetPayment_way()
        {
            return SqlAdapter.Create("select top 1 *  from  [Gungnir].[dbo].[Payment_way] where id=1", System.Data.CommandType.Text, "Gungnir_AlwaysOnRead")
                    .ExecuteModel();
        }

        public static int UpdateState(int state)
        {
            return SqlAdapter.Create("update [Gungnir].[dbo].[Payment_way]  set  State=@State  where id=1", System.Data.CommandType.Text, "Gungnir")
                   .Par("@State", state, SqlDbType.Int)
                   .ExecuteNonQuery();
        }
    }
}
