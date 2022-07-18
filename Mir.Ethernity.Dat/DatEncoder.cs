using System;
using System.Runtime.CompilerServices;

namespace Mir.Ethernity.Dat
{
    public static class DatEncoder
    {
        private const uint ChecksumMask = 0x9fde1a93;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ChecksumByte(byte value, uint index) => (uint)(((byte)(value & 0x0ff) + 0x1) * index);

        public static byte[] Decode(byte[] input)
        {
            var sourceMask = BitConverter.ToUInt32(input, 0);
            sourceMask = (sourceMask & 0x000000FFU) << 24 | (sourceMask & 0x0000FF00U) << 8 | (sourceMask & 0x00FF0000U) >> 8 | (sourceMask & 0xFF000000U) >> 24;

            var output = new byte[input.Length - 8];
            Array.Copy(input, 8, output, 0, output.Length);
            MaskData(sourceMask, output);

            var sourceChecksum = BitConverter.ToInt32(input, 4);

            return output;
        }

        public static byte[] Encode(byte[] input)
        {
            var data = new byte[input.Length];
            Array.Copy(input, data, input.Length);

            var output = new byte[input.Length + 8];
            var mask = GenerateMask((ushort)input.Length);
            MaskData(mask, data);
            uint checksum = 0;

            for (uint i = 0; i < data.Length; i++)
                checksum += ChecksumByte(data[i], i);

            checksum ^= ChecksumMask;

            Array.Copy(BitConverter.GetBytes(mask), 0, output, 0, 4);
            Array.Copy(BitConverter.GetBytes(checksum), 0, output, 4, 4);
            Array.Copy(data, 0, output, 8, data.Length);

            return output;
        }

        private static uint GenerateMask(ushort length)
        {
            return BitConverter.ToUInt32(new byte[]
            {
                0xF0,
                0x39,
                (byte)(length >> 8 ^ 0xAB),
                (byte)(length & 0xFF ^ 0x8E),
            }, 0);
        }

        private static void MaskData(uint mask, byte[] data)
        {
            var maskBuffer = BitConverter.GetBytes(mask);
            for (var m = 0; m < 4; m++)
            {
                var flag = maskBuffer[m];

                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)(data[i] ^ flag);
                    flag += 0x1;
                }
            }
        }
    }
}
