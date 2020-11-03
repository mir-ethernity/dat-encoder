using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mir.Ethernity.Dat
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var inputEncodedData = await File.ReadAllBytesAsync(@"example.dat");
            var decodedData = DatEncoder.Decode(inputEncodedData);
            var encodedData = DatEncoder.Encode(decodedData);

            if (inputEncodedData.Length != encodedData.Length)
            {
                Console.WriteLine($"ERROR: Mismatch length between original encoded data and rencoded after decoded");
                return;
            }

            for(var i = 0; i < inputEncodedData.Length; i++)
            {
                if(inputEncodedData[i] != encodedData[i])
                {
                    Console.WriteLine($"ERROR: Byte position {i} is not match, original: {inputEncodedData[i]}, rencoded: {encodedData[i]}");
                    return;
                }
            }

            Console.WriteLine("Decode and Encode works");
        }
    }
}
