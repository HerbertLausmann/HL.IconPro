using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace HL.IconPro.Lib.Core.Motion
{
    /// <summary>
    /// This class describes the anih chunk data, the header of any animated cursor
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 36)]
    public struct ANIHEADER
    {
        ushort cbSize;         // The structure’s size (in bytes) and is always set to 36
        ushort nFrames;        // Number of images (also known as frames) stored in the file
        ushort nSteps;         // Number of frames to be displayed before the animation repeats
        ushort iWidth;         // Width of frame (in pixels)
        ushort iHeight;        // Height of frame (in pixels)
        ushort iBitCount;      // Number of bits per pixel
        ushort nPlanes;        // Number of color planes
        ushort iDispRate;      // Default frame display rate (measured in 1/60th-of-a-second units)
        ushort bfAttributes;   // ANI attribute bit flags

        public static ANIHEADER FromBuffer(byte[] source)
        {
            // Pin the managed memory while, copy it out the data, then unpin it
            GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            ANIHEADER header = (ANIHEADER)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(ANIHEADER));
            handle.Free();
            return header;
        }

        public byte[] GetBuffer()
        {
            int size = Marshal.SizeOf(this);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }
}
