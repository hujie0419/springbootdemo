//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Configuration;
//using System.Web.Mvc;
//using Tuhu.Component.Framework;
//using Tuhu.Component.Framework.Zookeeper;
//using ZooKeeperNet;

//namespace Tuhu.Provisioning.Controllers
//{
//	public class ConfigManageController : Controller
//	{
//		public ActionResult Index()
//		{
//			return View();
//		}

//		public ActionResult Config(string e)
//		{
//			var configPath = "/ConfigCenter";
//			if (WebConfigurationManager.AppSettings["EnvironmentConfigPrefix"] != null && !WebConfigurationManager.AppSettings["EnvironmentConfigPrefix"].Contains("/" + e))
//				configPath = "/" + e + configPath;

//			using (var zookeeper = new ZooKeeperClient())
//			{
//				var model = new TreeNode<string>();

//				if (zookeeper.CreatePersistentNode(configPath))
//					return View(new string[0]);
//				else
//					return View(zookeeper.GetChildren(configPath));
//			}
//		}
//	}
//}
