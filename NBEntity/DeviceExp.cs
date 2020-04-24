using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.NBEntity
{
    public class DeviceExp
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
    }
}
