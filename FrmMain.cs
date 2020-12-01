using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Formatter;
using NbIotCmd.Notify;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private static IScheduler _scheduler = null;
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
            UPLogger.ShowEvent += showMessage;
            btnStop.Enabled = false;
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
            try
            {
                await MQTTContext.getInstance().
                    Initialize(txtServer.Text.Trim(), int.Parse(txtPort.Text.Trim()), notification);

                if (_scheduler == null)
                {
                    _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                    //任务、触发器执行配置
                    XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
                    Stream s = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\Quartz.config").BaseStream;
                    await processor.ProcessStream(s, null);
                    await processor.ScheduleJobs(_scheduler);
                    await _scheduler.Start();
                }
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
            catch (Exception ex)
            {
                showMessage(ex.Message);
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnStop_Click(object sender, EventArgs e)
        {
            await MQTTContext.getInstance().Close();
            if (_scheduler != null)
            {
                await _scheduler.Shutdown(false);
                _scheduler = null;
            }
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                if (!btnStart.Enabled)//判断是否在运行中
                {
                    if (!MQTTContext.getInstance().Connected.Value)
                    {
                        btnStart_Click(null, null);//重试连接
                    }
                }
            });
        }
    }
}
