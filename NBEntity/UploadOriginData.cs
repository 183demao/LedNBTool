using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd
{
    /// <summary>
    /// 数据上报后，解析出的实体
    /// </summary>
    public class UploadOriginData : IBaseMQTTEntity
    {
        /// <summary>
        /// 帧头
        /// </summary>
        public byte frameHeader { get; set; }
        /// <summary>
        /// 帧类型
        /// </summary>
        public byte frameType { get; set; }
        /// <summary>
        /// 是否主动上报
        /// </summary>
        public bool actived { get; set; }
        /// <summary>
        /// 是否请求
        /// </summary>
        public bool requested { get; set; }

        /// <summary>
        /// 地址域
        /// </summary>
        public byte[] addressDomain { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public byte[] timeStamp { get; set; }
        /// <summary>
        /// UUID
        /// </summary>
        public byte[] uuid { get; set; }
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
        public byte ack { get; set; }
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
        public byte frameTail { get; set; }
        //是否有地址
        public bool hasAddress { get; set; }
        //是否有时间戳
        public bool hasTimeStramp { get; set; }
        public bool hasUUID { get; set; }
        public Dictionary<byte, UploadEntity> uploadEntitys { get; set; }
    }
    public class UploadEntity : IBaseMQTTEntity
    {
        public byte ChannelNumber { get; set; }
        public byte MemeroyID { get; set; }
        public byte MemeroyLength { get; set; }
        public byte[] MemeroyData { get; set; }
    }

    /// <summary>
    /// 标识
    /// </summary>
    public interface IBaseMQTTEntity { }
}
