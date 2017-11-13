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
using System.Text;

namespace HL.IconPro.Lib.Core
{
    /// <summary>
    /// Describes a single frame inside a cursor or a icon file.
    /// If this is a cursor file, then iconDir.wPlanes will mean the cursor hotspot X position.
    /// iconDir.wBitCount will be the Y hotspot position.
    /// As you see, the data structure is exactly the same for both Icon Frames and Cursor Frames.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// The ICONDIRENTRY of for this frame. Note that, it might be null sometimes.
        /// Normally, it will be initialized only when saving the whole icon file.
        /// </summary>
        public ICONDIRENTRY iconDir;
        /// <summary>
        /// The Bitmap data of this frame (Containing the XOR image and the AND mask).
        /// If this frame is a PNG frame, this field will be null
        /// </summary>
        public ICONIMAGE iconImage;
        /// <summary>
        /// Means the Type of this frame, might be PNG or bitmap
        /// </summary>
        public FrameType type;
        /// <summary>
        /// If this is a PNG frame, then this field will be filled with the PNG file. Note that, 
        /// If this is a BMP frame, this field will be null.
        /// </summary>
        public byte[] pngBuffer;

        internal void FromFileBuffer(byte[] buffer)
        {
            MemoryStream memBuffer = new MemoryStream(buffer);
            BinaryReader discover = new BinaryReader(memBuffer);
            if (discover.ReadByte() == 137) //137 or 89 in hex, is the first byte of a PNG file
            {
                byte p = discover.ReadByte();
                byte n = discover.ReadByte();
                byte g = discover.ReadByte();
                if ((p == 80) && (n == 78) && (g == 71)) //In ASCII, the bytes 80 78 71 means PNG
                {
                    memBuffer.Position = 0;
                    ReadPNGBuffer(memBuffer);
                    return;
                }
            }
            memBuffer.Position = 0;
            ReadBMPBuffer(memBuffer);
        }
        private void ReadPNGBuffer(MemoryStream buffer)
        {
            type = FrameType.PNG;
            buffer.Position = 0;
            pngBuffer = buffer.ToArray();
        }
        private void ReadBMPBuffer(MemoryStream buffer)
        {
            type = FrameType.BITMAP;
            buffer.Position = 0;
            BinaryReader reader = new BinaryReader(buffer);
            byte firstByte = reader.ReadByte();
            byte secondByte = reader.ReadByte();

            if ((firstByte == 0x42) && (secondByte == 0x4D)) //In BMP file header, the first two bytes represents, in ASCII code, BM
            {
                buffer.Position = 0;
                buffer.Seek(14, SeekOrigin.Begin);
                iconImage = new ICONIMAGE();
                iconImage.Read(reader);
            }
            else
            {
                buffer.Position = 0;
                var i = new ICONIMAGE();
                i.Read(reader);
                iconImage = i;
            }
        }

        public static Frame Create(byte[] buffer)
        {
            var f = new Frame();
            f.FromFileBuffer(buffer);
            return f;
        }

        public static Frame Create(byte[] buffer, ICONDIRENTRY entry)
        {
            var f = new Frame();
            f.FromFileBuffer(buffer);
            f.iconDir = entry;
            return f;
        }

        public int Width
        {
            get
            {
                if (type == FrameType.BITMAP)
                    return iconImage.Width;
                else
                    //Just for information, in PNG files, the Width and Height are stored in BigEndian byte order
                    //That means, the inverted byte order we are usual to work with.
                    //That's why I built the method GetBigEndianUInt
                    return (int)Utilities.GetBigEndianUInt(pngBuffer, 16);
            }
        }
        public int Height
        {
            get
            {
                if (type == FrameType.BITMAP)
                {
                    return iconImage.Height;
                }
                else
                    //Just for information, in PNG files, the Width and Height are stored in BigEndian byte order
                    //That means, the inverted byte order we are usual to work with.
                    //That's why I built the method GetBigEndianUInt
                    return (int)Utilities.GetBigEndianUInt(pngBuffer, 20);
            }
        }

        public int BitsPerPixel
        {
            get
            {
                if (type == FrameType.BITMAP)
                    return iconImage.icHeader.biBitCount;
                else
                {
                    byte bitdepth = pngBuffer[24];
                    byte colorType = pngBuffer[25];
                    if (colorType == 6)
                        return 32;
                    else if (colorType == 2)
                        return 24;
                    else if (colorType == 3)
                        return 4;
                    else
                        return 8;
                }
            }
        }

        /// <summary>
        /// When creating Icons, this method will generate a valid ICONDIRENTRY 
        /// for this frame based on the Bitmap/PNG from which this Frame was built.
        /// </summary>
        public void CreateIconDir()
        {
            iconDir = new ICONDIRENTRY();
            if (type == FrameType.BITMAP)
            {
                // Width, in pixels, of the image
                if (iconImage.Width == 256)
                    iconDir.bWidth = 0;
                else
                    iconDir.bWidth = (byte)iconImage.Width;
                // Height, in pixels, of the image
                if (iconImage.Height == 256)
                    iconDir.bHeight = 0;
                else
                    iconDir.bHeight = (byte)iconImage.Height;
                // Number of colors in image (0 if >=8bpp)
                if (BitsPerPixel == 4)
                    iconDir.bColorCount = 16;
                else
                    iconDir.bColorCount = 0;

                iconDir.bReserved = 0; // Reserved ( must be 0)
                iconDir.wPlanes = 1; // Color Planes
                iconDir.wBitCount = (ushort)BitsPerPixel; // Bits per pixel

                iconDir.dwBytesInRes = iconImage.SizeOf;

            }
            else
            {
                // Width, in pixels, of the image
                if (Width == 256)
                    iconDir.bWidth = 0;
                else
                    iconDir.bWidth = (byte)Width;
                // Height, in pixels, of the image
                if (Height == 256)
                    iconDir.bHeight = 0;
                else
                    iconDir.bHeight = (byte)Height;
                // Number of colors in image (0 if >=8bpp)
                if (BitsPerPixel == 4)
                    iconDir.bColorCount = 16;
                else
                    iconDir.bColorCount = 0;

                iconDir.bReserved = 0; // Reserved ( must be 0)
                iconDir.wPlanes = 1; // Color Planes
                iconDir.wBitCount = (ushort)BitsPerPixel; // Bits per pixel


                iconDir.dwBytesInRes = (uint)pngBuffer.Length;

            }
        }
    }
}
