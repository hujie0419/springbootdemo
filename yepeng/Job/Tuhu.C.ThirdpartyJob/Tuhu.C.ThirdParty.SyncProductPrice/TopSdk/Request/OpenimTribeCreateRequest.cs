using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.tribe.create
    /// </summary>
    public class OpenimTribeCreateRequest : BaseTopRequest<Top.Api.Response.OpenimTribeCreateResponse>
    {
        /// <summary>
        /// 创建群时候拉入群的成员tribe_type = 1（即为讨论组类型)时  该参数为必选tribe_type = 0 (即为普通群类型)时，改参数无效，可不填
        /// </summary>
        public string Members { get; set; }

        public List<OpenImUser> Members_ { set { this.Members = TopUtils.ObjectToJson(value); } } 

        /// <summary>
        /// 群公告
        /// </summary>
        public string Notice { get; set; }

        /// <summary>
        /// 群名称
        /// </summary>
        public string TribeName { get; set; }

        /// <summary>
        /// 群类型有两种tribe_type = 0  普通群  普通群有管理员角色，对成员加入有权限控制tribe_type = 1  讨论组  讨论组没有管理员，不能解散
        /// </summary>
        public Nullable<long> TribeType { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string User { get; set; }

        public OpenImUser User_ { set { this.User = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.tribe.create";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("members", this.Members);
            parameters.Add("notice", this.Notice);
            parameters.Add("tribe_name", this.TribeName);
            parameters.Add("tribe_type", this.TribeType);
            parameters.Add("user", this.User);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateObjectMaxListSize("members", this.Members, 1000);
            RequestValidator.ValidateRequired("notice", this.Notice);
            RequestValidator.ValidateRequired("tribe_name", this.TribeName);
            RequestValidator.ValidateRequired("tribe_type", this.TribeType);
            RequestValidator.ValidateRequired("user", this.User);
        }

        #endregion
    }
}
