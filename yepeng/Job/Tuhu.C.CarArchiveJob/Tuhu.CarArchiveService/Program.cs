using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Tuhu.CarArchiveService
{
    public class Program
    {
        static void Main(string[] args)
        {
            MainService service = new MainService();
            HostFactory.Run(x =>
            {
                x.Service<MainService>(s =>
                {
                    s.ConstructUsing(name => service);
                    s.WhenStarted(tc => tc.StartIt());
                    s.WhenStopped(tc => tc.StopIt());
                    s.WhenPaused(tc => tc.PauseIt());
                    s.WhenContinued(tc => tc.ContinueIt());
                });
                x.RunAsLocalSystem();
                x.EnablePauseAndContinue();
            });
        }
    }
}
