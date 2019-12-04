using System;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Tuhu.C.ActivityJob
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {

            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            new MainForm().ShowDialog();
#else
			ServiceBase.Run(new ServiceBase[] 
			{ 
				new QuartzService() 
			});
#endif
        }
    }

#if DEBUG
    public class MainForm : Form
    {
        QuartzService _service;
        Button _startButton;
        Button _stopButton;
        Button _pauseButton;
        Button _continueButton;

        public MainForm()
        {
            this.Load += MainForm_Load;

            this.FormClosed += MainForm_FormClosed;

            this.Text = "作业调度服务";
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        protected void MainForm_Load(object source, EventArgs e)
        {
            _service = new QuartzService();

            _startButton = new Button();
            _startButton.Text = "开始服务";
            _startButton.Top = 40;
            _startButton.Left = 40;
            this.Controls.Add(_startButton);
            _startButton.Click += startButton_Click;

            _stopButton = new Button();
            _stopButton.Text = "停止服务";
            _stopButton.Top = 40;
            _stopButton.Left = 160;
            _stopButton.Enabled = false;
            this.Controls.Add(_stopButton);
            _stopButton.Click += stopButton_Click;

            _pauseButton = new Button();
            _pauseButton.Text = "暂停服务";
            _pauseButton.Top = 80;
            _pauseButton.Left = 40;
            _pauseButton.Enabled = false;
            this.Controls.Add(_pauseButton);
            _pauseButton.Click += pauseButton_Click;

            _continueButton = new Button();
            _continueButton.Text = "继续服务";
            _continueButton.Top = 80;
            _continueButton.Left = 160;
            _continueButton.Enabled = false;
            this.Controls.Add(_continueButton);
            _continueButton.Click += continueButton_Click;
        }

        protected void MainForm_FormClosed(object source, EventArgs e)
        {
            if (_stopButton.Enabled)
                _service.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            _startButton.Enabled = false;
            _stopButton.Enabled = true;
            _pauseButton.Enabled = true;
            _continueButton.Enabled = false;

            new Tuhu.C.ActivityJob.Job.ActivityPage.AutoPassUserActivityApplyJob().Execute(null);
            _service.StartIt();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _startButton.Enabled = true;
            _stopButton.Enabled = false;
            _pauseButton.Enabled = false;
            _continueButton.Enabled = false;

            _service.Stop();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            _startButton.Enabled = false;
            _stopButton.Enabled = true;
            _pauseButton.Enabled = false;
            _continueButton.Enabled = true;

            _service.PauseIt();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            _startButton.Enabled = false;
            _stopButton.Enabled = true;
            _pauseButton.Enabled = true;
            _continueButton.Enabled = false;
            _service.ContinueIt();
        }
    }
#endif
}
