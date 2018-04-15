using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBstegano
{
    public class BitCollector
    {
        List<byte> buffer;
        int nextByteIndex = 0;
        int nextBitIndex = 0;

        public BitCollector()
        {
            buffer = new List<byte>{ 0 };
        }

        public void PushBit(int bit)
        {
            if (nextBitIndex >= 8)
            {
                nextBitIndex = 0;
                nextByteIndex++;
                buffer.Add(0);
            }

            byte bitToPush = (byte)(bit & 0b0000_0001);
            buffer[nextByteIndex] <<= 1;
            buffer[nextByteIndex] += bitToPush;
            nextBitIndex++;
        }

        public byte[] GetBuffer()
        {
            return buffer.ToArray();
        }
    }
}
