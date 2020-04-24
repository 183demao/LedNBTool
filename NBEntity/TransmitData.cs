using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.NBEntity
{
    public class TransmitData
    {
        public string Topic { get; set; }
        public Guid UUID { get; set; }
        public string CommandCode { get; set; }
        public int MesssageID { get; set; }
        public byte[] Data { get; set; }
    }
}
