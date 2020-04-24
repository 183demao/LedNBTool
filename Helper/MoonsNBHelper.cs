using System.Collections.Generic;
using System;
using Led.Tools;
using System.Collections;
using System.Linq;
using Led.SingleLight;

namespace Led.MoonsNBCmdWrapper
{

    public class MoonsNBFrameType
    {
        public int HasDeviceAddress { get; set; }

        public int HasTimeStamp { get; set; }

        public int HasUUID { get; set; }

        public int ActiveReport { get; set; }

        public int CommandType { get; set; }
    }

    public static class MoonsNBHelper
    {
        private static object _lock = new object();

        private static int _MessageId = 0;

        #region 帧标志

        //  IoT -> 设备(device)
        public const byte IotToDevice_HeadByte = 0x9A;
        public const byte IotToDevice_TailByte = 0xA9;


        // 设备(device) -> IoT 
        public const byte DeviceToIot_HeadByte = 0x95;
        public const byte DeviceToIot_TailByte = 0x59;

        public const int Max_MessageID = 65535;

        #endregion

        #region 帧类型

        /// <summary>
        /// 被动回复
        /// </summary>
        public const int Report_Reply = 0;

        /// <summary>
        /// 主动上报
        /// </summary>
        public const int Report_ActiveReport = 1;

        /// <summary>
        /// 命令请求标志：请求
        /// </summary>
        public const int CommandType_Reuqest = 1;

        /// <summary>
        /// 命令请求标志：响应
        /// </summary>
        public const int CommandType_Response = 0;

        public const byte Channel_All = 0xFF;

        #endregion

        #region 帧格式Field

        public const string Frame_Field_UUID = "UUID";
        public const string Frame_Field_Head = "Head";
        public const string Frame_Field_FrameType = "FrameType";
        public const string Frame_Field_Address = "Address";
        public const string Frame_Field_TimeStamp = "TimeStamp";
        public const string Frame_Field_Cmd = "Cmd";
        public const string Frame_Field_MessageID = "MessageID";
        public const string Frame_Field_Ack = "Ack";
        public const string Frame_Field_PDLen = "PDLen";
        public const string Frame_Field_Data = "Data";
        public const string Frame_Field_Crc = "Crc";
        public const string Frame_Field_Tail = "Tail";

        //寄存器地址列表
        public const string Frame_Field_RegList = "RegList";

        #endregion

        #region 辅助函数

        /// <summary>
        /// 命令下发默认的帧类型
        /// </summary>
        /// <returns></returns>
        public static byte GetByte_DefaultCommandFrameType()
        {
            return 0b00001101;
        }


        /// <summary>
        /// MessageID 状态位清零
        /// </summary>
        public static void ResetMessageID()
        {
            _MessageId = 0;
        }

        /// <summary>
        /// MessageId：顺序生成
        /// </summary>
        /// <returns></returns>
        public static int NextMessageID()
        {
            lock (_lock)
            {
                _MessageId++;
                if (_MessageId >= 65536)
                {
                    _MessageId = 0;
                }
            }

            return _MessageId;
        }

        /// <summary>
        /// 获取MessageId的字节数组：顺序生成 
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBytes_NextMessageID()
        {
            return DataHelper.UInt16ToByte2(NextMessageID());
        }

        /// <summary>
        /// 获取MessageID的字节数组：随机生成
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBytes_RandMessageID()
        {
            var a = new Random().Next(0, Max_MessageID);
            return DataHelper.UInt16ToByte2(a);
        }


        public static byte[] GetBytes_DateTimeList(List<DateTime> dtList)
        {
            var byteList = new List<byte>();

            foreach (var dt in dtList)
            {
                byteList.AddRange(DataHelper.UInt16ToByte2(dt.Year));
                byteList.Add((byte)dt.Month);
                byteList.Add((byte)dt.Day);
                byteList.Add((byte)dt.DayOfWeek);
                byteList.Add((byte)dt.Hour);
                byteList.Add((byte)dt.Minute);
                byteList.Add((byte)dt.Second);
            }

            return byteList.ToArray();
        }

