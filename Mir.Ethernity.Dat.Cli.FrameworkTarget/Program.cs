using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mir.Ethernity.Dat.Cli.FrameworkTarget
{
    public static class DatEncoder
    {
        private const uint ChecksumMask = 0x9fde1a93;

        public static byte[] Decode(byte[] input)
        {
            var sourceMask = BitConverter.ToUInt32(input, 0);
            sourceMask = (sourceMask & 0x000000FFU) << 24 | (sourceMask & 0x0000FF00U) << 8 | (sourceMask & 0x00FF0000U) >> 8 | (sourceMask & 0xFF000000U) >> 24;

            var output = new byte[input.Length - 8];
            Array.Copy(input, 8, output, 0, output.Length);
            output = MaskData(sourceMask, output);

            var sourceChecksum = BitConverter.ToInt32(input, 4);

            return output;
        }

        public static byte[] Encode(byte[] input)
        {
            var data = new byte[input.Length];
            Array.Copy(input, data, input.Length);

            var output = new byte[input.Length + 8];
            var mask = GenerateMask((ushort)input.Length);
            data = MaskData(mask, input);
            uint checksum = 0;

            for (uint i = 0; i < data.Length; i++)
                checksum += (uint)(((byte)(data[i] & 0x0ff) + 0x1) * i);

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

        private static byte[] MaskData(uint mask, byte[] data)
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

            return data;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3 || (args[0] != "-d" && args[0] != "-e"))
            {
                Console.WriteLine("Wrong num arguments, example usage:");
                Console.WriteLine(" - decode: dat-encoder -d \"CMList.dat\" \"CMList.txt\"");
                Console.WriteLine(" - encode: dat-encoder -e \"CMList.txt\" \"CMList.dat\"");
                return;
            }

            var sourceFile = args[1];
            var destFile = args[2];

            if (!File.Exists(sourceFile))
            {
                Console.WriteLine($"Source file: {sourceFile} not found");
                return;
            }

            byte[] inputBuffer = File.ReadAllBytes(sourceFile);
            byte[] outputBuffer;

            switch (args[0])
            {
                case "-d":
                    outputBuffer = DatEncoder.Decode(inputBuffer);
                    break;
                case "-e":
                    outputBuffer = DatEncoder.Encode(inputBuffer);
                    break;
                default:
                    throw new NotImplementedException();
            };

            try
            {
                File.WriteAllBytes(destFile, outputBuffer);
                Console.WriteLine($"File {(args[0] == "-d" ? "decoded" : "encoded")} successfully!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: An error ocurred writing file: {ex.Message}");
            }

        }
    }
}
