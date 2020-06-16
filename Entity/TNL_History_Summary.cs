using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NbIotCmd.Entity
{
    /// <summary>
    /// NB历史记录表
    /// </summary>
    [Table("TNL_History_Summary")]
    public class TNL_History_Summary  
    {
        /// <summary>
        /// 时间ID
        /// </summary>
        [Key]
        public long Partition_ID { get; set; }
        /// <summary>
        /// 流水标识
        /// </summary>
        public long History_ID { get; set; }
        /// <summary>
        /// 单灯ID
        /// </summary>
        public long? TunnelLight_ID { get; set; }
        /// <summary>
        /// 电流
        /// </summary>
        public double CurrentFeatureValue_NR { get; set; }
        /// <summary>
        /// 电压
        /// </summary>
        public double VoltageFeatureValue_NR { get; set; }
        /// <summary>
        /// 调光等级
        /// </summary>
        public double DimmingFeatureValue_NR { get; set; }
        /// <summary>
        /// 有功功率
        /// </summary>
        public double PowerFeatureValue_NR { get; set; }
        /// <summary>
        /// 光感值
        /// </summary>
        public double LuminousIntensity_NR { get; set; }
        /// <summary>
        /// 内部温度
        /// </summary>
        public double Temperature_NR { get; set; }
        /// <summary>
        /// 用电量
        /// </summary>
        public double PowerConsumption_NR { get; set; }
        /// <summary>
        /// 累计工作时间
        /// </summary>
        public double WorkingTimeInMinute_NR { get; set; }
        /// <summary>
        /// 风向
        /// </summary>
        public double VehicleFlow_NR { get; set; }
        /// <summary>
        /// 风速
        /// </summary>
        public double VehicleSpeed_NR { get; set; }
        /// <summary>
        /// 固件版本
        /// </summary>
        public int FirmwareVersion_NR { get; set; }
        /// <summary>
        /// 服务器入库时间
        /// </summary>
        public DateTime SampTime_DT { get; set; }
        /// <summary>
        /// 报警ID
        /// </summary>
        public int AlmLevel_ID { get; set; }
        /// <summary>
        /// 功率因素
        /// </summary>
        public double PowerFactor_NR { get; set; }
        /// <summary>
        /// 通道号
        /// </summary>
        public int ChannelNumber { get; set; }
        /// <summary>
        /// 本地时间
        /// </summary>
        public DateTime LocalDate { get; set; }
        /// <summary>
        /// 时区ID
        /// </summary>
        public string TimeZone_CD { get; set; }
        /// <summary>
        /// 雷击次数
        /// </summary>
        public int LightningCount { get; set; }
        /// <summary>
        /// 白天/黑夜
        /// </summary>
        public int IsDay { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public int DataSource { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string RemoteEndPoint { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 信号质量
        /// </summary>
        public int Signal_NR { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// IMEI
        /// </summary>
        public string IMEI { get; set; }
        /// <summary>
        /// IMSI
        /// </summary>
        public string IMSI { get; set; }
        /// <summary>
        /// ICCID
        /// </summary>
        public string ICCID { get; set; }
        /// <summary>
        /// bandNo
        /// </summary>
        public int bandNo { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// GPS信息
        /// </summary>
        public string GpsInfo { get; set; }
        /// <summary>
        /// 上报数据
        /// </summary>
        public string UploadData { get; set; }
        /// <summary>
        /// NB模块的CSQ
        /// </summary>
        public string CSQ { get; set; }

    }
}
