cd "LZMA/LZMA-ThreadedTest/bin/Release/" &
LZMA-ThreadedTest.exe -s & REM LZMA Multithreaded

cd "../../../../LZMA/LZMA-Test/bin/Release/" &
LZMA-Test.exe -s & REM LZMA Singlethreaded

cd "../../../../NetCompression-Test/NetCompression-Test/bin/Release/net6.0/" &
dotnet NetCompression-Test.dll 1 0 -s & REM Deflate Optimal
dotnet NetCompression-Test.dll 1 1 -s & REM Deflate Fastest
dotnet NetCompression-Test.dll 2 0 -s & REM Gzip Optimal
dotnet NetCompression-Test.dll 2 1 -s & REM Gzip Fastest
dotnet NetCompression-Test.dll 3 0 -s & REM Brotli Optimal
dotnet NetCompression-Test.dll 3 1 -s & REM Brotli Fastest
dotnet NetCompression-Test.dll 4 0 -s & REM ZLib Optimal
dotnet NetCompression-Test.dll 4 1 -s & REM ZLib Fastest
