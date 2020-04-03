using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NbIotCmd.Entity
{
    /// <summary>
    /// 多通道单灯实时表
    /// </summary>
    [Table("TNL_TunnelLightChanAlm")]
    public class TNL_TunnelLightChanAlm
    {
        /// <summary>主键自增</summary>	                                    
        [Key]
        public int ID { get; set; }
        /// <summary>单灯ID</summary>	                                    
        public long TunnelLight_ID { get; set; }
        /// <summary>报警类型</summary>	                                    
        public int AlmLevel_ID { get; set; }
        /// <summary>调光等级</summary>	                                    
        public float DimmingFeatureValue_NR { get; set; }
        /// <summary>电流</summary>	                                        
        public float CurrentFeatureValue_NR { get; set; }
        /// <summary>电压</summary>	                                        
        public float VoltageFeatureValue_NR { get; set; }
        /// <summary>功率</summary>	                                        
        public float PowerFeatureValue_NR { get; set; }
        /// <summary>光照度</summary>	                                    
        public float LuminousIntensity_NR { get; set; }
        /// <summary>温度</summary>	                                        
        public float Temperature_NR { get; set; }
        /// <summary>电量</summary>	                                        
        public float PowerConsumption_NR { get; set; }
        /// <summary>工作时间</summary>	                                    
        public float WorkingTimeInMinute_NR { get; set; }
        /// <summary>风向</summary>	                                        
        public float VehicleFlow_NR { get; set; }
        /// <summary>风速</summary>	                                        
        public float VehicleSpeed_NR { get; set; }
        /// <summary>固件版本</summary>	                                    
        public float FirmwareVersion_NR { get; set; }
        /// <summary>服务器时间</summary>	                                
        public DateTime SampTime_DT { get; set; }
        /// <summary>功率因素</summary>	                                    
        public float PowerFactor_NR { get; set; }
        /// <summary>通道</summary>	                                        
        public int ChannelNumber { get; set; }
        /// <summary>本地时间</summary>	                                    
        public DateTime LocalDate { get; set; }
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
        public string InspectSuccess_DT { get; set; }
        /// <summary>ISDay</summary>	                                    
        public string IsDay { get; set; }
        /// <summary>LIGHTNINGCount</summary>	                            
        public string LightningCount { get; set; }
        /// <summary>UPUIDupUID</summary>	                                
        public string upUID { get; set; }
        /// <summary>NB模块的CSQ</summary>	                                
        public string CSQ { get; set; }
    }
}
