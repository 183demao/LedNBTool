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
        public void analyzeMessage(byte[] messages)
        {
            byte H0 = messages[0],//帧头
              H1 = messages[1];//帧类型
                               //H2 = messages[2],//地址域
                               //地址域
            string addressField = string.Empty;
            for (int i = 2; i < 10; i++)
            {
                addressField += Convert.ToString(messages[i], 16).PadLeft(2, '0');//  messages[i];
            }
            //H3 = messages[3],//时间戳
            string timeStamp = string.Empty;
            for (int i = 10; i < 18; i++)
            {
                timeStamp += Convert.ToString(messages[i], 16).PadLeft(2, '0');//  messages[i];
            }

            byte H4 = messages[18];//命令码
            string messageIndex = Convert.ToString(messages[19], 16).PadLeft(2, '0');
            messageIndex += Convert.ToString(messages[20], 16).PadLeft(2, '0');
            //H5 = messages[19],//消息索引
            //H5 = messages[19],//消息索引
            //H6 = messages[20],//消息响应
            //H7 = messages[7],//长度
            //H8 = messages[8],//数据
            //H9 = messages[9],//校验
            // H10 = messages[10],//帧尾
            //H11 = messages[11],
            //H12 = messages[12];

            Convert.ToString(H1, 2).PadLeft(8, '0');

        }
    }
}
