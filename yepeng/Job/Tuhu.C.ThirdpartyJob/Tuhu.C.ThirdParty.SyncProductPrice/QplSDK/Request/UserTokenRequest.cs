using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Qpl.Api.Response;

namespace Qpl.Api.Request
{
    public class UserTokenRequest : IQplRequest<UserTokenResponse>
    {
        public string userinfo { get; set; }
        public string ApiName
        {
            get { return "Qpl.UserInfo.GetUserToken"; }

        }

        public bool EncryptParam
        {
            get
            {
                return true;
                //throw new NotImplementedException();
            }
        }

        public HttpMethod Method
        {
            get { return HttpMethod.Post; }

        }

        public string Uri
        {
            get { return "UserInfo/GetUserToken"; }
        }

        public object GetParam() => userinfo;



        public void Validate()
        {
            //throw new NotImplementedException();
        }
    }
}
