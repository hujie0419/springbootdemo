using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ServiceTypeSetting
    {
        public int Id { get; set; }

        public string ServiceType { get; set; }

        public string ServiceTypeName { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string CatalogName { get; set; }
    }
 
}
