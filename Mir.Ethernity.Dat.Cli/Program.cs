using System;
using System.IO;

namespace Mir.Ethernity.Dat.Cli
{
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
            byte[] outputBuffer = (args[0]) switch
            {
                "-d" => DatEncoder.Decode(inputBuffer),
                "-e" => DatEncoder.Encode(inputBuffer),
                _ => throw new NotImplementedException(),
            };

            try
            {
                File.WriteAllBytes(destFile, outputBuffer);
                Console.WriteLine($"File {(args[0] == "-d" ? "decoded" : "encoded")} successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: An error ocurred writing file: {ex.Message}");
            }

        }
    }
}
