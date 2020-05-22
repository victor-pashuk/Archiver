namespace GZipTestApplication
{
    public class ByteBlock
    {
        public byte[] Buffer { get; }
        public long Position { get; set; }
        public int ActualLength { get; set; }

        public ByteBlock(byte[] buffer, long position)
        {
            Buffer = buffer;
            Position = position;
        }

        public ByteBlock(byte[] buffer, long position, int actualLength)
        {
            Buffer = buffer;
            Position = position;
            ActualLength = actualLength;
        }
    }
}
