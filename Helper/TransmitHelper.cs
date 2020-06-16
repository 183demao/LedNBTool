using Led.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NbIotCmd.Helper
{
    public class TransmitHelper
    {
        public static byte[] SendNBComand(byte[] GUID, byte[] DATA)
        {
            // $"9A13{GUID}14000100{LENGTH}{DATA}{CRC}A9";
            List<byte> result = new List<byte>();
            result.Add(0x9A);
            result.Add(0x13);
            result.AddRange(GUID);//GUID
            result.Add(0x14);
            result.Add(0x00);
            result.Add(0x01);
            result.Add(0x00);
            var length = (DATA.Length);
            if (length > 255) result.AddRange(new byte[] { (byte)(length - 255), 0x00 });//Length
            else result.AddRange(new byte[] { 0x00, (byte)length });
            result.AddRange(DATA);//Length
            var CRC = DataHelper.GetCRC16_2Bytes(result.ToArray());
            result.AddRange(CRC);//CRC
            result.Add(0xA9);//
            return result.ToArray();
        }
        public static byte[] GetGroupHex(long val)
        {
            List<byte> res = new List<byte>();
            try
            {
                string HexStr = Convert.ToString(val, 16);
                HexStr = HexStr.Length % 2 != 0 ? "0" + HexStr : HexStr;
                HexStr = Regex.Replace(HexStr, @"(\d{2})", "$1 ");
                res = HexStr.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(D => Convert.ToByte(D, 16)).ToList();
            }
            catch { throw; }
            return res.ToArray();
        }
    }
}
