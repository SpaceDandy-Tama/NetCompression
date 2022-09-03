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

using System.Threading.Tasks;

namespace System.IO.Compression
{
    #region Misc Declarations
    public enum CompressionAlgorithm : byte
    {
        None = 0,
        Deflate = 1,
        Gzip = 2,
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        Brotli = 3,
#endif
#if NET6_0_OR_GREATER
        ZLib = 4,
#endif
    }

    public enum LoggingType : byte
    {
        None = 0,
        Console = 1,
        Event = 2,
        ConsoleAndEvent = 3,
    }

    public class NetCompressionEventArgs : EventArgs
    {
        public NetCompressionEventArgs(string message, CompressionAlgorithm algorithm, CompressionMode mode, MemoryStream stream)
        {
            Message = message;
            Algorithm = algorithm;
            Mode = mode;
            Stream = stream;
        }

        public string Message { get; set; }
        public CompressionAlgorithm Algorithm { get; set; }
        public CompressionMode Mode { get; set; }
        public MemoryStream Stream { get; set; } 
    }
    #endregion

    public static class NetCompression
    {
        public delegate void NetCompressionEventHandler(object sender, NetCompressionEventArgs args);

        public static LoggingType LoggingType = LoggingType.Event;
        public static event NetCompressionEventHandler OnCompressed;
        public static event NetCompressionEventHandler OnDecompressed;

