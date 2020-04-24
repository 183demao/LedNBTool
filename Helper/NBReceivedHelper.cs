using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.Helper
{
    public class NBReceivedHelper
    {
        /// <summary>
        /// 解析报文
        /// 设备(device) -> IoT 
        /// </summary>
        public static UploadOriginData AnalyzeMessage(byte[] messages)
        {
            var messages1 = new byte[] {
                0x95,//帧头
                0x03,//地址域
                0x0E,0x12,0x34,0x01,0x79,0x00,0x00,0x00,//地址域
                0x00,0x06,0xA5,0x5A,0x00,0x21,0x04,0x00,//时间戳
                0x00,//命令码
                0x0E,0x01,//消息索引
                0x00,//messages.Length - 29
                0x20,0x04,//长度
                0x00,0x00,0x0F,0x01,0x00,0x0C,0x04,0x00,0x05,0x09,
                0x10,0x00,0x0C,0x04,0x00,0x12,0x76,0x90,0x00,0x16,
                0x02,0x00,0x78,0x0F,0x30,0x36,0x38,0x37,0x35,0x36,
                0x37,0x34,0x30,0x30,0x31,0x34,0x31,0x36,0x38,0x00,
                0x09,0x0F,0x30,0x33,0x30,0x39,0x30,0x38,0x38,0x31,
                0x30,0x37,0x34,0x30,0x30,0x36,0x34,0x00,0x19,0x00,
                0x00,0x07,0x00,0x0A,0x04,0x62,0x6E,0x74,0x63,0x00,
                0x04,0x04,0xBC,0x4D,0xDE,0x6F,0x00,0x05,0x04,0xC7,
                0xFF,0x70,0x7A,0x00,0x06,0x01,0x5B,0x00,0x2E,0x04,
                0x00,0x00,0x00,0x00,0x00,0x2F,0x04,0x00,0x00,0x00,
                0x00,0x00,0x30,0x04,0x00,0x00,0x00,0x00,0x00,0x31,
                0x04,0x00,0x00,0x00,0x00,0x00,0x32,0x04,0x00,0x00,
                0x00,0x00,0x00,0x33,0x04,0x00,0x00,0x00,0x00,0x00,
                0x34,0x04,0x00,0x00,0x00,0x00,0x00,0x35,0x04,0x00,
                0x00,0x00,0x00,
                0x01,0xBC,//crc16
                0x59//帧尾
            };
            UploadOriginData uploadOriginData = new UploadOriginData();
            try
            {
                var index = 0;
                //组装类
                uploadOriginData.frameHeader = messages[index];//0
                uploadOriginData.frameType = messages[index += 1];//1
                //帧类型
                string frametype = Convert.ToString(uploadOriginData.frameType, 2).PadLeft(8, '0');
                uploadOriginData.hasAddress = frametype.Substring(4, 1) == "1";
                uploadOriginData.hasTimeStramp = frametype.Substring(5, 1) == "1";
                uploadOriginData.actived = frametype.Substring(6, 1) == "1";
                uploadOriginData.requested = frametype.Substring(7, 1) == "1";
                index += 1;//地址域和时间戳存在不确定性， 3
                if (uploadOriginData.hasAddress && uploadOriginData.hasTimeStramp)//如果存在地址域
                {
                    uploadOriginData.addressDomain = GetHexBytes(messages, index, 8);
                    uploadOriginData.timeStamp = GetHexBytes(messages, (index += 8), 8);//11
                    index += 8;//19
                }
                else
                {
                    if (uploadOriginData.hasAddress)//如果存在地址域
                    {
                        uploadOriginData.addressDomain = GetHexBytes(messages, index, 8);
                        index += 8;
                    }
                    if (uploadOriginData.hasTimeStramp)//如果存在时间戳
                    {
                        uploadOriginData.timeStamp = GetHexBytes(messages, index, 8);
                        index += 8;
                    }
                }
                uploadOriginData.commandCode = messages[index];//19
                uploadOriginData.messsageId = GetHexBytes(messages, (index += 1), 2); //19
                uploadOriginData.ack = messages[index += 2];//21
                uploadOriginData.length = GetHexBytes(messages, (index += 1), 2);//22
                uploadOriginData.data = GetHexBytes(messages, (index += 2), (messages.Length - index - 3));//24
                uploadOriginData.crc = GetHexBytes(messages, messages.Length - 3, 2);
                uploadOriginData.frameTail = messages[messages.Length - 1];
            }
            catch (Exception)
            {
                throw;
            }

            return uploadOriginData;
        }

        public static void GetUploadEntity(Dictionary<byte, UploadEntity> result, byte[] originData, int index = 0)
        {
            try
            {
                result = result ?? new Dictionary<byte, UploadEntity>();
                UploadEntity uploadData = new UploadEntity();
                uploadData.ChannelNumber = originData[index];
                uploadData.MemeroyID = originData[index + 1];
                uploadData.MemeroyLength = originData[index + 2];
                uploadData.MemeroyData = GetHexBytes(originData, index + 3, uploadData.MemeroyLength);
                if (!result.ContainsKey(uploadData.MemeroyID)) result.Add(uploadData.MemeroyID, uploadData);
                index = index + 3 + uploadData.MemeroyLength;
                if (index > originData.Length - 1) return;
                GetUploadEntity(result, originData, index);
            }
            catch (Exception ex)
            {
                Console.Write($"Index:{index}");
            }
        }
        private static string GetHexString(byte[] data, int startIndex, int endIndex)
        {
            string result = string.Empty;
            for (int i = startIndex; i < endIndex; i++)
            {
                result += Convert.ToString(data[i], 16).PadLeft(2, '0');
            }
            return result;
        }

        private static byte[] GetHexBytes(byte[] data, int startIndex, int length)
        {
            List<byte> result = new List<byte>();
            for (int i = startIndex; i < startIndex + length; i++)
            {
                result.Add(data[i]);
            }
            return result.ToArray();
        }

    }
}
