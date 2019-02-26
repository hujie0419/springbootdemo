using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FriendLink
    {
        public int Fid { get; set; }

        public string FriendLinkName { get; set; }

        public string Link { get; set; }

        public int? Position { get; set; }
    }
}
