using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Identity;

namespace Tuhu.MMS.Web.Controllers
{
    public class BaseController : Controller
    {
        private IIdentifier _currentUser = ThreadIdentity.Operator;

        public IIdentifier CurrentUser
        {
            get { return _currentUser; }
        }
    }
}