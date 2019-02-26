#region Generate Code
/*
* The code is generated automically by codesmith. Please do NOT change any code.
* Generate time：2014/8/29 星期五 12:14:18
*/
#endregion

using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
	///<summary>
	/// The entity class for DB table tbl_OprLog.
	///</summary>
	public class OprLog
	{
		public int PKID { get; set; }
        public string Author { get; set; }
        public string EmployeeName { get; set; }
		public string ObjectType { get; set; }
		public int ObjectID { get; set; }
		public string BeforeValue { get; set; }
		public string AfterValue { get; set; }
		public DateTime? ChangeDatetime { get; set; }
		public string IPAddress { get; set; }
		public string HostName { get; set; }
		public string Operation { get; set; }
        public string InstallType { get; set; }
        public string LogType { get; set; }
	}
}