        public static byte[] GetBytes_DateTime(DateTime dt)
        {
            var byteList = new List<byte>();

            byteList.AddRange(DataHelper.UInt16ToByte2(dt.Year));
            byteList.Add((byte)dt.Month);
            byteList.Add((byte)dt.Day);
            byteList.Add((byte)dt.DayOfWeek);
            byteList.Add((byte)dt.Hour);
            byteList.Add((byte)dt.Minute);
            byteList.Add((byte)dt.Second);

            return byteList.ToArray();
        }

        public static UInt32 GetInt_IPAddress(byte address0, byte address01, byte address02, byte address03)
        {
            var a = new byte[] { address0, address01, address02, address03 };

            var hstr = DataHelper.BytesToHexStr(a);

            return Convert.ToUInt32(hstr, 16);
        }

        /// <summary>
        /// 解析出字节数组中的帧类型
        /// </summary>
        public static MoonsNBFrameType ParseFrameType(byte[] byteData)
        {
            if (byteData.Length < 2)
            {
                return null;
            }

            var b = byteData[1];
            var frameType = new MoonsNBFrameType();

            frameType.CommandType = (b & 1) == 1 ? 1 : 0;
            frameType.ActiveReport = (b & 2) == 2 ? 1 : 0;
            frameType.HasTimeStamp = (b & 4) == 4 ? 1 : 0;
            frameType.HasDeviceAddress = (b & 8) == 8 ? 1 : 0;
            frameType.HasUUID = (b & 16) == 16 ? 1 : 0;

            return frameType;
        }

        /// <summary>
        /// 生成UUID
        /// </summary>
        public static byte[] GetBytes_NewUUID()
        {
            return Guid.NewGuid().ToByteArray();
        }

        /// <summary>
        /// UUID字节数组转字符串
        /// </summary
        public static string GetString_UUID(byte[] bytes)
        {
            return DataHelper.BytesToHexStr(bytes);
        }


        #endregion

        #region 直接调光(0x03)

        public static byte[] GetBytes_Dimming(int dimmingValue, byte[] uuid)
        {
            return GetBytes_Cmd03(dimmingValue, uuid);
        }

        public static byte[] GetBytes_Cmd03(int dimmingValue, byte[] uuid, byte channel = 0x00)
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            if (uuid == null || uuid.Length == 0)
            {
                uuid = GetBytes_NewUUID();
            }
            byteList.AddRange(uuid);

            //Cmd
            byteList.Add(0x03);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            //Len

            //PD
            bytePD.Add(channel);
            //0x01 正常调光
            bytePD.Add(0x01);
            bytePD.AddRange(DataHelper.UInt16ToByte2(dimmingValue));
            bytePD.AddRange(bytesDateTime);

            //CRC
            byteList.AddRange(DataHelper.UInt16ToByte2(bytePD.Count));
            byteList.AddRange(bytePD);

            bytePD = null;

            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();
        }

        #endregion

        #region 参数设置(0x04)

