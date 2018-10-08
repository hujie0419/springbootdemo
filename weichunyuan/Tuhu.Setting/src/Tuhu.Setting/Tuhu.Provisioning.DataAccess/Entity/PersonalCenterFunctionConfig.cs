using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PersonalCenterFunctionConfig
    {

        public int Id { get; set; }

        public string Icon { get; set; }

        public string Title { get; set; }

        public int Sort { get; set; }

        public bool Status { get; set; }

        public bool Highlight { get; set; }

        public string DisplayName { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// app万能跳转
        /// </summary>
        public string AppLink { get; set; }

        public string IOSStartVersions { get; set; }

        public string IOSEndVersions { get; set; }

        public string AndroidStartVersions { get; set; }

        public string AndroidEndVersions { get; set; }

    }
}
