using System.Collections.Generic;

namespace Tuhu.Provisioning.Models.Push
{
    public class TagModel
    {
        public string name { get; set; }

        public string key { get; set; }

        public bool open { get; set; } 

        public List<TagModel> children { get; set; } = new List<TagModel>();
    }
}
