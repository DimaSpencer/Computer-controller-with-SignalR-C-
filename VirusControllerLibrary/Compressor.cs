using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace VirusControllerLibrary
{
    public class Compressor
    {
        public byte[] CompressBytes(byte[] data)
        {
            using var compressedStream = new MemoryStream();
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Compress);

            zipStream.Write(data, 0, data.Length);
            zipStream.Close();

            return compressedStream.ToArray();
        }
        public byte[] DecompressBytes(byte[] data)
        {
            using var compressedStream = new MemoryStream(data);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();

            zipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }
}
