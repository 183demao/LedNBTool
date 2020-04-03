using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Server;
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
        private Notification notification;
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
        public async Task Initialize(string server, int port, Notification notification)
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
            var hexBytes = new[] { 0x95, 0x03, 0x0e, 0x56, 0x78, 0x01, 0x04, 0x00, 0x1f, 0x06, 0xa5, 0x5a, 0x01, 0x30, 0x59 };

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
