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

namespace HL.IconPro.Lib.Core
{
    /// <summary>
    /// A struct in C/C++, this class describes the main header of a bitmap
    /// </summary>
    public class BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes = 1;
        public ushort biBitCount;
        public uint biCompression = 0;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;

        //public const int BI_RGB = 0;
        //public const int BI_PNG = 5;

        public void Read(BinaryReader Reader)
        {
            biSize = Reader.ReadUInt32();
            biWidth = Reader.ReadInt32();
            biHeight = Reader.ReadInt32();
            biPlanes = Reader.ReadUInt16();
            biBitCount = Reader.ReadUInt16();
            biCompression = Reader.ReadUInt32();
            biSizeImage = Reader.ReadUInt32();
            biXPelsPerMeter = Reader.ReadInt32();
            biYPelsPerMeter = Reader.ReadInt32();
            biClrUsed = Reader.ReadUInt32();
            biClrImportant = Reader.ReadUInt32();
        }
        public void Write(BinaryWriter Writer)
        {
            biSize = 40;
            Writer.Write(biSize);
            Writer.Write(biWidth);
            Writer.Write(biHeight);
            Writer.Write(biPlanes);
            Writer.Write(biBitCount);
            Writer.Write(biCompression);
            Writer.Write(biSizeImage);
            Writer.Write(biXPelsPerMeter);
            Writer.Write(biYPelsPerMeter);
            Writer.Write(biClrUsed);
            Writer.Write(biClrImportant);
        }
    }
}
