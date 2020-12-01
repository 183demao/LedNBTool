using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.WebSockets;

namespace NbIotCmd.Entity
{
    /// <summary>
    /// 发送指令表
    /// </summary>
    [Table("NbCommand")]
    public class NbCommand
    {
        /// <summary>主键自增</summary>	                                   
        [Key]
        public long Id { get; set; }
        /// <summary>UUID</summary>	                                       
        public string CmdId { get; set; }
        /// <summary>命令码</summary>	                                   
        public string CmdCode { get; set; }
        /// <summary>消息索引</summary>	                                   
        public int MessageID { get; set; }
        /// <summary>时间戳</summary>	                                   
        public DateTime Timestamp { get; set; }
        /// <summary>数据包</summary>	                                   
        public string CmdData { get; set; }
        /// <summary>发送用户ID</summary>	                               
        public int UserId { get; set; }
        /// <summary>账户</summary>	                                       
        public string Account { get; set; }
        /// <summary>主题类型</summary>	                                   
        public string TopicType { get; set; }
        /// <summary>主题</summary>	                                       
        public string Topic { get; set; }
        /// <summary>命令回复数</summary>	                               
        public int ReplyCount { get; set; }
        /// <summary>事件发送时间</summary>	                               
        public DateTime EventTime { get; set; }
    }
   
}
