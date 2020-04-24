namespace Led.MoonsNBCmdWrapper
{
    public class Register
    {
        public byte Key { get; set; }

        public byte Channel { get; set; }

        public object Value { get; set; }

        public byte ErrCode { get; set; }

        public int Len { get; set; }

        public byte[] ByteData { get; set; }
    }
}
