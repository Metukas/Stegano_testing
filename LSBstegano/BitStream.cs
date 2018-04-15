using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBstegano
{
    public class BitStream
    {
        int bytePtr = 0;
        int bitPtr = 0;
        int GetNextBitPos()
        {
            if (bitPtr >= 8)
            {
                bitPtr = 0;
                ++bytePtr;
            }

            int toReturn = bitPtr;
            ++bitPtr;          
            return toReturn;
        }
        byte[] buffer;

        public int Length { get => buffer.Length * 8; }

        public BitStream(byte[] buffer)
        {
            this.buffer = buffer;
        }

        // grąžina:
        // 0000 0001 - jei bitas = 1
        // 0000 0000 - jei bitas = 0
        // 1111 1111 - jeigu bitstreamo pabaiga (nebėra bitų)
        public byte NextBit()
        {
            const int LAST_BIT_INDEX = 7;
            int shiftAmount = LAST_BIT_INDEX - GetNextBitPos();

            if(bytePtr >= buffer.Length)
            {
                return 255;
            }

            int currentByte = buffer[bytePtr];

            int bitInPosition = (currentByte >> shiftAmount);
            return (byte)(bitInPosition & 0b0000_0001);
        }
    }
}
