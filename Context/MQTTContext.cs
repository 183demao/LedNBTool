using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Server;
using NbIotCmd.Context;
using NbIotCmd.Helper;
using NbIotCmd.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NbIotCmd
{
    public class MQTTContext
    {
        private static readonly MQTTContext instance = new MQTTContext();
        private WinformAsyncNotification notification;

        private MQTTContext()
        { }

        public static MQTTContext getInstance()
        {
            return instance;
        }

        private IManagedMqttClient managedMqttClient;
        /// <summary>
        /// 初始化
        /// </summary>
        public async Task Initialize(string server, int port, WinformAsyncNotification notification)
        {
            var mqttFactory = new MqttFactory();

            var tlsOptions = new MqttClientTlsOptions
            {
                UseTls = false,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
                AllowUntrustedCertificates = true
            };

            var options = new MqttClientOptions
            {
                ClientId = Guid.NewGuid().ToString().ToUpper(),
                ProtocolVersion = MqttProtocolVersion.V311,
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = server,
                    Port = port,
                    TlsOptions = tlsOptions
                }
            };

            if (options.ChannelOptions == null)
            {
                throw new InvalidOperationException();
            }

            options.CleanSession = true;
            options.KeepAlivePeriod = TimeSpan.FromSeconds(5);

            managedMqttClient = mqttFactory.CreateManagedMqttClient();
            managedMqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnSubscriberConnectedAsync);
            managedMqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnSubscriberDisconnected);
            managedMqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnSubscriberMessageReceived);
            await managedMqttClient.StartAsync(
            new ManagedMqttClientOptions
            {
                ClientOptions = options
            });
            //注册通知接口
            this.notification = notification;

        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <returns></returns>
        public async Task Subscribe(List<string> topics)
        {
            var subTopics = from topic in topics
                            select new TopicFilter
                            {
                                Topic = topic
                            };
            await managedMqttClient.SubscribeAsync(subTopics);
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
            await managedMqttClient.PublishAsync(values);
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task OnSubscriberMessageReceived(MqttApplicationMessageReceivedEventArgs obj)
        {
            var playload = obj.ApplicationMessage.Payload;
            byte[] hexBytes = new byte[]  {
                0x95,
                0x03,
                0x0E,
                0x12,0x34,
                0x01,
                0x00,//ack
                0xA9,0x00,
                0x1F,0x08,0x07,0x06,
                0x05,0x04,0x03,0x02,
                0x01,0x00,0x00,0x21,
                0x04,0x00,0x00,0x0E,
                0x01,0x00,0x20,0x04,
                0x00,0x00,0x0F,0x01,
                0x00,0x0C,0x04,0x00,
                0x05,0x09,0x10,0x00,
                0x0C,0x04,0x00,0x12,
                0x76,0x90,0x00,0x16,
                0x02,0x00,0x78,0x00,
                0x08,0x0F,0x30,0x36,
                0x38,0x37,0x35,0x36,
                0x37,0x34,0x30,0x30,
                0x31,0x34,0x31,0x36,
                0x38,0x00,0x09,0x0F,
                0x37,0x33,0x30,0x39,
                0x30,0x38,0x38,0x31,
                0x30,0x37,0x34,0x30,
                0x30,0x36,0x34,0x00,
                0x19,0x00,0x00,0x07,
                0x01,0x08,0x00,0x0A,
                0x04,0x62,0x6E,0x74,
                0x63,0x00,0x04,0x04,
                0xBC,0x4D,0xDE,0x6F,
                0x00,0x05,0x04,0xC7,
                0xFF,0x70,0x7A,0x00,
                0x06,0x01,0x5B,0x00,
                0x2E,0x04,0x00,0x00,
                0x00,0x00,0x00,0x2F,
                0x04,0x00,0x00,0x00,
                0x01,0x00,0x30,0x04,
                0x00,0x00,0x00,0x02,
                0x00,0x31,0x04,0x00,
                0x00,0x00,0x03,0x00,
                0x32,0x04,0x00,0x00,
                0x00,0x04,0x00,0x33,
                0x04,0x00,0x00,0x00,
                0x05,0x00,0x34,0x04,
                0x00,0x00,0x00,0x06,
                0x00,0x35,0x04,0x00,
                0x00,0x00,0x07,
                0x01,0x31,
                0x59
            };

            var uploadOrigin = NBReceivedHelper.AnalyzeMessage(hexBytes);
            if (uploadOrigin != null)
            {
                uploadOrigin.uploadEntitys = new Dictionary<byte, UploadEntity>();
                NBReceivedHelper.GetUploadEntity(uploadOrigin.uploadEntitys, uploadOrigin.data, 0);
                UploadContext.GetInstance().GetUploadSchedule().Run(uploadOrigin);
            }
        }
        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task OnSubscriberDisconnected(MqttClientDisconnectedEventArgs obj)
        {
        }

        private async Task OnSubscriberConnectedAsync(MqttClientConnectedEventArgs obj)
        {
            if (managedMqttClient.IsConnected)
            {
                notification.Show("连接成功...");
                ///上位机默认订阅UPLOAD
                await Subscribe(new List<string>
                {
                    "UPLOAD"
                });
            }
            else
            {
                notification.Show("连接失败...");
            }
        }

    }
}
