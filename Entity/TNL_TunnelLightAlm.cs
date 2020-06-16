using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NbIotCmd.Entity
{
    /// <summary>
    /// NB单灯实时表
    /// </summary>
    [Table("TNL_TunnelLightAlm")]
    public class TNL_TunnelLightAlm
    {
        /// <summary>单灯ID</summary>	                      
        [Key]
        public long? TunnelLight_ID { get; set; }
        /// <summary>报警类型</summary>	                      
        public int AlmLevel_ID { get; set; }
        /// <summary>调光等级</summary>	                      
        public double DimmingFeatureValue_NR { get; set; }
        /// <summary>电流</summary>	                          
        public double CurrentFeatureValue_NR { get; set; }
        /// <summary>电压</summary>	                          
        public double VoltageFeatureValue_NR { get; set; }
        /// <summary>功率</summary>	                          
        public double PowerFeatureValue_NR { get; set; }
        /// <summary>光照度</summary>	                      
        public double LuminousIntensity_NR { get; set; }
        /// <summary>温度</summary>	                          
        public double Temperature_NR { get; set; }
        /// <summary>电量</summary>	                          
        public double PowerConsumption_NR { get; set; }
        /// <summary>工作时间</summary>	                      
        public double WorkingTimeInMinute_NR { get; set; }
        /// <summary>风向</summary>	                          
        public double VehicleFlow_NR { get; set; }
        /// <summary>风速</summary>	                          
        public double VehicleSpeed_NR { get; set; }
        /// <summary>固件版本</summary>	                      
        public int FirmwareVersion_NR { get; set; }
        /// <summary>服务器时间</summary>	                  
        public DateTime SampTime_DT { get; set; }
        /// <summary>功率因素</summary>	                      
        public double PowerFactor_NR { get; set; }
        /// <summary>通道</summary>	                          
        public int? ChannelNumber { get; set; }
        /// <summary>本地时间</summary>	                      
        public DateTime? LocalDate { get; set; }
        /// <summary>数据来源</summary>	                      
        public int DataSource { get; set; }
        /// <summary>IP</summary>	                          
        public string RemoteEndPoint { get; set; }
        /// <summary>信号强度</summary>	                      
        public int Signal_NR { get; set; }
        /// <summary>版本</summary>	                          
        public string Version { get; set; }
        /// <summary>IMEI</summary>		                      
        public string IMEI { get; set; }
        /// <summary>IMSI</summary>		                      
        public string IMSI { get; set; }
        /// <summary>ICCID</summary>		                  
        public string ICCID { get; set; }
        /// <summary>bandNo</summary>		                  
        public int bandNo { get; set; }
        /// <summary>State</summary>		                  
        public int State { get; set; }
        /// <summary>GPSInfo</summary>		                  
        public string GpsInfo { get; set; }
        /// <summary>TIMEPlan</summary>		                  
        public string TimePlan { get; set; }
        /// <summary>PHOTOCell</summary>		              
        public string PhotoCell { get; set; }
        /// <summary>INSPECTSUCCESS_DTuccess_DT</summary>		
        public DateTime? InspectSuccess_DT { get; set; }
        /// <summary>ISDay</summary>		                  
        public int IsDay { get; set; }
        /// <summary>LIGHTNINGCount</summary>		          
        public int LightningCount { get; set; }
        /// <summary>UPUIDupUID</summary>		              
        public string upUID { get; set; }
        /// <summary>NB模块的CSQ</summary>		              
        public string CSQ { get; set; }
    }
}
