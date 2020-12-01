using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NbIotCmd.Helper
{
    public class HexFormatHelper
    {
        /// <summary>
        /// 将字符串的16进制转换成16进制数组
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static byte[] StringConvertHexBytes(string Data)
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < Data.Length; i++)
            {
                if (i % 2 == 0) list.Add(byte.Parse(Data.Substring(i, 2), NumberStyles.HexNumber));
            }
            return list.ToArray();
        }
        public static string HexBytesConvertString(byte[] Data)
        {
            return string.Join(string.Empty, from d in Data.ToList()
                                             select d.ToString("X2"));
        }
    }
}
