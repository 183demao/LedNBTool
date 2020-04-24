using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using NbIotCmd.Notify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace NbIotCmd
{
    public partial class FrmMain : Form
    {
        private WinformAsyncNotification notification = new WinformAsyncNotification();

        public FrmMain()
        {
            InitializeComponent();
            var timer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = 1000
            };

            //timer.Elapsed += this.TimerElapsed;
            //绑定事件
            notification.showEvent += showMessage;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {

            });
        }

        private void showMessage(string message)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                this.txtMessage.Text += $"{DateTime.Now} {message}{Environment.NewLine}";
            });
        }
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnStart_Click(object sender, EventArgs e)
        {
            await MQTTContext.getInstance().
                Initialize(txtServer.Text.Trim(), int.Parse(txtPort.Text.Trim()), notification);
        }

    }
}