        #region Public Methods
        public static MemoryStream Compress(Stream srcStream, CompressionLevel lvl, CompressionAlgorithm algorithm)
        {
            if (lvl == CompressionLevel.NoCompression || algorithm == CompressionAlgorithm.None)
                return null;

            MemoryStream stream = null;

            if (algorithm == CompressionAlgorithm.Deflate)
                stream = CompressDeflate(srcStream, lvl);
            else if (algorithm == CompressionAlgorithm.Gzip)
                stream = CompressGzip(srcStream, lvl);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            else if (algorithm == CompressionAlgorithm.Brotli)
                stream = CompressBrotli(srcStream, lvl);
#endif
#if NET6_0_OR_GREATER
            else if (algorithm == CompressionAlgorithm.ZLib)
                stream = CompressZLib(srcStream, lvl);
#endif
            stream.Position = 0;
            return stream;
        }
        public static MemoryStream Compress(byte[] srcBuffer, CompressionLevel lvl, CompressionAlgorithm algorithm, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return Compress(srcStream, lvl, algorithm);
        }
        public static async Task<MemoryStream> CompressAsync(Stream srcStream, CompressionLevel lvl, CompressionAlgorithm algorithm)
        {
            if (lvl == CompressionLevel.NoCompression || algorithm == CompressionAlgorithm.None)
                return null;

            MemoryStream stream = null;

            if (algorithm == CompressionAlgorithm.Deflate)
                stream = await CompressDeflateAsync(srcStream, lvl);
            else if (algorithm == CompressionAlgorithm.Gzip)
                stream = await CompressGzipAsync(srcStream, lvl);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            else if (algorithm == CompressionAlgorithm.Brotli)
                stream = await CompressBrotliAsync(srcStream, lvl);
#endif
#if NET6_0_OR_GREATER
            else if (algorithm == CompressionAlgorithm.ZLib)
                stream = await CompressZLibAsync(srcStream, lvl);
#endif
            stream.Position = 0;
            return stream;
        }
        public static async Task<MemoryStream> CompressAsync(byte[] srcBuffer, CompressionLevel lvl, CompressionAlgorithm algorithm, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return await CompressAsync(srcStream, lvl, algorithm);
        }
        public static MemoryStream Decompress(Stream srcStream, CompressionAlgorithm algorithm)
        {
            MemoryStream stream = null;

            if (algorithm == CompressionAlgorithm.Deflate)
                stream = DecompressDeflate(srcStream);
            else if (algorithm == CompressionAlgorithm.Gzip)
                stream = DecompressGzip(srcStream);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            else if (algorithm == CompressionAlgorithm.Brotli)
                stream = DecompressBrotli(srcStream);
#endif
#if NET6_0_OR_GREATER
            else if (algorithm == CompressionAlgorithm.ZLib)
                stream =  DecompressZLib(srcStream);
#endif
            stream.Position = 0;
            return stream;
        }
        public static MemoryStream Decompress(byte[] srcBuffer, CompressionAlgorithm algorithm)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return Decompress(srcStream, algorithm);
        }
        public static async Task<MemoryStream> DecompressAsync(Stream srcStream, CompressionAlgorithm algorithm)
        {
            MemoryStream stream = null;

            if (algorithm == CompressionAlgorithm.Deflate)
                stream = await DecompressDeflateAsync(srcStream);
            else if (algorithm == CompressionAlgorithm.Gzip)
                stream = await DecompressGzipAsync(srcStream);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            else if (algorithm == CompressionAlgorithm.Brotli)
                stream =  await DecompressBrotliAsync(srcStream);
#endif
#if NET6_0_OR_GREATER
            else if (algorithm == CompressionAlgorithm.ZLib)
                stream =  await DecompressZLibAsync(srcStream);
#endif
            stream.Position = 0;
            return stream;
        }
        public static async Task<MemoryStream> DecompressAsync(byte[] srcBuffer, CompressionAlgorithm algorithm)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return await DecompressAsync(srcStream, algorithm);
        }
        #endregion

        #region Deflate
        private static MemoryStream CompressDeflate(Stream srcStream, CompressionLevel lvl)
        {
            MemoryStream stream = new MemoryStream();
            using (DeflateStream compressor = new DeflateStream(stream, lvl, true))
                srcStream.CopyToAsync(compressor);
            CompressResults(CompressionAlgorithm.Deflate, srcStream.Length, stream);
            return stream;
        }
        private static async Task<MemoryStream> CompressDeflateAsync(Stream srcStream, CompressionLevel lvl)
        {
            MemoryStream stream = new MemoryStream();
            using (DeflateStream compressor = new DeflateStream(stream, lvl, true))
                await srcStream.CopyToAsync(compressor);
            CompressResults(CompressionAlgorithm.Deflate, srcStream.Length, stream);
            return stream;
        }
        private static MemoryStream CompressDeflate(byte[] srcBuffer, CompressionLevel lvl, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return CompressDeflate(srcStream, lvl);
        }
        private static async Task<MemoryStream> CompressDeflateAsync(byte[] srcBuffer, CompressionLevel lvl, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return await CompressDeflateAsync(srcStream, lvl);
        }
        private static MemoryStream DecompressDeflate(Stream srcStream)
        {
            MemoryStream stream = new MemoryStream();
            using (DeflateStream decompressor = new DeflateStream(srcStream, CompressionMode.Decompress, true))
                decompressor.CopyToAsync(stream);
            DecompressResults(CompressionAlgorithm.Deflate, srcStream.Length, stream);
            return stream;
        }
        private static async Task<MemoryStream> DecompressDeflateAsync(Stream srcStream)
        {
            MemoryStream stream = new MemoryStream();
            using (DeflateStream decompressor = new DeflateStream(srcStream, CompressionMode.Decompress, true))
                await decompressor.CopyToAsync(stream);
            DecompressResults(CompressionAlgorithm.Deflate, srcStream.Length, stream);
            return stream;
        }
        private static MemoryStream DecompressDeflate(byte[] srcBuffer)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return DecompressDeflate(srcStream);
        }
        private static async Task<MemoryStream> DecompressDeflateAsync(byte[] srcBuffer)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return await DecompressDeflateAsync(srcStream);
        }
        #endregion

        #region Gzip
        private static MemoryStream CompressGzip(Stream srcStream, CompressionLevel lvl)
        {
            MemoryStream stream = new MemoryStream();
            using (GZipStream compressor = new GZipStream(stream, lvl, true))
                srcStream.CopyToAsync(compressor);
            CompressResults(CompressionAlgorithm.Gzip, srcStream.Length, stream);
            return stream;

        }
        private static async Task<MemoryStream> CompressGzipAsync(Stream srcStream, CompressionLevel lvl)
        {
            MemoryStream stream = new MemoryStream();
            using (GZipStream compressor = new GZipStream(stream, lvl, true))
                await srcStream.CopyToAsync(compressor);
            CompressResults(CompressionAlgorithm.Gzip, srcStream.Length, stream);
            return stream;

        }
        private static MemoryStream CompressGzip(byte[] srcBuffer, CompressionLevel lvl, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return CompressGzip(srcStream, lvl);
        }
        private static async Task<MemoryStream> CompressGzipAsync(byte[] srcBuffer, CompressionLevel lvl, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return await CompressGzipAsync(srcStream, lvl);
        }
        private static MemoryStream DecompressGzip(Stream srcStream)
        {
            MemoryStream stream = new MemoryStream();
            using (GZipStream decompressor = new GZipStream(srcStream, CompressionMode.Decompress, true))
                decompressor.CopyToAsync(stream);
            DecompressResults(CompressionAlgorithm.Gzip, srcStream.Length, stream);
            return stream;
        }
        private static async Task<MemoryStream> DecompressGzipAsync(Stream srcStream)
        {
            MemoryStream stream = new MemoryStream();
            using (GZipStream decompressor = new GZipStream(srcStream, CompressionMode.Decompress, true))
                await decompressor.CopyToAsync(stream);
            DecompressResults(CompressionAlgorithm.Gzip, srcStream.Length, stream);
            return stream;
        }
        private static MemoryStream DecompressGzip(byte[] srcBuffer)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return DecompressGzip(srcStream);
        }
        private static async Task<MemoryStream> DecompressGzipAsync(byte[] srcBuffer)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return await DecompressGzipAsync(srcStream);
        }
        #endregion

        #region Brotli
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        private static MemoryStream CompressBrotli(Stream srcStream, CompressionLevel lvl)
        {
            MemoryStream stream = new MemoryStream();
            using (BrotliStream compressor = new BrotliStream(stream, lvl, true))
                srcStream.CopyTo(compressor);
            CompressResults(CompressionAlgorithm.Brotli, srcStream.Length, stream);
            return stream;
        }
        private static async Task<MemoryStream> CompressBrotliAsync(Stream srcStream, CompressionLevel lvl)
        {
            MemoryStream stream = new MemoryStream();
            using (BrotliStream compressor = new BrotliStream(stream, lvl, true))
                await srcStream.CopyToAsync(compressor);
            CompressResults(CompressionAlgorithm.Brotli, srcStream.Length, stream);
            return stream;
        }
        private static MemoryStream CompressBrotli(byte[] srcBuffer, CompressionLevel lvl, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return CompressBrotli(srcStream, lvl);
        }
        private static async Task<MemoryStream> CompressBrotliAsync(byte[] srcBuffer, CompressionLevel lvl, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return await CompressBrotliAsync(srcStream, lvl);
        }
        private static MemoryStream DecompressBrotli(Stream srcStream)
        {
            MemoryStream stream = new MemoryStream();
            using (BrotliStream decompressor = new BrotliStream(srcStream, CompressionMode.Decompress, true))
                decompressor.CopyToAsync(stream);
            DecompressResults(CompressionAlgorithm.Brotli, srcStream.Length, stream);
            return stream;
        }
        private static async Task<MemoryStream> DecompressBrotliAsync(Stream srcStream)
        {
            MemoryStream stream = new MemoryStream();
            using (BrotliStream decompressor = new BrotliStream(srcStream, CompressionMode.Decompress, true))
                await decompressor.CopyToAsync(stream);
            DecompressResults(CompressionAlgorithm.Brotli, srcStream.Length, stream);
            return stream;
        }
        private static MemoryStream DecompressBrotli(byte[] srcBuffer)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return DecompressBrotli(srcStream);
        }
        private static async Task<MemoryStream> DecompressBrotliAsync(byte[] srcBuffer)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return await DecompressBrotliAsync(srcStream);
        }
