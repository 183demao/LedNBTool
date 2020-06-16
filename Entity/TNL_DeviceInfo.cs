using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NbIotCmd.Entity
{
    /// <summary>
    /// NB设备信息表
    /// </summary>
    [Table("TNL_DeviceInfo")]
    public class TNL_DeviceInfo
    {
        /// <summary>主键自增</summary>	                         
        [Key]
        public long ID { get; set; }
        /// <summary>
        /// 本地时间
        /// </summary>
        public DateTime LocalDate { get; set; }
        /// <summary>
        /// 本地时间
        /// </summary>
        public DateTime SampTime { get; set; }
        /// <summary>设备类型码</summary>	                     
        public string DeviceType { get; set; }
        /// <summary>硬件版本号</summary>	                     
        public string HDVersion { get; set; }
        /// <summary>固件版本号</summary>	                     
        public string Version { get; set; }
        /// <summary>GPS经纬度</summary>	                         
        public string GPSInfo { get; set; }
        /// <summary>上报间隔</summary>	                         
        public int ReportInterval { get; set; }
        /// <summary>NB模块固件版本</summary>	                     
        public string TAVersion { get; set; }
        /// <summary>模块设备号,15位纯数字</summary>	             
        public string IMEI { get; set; }
        /// <summary>SIM卡号,15位纯数字</summary>	                 
        public string IMSI { get; set; }
        /// <summary>SIM卡ID,19位纯数字</summary>	             
        public string ICCID { get; set; }
        /// <summary>注册的频段</summary>	                     
        public string BAND { get; set; }
        /// <summary>基站编号</summary>	                         
        public string CELLID { get; set; }
        /// <summary>接收信号强度</summary>	                     
        public string RSSI { get; set; }
        /// <summary>参考接收信号功率</summary>	                 
        public string RSRP { get; set; }
        /// <summary>基站时间</summary>	                         
        public string UTC { get; set; }
        /// <summary>接入点名称</summary>	                     
        public string APN { get; set; }
        /// <summary>本次连接IP地址</summary>	                     
        public string IP { get; set; }
        /// <summary>SERVER地址或域名</summary>	                 
        public string Server { get; set; }
        /// <summary>SERVER端口</summary>	                     
        public string Port { get; set; }
        /// <summary>单灯所属群组(Org)</summary>	                 
        public long Group0 { get; set; }
        /// <summary>单灯所属群组(Area)</summary>	                 
        public long Group1 { get; set; }
        /// <summary>单灯所属群组(Line)</summary>	                 
        public long Group2 { get; set; }
        /// <summary>单灯所属群组(Section)</summary>	             
        public long Group3 { get; set; }
        /// <summary>单灯所属群组(Group)</summary>	             
        public long Group4 { get; set; }
        /// <summary>单灯所属群组</summary>	                     
        public long Group5 { get; set; }
        /// <summary>单灯所属群组</summary>	                     
        public long Group6 { get; set; }
        /// <summary>单灯所属群组</summary>	                     
        public long Group7 { get; set; }
        /// <summary>
        /// 地址域
        /// </summary>
        public string DeviceAddress { get; set; }

        public static implicit operator TNL_DeviceInfo(EntityEntry<TNL_DeviceInfo> v)
        {
            throw new NotImplementedException();
        }
    }
}
