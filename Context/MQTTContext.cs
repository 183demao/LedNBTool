using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using NbIotCmd.Context;
using NbIotCmd.Helper;
using NbIotCmd.Notify;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NbIotCmd
{
    public class MQTTContext
    {
        private static readonly MQTTContext instance = new MQTTContext();
        private WinformAsyncNotification notification;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MQTTContext()
        { }

        public static MQTTContext getInstance()
        {
            return instance;
        }
        public bool? Connected
        {
            get
            {
                return MqttClient?.IsConnected;
            }
        }

        private static IMqttClient MqttClient;
        /// <summary>
        /// 初始化
        /// </summary>
        public async Task Initialize(string server, int port, WinformAsyncNotification notification)
        {
            Console.WriteLine(MqttClient == null ? "是" : "否");
            if (MqttClient != null && MqttClient.IsConnected) return;

            var mqttFactory = new MqttFactory();

            var tlsOptions = new MqttClientTlsOptions
            {
                UseTls = false,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
                AllowUntrustedCertificates = true
            };

            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString().ToUpper())//客户端标识
                .WithTcpServer(server, port)//服务器地地址及端口号
                .WithCredentials("admin", "public")//用户名密码
                .WithCleanSession(true)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(600))
                .WithCommunicationTimeout(TimeSpan.FromSeconds(50))
                .Build();

            //var  options = new MqttClientOptions
            // {
            //     ClientId = Guid.NewGuid().ToString().ToUpper(),
            //     ProtocolVersion = MqttProtocolVersion.V500,
            //     ChannelOptions = new MqttClientTcpOptions
            //     {
            //         Server = server,
            //         Port = port,
            //         TlsOptions = tlsOptions
            //     },
            //     Credentials = new MqttClientCredentials
            //     {
            //         Username = "admin",
            //         Password = Encoding.UTF8.GetBytes("public"),
            //     },
            // };

            //if (options.ChannelOptions == null)
            //{
            //    throw new InvalidOperationException();
            //}

            //options.CleanSession = true;
            //options.KeepAlivePeriod = TimeSpan.FromSeconds(15);
            MqttClient = mqttFactory.CreateMqttClient();
            MqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnMqttConnectedAsync);
            MqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnMqttDisconnected);
            MqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnMqttMessageReceived);
            //注册通知接口
            this.notification = notification;
            var result = await MqttClient.ConnectAsync(options);

            if (result.ResultCode == MqttClientConnectResultCode.Success)
            {
                notification.Show("连接成功...");
            }

        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <returns></returns>
        public async Task<MqttClientSubscribeResult> Subscribe(List<string> topics)
        {
            var subTopics = from topic in topics
                            select new TopicFilter
                            {
                                Topic = topic
                            };
            var xxx = subTopics.ToArray();
            return await MqttClient.SubscribeAsync(xxx);
        }
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="publishData"></param>
        /// <returns></returns>
        public async Task Publish(Dictionary<string, List<byte[]>> publishData)
        {
            var values = from pdata in publishData
                         let b = pdata.Value
                         from payload in b
                         select new MqttApplicationMessage
                         {
                             Topic = pdata.Key,
                             Payload = payload
                         };

            await MqttClient.PublishAsync(values);
        }
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="publishData"></param>
        /// <returns></returns>
        public async Task<MqttClientPublishResult> Publish(string topic, string payload)
        {

            return await MqttClient.PublishAsync(topic, payload, MqttQualityOfServiceLevel.AtMostOnce);
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task OnMqttMessageReceived(MqttApplicationMessageReceivedEventArgs obj)
        {
            var payload = obj.ApplicationMessage.Payload;
            var topic = obj.ApplicationMessage.Topic;
            //byte[] hexbytes = new byte[] {
            //    0x95,
            //    0x03,
            //    0x0E,
            //    0x56,0x78,
            //    0x01,
            //    0x00,0x88,
            //    0x00,0x1F,0x08,0x07,0x06,0x05,0x04,0x03,0x02,0x01,
            //    0x00,0x00,0x21,0x04,0x00,0x00,0x0E,0x01,0x00,0x20,
            //    0x04,0x00,0x00,0x0F,0x01,0x00,0x0C,0x04,0x00,0x05,
            //    0x09,0x10,0x00,0x0C,0x04,0x00,0x12,0x76,0x90,0x00,
            //    0x16,0x02,0x00,0x78,0x00,0x08,0x00,0x00,0x09,0x00,
            //    0x00,0x19,0x00,0x00,0x07,0x01,0x08,0x00,0x0A,0x00,
            //    0x00,0x04,0x04,0xBC,0x4D,0xDE,0x5B,0x00,0x05,0x04,
            //    0xC7,0xFF,0x70,0x6F,0x00,0x06,0x02,0x07,0x7A,0x00,
            //    0x2E,0x04,0x00,0x00,0x00,0x00,0x00,0x2F,0x04,0x00,
            //    0x00,0x00,0x01,0x00,0x30,0x04,0x00,0x00,0x00,0x02,
            //    0x00,0x31,0x04,0x00,0x00,0x00,0x03,0x00,0x32,0x04,
            //    0x00,0x00,0x00,0x04,0x00,0x33,0x04,0x00,0x00,0x00,
            //    0x05,0x00,0x34,0x04,0x00,0x00,0x00,0x06,0x00,0x35,
            //    0x04,0x00,0x00,0x00,0x07,
            //    0x01,0xE1,
            //    0x59
            //};
            byte[] hexBytes = new byte[]  {
                 0x95,0x0E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x0E,0x00,0x00,0x00,0x00,0xA9,0x00,0x1F,0x04,0x80,0x05,0x05,0x16,0x00,0x20,0x02,0x20,0x06,0x00,0x21,0x02,0x01,0x05,0x00,0x0C,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x16,0x02,0x00,0x78,0x00,0x08,0x0F,0x38,0x36,0x31,0x34,0x31,0x30,0x30,0x34,0x37,0x36,0x38,0x39,0x36,0x30,0x38,0x00,0x09,0x0F,0x34,0x36,0x30,0x30,0x31,0x33,0x31,0x30,0x37,0x37,0x30,0x37,0x31,0x35,0x35,0x00,0x19,0x00,0x00,0x07,0x02,0x00,0x14,0x00,0x0A,0x04,0x63,0x74,0x6E,0x62,0x00,0x04,0x0E,0x31,0x30,0x2E,0x32,0x31,0x32,0x2E,0x31,0x34,0x31,0x2E,0x31,0x30,0x35,0x00,0x05,0x04,0x7A,0x70,0xFF,0xC7,0x00,0x06,0x02,0x07,0x5B,0x00,0x2E,0x04,0x00,0x00,0x00,0x00,0x00,0x2F,0x04,0x00,0x00,0x00,0x00,0x00,0x30,0x04,0x00,0x00,0x00,0x00,0x00,0x31,0x04,0x00,0x00,0x00,0x00,0x00,0x32,0x04,0x00,0x00,0x00,0x00,0x00,0x33,0x04,0x00,0x00,0x00,0x00,0x00,0x34,0x04,0x00,0x00,0x00,0x00,0x00,0x35,0x04,0x00,0x00,0x00,0x00,0x00,0x41,0x59
            };

            byte[] xjsj = new byte[] {
                0x95,0x0E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x03,0x07,0xE4,0x06,0x0B,0x04,0x0B,0x04,0x16,0x0E,0x00,0x03,0x00,0x00,0x3B,0x00,0x0D,0x01,0xFF,0x00,0x0F,0x02,0x00,0xD5,0x00,0x0E,0x02,0x01,0xFC,0x00,0x10,0x02,0x00,0x6A,0x00,0x1E,0x02,0x00,0x00,0x00,0x13,0x04,0x00,0x00,0x00,0x1C,0x00,0x12,0x04,0x00,0x00,0x1A,0xF4,0x00,0x1B,0x02,0x00,0x1E,0x00,0x15,0x02,0x00,0xF0,0x00,0x02,0x01,0x12,0x00,0x28,0x04,0x00,0x00,0x00,0x04,0x52,0x01,0x59
            };

            try
            {
                if (topic.Contains("cmdstr"))
                {
                    string payloadstr = Encoding.UTF8.GetString(payload);
                    string padleftStr = string.Empty;
                    payloadstr = payloadstr.Replace("\0", string.Empty);
                    logger.Info(payloadstr);
                    payload = HexFormatHelper.StringConvertHexBytes(payloadstr);
                }
                var uploadOrigin = NBReceivedHelper.AnalyzeMessage(payload);
                if (uploadOrigin != null)
                {
                    if (uploadOrigin.commandCode == 0x04 || uploadOrigin.commandCode == 0x0E)//参数设置和数据上报
                    {
                        uploadOrigin.uploadEntitys = new Dictionary<byte, Dictionary<byte, UploadEntity>>();
                        NBReceivedHelper.GetUploadEntity(uploadOrigin.uploadEntitys, uploadOrigin.data, 0);
                        await UploadContext.GetInstance().GetUploadSchedule().Run(uploadOrigin);
                    }
                }
            }
            catch (Exception ex)
            {
                var strPayLoad = HexFormatHelper.HexBytesConvertString(payload);
                logger.Error(this.GetType().FullName + " Topic:" + topic + ",Payload:" + strPayLoad + ex.ToString());
                UPLogger.Show(ex.Message);
                //throw;
            }
        }
        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Task OnMqttDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            if (obj.ClientWasConnected)
            {
                notification.Show("断开连接...");
            }
            return Task.CompletedTask;
        }
        /// <summary>
        ///  关闭连接
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            await MqttClient.DisconnectAsync();
            MqttClient = null;
        }

        private async Task OnMqttConnectedAsync(MqttClientConnectedEventArgs obj)
        {
            if (MqttClient.IsConnected)
            {

                ///上位机默认订阅UPLOAD
                await Subscribe(new List<string>
                {
                    "uploadhex/358826100269329",
                    //"uploadhex/861410047695456",
                    //"uploadhex/861410047695456",
                    "uploadstr/358826100269329",
                    "cmdstr/light/#",
                    "cmdhex/light/#",
                });
                ///上位机默认订阅UPLOAD
                //await Subscribe(new List<string>
                //{
                //    "uploadhex/861410047743793",
                //    "light/861410047743793",
                //    "uploadstr/#",
                //    //"cmdstr/#",
                //    "cmdstr/light/861410047743793",
                //    "cmdhex/#",
                //});
            }
            else
            {
                notification.Show("连接失败...");
            }
        }

    }
}
