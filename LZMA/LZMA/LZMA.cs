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

//HEADER INFORMATION
//position 0                  |  string(UTF8)       |   ARDA7z
//position 6                  |  ushort             |   chunkCount
//position 8                  |  int*chunkCount     |   chunkLength for each chunk
//position 8+(int*chunkCount) |  byte[]*chunkCount  |   data for each chunk

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SevenZip
{
    public class LZMA
    {
        private static byte[] ArdaZipDescriptor = Encoding.UTF8.GetBytes("ARDA7z");

        private static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        public static void Compress(MemoryStream inStream, MemoryStream outStream)
        {
            BinaryWriter bw = new BinaryWriter(outStream);
            bw.Write(ArdaZipDescriptor, 0, ArdaZipDescriptor.Length);
            bw.Write((ushort)1);
            bw.BaseStream.Position += 4;

            CompressRaw(inStream, outStream);

            bw.BaseStream.Position = 8;
            bw.Write((int)outStream.Length - 12);
            bw.BaseStream.Seek(0, SeekOrigin.End);
        }
        public static byte[] Compress(byte[] toCompress, int index = 0, int count = -1)
        {
            using (MemoryStream input = new MemoryStream(toCompress, index, count < 0 ? toCompress.Length : count))
            using (MemoryStream output = new MemoryStream())
            {
                Compress(input, output);
                return output.ToArray();
            }
        }
        public static async Task<byte[]> CompressAsync(byte[] toCompress, int index = 0, int count = -1)
        {
            return await Task.Run(() => Compress(toCompress, index, count));
        }

        private static void CompressRaw(MemoryStream inStream, MemoryStream outStream)
        {
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();

            coder.WriteCoderProperties(outStream);

            byte[] fileLengthBytes = BitConverter.GetBytes(inStream.Length);
            outStream.Write(fileLengthBytes, 0, fileLengthBytes.Length);

            coder.Code(inStream, outStream, -1, -1, null);
        }
        private static byte[] CompressRaw(byte[] toCompress, int index = 0, int count = -1)
        {
            using (MemoryStream input = new MemoryStream(toCompress, index, count < 0 ? toCompress.Length : count))
            using (MemoryStream output = new MemoryStream())
            {
                CompressRaw(input, output);
                return output.ToArray();
            }
        }
        private static async Task<byte[]> CompressAsyncRaw(byte[] toCompress, int index = 0, int count = -1)
        {
            return await Task.Run(() => CompressRaw(toCompress, index, count));
        }

        public static async Task<byte[]> CompressMultiThreadAsync(byte[] toCompress, ushort chunkCount)
        {
            Task[] tasks = new Task[chunkCount];
            byte[][] tgaBytesLZMATemp = new byte[chunkCount][];

            {
                int byteCount = toCompress.Length / chunkCount;
                int remaining = toCompress.Length % chunkCount;
                int position = 0;
                for (int i = 0; i < chunkCount; i++)
                {
                    int count = byteCount;
                    if (i == chunkCount - 1)
                        count += remaining;

                    async Task<byte[]> ThreadedCompress() => tgaBytesLZMATemp[i] = await LZMA.CompressAsyncRaw(toCompress, position, count);
                    tasks[i] = ThreadedCompress();
                    position += count;
                }
            }

            {
                int i = 0;
                while (i < tasks.Length)
                {
                    if (tasks[i].Status == TaskStatus.RanToCompletion)
                    {
                        tasks[i].Dispose();
                        tasks[i] = null;
                        i++;
                        continue;
                    }

                    await Task.Delay(1);
                }
                tasks = null;
            }

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(ArdaZipDescriptor, 0, ArdaZipDescriptor.Length);
                bw.Write(chunkCount);
                for (int i = 0; i < chunkCount; i++)
                    bw.Write(tgaBytesLZMATemp[i].Length);
                for (int i = 0; i < chunkCount; i++)
                {
                    await ms.WriteAsync(tgaBytesLZMATemp[i], 0, tgaBytesLZMATemp[i].Length);
                    tgaBytesLZMATemp[i] = null;
                }

                return ms.ToArray();
            }
        }

        private static void DecompressRaw(MemoryStream inStream, MemoryStream outStream)
        {
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();

            // Read the decoder properties
            byte[] properties = new byte[5];
            inStream.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            inStream.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(inStream, outStream, inStream.Length, fileLength, null);
        }
        private static byte[] DecompressRaw(byte[] toDecompress, int index = 0, int count = -1)
        {
            using (MemoryStream input = new MemoryStream(toDecompress, index, count < 0 ? toDecompress.Length : count))
            using (MemoryStream output = new MemoryStream())
            {
                DecompressRaw(input, output);
                return output.ToArray();
            }
        }
        private static async Task<byte[]> DecompressRawAsync(byte[] toDecompress, int index = 0, int count = -1)
        {
            return await Task.Run(() => DecompressRaw(toDecompress, index, count));
        }

        //Disabled because these are unsafe.
        /*
        public static void Decompress(MemoryStream inStream, MemoryStream outStream)
        {
            inStream.Position = 12;
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();

            // Read the decoder properties
            byte[] properties = new byte[5];
            inStream.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            inStream.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(inStream, outStream, inStream.Length, fileLength, null);
        }
        public static byte[] Decompress(byte[] toDecompress, int index = 0, int count = -1)
        {
            using (MemoryStream input = new MemoryStream(toDecompress, index, count < 0 ? toDecompress.Length : count))
            using (MemoryStream output = new MemoryStream())
            {
                Decompress(input, output);
                return output.ToArray();
            }
        }
        public static async Task<byte[]> DecompressAsync(byte[] toDecompress, int index = 0, int count = -1)
        {
            return await Task.Run(() => Decompress(toDecompress, index, count));
        }
        */

        public static async Task<byte[]> DecompressMultiChunkAsync(byte[] toCompress)
        {
            using (MemoryStream outStream = new MemoryStream())
            using (MemoryStream ms = new MemoryStream(toCompress))
            using (BinaryReader br = new BinaryReader(ms))
            {
                byte[] descriptor = new byte[ArdaZipDescriptor.Length];
                br.Read(descriptor, 0, descriptor.Length);
                if (!ByteArrayCompare(descriptor, ArdaZipDescriptor))
                    return null;

                ushort chunkCount = br.ReadUInt16();
                int[] chunkLengths = new int[chunkCount];
                for (int i = 0; i < chunkCount; i++)
                    chunkLengths[i] = br.ReadInt32();

                for (int i = 0; i < chunkCount; i++)
                {
                    byte[] buffer = new byte[chunkLengths[i]];
                    await ms.ReadAsync(buffer, 0, buffer.Length);
                    byte[] decompressedBuffer = await DecompressRawAsync(buffer);
                    await outStream.WriteAsync(decompressedBuffer, 0, decompressedBuffer.Length);
                }

                return outStream.ToArray();
            }
        }
    }
}