        public static byte[] GetBytes_Cmd04_Write(Hashtable ht, byte channel = 0xFF)
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x04);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            foreach (byte key in ht.Values)
            {
                //服务器IP地址 U32
                if (key == 0x05)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x04);
                    bytePD.AddRange(DataHelper.UIntToByte4((UInt32)ht[key]));
                }
                //服务器IP地址 U16
                else if (key == 0x06)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x02);
                    bytePD.AddRange(DataHelper.UInt16ToByte2((int)ht[key]));
                }
                // Band No
                else if (key == 0x07)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x01);
                    bytePD.Add((byte)ht[key]);
                }
                //调光值,(0 - 255)
                else if (key == 0x0D)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x01);
                    bytePD.Add((byte)ht[key]);
                }
                //通道工作时间,单位Minut read/reset
                else if (key == 0x12)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x04);
                    bytePD.AddRange(DataHelper.UIntToByte4((UInt32)ht[key]));
                }
                //通道总耗电量,单位0.1kW.h,read/reset 
                else if (key == 0x13)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x04);
                    bytePD.AddRange(DataHelper.UIntToByte4((UInt32)ht[key]));
                }
                //上报间隔,单位S, U16
                else if (key == 0x16)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x04);
                    bytePD.AddRange(DataHelper.UIntToByte4((UInt32)ht[key]));
                }
                //0x18    read / reset  U32 收发的数据包数
                else if (key == 0x18)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x04);
                    bytePD.AddRange(DataHelper.UIntToByte4((UInt32)ht[key]));
                }
                //0x29    Read / write  U16 过电流告警阈值
                //0x2A    Read / write  U16 过功率告警阈值
                //0x2B    Read / write  U16 低电流告警阈值
                //0x2C    Read / write  U16 低功率告警阈值
                //0x2D    Read / write  U16 电容告警阈值
                else if (key == 0x29 || key == 0x2A || key == 0x2B || key == 0x2C || key == 0x2D)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x02);
                    bytePD.AddRange(DataHelper.UInt16ToByte2((UInt16)ht[key]));
                }

                // 组0=Org 组1=Area 组2=Line 组3=Section 组4=Group
                else if (key == 0x2E || key == 0x2F || key == 0x30 || key == 0x31 || key == 0x32
                    || key == 0x33 || key == 0x34 || key == 0x35)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x04);
                    bytePD.AddRange(DataHelper.UIntToByte4((long)ht[key]));
                }
                // 帧格式地址域, 默认值是1
                else if (key == 0x36 || key == 0x37)
                {
                    bytePD.Add(key);
                    bytePD.Add(channel);
                    bytePD.Add(0x01);
                    bytePD.Add((byte)ht[key]);
                }
                else
                {
                    continue;
                }
            }

            //CRC
            byteList.AddRange(DataHelper.UInt16ToByte2(bytePD.Count));
            byteList.AddRange(bytePD);

            bytePD = null;

            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();
        }

        #endregion

        #region 时间同步(0x05)

        public static byte[] GetBytes_Cmd05(DateTime dt)
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(GetBytes_DateTime(DateTime.Now));

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x05);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            //Len

            //PD
            bytePD.AddRange(GetBytes_DateTime(DateTime.Now));

            //CRC
            byteList.AddRange(DataHelper.UInt16ToByte2(bytePD.Count));
            byteList.AddRange(bytePD);

            bytePD = null;

            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();
        }

        #endregion

        #region 参数读取命令(0x07)

        public static byte[] GetBytes_Cmd07_Read(List<Register> registerList)
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x07);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            foreach (var reg in registerList)
            {
                bytePD.Add(reg.Key);
                bytePD.Add(reg.Channel);
            }

            //CRC
            byteList.AddRange(DataHelper.UInt16ToByte2(bytePD.Count));
            byteList.AddRange(bytePD);

            bytePD = null;

            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();


        }

        public static byte[] GetBytes_Cmd07_Read(Hashtable ht, byte channel = 0xFF)
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x07);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            foreach (byte key in ht.Values)
            {
                bytePD.Add(key);
                bytePD.Add(channel);
            }

            //CRC
            byteList.AddRange(DataHelper.UInt16ToByte2(bytePD.Count));
            byteList.AddRange(bytePD);

            bytePD = null;

            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();


        }

        #endregion

        #region Parse 参数读取(0x07)

        public static Hashtable Parse_Cmd07_Read(byte[] byteData)
        {
            var ht = new Hashtable();
            var frameType = ParseFrameType(byteData);
            var hstr = string.Empty;
            var startIndex = 2;
            var pdLen = 0;
            var byteDataLen = byteData.Length;
            ht.Add(Frame_Field_FrameType, frameType);

            // 解析设备物理地址
            if (frameType.HasDeviceAddress == 1)
            {
                hstr = DataHelper.BytesToHexStr(byteData.Skip(startIndex).Take(8).ToArray());
                ht.Add(Frame_Field_Address, Convert.ToUInt64(hstr, 16));
                startIndex += 8;
            }

            // 解析时间搓
            if (frameType.HasTimeStamp == 1)
            {
                hstr = DataHelper.BytesToHexStr(byteData.Skip(startIndex).Take(8).ToArray());
                ht.Add(Frame_Field_TimeStamp, Convert.ToUInt64(hstr, 16));
                startIndex += 8;
            }

            // 解析UUID
            if (frameType.HasUUID == 1)
            {
                hstr = DataHelper.BytesToHexStr(byteData.Skip(startIndex).Take(16).ToArray());
                ht.Add(Frame_Field_UUID, Convert.ToUInt64(hstr, 16));
                startIndex += 16;
            }

            ht.Add(Frame_Field_Cmd, byteData.Skip(startIndex).Take(1).ToArray());
            startIndex++;

            hstr = DataHelper.BytesToHexStr(byteData.Skip(startIndex).Take(2).ToArray());
            ht.Add(Frame_Field_MessageID, Convert.ToUInt16(hstr, 16));
            startIndex += 2;

            hstr = DataHelper.BytesToHexStr(byteData.Skip(startIndex).Take(2).ToArray());
            pdLen = Convert.ToUInt16(hstr, 16);
            ht.Add(Frame_Field_PDLen, pdLen);
            startIndex += 2;

            //开始解析读取RegisterList
            var regList = new List<Register>();
            while (startIndex < byteDataLen - 3)
            {
                var reg = new Register();
                reg.Key = byteData.Skip(startIndex).Take(1).ToArray()[0];
                reg.Channel = byteData.Skip(startIndex + 1).Take(1).ToArray()[0];
                reg.ErrCode = byteData.Skip(startIndex + 2).Take(1).ToArray()[0];
                reg.Len = (int)byteData.Skip(startIndex + 3).Take(1).ToArray()[0];
                reg.ByteData = byteData.Skip(startIndex + 4).Take(reg.Len).ToArray();
                startIndex += 4 + reg.Len;
                if (reg.Key == 0x01 || reg.Key == 0x02)
                {
                    reg.Value = reg.ByteData[0];
                }
                else if (reg.Key == 0x04 || reg.Key == 0x05)
                {
                    reg.Value = Convert.ToUInt32(DataHelper.BytesToHexStr(reg.ByteData), 16);
                }
                else
                {
                    continue;
                }
                //ht.Add(key, byteData.Skip(startIndex + 1).Take(1).ToArray());
                //startIndex +=2;
            }

            return ht;
        }

        #endregion

        #region 全年调光计划设置(0x09)

        public static byte[] GetBytes_SingleLightTimePlan(SingleLightDimmingScheme SingleLightDimmingScheme)
        {
            return GetBytes_Cmd09(SingleLightDimmingScheme);
        }

        /// <summary>
        /// 全年调光计划设置(0x09) 组包
        /// </summary>
        public static byte[] GetBytes_Cmd09(SingleLightDimmingScheme SingleLightDimmingScheme)
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x03);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            //PD 
            //经度、纬度
            bytePD.AddRange(DataHelper.UIntToByte4((int)(SingleLightDimmingScheme.Lon * 100 * 10000)));
            bytePD.AddRange(DataHelper.UIntToByte4((int)(SingleLightDimmingScheme.Lat * 100 * 10000)));

            //网关时区，例如 +-8
            bytePD.Add((byte)SingleLightDimmingScheme.BaseUtcOffset);

            foreach (var m in SingleLightDimmingScheme.SchemeItems)
            {
                // 夏令时
                if (m.Item_Mode == 7)
                {
                    //模式
                    //bytePD.Add((byte)m.Item_Mode);

                    //周掩码
                    //bytePD.Add((byte)m.Item_Week);

                    //夏令时
                    //if (SingleLightDimmingScheme.RuleForDayLight.IsFixedDateRule)
                    //{
                    //    var hstr = $"{SingleLightDimmingScheme.RuleForDayLight.StartMonth.ToString("X")}"
                    //        + $"{ SingleLightDimmingScheme.RuleForDayLight.StartDay.ToString("X2") }"
                    //        + $"{SingleLightDimmingScheme.RuleForDayLight.EndMonth.ToString("X")}"
                    //        + $"{ SingleLightDimmingScheme.RuleForDayLight.EndDay.ToString("X2") }";
                    //    bytePD.AddRange(DataHelper.HexStringToByte(hstr));
                    //    bytePD.Add((byte)SingleLightDimmingScheme.RuleForDayLight.DaylightDelta);
                    //}
                    //else
                    //{
                    //    var hstr = $"{SingleLightDimmingScheme.RuleForDayLight.StartMonth.ToString("X")}{SingleLightDimmingScheme.RuleForDayLight.StartWeekofMonth.ToString("X")}"
                    //        + $"{SingleLightDimmingScheme.RuleForDayLight.StartDayOfWeek.ToString("X")}{SingleLightDimmingScheme.RuleForDayLight.EndMonth.ToString("X")}"
                    //        + $"{SingleLightDimmingScheme.RuleForDayLight.EndWeekofMonth.ToString("X")}{SingleLightDimmingScheme.RuleForDayLight.EndDayOfWeek.ToString("X")}";
                    //    bytePD.AddRange(DataHelper.HexStringToByte(hstr));
                    //    bytePD.Add((byte)SingleLightDimmingScheme.RuleForDayLight.DaylightDelta);
                    //}
                }
                else
                {
                    bytePD.Add((byte)m.Item_Mode);
                    bytePD.Add((byte)m.Item_Week);
                    bytePD.AddRange(DataHelper.UInt16ToByte2(m.Item_Time));
                    bytePD.Add((byte)m.Item_Channel);
                    bytePD.Add((byte)m.Item_DimmingValue);
                }
            }

            //Length
            byteList.AddRange(DataHelper.UInt16ToByte2(bytePD.Count));

            //PD
            byteList.AddRange(bytePD);

            //CRC
            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //结束标志
            byteList.Add(IotToDevice_TailByte);

            bytePD = null;

            return byteList.ToArray();
        }

        #endregion

        #region 全年调光计划查询(0x0A)
        public static byte[] GetBytes_Cmd0A()
        {
            var byteList = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x0A);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            //len
            byteList.AddRange(new byte[] { 0x00, 0x00 });

            //CRC
            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();
        }

        #endregion

        #region 预约时控计划(0x0F)
        public static byte[] GetBytes_Cmd0F()
        {
            return null;
        }

        #endregion

        #region 下载固件包(0x0C)

        public static byte[] GetBytes_Cmd0C(int fileSize, int hasSent, int packSize, byte[] packData)
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x0C);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            bytePD.AddRange(DataHelper.UIntToByte4(fileSize));
            bytePD.AddRange(DataHelper.UIntToByte4(hasSent));
            bytePD.AddRange(DataHelper.UInt16ToByte2((UInt16)packSize));
            bytePD.AddRange(packData);


            //CRC
            byteList.AddRange(DataHelper.UInt16ToByte2(bytePD.Count));
            byteList.AddRange(bytePD);

            bytePD = null;

            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();


        }

        #endregion

        #region 设备重启(0x0D)

        public static byte[] GetBytes_Cmd0D()
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x0D);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            //len
            byteList.AddRange(new byte[] { 0x00, 0x00 });

            //CRC
            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();
        }

        #endregion

        #region 查询预约时控计划(0x10)

        public static byte[] GetBytes_Cmd10()
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x10);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            //len
            byteList.AddRange(new byte[] { 0x00, 0x00 });

            //CRC
            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();
        }

        #endregion

        #region  取消预约计划(0x11)

        public static byte[] GetBytes_Cmd11(int mode, List<DateTime> dtList)
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x03);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            //PD
            bytePD.Add((byte)mode);

            if (mode == 01)
            {
                bytePD.AddRange(GetBytes_DateTimeList(dtList));
            }

            //CRC
            byteList.AddRange(DataHelper.UInt16ToByte2(bytePD.Count));
            byteList.AddRange(bytePD);

            bytePD = null;

            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();
        }

        #endregion

        #region 查询当天时控计划(0x12)

        public static byte[] GetBytes_Cmd12()
        {
            var byteList = new List<byte>();
            var bytePD = new List<byte>();
            var bytesDateTime = GetBytes_DateTime(DateTime.Now);

            //Head
            byteList.Add(IotToDevice_HeadByte);

            //FrameType
            byteList.Add(GetByte_DefaultCommandFrameType());

            //Time
            byteList.AddRange(bytesDateTime);

            //UUID
            byteList.AddRange(GetBytes_NewUUID());

            //Cmd
            byteList.Add(0x12);

            //MessageID
            byteList.AddRange(GetBytes_RandMessageID());

            //Ack
            byteList.Add(0x00);

            //len
            byteList.AddRange(new byte[] { 0x00, 0x00 });

            //CRC
            byteList.AddRange(DataHelper.GetCRC16_2Bytes(byteList.ToArray()));

            //Tail
            byteList.Add(IotToDevice_TailByte);

            return byteList.ToArray();
        }

        #endregion
    }
}
