using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace Tuhu.MMS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            List<string> startup_css = new List<string>();
            List<string> startup_js = new List<string>();
            List<string> startup_other = new List<string>();
            var basePath = $"{System.AppDomain.CurrentDomain.BaseDirectory}TuhuUI\\";
            DirectoryInfo directory = new DirectoryInfo(basePath);
            foreach (var childDirectory in directory.GetDirectories())
            {
                var files = childDirectory.GetFiles();
                foreach (var file in files)
                {
                    string filename = file.Name;
                    switch (childDirectory.Name)
                    {
                        case "UIcss":
                            if (filename.EndsWith("css"))
                            {
                                startup_css.Add($"~/TuhuUI/{childDirectory.Name}/{filename}");
                            }
                            break;
                        case "UIjs":
                            if (filename.EndsWith("js"))
                            {
                                startup_js.Add($"~/TuhuUI/{childDirectory.Name}/{filename}");
                            }
                            break;
                    }
                }
            }

            bundles.Add(new StyleBundle("~/bundles/startup_css").Include(startup_css.ToArray()));
            bundles.Add(new ScriptBundle("~/bundles/startup_js").Include(startup_js.ToArray()));
        }
    }
}
