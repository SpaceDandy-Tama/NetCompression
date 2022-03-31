//MIT License

//Copyright (c) 2022 Oğuz Can Soyselçuk

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using SevenZip;

namespace LZMA_Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            byte[] textBytes = File.ReadAllBytes("../../../../Resources/Lorem Ipsum.txt");
            Console.WriteLine($"Original text is {textBytes.Length} bytes.");

            byte[] jpegBytes = File.ReadAllBytes("../../../../Resources/20170212.jpg");
            Console.WriteLine($"Original jpeg is {jpegBytes.Length} bytes.");

            byte[] tgaBytes = File.ReadAllBytes("../../../../Resources/mid.tga");
            Console.WriteLine($"Original tga is {tgaBytes.Length} bytes.");

            byte[] wavBytes = File.ReadAllBytes("../../../../Resources/Levianth & Axol - Remember (feat. The Tech Thieves) [NCS Release].wav");
            Console.WriteLine($"Original wav is {wavBytes.Length} bytes.");

            Stopwatch stopwatch = new Stopwatch();
            TimeSpan totalTimeSpent = TimeSpan.Zero;
            stopwatch.Start();

            byte[] textBytesLZMA = await LZMA.CompressAsync(textBytes);

            stopwatch.Stop();
            decimal ratio = Math.Truncate(10000m * ((decimal)textBytesLZMA.Length / textBytes.Length)) / 100m;
            Console.WriteLine($"Compressing Text file took {stopwatch.Elapsed.TotalSeconds} seconds. {textBytesLZMA.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            textBytes = null;
            stopwatch.Restart();

            byte[] textBytesFullCircle = await LZMA.DecompressMultiChunkAsync(textBytesLZMA);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Text file took {stopwatch.Elapsed.TotalSeconds} seconds. {textBytesFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            textBytesLZMA = null;
            textBytesFullCircle = null;
            stopwatch.Restart();

            byte[] jpegBytesLZMA = await LZMA.CompressAsync(jpegBytes);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)jpegBytesLZMA.Length / jpegBytes.Length)) / 100m;
            Console.WriteLine($"Compressing Jpeg file took {stopwatch.Elapsed.TotalSeconds} seconds. {jpegBytesLZMA.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            jpegBytes = null;
            stopwatch.Restart();

            byte[] jpegBytesFullCircle = await LZMA.DecompressMultiChunkAsync(jpegBytesLZMA);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Jpeg file took {stopwatch.Elapsed.TotalSeconds} seconds. {jpegBytesFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            jpegBytesLZMA = null;
            jpegBytesFullCircle = null;
            stopwatch.Restart();

            byte[] tgaBytesLZMA = await LZMA.CompressAsync(tgaBytes);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)tgaBytesLZMA.Length / tgaBytes.Length)) / 100m;
            Console.WriteLine($"Compressing Raw tga file took {stopwatch.Elapsed.TotalSeconds} seconds. {tgaBytesLZMA.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            tgaBytes = null;
            stopwatch.Restart();

            byte[] tgaBytesFullCircle = await LZMA.DecompressMultiChunkAsync(tgaBytesLZMA);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Raw tga file took {stopwatch.Elapsed.TotalSeconds} seconds. {tgaBytesFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            tgaBytesLZMA = null;
            tgaBytesFullCircle = null;
            stopwatch.Restart();

            byte[] wavBytesLZMA = await LZMA.CompressAsync(wavBytes);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)wavBytesLZMA.Length / wavBytes.Length)) / 100m;
            Console.WriteLine($"Compressing Wav file took {stopwatch.Elapsed.TotalSeconds} seconds. {wavBytesLZMA.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            wavBytes = null;
            stopwatch.Restart();

            byte[] wavBytesFullCircle = await LZMA.DecompressMultiChunkAsync(wavBytesLZMA);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Wav file took {stopwatch.Elapsed.TotalSeconds} seconds. {wavBytesFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            wavBytesLZMA = null;
            wavBytesFullCircle = null;

            Console.WriteLine($"Finished in {totalTimeSpent.TotalSeconds} seconds.");

            if (args != null && args.Length > 0 && args[0] == "-s")
                return;
            else
                Console.Read();
        }
    }
}
