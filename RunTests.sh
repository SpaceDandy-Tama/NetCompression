#!/bin/bash

cd "LZMA/LZMA-ThreadedTest/bin/Release/"
echo "LZMA Multithreaded"
mono LZMA-ThreadedTest.exe -s

cd "../../../../LZMA/LZMA-Test/bin/Release/"
echo "LZMA Singlethreaded"
mono LZMA-Test.exe -s

cd "../../../../NetCompression-Test/NetCompression-Test/bin/Release/net6.0/"
echo "Deflate Optimal"
dotnet NetCompression-Test.dll 1 0 -s
echo "Deflate Fastest"
dotnet NetCompression-Test.dll 1 1 -s
echo "Gzip Optimal"
dotnet NetCompression-Test.dll 2 0 -s
echo "Gzip Fastest"
dotnet NetCompression-Test.dll 2 1 -s
echo "Brotli Optimal"
dotnet NetCompression-Test.dll 3 0 -s
echo "Brotli Fastest"
dotnet NetCompression-Test.dll 3 1 -s
echo "ZLib Optimal"
dotnet NetCompression-Test.dll 4 0 -s
echo "ZLib Fastest"
dotnet NetCompression-Test.dll 4 1 -s
