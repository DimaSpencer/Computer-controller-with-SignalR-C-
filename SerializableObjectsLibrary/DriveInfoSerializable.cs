using System;
using System.IO;

namespace SerializableObjectsLibrary
{
    [Serializable]
    public class DriveInfoSerializable
    {
        public string Name { get; set; }
        public long TotalSize { get; }
        public DriveType DriveType { get; }
        public long AvailableFreeSpace { get; }
        public long TotalFreeSpace { get; }
        public string VolumeLabel { get; }
        public bool IsReady { get; }

        public DriveInfoSerializable() { }
        public DriveInfoSerializable(DriveInfo driveInfo)
        {
            Name = driveInfo.Name;
            TotalSize = driveInfo.TotalSize;
            DriveType = driveInfo.DriveType;
            AvailableFreeSpace = driveInfo.AvailableFreeSpace;
            TotalFreeSpace = driveInfo.TotalFreeSpace;
            VolumeLabel = driveInfo.VolumeLabel;
            IsReady = driveInfo.IsReady;
        }
    }
}
