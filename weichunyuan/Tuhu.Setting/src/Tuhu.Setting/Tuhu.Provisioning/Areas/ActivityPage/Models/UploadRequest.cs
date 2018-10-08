using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Areas.ActivityPage.Model
{
    public class UploadRequest
    {
        public string FileType { get; set; }

        public byte[] Content { get; set; }

        public string Extension { get; set; }
    }
}