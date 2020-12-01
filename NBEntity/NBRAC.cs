using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.NBEntity
{
    /// <summary>
    /// NB类寄存器地址
    /// </summary>
    public class NBRAC
    {
        /// <summary>
        /// 设备类型码
        /// </summary>
        public static readonly byte DeviceType = 0x1f;
        /// <summary>
        /// 硬件版本号
        /// </summary>
        public static readonly byte HDVersion = 0x20;
        /// <summary>
        /// 固件版本号
        /// </summary>
        public static readonly byte Version = 0x21;
        /// <summary>
        /// GPS经纬度
        /// </summary>
        public static readonly byte GPSInfo = 0x0C;
        /// <summary>
        /// 上报间隔
        /// </summary>
        public static readonly byte ReportInterval = 0x16;
        /// <summary>
        /// NB模块固件版本
        /// </summary>
        public static readonly byte TAVersion = 0x26;
        /// <summary>
        /// 模块设备号,15位纯数字
        /// </summary>
        public static readonly byte IMEI = 0x08;
        /// <summary>
        /// SIM卡号,15位纯数字
        /// </summary>
        public static readonly byte IMSI = 0x09;
        /// <summary>
        /// SIM卡ID,19位纯数字
        /// </summary>
        public static readonly byte ICCID = 0x19;
        /// <summary>
        /// 注册的频段
        /// </summary>
        public static readonly byte BAND = 0x07;
        /// <summary>
        /// 基站编号
        /// </summary>
        public static readonly byte CELLID = 0x24;
        /// <summary>
        /// 接收信号强度
        /// </summary>
        public static readonly byte RSSI = 0x00;
        /// <summary>
        /// 参考接收信号功率
        /// </summary>
        public static readonly byte RSRP = 0x25;
        /// <summary>
        /// 基站时间
        /// </summary>
        public static readonly byte UTC = 0x27;
        /// <summary>
        /// 接入点名称
        /// </summary>
        public static readonly byte APN = 0x0A;
        /// <summary>
        /// 本次连接IP地址
        /// </summary>
        public static readonly byte IP = 0x04;
        /// <summary>
        /// SERVER地址或域名
        /// </summary>
        public static readonly byte Server = 0x05;
        /// <summary>
        /// SERVER端口
        /// </summary>
        public static readonly byte Port = 0x06;
        /// <summary>
        /// 单灯所属群组(Org)
        /// </summary>
        public static readonly byte Group0 = 0x2E;
        /// <summary>
        /// 单灯所属群组(Area)
        /// </summary>
        public static readonly byte Group1 = 0x2F;
        /// <summary>
        /// 单灯所属群组(Line)
        /// </summary>
        public static readonly byte Group2 = 0x30;
        /// <summary>
        /// 单灯所属群组(Section)
        /// </summary>
        public static readonly byte Group3 = 0x31;
        /// <summary>
        /// 单灯所属群组(Group)
        /// </summary>
        public static readonly byte Group4 = 0x32;
        /// <summary>
        /// 单灯所属群组
        /// </summary>
        public static readonly byte Group5 = 0x33;
        /// <summary>
        /// 单灯所属群组
        /// </summary>
        public static readonly byte Group6 = 0x34;
        /// <summary>
        /// 单灯所属群组
        /// </summary>
        public static readonly byte Group7 = 0x35;

        #region 调光类
        /// <summary>
        ///调光等级
        ///</summary>	
        public static readonly byte DimmingFeature = 0x0D;
        /// <summary>
        ///电压
        ///</summary>	       
        public static readonly byte VoltageFeature = 0x0F;
        /// <summary>
        ///电流
        ///</summary>	    
        public static readonly byte CurrentFeature = 0x0E;
        /// <summary>
        ///有功功率
        ///</summary>	 
        public static readonly byte PowerFeature = 0x10;
        /// <summary>
        /// 功率因素
        /// </summary>
        public static readonly byte PowerFactor = 0x1E;
        /// <summary>
        ///用电量
        ///</summary>	  
        public static readonly byte PowerConsumption = 0x13;
        /// <summary>
        ///工作时间
        ///</summary>	 
        public static readonly byte WorkingTimeInMinute = 0x12;
        /// <summary>
        ///内部温度
        ///</summary>	  
        public static readonly byte Temperature = 0x1B;
        /// <summary>
        ///光感值
        ///</summary>	   
        public static readonly byte LuminousIntensity = 0x15;
        /// <summary>
        /// 单双灯类型
        /// </summary>
        public static readonly byte LightType = 0x40;
        #endregion

        #region 警报类型
        /// <summary>
        /// 报警信息
        /// </summary>
        public static readonly byte AlarmInfo = 0x28;
        /// <summary>
        /// 过流告警
        /// </summary>
        public static readonly int Alarm0 = 0;
        /// <summary>
        /// 过功率告警
        /// </summary>
        public static readonly int Alarm1 = 1;
        /// <summary>           
        /// 电容告警             
        /// </summary>          
        public static readonly int Alarm2 = 2;
        /// <summary>          
        /// 光源故障告警        
        /// </summary>         
        public static readonly int Alarm3 = 3;
        /// <summary>           
        /// 熔丝故障(掉电)告警   
        /// </summary>          
        public static readonly int Alarm4 = 4;
        /// <summary>
        /// 低电流告警
        /// </summary>
        public static readonly int Alarm5 = 5;
        /// <summary>
        /// 低功率告警
        /// </summary>
        public static readonly int Alarm6 = 6;

        #endregion
    }
}
