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
using System.Threading.Tasks;
using System.Diagnostics;

namespace NetCompression_Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                Console.WriteLine($"Two Arguments Required");
                Console.WriteLine($"First Argument");
                Console.WriteLine($"1 - Deflate");
                Console.WriteLine($"2 - Gzip");
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                Console.WriteLine($"3 - Brotli");
#endif
#if NET6_0_OR_GREATER
                Console.WriteLine($"4 - ZLib");
#endif
                Console.WriteLine($"Second Argument");
                Console.WriteLine($"0 - Optimal");
                Console.WriteLine($"1 - Fastest");
                return;
            }

            CompressionAlgorithm compressionAlgorithm = (CompressionAlgorithm)byte.Parse(args[0]);
            CompressionLevel compressionLevel = (CompressionLevel)byte.Parse(args[1]);
            Console.WriteLine($"Algorithm: {compressionAlgorithm}\nLevel: {compressionLevel}");

            byte[] textBytes = File.ReadAllBytes("../../../../../Resources/Lorem Ipsum.txt");
            MemoryStream textStream = new MemoryStream(textBytes);
            Console.WriteLine($"Original text is {textBytes.Length} bytes.");

            byte[] jpegBytes = File.ReadAllBytes("../../../../../Resources/20170212.jpg");
            MemoryStream jpegStream = new MemoryStream(jpegBytes);
            Console.WriteLine($"Original jpeg is {jpegBytes.Length} bytes.");

            byte[] tgaBytes = File.ReadAllBytes("../../../../../Resources/mid.tga");
            MemoryStream tgaStream = new MemoryStream(tgaBytes);
            Console.WriteLine($"Original tga is {tgaBytes.Length} bytes.");

            byte[] wavBytes = File.ReadAllBytes("../../../../../Resources/Levianth & Axol - Remember (feat. The Tech Thieves) [NCS Release].wav");
            MemoryStream wavStream = new MemoryStream(wavBytes);
            Console.WriteLine($"Original wav is {wavBytes.Length} bytes.");

            Stopwatch stopwatch = new Stopwatch();
            TimeSpan totalTimeSpent = TimeSpan.Zero;
            stopwatch.Start();

            MemoryStream textStreamBrotli = NetCompression.Compress(textStream, compressionLevel, compressionAlgorithm);

            stopwatch.Stop();
            decimal ratio = Math.Truncate(10000m * ((decimal)textStreamBrotli.Length / textStream.Length)) / 100m;
            Console.WriteLine($"Compressing Text file took {stopwatch.Elapsed.TotalSeconds} seconds. {textStreamBrotli.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            textBytes = null;
            textStream.Dispose();
            stopwatch.Restart();

            MemoryStream textStreamFullCircle = NetCompression.Decompress(textStreamBrotli, compressionAlgorithm);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Text file took {stopwatch.Elapsed.TotalSeconds} seconds. {textStreamFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            textStreamBrotli.Dispose();
            textStreamFullCircle.Dispose();
            stopwatch.Restart();

            MemoryStream jpegStreamBrotli = NetCompression.Compress(jpegStream, compressionLevel, compressionAlgorithm);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)jpegStreamBrotli.Length / jpegStream.Length)) / 100m;
            Console.WriteLine($"Compressing Jpeg file took {stopwatch.Elapsed.TotalSeconds} seconds. {jpegStreamBrotli.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            jpegBytes = null;
            jpegStream.Dispose();
            stopwatch.Restart();

            MemoryStream jpegStreamFullCircle = NetCompression.Decompress(jpegStreamBrotli, compressionAlgorithm);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Jpeg file took {stopwatch.Elapsed.TotalSeconds} seconds. {jpegStreamFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            jpegStreamBrotli.Dispose();
            jpegStreamFullCircle.Dispose();
            stopwatch.Restart();

            MemoryStream tgaStreamBrotli = await NetCompression.CompressAsync(tgaStream, compressionLevel, compressionAlgorithm);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)tgaStreamBrotli.Length / tgaStream.Length)) / 100m;
            Console.WriteLine($"Compressing Raw tga file took {stopwatch.Elapsed.TotalSeconds} seconds. {tgaStreamBrotli.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            tgaBytes = null;
            tgaStream.Dispose();
            stopwatch.Restart();

            MemoryStream tgaStreamFullCircle = await NetCompression.DecompressAsync(tgaStreamBrotli, compressionAlgorithm);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Raw tga file took {stopwatch.Elapsed.TotalSeconds} seconds. {tgaStreamFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            tgaStreamBrotli.Dispose();
            tgaStreamFullCircle.Dispose();
            stopwatch.Restart();

            MemoryStream wavStreamBrotli = await NetCompression.CompressAsync(wavStream, compressionLevel, compressionAlgorithm);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)wavStreamBrotli.Length / wavStream.Length)) / 100m;
            Console.WriteLine($"Compressing Wav file took {stopwatch.Elapsed.TotalSeconds} seconds. {wavStreamBrotli.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            wavBytes = null;
            wavStream.Dispose();
            stopwatch.Restart();

            MemoryStream wavStreamFullCircle = await NetCompression.DecompressAsync(wavStreamBrotli, compressionAlgorithm);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Wav file took {stopwatch.Elapsed.TotalSeconds} seconds. {wavStreamFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            wavStreamBrotli.Dispose();
            wavStreamFullCircle.Dispose();

            Console.WriteLine($"Finished in {totalTimeSpent.TotalSeconds} seconds.");

            if (args != null && args.Length > 2 && args[2] == "-s")
                return;
            else
                Console.Read();
        }
    }
}
