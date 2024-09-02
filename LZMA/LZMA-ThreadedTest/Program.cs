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
using System.Collections.Generic;

namespace LZMA_ThreadedTest
{
    class Program
    {
        

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter folder name: ");
            string path = Console.ReadLine();

            byte[] testThreads = new byte[] { 1, 2, 4, 6, 8, 12 };
            

            List<string> files = new List<string>();

            files.AddRange(System.IO.Directory.EnumerateFiles(path));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            stopwatch.Stop();

            using (StreamWriter sw = new StreamWriter($"{path} test log.txt"))
            {
                foreach (string file in files)
                {
                    byte[] bytes = File.ReadAllBytes(file);
                    string info = $"{Path.GetFileName(file)}: ({bytes.Length} bytes)";
                    Console.WriteLine(info);
                    sw.WriteLine(info);

                    byte processorCount = testThreads[0];

                    for (int i = 0; i < testThreads.Length; i++)
                    {
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();

                        await System.Threading.Tasks.Task.Delay(100 * processorCount);

                        processorCount = testThreads[i];

                        stopwatch.Restart();
                        byte[] bytesLZMA;
                        if (processorCount > 1)
                            bytesLZMA = await LZMA.CompressMultiThreadAsync(bytes, processorCount);
                        else
                            bytesLZMA = await LZMA.CompressAsync(bytes);
                        stopwatch.Stop();
                        decimal ratio = Math.Truncate(10000m * ((decimal)bytesLZMA.Length / bytes.Length)) / 100m;
                        string info1 = $"Compressed in {stopwatch.Elapsed.TotalSeconds}";
                        stopwatch.Restart();
                        byte[] bytesFullCircle = await LZMA.DecompressMultiChunkAsync(bytesLZMA);
                        stopwatch.Stop();
                        info1 += $" and decompressed in {stopwatch.Elapsed.TotalSeconds} seconds. using {processorCount} threads, down to {ratio}%";

                        Console.WriteLine(info1);
                        sw.WriteLine(info1);

                        bytesLZMA = null;
                        bytesFullCircle = null;
                    }

                    Console.WriteLine("");
                    sw.WriteLine("");
                    bytes = null;

                }
            }

            /*
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

            byte processorCount = 12;
            byte[] textBytesLZMA = await LZMA.CompressMultiThreadAsync(textBytes, processorCount);

            stopwatch.Stop();
            decimal ratio = Math.Truncate(10000m * ((decimal)textBytesLZMA.Length / textBytes.Length)) / 100m;
            Console.WriteLine($"Compressing Text file took {stopwatch.Elapsed.TotalSeconds} seconds multi threaded({processorCount}). {textBytesLZMA.Length} bytes. {ratio}%");
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

            processorCount = 12;
            byte[] jpegBytesLZMA = await LZMA.CompressMultiThreadAsync(jpegBytes, processorCount);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)jpegBytesLZMA.Length / jpegBytes.Length)) / 100m;
            Console.WriteLine($"Compressing Jpeg file took {stopwatch.Elapsed.TotalSeconds} seconds multi threaded({processorCount}). {jpegBytesLZMA.Length} bytes. {ratio}%");
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

            processorCount = 24;
            byte[] tgaBytesArdaZip = await LZMA.CompressMultiThreadAsync(tgaBytes, processorCount);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)tgaBytesArdaZip.Length / tgaBytes.Length)) / 100m;
            Console.WriteLine($"Compressing Raw tga file took {stopwatch.Elapsed.TotalSeconds} seconds multi threaded({processorCount}). {tgaBytesArdaZip.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            tgaBytes = null;

            stopwatch.Restart();

            byte[] tgaBytesFullCircle = await LZMA.DecompressMultiChunkAsync(tgaBytesArdaZip);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Raw tga file took {stopwatch.Elapsed.TotalSeconds} seconds. {tgaBytesFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            tgaBytesArdaZip = null;
            tgaBytesFullCircle = null;

            stopwatch.Restart();

            processorCount = 12;
            byte[] wavBytesArdaZip = await LZMA.CompressMultiThreadAsync(wavBytes, processorCount);

            stopwatch.Stop();
            ratio = Math.Truncate(10000m * ((decimal)wavBytesArdaZip.Length / wavBytes.Length)) / 100m;
            Console.WriteLine($"Compressing Wav file took {stopwatch.Elapsed.TotalSeconds} seconds multi threaded({processorCount}). {wavBytesArdaZip.Length} bytes. {ratio}%");
            totalTimeSpent += stopwatch.Elapsed;
            wavBytes = null;

            stopwatch.Restart();

            byte[] wavBytesFullCircle = await LZMA.DecompressMultiChunkAsync(wavBytesArdaZip);

            stopwatch.Stop();
            Console.WriteLine($"Decompressing Wav file took {stopwatch.Elapsed.TotalSeconds} seconds. {wavBytesFullCircle.Length} bytes.");
            totalTimeSpent += stopwatch.Elapsed;
            wavBytesArdaZip = null;
            wavBytesFullCircle = null;

            Console.WriteLine($"Finished in {totalTimeSpent.TotalSeconds} seconds.");
            */

            if (args != null && args.Length > 0 && args[0] == "-s")
                return;
            else
            {
                Console.WriteLine("Test Finished, press any key to exit...");
                Console.Read();
            }
        }
    }
}
