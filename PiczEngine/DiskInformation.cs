using System;
using System.Runtime.InteropServices;

namespace Fenton.Picz.Engine
{
    public static class DiskInformation
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
           out ulong lpFreeBytesAvailable,
           out ulong lpTotalNumberOfBytes,
           out ulong lpTotalNumberOfFreeBytes);

        public static ulong GetDiskSpaceInSIMegabytes(string directoryName)
        {
            ulong freeBytesAvailable;
            ulong totalNumberOfBytes;
            ulong totalNumberOfFreeBytes;

            if (!GetDiskFreeSpaceEx(directoryName, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes))
            {
                throw new System.ComponentModel.Win32Exception();
            }

            return (ulong)Math.Floor((freeBytesAvailable / 1000M) / 1000M);
        }
    }
}