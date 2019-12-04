using Com.Ctrip.Framework.Apollo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Server
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            return CreateWebHostBuilder(args).Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder<Startup>(args)
                .ConfigureAppConfiguration(builder => builder
                    .AddApollo(builder.Build().GetSection("apollo"))
                    .AddNamespace("RD.SharedConfiguration")
                    .AddDefault())
                .UseSentry();
#if NETCOREAPP2_2
        private const string AspNetCoreModuleDll = "aspnetcorev2_inprocess.dll";

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport(AspNetCoreModuleDll, EntryPoint = "http_get_application_properties")]
        private static extern int HttpGetApplicationProperties(out IisConfigurationData iiConfigData);

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct IisConfigurationData
        {
            private readonly IntPtr NativeApplication;
            [MarshalAs(UnmanagedType.BStr)]
            public readonly string FullApplicationPath;
            [MarshalAs(UnmanagedType.BStr)]
            private readonly string VirtualApplicationPath;
            private readonly bool WindowsAuthEnabled;
            private readonly bool BasicAuthEnabled;
            private readonly bool AnonymousAuthEnable;
        }

        static Program()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            try
            {
                if (GetModuleHandle(AspNetCoreModuleDll) == IntPtr.Zero || HttpGetApplicationProperties(out var configurationData) != 0)
                    return;

                Environment.CurrentDirectory = configurationData.FullApplicationPath;
            }
            catch
            {
                // ignore
            }
        }
#endif
    }
}
