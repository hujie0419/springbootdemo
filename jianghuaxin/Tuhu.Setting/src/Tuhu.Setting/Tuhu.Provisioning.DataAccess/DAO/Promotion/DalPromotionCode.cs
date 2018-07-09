using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Tuhu.Component.Common;
namespace Tuhu.Provisioning.DataAccess.DAO
{
	public static class DalPromotionCode
	{
		public static int InsertPromotionCode(SqlDbHelper dbhelper, PromotionCode PC)
		{
			using (var cmd = new SqlCommand("[Gungnir]..[PromotionCode_CreatePromotionCode]"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@UserId",PC.UserID);
				cmd.Parameters.AddWithValue("@StartTime",PC.StartTime);
				cmd.Parameters.AddWithValue("@EndTime",PC.EndTime);
				cmd.Parameters.AddWithValue("@OrderId",PC.OrderID);
				cmd.Parameters.AddWithValue("@Status",PC.Status);
				cmd.Parameters.AddWithValue("@Type",PC.Type);
				cmd.Parameters.AddWithValue("@Description",PC.Description);
				cmd.Parameters.AddWithValue("@Discount",PC.Discount);
				cmd.Parameters.AddWithValue("@MinMoney",PC.MinMoney);
				cmd.Parameters.AddWithValue("@RuleID", PC.RuleID);
				cmd.Parameters.AddWithValue("@Code", PC.Code);
				return dbhelper.ExecuteNonQuery(cmd);
			}
		}
	}
}
