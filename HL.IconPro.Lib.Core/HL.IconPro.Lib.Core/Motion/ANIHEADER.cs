/*
 *  Icon Pro
 *  Copyright (C) 2017 Herbert Lausmann. All rights reserved.
 *  http://herbertdotlausmann.wordpress.com/
 *  herbert.lausmann@hotmail.com
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *   1. Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *   2. Redistributions in binary form must reproduce the above copyright
 *      notice and this list of conditions.    
 */
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
        public uint cbSize;         // The structure’s size (in bytes) and is always set to 36
        public uint nFrames;        // Number of images (also known as frames) stored in the file
        public uint nSteps;         // Number of frames to be displayed before the animation repeats
        public uint iWidth;         // Width of frame (in pixels)
        public uint iHeight;        // Height of frame (in pixels)
        public uint iBitCount;      // Number of bits per pixel
        public uint nPlanes;        // Number of color planes
        public uint iDispRate;      // Default frame display rate (measured in 1/60th-of-a-second units)
        public uint bfAttributes;   // ANI attribute bit flags

        public static ANIHEADER FromBuffer(byte[] source)
        {
            // Pin the managed memory while, copy it out the data, then unpin it
            GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            ANIHEADER header = (ANIHEADER)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(ANIHEADER));
            handle.Free();
            return header;

            // MemoryStream stream = new MemoryStream(source);
            // BinaryReader reader = new BinaryReader(stream);
            // ANIHEADER header = new ANIHEADER();
            //
            // header.cbSize = reader.ReadUInt32();
            // header.nFrames = reader.ReadUInt32();
            // header.nSteps = reader.ReadUInt32();
            // header.iWidth = reader.ReadUInt32();
            // header.iHeight = reader.ReadUInt32();
            // header.iBitCount = reader.ReadUInt32();
            // header.nPlanes = reader.ReadUInt32();
            // header.iDispRate = reader.ReadUInt32();
            // header.bfAttributes = reader.ReadUInt32();
            //
            // reader.Close();
            // stream.Close();
            // return header;
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

            // MemoryStream stream = new MemoryStream(36);
            // BinaryWriter writer = new BinaryWriter(stream);
            // cbSize = 36;
            // writer.Write(cbSize);
            // writer.Write(nFrames);
            // writer.Write(nSteps);
            // writer.Write(iWidth);
            // writer.Write(iHeight);
            // writer.Write(iBitCount);
            // writer.Write(nPlanes);
            // writer.Write(iDispRate);
            // writer.Write(bfAttributes);
            // writer.Flush();
            //
            // return stream.ToArray();
        }
    }
}
