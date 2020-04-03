﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NbIotCmd.Entity
{
    /// <summary>
    /// NB报警表
    /// </summary>
    [Table("TNL_AlarmInfo")]
    public class TNL_AlarmInfo
    {
        /// <summary>
        ///主键自增
        ///</summary>	                                        
        [Key]
        public int ID { get; set; }
        /// <summary>
        ///地址域
        ///</summary>	                                        
        public string DeviceAddress { get; set; }
        /// <summary>
        ///灯号
        ///</summary>	                                        
        public long TunnelLight_ID { get; set; }
        /// <summary>
        ///调光等级
        ///</summary>	                                        
        public float DimmingFeatureValue_NR { get; set; }
        /// <summary>
        ///电压
        ///</summary>	                                        
        public float VoltageFeatureValue_NR { get; set; }
        /// <summary>
        ///电流
        ///</summary>	                                        
        public float CurrentFeatureValue_NR { get; set; }
        /// <summary>
        ///有功功率
        ///</summary>	                                        
        public float PowerFeatureValue_NR { get; set; }
        /// <summary>
        ///功率因素
        ///</summary>	                                        
        public float PowerFactor { get; set; }
        /// <summary>
        ///用电量
        ///</summary>	                                        
        public float PowerConsumption_NR { get; set; }
        /// <summary>
        ///工作时间
        ///</summary>	                                        
        public float WorkingTimeInMinute_NR { get; set; }
        /// <summary>
        ///内部温度
        ///</summary>	                                        
        public float Temperature_NR { get; set; }
        /// <summary>
        ///光感值
        ///</summary>	                                        
        public float LuminousIntensity_NR { get; set; }
        /// <summary>
        ///NB模块的CSQ
        ///</summary>	                                        
        public string CSQ { get; set; }
        /// <summary>
        ///警报信息
        ///</summary>	                                        
        public string AlarmInfo { get; set; }
        /// <summary>
        ///过流告警
        ///</summary>	                                        
        public int Alarm0 { get; set; }
        /// <summary>
        ///过功率告警
        ///</summary>	                                        
        public int Alarm1 { get; set; }
        /// <summary>
        ///电容告警
        ///</summary>	                                        
        public int Alarm2 { get; set; }
        /// <summary>
        ///光源故障告警
        ///</summary>	                                        
        public int Alarm3 { get; set; }
        /// <summary>
        ///熔丝故障(掉电)告警
        ///</summary>	                                        
        public int Alarm4 { get; set; }
        /// <summary>
        ///本地时间
        ///</summary>	                                        
        public DateTime LocalDate { get; set; }
        /// <summary>
        /// ///创建时间
        /// </summary>	                                        
        public DateTime SimpleTime { get; set; }

    }
}