using System;

namespace K.Domain
{
    public class MonitorOperation
    {
        public string SubjectType { get; set; }
        public string SubjectId { get; set; }
        public string ErrorMessage { get; set; }
        public string OperationUser { get; set; }
        public string OperationName { get; set; }
        public int MonitorLevel { get; set; }
        public string MonitorModule { get; set; }
        public DateTime OperationTime { get; set; }
    }
}
