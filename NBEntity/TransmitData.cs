using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.NBEntity
{
    public class TransmitData
    {
        public string Topic { get; set; }
        public Guid UUID { get; set; }
        public string CommandCode { get; set; }
        public int MesssageID { get; set; }
        public byte[] Data { get; set; }
        public MessageType MessageType { get; set; }

    }
    //
    // 摘要:
    //     Indicates the message type.
    public enum MessageType
    {
        /// <summary>
        /// 文字
        /// </summary>
        Text = 0,
        /// <summary>
        /// 二进制
        /// </summary>
        Binary = 1,
    }
}
