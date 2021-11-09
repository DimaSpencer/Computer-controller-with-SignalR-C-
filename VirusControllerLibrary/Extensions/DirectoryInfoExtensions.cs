using System;
using System.Collections.Generic;
using System.IO;

namespace VirusControllerLibrary.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<DirectoryInfo> GetAllParentDirectories(this DirectoryInfo directoryToScan)
        {
            Stack<DirectoryInfo> ret = new Stack<DirectoryInfo>();
            GetAllParentDirectories(directoryToScan, ref ret);
            return ret;
        }
        public static string GetAllParentDirectoriesPath(this DirectoryInfo directoryToScan)
        {
            Stack<DirectoryInfo> ret = new Stack<DirectoryInfo>();
            GetAllParentDirectories(directoryToScan, ref ret);

            string directoryPath = string.Empty;
            foreach (var directoryInfo in ret)
                directoryPath += "/" + directoryInfo.Name;

            return directoryPath;
        }

        private static void GetAllParentDirectories(DirectoryInfo directoryToScan, ref Stack<DirectoryInfo> directories)
        {
            if (directoryToScan == null || directoryToScan.Name == directoryToScan.Root.Name)
                return;

            directories.Push(directoryToScan);
            GetAllParentDirectories(directoryToScan.Parent, ref directories);
        }
    }
}
