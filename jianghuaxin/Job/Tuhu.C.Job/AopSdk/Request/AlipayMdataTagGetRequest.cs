using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.mdata.tag.get
    /// </summary>
    public class AlipayMdataTagGetRequest : IAopRequest<AlipayMdataTagGetResponse>
    {
        /// <summary>
        /// 所需标签列表, 以","分割; 如果列表为空, 则返回值为空.
        /// </summary>
        public List<String> RequiredTags { get; set; }

        /// <summary>
        /// 用户的支付宝Id
        /// </summary>
        public string UserId { get; set; }

        #region IAopRequest Members
		private string terminalType;
		private string terminalInfo;
        private string prodCode;

		public void SetTerminalType(String terminalType){
			this.terminalType=terminalType;
		}

    	public string GetTerminalType(){
    		return this.terminalType;
    	}

    	public void SetTerminalInfo(String terminalInfo){
    		this.terminalInfo=terminalInfo;
    	}

    	public string GetTerminalInfo(){
    		return this.terminalInfo;
    	}

        public void SetProdCode(String prodCode){
            this.prodCode=prodCode;
        }

        public string GetProdCode(){
            return this.prodCode;
        }

        public string GetApiName()
        {
            return "alipay.mdata.tag.get";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("required_tags", this.RequiredTags);
            parameters.Add("user_id", this.UserId);
            return parameters;
        }

        #endregion
    }
}
