using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSBstegano;

namespace ImageSteganography
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] buffer = { 0xAA, 0xCC };
            BitStream bitStream = new BitStream(buffer);
            byte bit = 0;
            bit = bitStream.NextBit();
            while (bit != 255)
            {
                Console.WriteLine(bit);
                bit = bitStream.NextBit();
            }

            //////////////////////////////////////////////
            string data = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio. Praesent libero. Sed cursus ante dapibus diam. Sed nisi. Nulla quis sem at nibh elementum imperdiet. Duis sagittis ipsum. Praesent mauris. Fusce nec tellus sed augue semper porta. Mauris massa. Vestibulum lacinia arcu eget nulla. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Curabitur sodales ligula in libero. Sed dignissim lacinia nunc. Curabitur tortor.";

            string png = "test.png";
            Stegano stegano = new Stegano();
            stegano.EncodeMessage(data, png, Stegano.UsedBitCount.One);

            string messageInFile = stegano.ReadImage(png + ".steg", Stegano.UsedBitCount.One);
            System.IO.File.WriteAllText("output.txt", messageInFile);
            Console.WriteLine(messageInFile);
        }
    }
}
