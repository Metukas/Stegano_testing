using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace LSBstegano
{
    public class Stegano
    {
        public enum UsedBitCount
        {
            One = 1,
            Two = 2,
        }

        const int BYTE_COUNT_IN_PIXEL = 4;
        const int ALPHA_CHANNEL_LIMIT = 255;

        Bitmap GetBitmap(string fileName)
        {
            Image img = Image.FromFile(fileName);
            Bitmap bitmap = new Bitmap(img);
            return bitmap;
        }

        public string ReadImage(string imageFileName, UsedBitCount usedBitCount)
        {
            bool pushTwoBitPerByte = false;
            switch(usedBitCount)
            {
                case UsedBitCount.One:
                    pushTwoBitPerByte = false;
                    break;
                case UsedBitCount.Two:
                    pushTwoBitPerByte = true;
                    break;
            }

            Bitmap bitmap = GetBitmap(imageFileName);
            BitCollector bitCollector = new BitCollector();
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);

                    // alpha chanelis daro kažkokias nesąmones :D. Reiks išsiaiškint
                    if (pixel.A < ALPHA_CHANNEL_LIMIT)
                    {
                        continue;
                    }

                    if (pushTwoBitPerByte)
                    {
                        bitCollector.PushBit((pixel.R >> 1) & 0b0000_0001);
                    }
                    bitCollector.PushBit(pixel.R & 0b0000_0001);

                    if (pushTwoBitPerByte)
                    {
                        bitCollector.PushBit((pixel.G >> 1) & 0b0000_0001);
                    }
                    bitCollector.PushBit(pixel.G & 0b0000_0001);

                    if (pushTwoBitPerByte)
                    {
                        bitCollector.PushBit((pixel.B >> 1 )& 0b0000_0001);
                    }
                    bitCollector.PushBit(pixel.B & 0b0000_0001);
                }
            }

            return Encoding.ASCII.GetString(bitCollector.GetBuffer());
        }

        public void EncodeMessage(string message, string imageFileName, UsedBitCount bitCountToUse)
        {
            Bitmap bitmap = GetBitmap(imageFileName);
            bitmap.Save(imageFileName + ".orig");
            BitStream msgData = new BitStream(Encoding.ASCII.GetBytes(message));

            //jeigu neužtenka baitų paveikslėlyje
            if(bitmap.Width * bitmap.Height < (msgData.Length / (int)bitCountToUse))
            {
                throw new Exception("image not big enough for message to fit"); //TODO (never :) ) handle dis
            }

            // j i ---> 
            // |
            // *
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color pixelColor = bitmap.GetPixel(i, j);

                    // alpha chanelis daro kažkokias nesąmones :D. Reiks išsiaiškint
                    if(pixelColor.A < ALPHA_CHANNEL_LIMIT)
                    {
                        continue;
                    }

                    var (isEnd, newPixel) = InjectDataIntoPixel(pixelColor, msgData, bitCountToUse);

                    bitmap.SetPixel(i, j, newPixel);

                    // jeigu nebeturim daugiau message
                    if(isEnd)
                    {
                        bitmap.Save(imageFileName + ".steg");
                        return;
                    }
                }
            }

        }

        // byte data naudoja tik (usedBitCount) kiekį duomenų
        public (bool end, Color pixel) InjectDataIntoPixel(Color pixelToSet, BitStream data, UsedBitCount usedBitCount)
        {
            byte shiftInCount = (byte)usedBitCount;

            var (end, temp) = ShiftInBytes(data, shiftInCount);
            int newR = ZeroOutBottomBits(pixelToSet.R, shiftInCount) | temp;
            (end, temp) = ShiftInBytes(data, shiftInCount);
            int newG = ZeroOutBottomBits(pixelToSet.G, shiftInCount) | temp;
            (end, temp) = ShiftInBytes(data, shiftInCount);
            int newB = ZeroOutBottomBits(pixelToSet.B, shiftInCount) | temp;

            return (end, Color.FromArgb(pixelToSet.A, newR, newG, newB));
        }



        /////////////////////////////////////////////////////////////////////////////////////////
        // Išiftina bitus iš BitStreamo į baitą
        // jeigu bitStreamas baigias, išiftinamas nulis
        public (bool end, byte b) ShiftInBytes(BitStream bitStream, int count)
        {
            byte finalByte = 0;
            byte nextBit = 0;
            for (int i = 0; i < count; i++)
            {
                finalByte = (byte)(finalByte << 1);
                nextBit = bitStream.NextBit();
                finalByte += (byte)(nextBit == 255 ? 0 : nextBit);
            }

            // bitstreamo pabaiga, jeigu bitstreamas grąžina 255
            return (nextBit == 255, finalByte);
        }

        IEnumerable<byte> GetFormatedData(BitStream bitStream, UsedBitCount usedBitCount)
        {
            int count = 0;
            switch (usedBitCount)
            {
                case UsedBitCount.One:
                    count = 1;
                    break;
                case UsedBitCount.Two:
                    count = 2;
                    break;
            }

            bool isEnd;
            byte shiftedByte;
            do
            {
                // count * 3 nes piksely yra 3 baitai: RGB, o mes naudojam count kiekį bitų vienam baitui
                (isEnd, shiftedByte) = ShiftInBytes(bitStream, count * 3);
                yield return shiftedByte;
            }while (!isEnd);

        }

        int ZeroOutBottomBits(int num, int count)
        {
            if (count >= 8)
            { 
                return 0;
            }
            return (num >> count) << count;
        }

    }
}
