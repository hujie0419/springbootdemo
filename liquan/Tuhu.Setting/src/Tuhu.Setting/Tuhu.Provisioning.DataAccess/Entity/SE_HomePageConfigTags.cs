using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SE_HomePageConfigTags
    {
        public int PKID { get; set; }

        public string Category { get; set; }

        public int ParentId { get; set; }

        public string PCategory { get; set; }

        public List<SE_HomePageConfigTags> Child { get; set; }
    }
}
