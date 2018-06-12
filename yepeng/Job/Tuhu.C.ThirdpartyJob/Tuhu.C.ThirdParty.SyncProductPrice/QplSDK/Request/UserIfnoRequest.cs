using Qpl.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Request
{
    public class UserIfnoRequest : IQplRequest<UserInfoResponse>
    {

        public string userinfo { get; set; }
        public string ApiName
        {
            get { return "Qpl.UserInfo.CreateUserInfo"; }

        }

        public bool EncryptParam
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public HttpMethod Method
        {
            get { return HttpMethod.Post; }

        }

        public string Uri
        {
            get { return "UserInfo/CreateUserInfo"; }
        }

        public object GetParam() => userinfo;



        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
