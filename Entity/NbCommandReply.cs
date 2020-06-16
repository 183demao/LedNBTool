using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NbIotCmd.Entity
{
    /// <summary>
    /// 回复指令表
    /// </summary>
    [Table("NbCommandReply")]
    public class NbCommandReply 
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
        /// <summary>回复包</summary>	                                        
        public string ReplyData { get; set; }
        /// <summary>地址域</summary>	                                        
        public string DeviceAddress { get; set; }
        /// <summary>本地时间</summary>	                                        
        public DateTime LocalDate { get; set; }
        /// <summary>创建时间</summary>	                                        
        public DateTime SimpleTime { get; set; }
        /// <summary>灯号</summary>	                                          	
        public long? TunnelLight_ID { get; set; }
    }
}
