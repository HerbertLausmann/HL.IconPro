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

namespace HL.IconPro.Lib.Core
{
    /// <summary>
    /// A struct in C/C++, this class represents something like a Header for a frame inside an icon/cursor file
    /// </summary>
    public class ICONDIRENTRY
    {
        public byte bWidth;          // Specifies image width in pixels. Can be any number between 0 and 255. Value 0 means image width is 256 pixels.
        public byte bHeight;         // Specifies image height in pixels. Can be any number between 0 and 255. Value 0 means image height is 256 pixels.
        public byte bColorCount;     // Specifies number of colors in the color palette. Should be 0 if the image does not use a color palette.
        public byte bReserved;       // Reserved. Should be 0.


        /// <summary>
        /// In ICO format: Specifies color planes. Should be 0 or 1.
        /// In CUR format: Specifies the horizontal coordinates of the hotspot in number of pixels from the left.
        /// </summary>
        public ushort wPlanes = 1;


        /// <summary>
        /// In ICO format: Specifies bits per pixel.
        /// In CUR format: Specifies the vertical coordinates of the hotspot in number of pixels from the top.
        /// </summary>
        public ushort wBitCount;


        public uint dwBytesInRes;     // Specifies the size of the image's data in bytes
        public uint dwImageOffset;    // Specifies the offset of BMP or PNG data from the beginning of the ICO/CUR file

        public void Read(System.IO.BinaryReader Reader)
        {
            bWidth = Reader.ReadByte();
            bHeight = Reader.ReadByte();
            bColorCount = Reader.ReadByte();
            bReserved = Reader.ReadByte();

            wPlanes = Reader.ReadUInt16();
            wBitCount = Reader.ReadUInt16();

            dwBytesInRes = Reader.ReadUInt32();
            dwImageOffset = Reader.ReadUInt32();
        }

        public void Write(System.IO.BinaryWriter Writer)
        {
            Writer.Write(bWidth);
            Writer.Write(bHeight);
            Writer.Write(bColorCount);
            Writer.Write(bReserved);
            Writer.Write(wPlanes);
            Writer.Write(wBitCount);
            Writer.Write(dwBytesInRes);
            Writer.Write(dwImageOffset);
        }
    }
}
