using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.NBEntity
{
    public class IssueEntity
    {
        /// <summary>
        /// 帧头
        /// </summary>
        public byte frameHeader { get; set; } = 0x9A;
        /// <summary>
        /// 帧类型
        /// </summary>
        public byte frameType { get; set; } = 0x0F;
        /// <summary>
        /// 地址域
        /// </summary>
        public byte[] addressDomain { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public byte[] timeStamp { get; set; }
        /// <summary>
        /// 命令码
        /// </summary>
        public byte commandCode { get; set; }
        /// <summary>
        /// 消息索引
        /// </summary>
        public byte[] messsageId { get; set; }
        /// <summary>
        /// 消息响应
        /// </summary>
        public byte ack { get; set; } = 0x00;
        /// <summary>
        /// 长度
        /// </summary>
        public byte[] length { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] data { get; set; }
        /// <summary>
        /// 校验
        /// </summary>
        public byte[] crc { get; set; }
        /// <summary>
        /// 帧尾
        /// </summary>
        public byte frameTail { get; set; } = 0xA9;
    }
}
