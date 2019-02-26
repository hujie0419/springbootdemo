using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PointsCenterConfig
    {
        public int Id { get; set; }

        public int Sort { get; set; }

        public int Grade { get; set; }

        public bool Status { get; set; }

        public string Image { get; set; }

        public string Link { get; set; }

        public string IOSProcessValue { get; set; }

        public string AndroidProcessValue { get; set; }

        public string IOSCommunicationValue { get; set; }

        public string AndroidCommunicationValue { get; set; }

        public DateTime? CreateTime { get; set; }

        public string Description { get; set; }

    }
}
