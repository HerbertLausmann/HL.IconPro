/*
 *  IconExtractor/IconUtil for .NET
 *  Copyright (C) 2014 Tsuda Kageyu. All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *   1. Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *   2. Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 *  TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 *  PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER
 *  OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 *  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 *  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 *  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HL.IconPro.Lib.Wpf.Tools
{
    public static class PEIconExtractor
    {
        #region Native
        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        private readonly static IntPtr RT_ICON = (IntPtr)3;
        private readonly static IntPtr RT_GROUP_ICON = (IntPtr)14;
        private const int MAX_PATH = 260;

        [UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Unicode)]
        [SuppressUnmanagedCodeSecurity]
        private delegate bool ENUMRESNAMEPROC(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool EnumResourceNames(IntPtr hModule, IntPtr lpszType, ENUMRESNAMEPROC lpEnumFunc, IntPtr lParam);
        #endregion

        #region For The Future
        private static IconBitmapDecoder[] Icons;
        private static void Load(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            Icons = null;
            IntPtr hModule = IntPtr.Zero;
            try
            {
                hModule = LoadLibraryEx(fileName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (hModule == IntPtr.Zero)
                    throw new Win32Exception();

                List<byte[]> tmpData = new List<byte[]>();

                ENUMRESNAMEPROC callback = (h, t, name, l) =>
                {
                    byte[] iconBuffer = GetDataFromResource(hModule, RT_GROUP_ICON, name);
                    tmpData.Add(iconBuffer);
                    return true;
                };
                EnumResourceNames(hModule, RT_GROUP_ICON, callback, IntPtr.Zero);
                Icons = new IconBitmapDecoder[tmpData.Count];
                for (int i = 0; i < tmpData.Count; i++)
                {
                    MemoryStream mem = new MemoryStream(tmpData[i]);
                    Icons[i] = new IconBitmapDecoder();
                    Icons[i].Open(mem);
                }
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    FreeLibrary(hModule);
            }
        }
        private static IconBitmapDecoder GetIcon(int IconIndex)
        {
            return Icons[IconIndex];
        }
        #endregion
        public static Stream ExtractIcon(string FileName, int Index)
        {
            MemoryStream output = null;
            byte[] iconBuffer = null;
            if (FileName == null)
                throw new ArgumentNullException("fileName");
            Icons = null;
            IntPtr hModule = IntPtr.Zero;
            try
            {
                hModule = LoadLibraryEx(FileName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (hModule == IntPtr.Zero)
                    throw new Win32Exception();
                int counter = 0;
                ENUMRESNAMEPROC callback = (h, t, name, l) =>
                {
                    if (counter == Index)
                    {
                        byte[] data = GetDataFromResource(hModule, RT_GROUP_ICON, name);
                        int count = BitConverter.ToUInt16(data, 4);  // GRPICONDIR.idCount
                        int len = 6 + 16 * count;                   // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count
                        for (int i = 0; i < count; ++i)
                            len += BitConverter.ToInt32(data, 6 + 14 * i + 8);   // GRPICONDIRENTRY.dwBytesInRes
                        using (var dst = new BinaryWriter(new MemoryStream(len)))
                        {
                            // Copy GRPICONDIR to ICONDIR.
                            dst.Write(data, 0, 6);
                            int picOffset = 6 + 16 * count; // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count
                            for (int i = 0; i < count; ++i)
                            {
                                // Load the picture.
                                ushort id = BitConverter.ToUInt16(data, 6 + 14 * i + 12);    // GRPICONDIRENTRY.nID
                                var pic = GetDataFromResource(hModule, RT_ICON, (IntPtr)id);
                                // Copy GRPICONDIRENTRY to ICONDIRENTRY.
                                dst.Seek(6 + 16 * i, SeekOrigin.Begin);
                                dst.Write(data, 6 + 14 * i, 8);  // First 8bytes are identical.
                                dst.Write(pic.Length);          // ICONDIRENTRY.dwBytesInRes
                                dst.Write(picOffset);           // ICONDIRENTRY.dwImageOffset
                                // Copy a picture.
                                dst.Seek(picOffset, SeekOrigin.Begin);
                                dst.Write(pic, 0, pic.Length);
                                picOffset += pic.Length;
                            }
                            iconBuffer = (((MemoryStream)dst.BaseStream).ToArray());
                        }
                    }
                    counter += 1;
                    return true;
                };
                EnumResourceNames(hModule, RT_GROUP_ICON, callback, IntPtr.Zero);
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    FreeLibrary(hModule);
            }
            if (iconBuffer != null)
            {
                MemoryStream iconStream = new MemoryStream(iconBuffer);
                output = new MemoryStream(iconBuffer);
            }
            return output;
        }
        private static byte[] GetDataFromResource(IntPtr hModule, IntPtr type, IntPtr name)
        {
            IntPtr hResInfo = FindResource(hModule, name, type);
            if (hResInfo == IntPtr.Zero)
                throw new Win32Exception();

            IntPtr hResData = LoadResource(hModule, hResInfo);
            if (hResData == IntPtr.Zero)
                throw new Win32Exception();

            IntPtr pResData = LockResource(hResData);
            if (pResData == IntPtr.Zero)
                throw new Win32Exception();

            uint size = SizeofResource(hModule, hResInfo);
            if (size == 0)
                throw new Win32Exception();

            byte[] buf = new byte[size];
            Marshal.Copy(pResData, buf, 0, buf.Length);

            return buf;
        }
    }
}