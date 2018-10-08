using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.mappprod.account.binding.sync
    /// </summary>
    public class AlipayMappprodAccountBindingSyncRequest : IAopRequest<AlipayMappprodAccountBindingSyncResponse>
    {
        /// <summary>
        /// 扩展数据，格式：JSON字符串。预留，暂时没用
        /// </summary>
        public string ExtInfo { get; set; }

        /// <summary>
        /// 用户的商户会员号
        /// </summary>
        public string UserAccountNo { get; set; }

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
            return "alipay.mappprod.account.binding.sync";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("ext_info", this.ExtInfo);
            parameters.Add("user_account_no", this.UserAccountNo);
            return parameters;
        }

        #endregion
    }
}
