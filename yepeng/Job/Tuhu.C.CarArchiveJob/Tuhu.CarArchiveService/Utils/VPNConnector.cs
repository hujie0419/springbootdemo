using Common.Logging;
using DotRas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tuhu.CarArchiveService.Utils
{
    public class VPNConnector
    {
        // 系统路径 C:\windows\system32\  
        private static string WinDir = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\";
        // rasdial.exe  
        private static string RasDialFileName = "rasdial.exe";
        // VPN路径 C:\windows\system32\rasdial.exe  
        private static string VPNPROCESS = WinDir + RasDialFileName;
        // VPN地址  
        public string IPToPing { get; set; }
        // VPN名称  
        public string VPNName { get; set; }
        // VPN用户名  
        public string UserName { get; set; }
        // VPN密码  
        public string PassWord { get; set; }

        public string PresharedKey { get; set; }

        private static ILog logger = LogManager.GetLogger("VPN");

        public VPNConnector()
        {
        }

        /// <summary>  
        /// 带参构造函数  
        /// </summary>  
        /// <param name="_vpnName"></param> 
        /// <param name="_vpnIP"></param>  
        /// <param name="_userName"></param>  
        /// <param name="_passWord"></param>  
        public VPNConnector(string _vpnName, string _vpnIP, string _userName, string _passWord, string _presharedKey)
        {
            this.IPToPing = _vpnIP;
            this.VPNName = _vpnName;
            this.UserName = _userName;
            this.PassWord = _passWord;
            this.PresharedKey = _presharedKey;
        }

        public bool Connect()
        {
            int tryCount = 3;
            while (tryCount > 0)
            {
                var names = GetCurrentConnectingVPNNames();
                if (names.Contains(this.VPNName))
                {
                    return true;
                }
                else
                {
                    this.TryConnectVPN();

                    int waitCount = 3;
                    while (waitCount > 0)
                    {
                        Thread.Sleep(5000);
                        names = GetCurrentConnectingVPNNames();
                        if (names.Contains(this.VPNName))
                        {
                            return true;
                        }
                        waitCount--;
                    }
                }

                this.TryDisConnectVPN();
                Thread.Sleep(5000);
                tryCount--;
            }

            return false;
        }

        public bool Disconnect()
        {
            int tryCount = 3;
            while (tryCount > 0)
            {
                var names = GetCurrentConnectingVPNNames();
                if (!names.Contains(this.VPNName))
                {
                    return true;
                }
                else
                {
                    this.TryDisConnectVPN();

                    int waitCount = 3;
                    while (waitCount > 0)
                    {
                        Thread.Sleep(5000);
                        names = GetCurrentConnectingVPNNames();
                        if (!names.Contains(this.VPNName))
                        {
                            return true;
                        }
                        waitCount--;
                    }
                }

                tryCount--;
            }

            return false;
        }

        /// <summary>  
        /// 尝试连接VPN(默认VPN)  
        /// </summary>  
        /// <returns></returns>  
        public void TryConnectVPN()
        {
            this.TryConnectVPN(this.VPNName, this.UserName, this.PassWord);
        }

        /// <summary>  
        /// 尝试断开连接(默认VPN)  
        /// </summary>  
        /// <returns></returns>  
        public void TryDisConnectVPN()
        {
            this.TryDisConnectVPN(this.VPNName);
        }
        /// <summary>  
        /// 尝试删除连接(默认VPN)  
        /// </summary>  
        /// <returns></returns>  
        public void TryDeleteVPN()
        {
            this.TryDeleteVPN(this.VPNName);
        }
        /// <summary>  
        /// 尝试连接VPN(指定VPN名称，用户名，密码)  
        /// </summary>  
        /// <returns></returns>  
        public void TryConnectVPN(string connVpnName, string connUserName, string connPassWord)
        {
            try
            {
                string args = string.Format("{0} {1} \"{2}\"", connVpnName, connUserName, connPassWord);
                Process proIP = new Process();
                proIP.StartInfo.FileName = "cmd.exe ";
                proIP.StartInfo.UseShellExecute = false;
                proIP.StartInfo.RedirectStandardInput = true;
                proIP.StartInfo.RedirectStandardOutput = true;
                proIP.StartInfo.RedirectStandardError = true;
                proIP.StartInfo.CreateNoWindow = true;//不显示cmd窗口  
                proIP.Start();
                proIP.StandardInput.WriteLine($"{RasDialFileName} {args}");
                proIP.StandardInput.WriteLine("exit");
                // 命令行运行结果  
                string strResult = proIP.StandardOutput.ReadToEnd();
                logger.Info(strResult);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        /// <summary>  
        /// 尝试断开VPN(指定VPN名称)  
        /// </summary>  
        /// <returns></returns>  
        public void TryDisConnectVPN(string disConnVpnName)
        {
            try
            {
                string args = string.Format(@"{0} /d", disConnVpnName);
                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);
                myProcess.CreateNoWindow = true;
                myProcess.UseShellExecute = false;
                Process.Start(myProcess);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        /// <summary>  
        /// 创建或更新一个VPN连接
        /// </summary>  
        public void CreateOrUpdateVPN()
        {
            RasDialer dialer = new RasDialer();
            RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
            allUsersPhoneBook.Open();
            // 如果已经该名称的VPN已经存在，则更新这个VPN服务器地址  
            if (!allUsersPhoneBook.Entries.Contains(this.VPNName))
            {
                var devices = RasDevice.GetDevices();
                var device = devices.First(o => o.Name.Contains("(L2TP)") && o.DeviceType == RasDeviceType.Vpn);
                RasEntry entry = RasEntry.CreateVpnEntry(this.VPNName, this.IPToPing, RasVpnStrategy.L2tpOnly, device);
                entry.Options.UsePreSharedKey = true;
                entry.Options.UseLogOnCredentials = true;
                entry.Options.RemoteDefaultGateway = false;
                allUsersPhoneBook.Entries.Add(entry);

                entry.UpdateCredentials(RasPreSharedKey.Client, PresharedKey);
                dialer.EntryName = this.VPNName;
                dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);

                logger.Info("VPN创建成功!");
            }
            else
            {
                logger.Info("VPN已存在!");
            }
        }
        /// <summary>  
        /// 删除指定名称的VPN  
        /// 如果VPN正在运行，一样会在电话本里删除，但是不会断开连接，所以，最好是先断开连接，再进行删除操作  
        /// </summary>  
        /// <param name="delVpnName"></param>  
        public void TryDeleteVPN(string delVpnName)
        {
            RasDialer dialer = new RasDialer();
            RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
            allUsersPhoneBook.Open();
            if (allUsersPhoneBook.Entries.Contains(delVpnName))
            {
                allUsersPhoneBook.Entries.Remove(delVpnName);
            }
        }
        /// <summary>  
        /// 获取当前正在连接中的VPN名称  
        /// </summary>  
        public List<string> GetCurrentConnectingVPNNames()
        {
            List<string> ConnectingVPNList = new List<string>();
            Process proIP = new Process();
            proIP.StartInfo.FileName = "cmd.exe ";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;//不显示cmd窗口  
            proIP.Start();
            proIP.StandardInput.WriteLine(RasDialFileName);
            proIP.StandardInput.WriteLine("exit");
            // 命令行运行结果  
            string strResult = proIP.StandardOutput.ReadToEnd();
            proIP.Close();

            var list = strResult.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            bool start = false;
            bool end = false;

            foreach (var item in list)
            {
                if (item.StartsWith("Command completed successfully") || item.StartsWith("命令已完成"))
                {
                    end = true;
                    break;
                }

                if (start && !end)
                {
                    ConnectingVPNList.Add(item.Replace("\r", ""));
                }

                if (string.Equals(item, "Connected to") || string.Equals(item, "已连接"))
                {
                    start = true;
                }
            }

            logger.Info(string.Join(",", list));
            // 没有正在连接的VPN，则直接返回一个空List<string>  
            logger.Info(string.Join(",", ConnectingVPNList) + ";" + strResult);

            return ConnectingVPNList;
        }
    }
}
