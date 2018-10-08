using System;

namespace K.Domain
{
    public class DOprLog
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
