using System;
using System.IO;

namespace SerializableObjectsLibrary
{
    [Serializable]
    public class FileInfoSerializable
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public long Length { get; set; }
        public string Extension { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string DirectoryName { get; set; }

        public FileInfoSerializable() { }
        public FileInfoSerializable(FileInfo fileInfo)
        {
            Name = fileInfo.Name;
            FullName = fileInfo.FullName;
            Length = fileInfo.Length;
            Extension = fileInfo.Extension;
            LastWriteTime = fileInfo.LastWriteTime;
            DirectoryName = fileInfo.DirectoryName;
        }
    }
}
