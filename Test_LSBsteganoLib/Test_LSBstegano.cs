using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LSBstegano;

namespace Test_LSBsteganoLib
{
    [TestClass]
    public class Test_LSBstegano
    {
        [TestMethod]
        public void Test_BitStream_NextBit()
        {
            byte[] buffer = { 0xAA, 0xBB, 0xCC };
            BitStream bitStream = new BitStream(buffer);
            Assert.IsTrue(bitStream.NextBit() == 1); // start 0xAA
            Assert.IsTrue(bitStream.NextBit() == 0);
            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 0);

            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 0);
            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 0); // end 0xAA


            Assert.IsTrue(bitStream.NextBit() == 1); // start 0xBB
            Assert.IsTrue(bitStream.NextBit() == 0);
            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 1);

            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 0);
            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 1); // end 0xBB


            Assert.IsTrue(bitStream.NextBit() == 1); // start 0xCC
            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 0);
            Assert.IsTrue(bitStream.NextBit() == 0);

            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 1);
            Assert.IsTrue(bitStream.NextBit() == 0);
            Assert.IsTrue(bitStream.NextBit() == 0); // end 0xCC

            Assert.IsTrue(bitStream.NextBit() == 255); //end
        }

        [TestMethod]
        public void Test_Stegano_ShiftInBytes()
        {
            Stegano steganography = new Stegano();
            byte[] buffer = { 0xAA };
            BitStream bitStream = new BitStream(buffer);
            var (_, b) = steganography.ShiftInBytes(bitStream, 5);
            Assert.IsTrue(b == 0b10101);
        }

        [TestMethod]
        public void Test_Stegano_InjectDataIntoPixel()
        {
            byte data = 0b0001_1001;
        }

        [TestMethod]
        public void Test_BitCollectorAndBitStream()
        {
            byte[] initialBuffer = new byte[] { 0xAA, 0x54 };
            BitStream bitStream = new BitStream(initialBuffer);
            BitCollector bitCollector = new BitCollector();
            byte nextBit = 0;
            while((nextBit = bitStream.NextBit()) != 255)
            {
                bitCollector.PushBit(nextBit);
            }
            Assert.AreEqual(initialBuffer[0], bitCollector.GetBuffer()[0]);
            Assert.AreEqual(initialBuffer[1], bitCollector.GetBuffer()[1]);

        }
    }
}
