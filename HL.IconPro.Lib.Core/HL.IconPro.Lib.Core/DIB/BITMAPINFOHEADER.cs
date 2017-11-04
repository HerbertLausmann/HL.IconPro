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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HL.IconPro.Lib.Core.DIB
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;

        public const int BI_RGB = 0;
        public const int BI_PNG = 5;
        public void Read(Stream Source)
        {
            BinaryReader bin = new BinaryReader(Source);
            biSize = bin.ReadUInt32();
            biWidth = bin.ReadInt32();
            biHeight = bin.ReadInt32();
            biPlanes = bin.ReadUInt16();
            biBitCount = bin.ReadUInt16();
            biCompression = bin.ReadUInt32();
            biSizeImage = bin.ReadUInt32();
            biXPelsPerMeter = bin.ReadInt32();
            biYPelsPerMeter = bin.ReadInt32();
            biClrUsed = bin.ReadUInt32();
            biClrImportant = bin.ReadUInt32();
        }
        public void Write(Stream Output)
        {
            BinaryWriter bin = new BinaryWriter(Output);
            biSize = 40;
            bin.Write(biSize);
            bin.Write(biWidth);
            bin.Write(biHeight);
            bin.Write(biPlanes);
            bin.Write(biBitCount);
            bin.Write(biCompression);
            bin.Write(biSizeImage);
            bin.Write(biXPelsPerMeter);
            bin.Write(biYPelsPerMeter);
            bin.Write(biClrUsed);
            bin.Write(biClrImportant);
        }
    }
}