#endif
        #endregion

        #region ZLib
#if NET6_0_OR_GREATER
        private static MemoryStream CompressZLib(Stream srcStream, CompressionLevel lvl)
        {
            MemoryStream stream = new MemoryStream();
            using (ZLibStream compressor = new ZLibStream(stream, lvl, true))
                srcStream.CopyTo(compressor);
            CompressResults(CompressionAlgorithm.ZLib, srcStream.Length, stream);
            return stream;
        }
        private static async Task<MemoryStream> CompressZLibAsync(Stream srcStream, CompressionLevel lvl)
        {
            MemoryStream stream = new MemoryStream();
            using (ZLibStream compressor = new ZLibStream(stream, lvl, true))
                await srcStream.CopyToAsync(compressor);
            CompressResults(CompressionAlgorithm.ZLib, srcStream.Length, stream);
            return stream;
        }
        private static MemoryStream CompressZLib(byte[] srcBuffer, CompressionLevel lvl, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return CompressZLib(srcStream, lvl);
        }
        private static async Task<MemoryStream> CompressZLibAsync(byte[] srcBuffer, CompressionLevel lvl, int index = 0, int count = -1)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer, index, count < 0 ? srcBuffer.Length : count))
                return await CompressZLibAsync(srcStream, lvl);
        }
        private static MemoryStream DecompressZLib(Stream srcStream)
        {
            MemoryStream stream = new MemoryStream();
            using (ZLibStream decompressor = new ZLibStream(srcStream, CompressionMode.Decompress, true))
                decompressor.CopyToAsync(stream);
            DecompressResults(CompressionAlgorithm.ZLib, srcStream.Length, stream);
            return stream;
        }
        private static async Task<MemoryStream> DecompressZLibAsync(Stream srcStream)
        {
            MemoryStream stream = new MemoryStream();
            using (ZLibStream decompressor = new ZLibStream(srcStream, CompressionMode.Decompress, true))
                await decompressor.CopyToAsync(stream);
            DecompressResults(CompressionAlgorithm.ZLib, srcStream.Length, stream);
            return stream;
        }
        private static MemoryStream DecompressZLib(byte[] srcBuffer)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return DecompressZLib(srcStream);
        }
        private static async Task<MemoryStream> DecompressZLibAsync(byte[] srcBuffer)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBuffer))
                return await DecompressZLibAsync(srcStream);
        }
