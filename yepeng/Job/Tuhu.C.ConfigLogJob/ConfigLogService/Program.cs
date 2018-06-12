using System;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;

namespace ConfigLogService
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
			ServiceBase.Run( new ServiceBase[] 
            { 
                new HostService()
            });
#endif
        }
    }

#if DEBUG
    public class MainForm : Form
    {
        HostService _service;
        Button _startButton;
        Button _stopButton;
        Button _pauseButton;
        Button _continueButton;
        //Button _setPriceButton;

        //TextBox _eticketTextBox;

        public MainForm()
        {
            this.Load += MainForm_Load;

            this.FormClosed += MainForm_FormClosed;

            this.Text = "日志队列消费者";
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        protected void MainForm_Load(object source, EventArgs e)
        {
            _service = new HostService();

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

            //_setPriceButton = new Button();
            //_setPriceButton.Text = "同步价格";
            //_setPriceButton.Top = 120;
            //_setPriceButton.Left = 40;
            //_setPriceButton.Enabled = false;
            //this.Controls.Add(_setPriceButton);
            //_setPriceButton.Click += setPriceButton_Click;

            //_eticketTextBox = new TextBox();
            //_eticketTextBox.Top = 120;
            //_eticketTextBox.Left = 160;
            //this.Controls.Add(_eticketTextBox);
        }

        protected void MainForm_FormClosed(object source, EventArgs e)
        {
            if (_stopButton.Enabled)
                _service.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            _startButton.Enabled = false;
            _continueButton.Enabled = false;

            _service.StartIt();

            _stopButton.Enabled = true;
            _pauseButton.Enabled = true;
            //_setPriceButton.Enabled = true;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _stopButton.Enabled = false;
            _pauseButton.Enabled = false;
            _continueButton.Enabled = false;

            _service.Stop();

            _startButton.Enabled = true;
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            _startButton.Enabled = false;
            _pauseButton.Enabled = false;

            _service.PauseIt();

            _stopButton.Enabled = true;
            _continueButton.Enabled = true;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            _startButton.Enabled = false;
            _continueButton.Enabled = false;

            _service.ContinueIt();

            _stopButton.Enabled = true;
            _pauseButton.Enabled = true;
        }

        //private void setPriceButton_Click(object sender, EventArgs e)
        //{
        //    _setPriceButton.Enabled = false;
        //    _setPriceButton.Enabled = true;
        //}
    }
#endif
}
