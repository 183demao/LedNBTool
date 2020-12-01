using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NbIotCmd.Config
{
    public class NBSetting
    {
        /// <summary>
        /// 设备信息寄存器
        /// </summary>
        public static List<byte> DeviceStorage = new List<byte> { };
        /// <summary>
        /// 巡检数据寄存器
        /// </summary>
        public static List<byte> InspectionData = new List<byte> { };

    }

    public enum NBCommondCode
    {
        /// <summary>
        /// 参数设置
        /// </summary>
        ParamsSetting = 0x04,
        /// <summary>
        /// 时间同步
        /// </summary>
        DateSync = 0x05
    }
}
