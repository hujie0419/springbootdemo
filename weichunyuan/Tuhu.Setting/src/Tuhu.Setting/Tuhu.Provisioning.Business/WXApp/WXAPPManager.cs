using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Utility.Request;
using Tuhu.Nosql;
using System.Net.Http;
using Tuhu.Service.Member.Models;

namespace Tuhu.Provisioning.Business.WXApp
{
   public class WXAPPManager
    {
        public string GetAccess_token(int paltform=3)
        {
            string appId = ConfigurationManager.AppSettings["wxappId"].ToString();
            string secret = ConfigurationManager.AppSettings["wxappsecret"].ToString();

            var client = HttpWebRequest.Create("https://wx.tuhu.cn/Packet/Test?platform="+paltform);
            client.Headers.Add("access_token", "access_token");
            var response = client.GetResponse();
            var stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            response.Dispose();
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenModel>(result);
            return model.Access_token;
        }

        public void Remove_token()
        {
            using (var client = CacheHelper.CreateCacheClient("GetAccess_token"))
            {
                client.Remove("token");
            }
        }

        public byte[] GetQcode(QCodeEntity entity,string user)
        {
            string uri = string.Empty;
            StringBuilder parameters = new StringBuilder();
            if (entity.Type == 1)
            {
                uri = "https://api.weixin.qq.com/wxa/getwxacode?access_token=" + GetAccess_token(entity.WXAPPType);
                //  parameters.Append(string.Format("path=\"{0}\"&width={1}&auto_color={2}&line_color={3}", entity.path, entity.width, entity.auto_color, string.Format("{\"r\":\"{0}\",\"g\":\"{1}\",\"b\":\"{2}\"}",entity.line_color.r,entity.line_color.g,entity.line_color.b)));
                parameters.Append(JsonConvert.SerializeObject(new
                {
                    path=entity.path,
                    width=entity.width,
                    auto_color=entity.auto_color,
                    line_color=new {
                        r=entity.line_color.r,
                        g=entity.line_color.g,
                        b=entity.line_color.b
                    }
                }));
            }
            else
            {
                uri = "https://api.weixin.qq.com/cgi-bin/wxaapp/createwxaqrcode?access_token=" + GetAccess_token();
                //   parameters.Append(string.Format("path=\"{0}\"&width={1}",entity.path,entity.width));
                parameters.Append(JsonConvert.SerializeObject(new {
                    path = entity.path,
                    width = entity.width
                }));
            }

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(parameters.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            WebResponse response = request.GetResponse() as WebResponse;
            Stream result = response.GetResponseStream();

            if (!response.ContentType.Contains("image"))
                throw new Exception(new StreamReader(result).ReadToEnd());

            byte[] bytes = default(byte[]);
            StreamReader reader = new StreamReader(result);

            using (var memstream = new MemoryStream())
            {
                reader.BaseStream.CopyTo(memstream);
                bytes = memstream.ToArray();
            }
            try
            {
                string url = ImageUploadToAli(bytes);
                RepositoryManager repository = new RepositoryManager();
                Tuhu.Provisioning.DataAccess.Mapping.QcodeRecordEntity qcodeEntity = new DataAccess.Mapping.QcodeRecordEntity()
                {
                    B = entity.line_color.b,
                    CreateDatetime = DateTime.Now,
                    CreateUser = user,
                    G = entity.line_color.g,
                    IsColor = entity.auto_color,
                    Path = entity.path,
                    QcodeType = entity.Type,
                    R = entity.line_color.r,
                    Uri = url,
                    Width = entity.width
                };
                if (qcodeEntity.QcodeType == 2)
                {
                    qcodeEntity.IsColor = null;
                    qcodeEntity.R = null;
                    qcodeEntity.G = null;
                    qcodeEntity.B = null;
                }
                repository.Add<Tuhu.Provisioning.DataAccess.Mapping.QcodeRecordEntity>(qcodeEntity);


            }
            catch (Exception em)
            {

            }
            return bytes;
        }


        private string ImageUploadToAli(byte[] buffer)
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            string _ImgGuid = Guid.NewGuid().ToString();
            Exception ex = null;
            if (buffer.Count() > 0)
            {
                try
                {
                    using (var client = new Tuhu.Service.Utility.FileUploadClient())
                    {
                        string dirName = System.Web.Configuration.WebConfigurationManager.AppSettings["UploadDoMain_image"];
                        var result = client.UploadImage(new ImageUploadRequest(dirName, buffer));
                        result.ThrowIfException(true);
                        if (result.Success)
                        {
                            _BImage = result.Result;
                            //_SImage= ImageHelper.GetImageUrl(result.Result, 100);
                        }
                    }
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            string imgUrl = WebConfigurationManager.AppSettings["DoMain_image"] + _BImage;

            return imgUrl;
        }

    }
}