#endif
        #endregion

        #region Console Ouput and Event Handling
        private static void CompressResults(CompressionAlgorithm algorithm, long uncompressedBytes, MemoryStream stream)
        {
            if (LoggingType == LoggingType.None)
                return;

            string message = $"{uncompressedBytes} was compressed to {stream.Length} using {algorithm} algorithm.";

            if (LoggingType == LoggingType.Console || LoggingType == LoggingType.ConsoleAndEvent)
                Console.WriteLine(message);

            if (LoggingType == LoggingType.Event || LoggingType == LoggingType.ConsoleAndEvent)
                OnCompressed?.Invoke(null, new NetCompressionEventArgs(message, algorithm, CompressionMode.Compress, stream));
        }
        private static void DecompressResults(CompressionAlgorithm algorithm, long compressedBytes, MemoryStream stream)
        {
            if (LoggingType == LoggingType.None)
                return;

            string message = $"{compressedBytes} was decompressed to {stream.Length} using {algorithm} algorithm.";

            if (LoggingType == LoggingType.Console || LoggingType == LoggingType.ConsoleAndEvent)
                Console.WriteLine(message);

            if (LoggingType == LoggingType.Event || LoggingType == LoggingType.ConsoleAndEvent)
                OnDecompressed?.Invoke(null, new NetCompressionEventArgs(message, algorithm, CompressionMode.Decompress, stream));
        }
        #endregion
    }
}