using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Service.Utility.Request;
using swc = System.Web.Configuration;
namespace Tuhu.Provisioning.Common
{
    public static class ImageUploadHelper
    {
        public static Tuple<bool, string> UpdateLoadImage(this byte[] buffer)
        {
            using (var client = new Tuhu.Service.Utility.FileUploadClient())
            {
                string dirName = swc.WebConfigurationManager.AppSettings["UploadDoMain_image"];

                var result = client.UploadImage(new ImageUploadRequest(dirName, buffer));
                result.ThrowIfException(true);
                if (result.Success)
                {
                    string imgUrl = swc.WebConfigurationManager.AppSettings["DoMain_image"] + result.Result;

                    return Tuple.Create(true, imgUrl);
                }
                else
                {
                    return Tuple.Create(false, result.Exception.ToString());
                }
            }
        }
    }
}