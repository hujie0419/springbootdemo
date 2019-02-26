using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.PromotionCodeManagerment
{
	public interface IPromotionCodeManager
	{
		List<BizPromotionCode> SelectPromotionCodesByUserId(string userId);
        

    }
}
