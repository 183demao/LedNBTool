using MQTTnet.Client.Publishing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.IHandler
{
    public interface IMQTTClientHandler
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        public Task Send(Dictionary<string, List<byte[]>> publishData);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        public Task<MqttClientPublishResult> Send(string topic, string payload);
    }
}
