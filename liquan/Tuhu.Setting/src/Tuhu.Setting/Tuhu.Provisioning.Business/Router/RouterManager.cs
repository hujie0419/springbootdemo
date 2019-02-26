using System;
using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.DAO.Router;
using Tuhu.Provisioning.DataAccess.Entity.Router;

namespace Tuhu.Provisioning.Business.Router
{
    public class RouterManager
    {
        public IEnumerable<RouterMainLink> GetMainLinkList(int linkKind)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetMainList(connection,linkKind);
            }
        }

        public RouterMainLink GetMainLink(string routerMainLinkDiscription,int linkKind)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetMain(connection, routerMainLinkDiscription, linkKind);
            }
        }

        public RouterLink GetParameterList(string routerMainLinkDiscription,int linkKind)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                return dal.GetParameterList(connection, routerMainLinkDiscription,linkKind);
            }
        }

        public RouterLink GetParameterState(string routerMainLinkDiscription, string url,int linkKind)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                var parameterList = dal.GetParameterList(connection, routerMainLinkDiscription, linkKind);

                //foreach (String i in array)
                {
                    foreach (var j in parameterList.RouterParameterList)
                        if (url.Equals(j.Content) && j.Kind == 1)
                            j.State = 1;
                        else if (j.Kind == 2)
                            j.Url = url;
                }
                return parameterList;
            }
        }

        public RouterLink GetParameterStateList(string routerMainLinkDiscription, List<string> list,int linkKind)
        {
            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                var parameterList = dal.GetParameterList(connection, routerMainLinkDiscription, linkKind);

                foreach (var i in list)
                foreach (var j in parameterList.RouterParameterList)
                    if (i.Equals(j.Content) && j.Kind == 1)
                        j.State = 1;
                return parameterList;
            }
        }


        public object GetParameterForOther(string routerMainLinkDiscription, string linkUrl, int linkKind)
        {
            var array = linkUrl.Split('?');
            var list = new List<string>();
            if (linkKind == 1)
            {
                switch (array[0])
                {
                    case "/tire":
                        list.Add(array[0]);
                        list.Add(linkUrl.Substring(linkUrl.IndexOf("=", StringComparison.Ordinal) + 1));
                        return list;
                    case "/searchResult":
                        var parameter = array[1].Split('=');
                        if (parameter[0].Equals("keyword"))
                            list.Add("/search");
                        else list.Add(array[0]);
                        list.Add(linkUrl.Substring(linkUrl.IndexOf("=", StringComparison.Ordinal) + 1));
                        return list;
                    case "/search":
                        if (array.Length > 1)
                        {
                            list.Add(array[0]);
                            try
                            {
                                list.Add(array[1].Split('=')[1]);

                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                            return list;
                        }
                        break;
                }
            }
            else if (linkKind == 2)
            {
                if (array[0] == "/pages/search/search"|| array[0] == "/pages/tire_list/tire_list")
                {
                    list.Add(array[0]);
                    list.Add(linkUrl.Substring(linkUrl.IndexOf("=", StringComparison.Ordinal) + 1));
                    return list;
                }
            }


            using (var connection = ProcessConnection.OpenGungnirReadOnly)
            {
                var dal = new DalRouter();
                var link = dal.GetParameterList(connection, routerMainLinkDiscription, linkKind);

                //foreach (String i in array)
                {
                    foreach (var j in link.RouterParameterList)
                        if (linkUrl == j.Content && j.Kind == 1)
                            j.State = 1;
                }
                return link;
            }
        }
    }
}