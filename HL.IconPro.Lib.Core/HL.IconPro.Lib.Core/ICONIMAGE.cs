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
    /// A struct in C/C++, this class is the representation of the Bitmap image that makes a Frame.
    /// It doesn't has the Bitmap File Header (The firt 14 bytes of a bitmap file).
    /// </summary>
    public class ICONIMAGE
    {
        public BITMAPINFOHEADER icHeader;      // DIB header
        /// <summary>
        /// This array is the Bitmap Colors Pallete (table)
        /// </summary>
        public RGBQUAD[] icColors;             // Color table
        public byte[] icXOR;                   // DIB bits for XOR mask
        public byte[] icAND;                   // DIB bits for AND mask

        public uint SizeOf => (uint)((icAND == null ? 0 : icAND.Length) +
            (icXOR == null ? 0 : icXOR.Length) +
            icHeader.biSize + ((icColors == null || icColors.Length == 0 ? 0 : icColors.Length) * 4));

        public int XorStride
        {
            get
            {
                return ((((icHeader.biBitCount * icHeader.biWidth) + 31) / 32) * 4);
            }
        }

        public int AndStride
        {
            get
            {
                return ((((1 * icHeader.biWidth) + 31) / 32) * 4);
            }
        }

        public int Width
        {
            get { return icHeader.biWidth; }
        }
        public int Height
        {
            get { return icHeader.biHeight == (icHeader.biWidth * 2) ? (icHeader.biHeight / 2) : icHeader.biHeight; }
        }
        public static ICONIMAGE FromBitmapFile(System.IO.Stream Source)
        {
            Source.Position = 14;
            return FromStream(Source);
        }
        public static ICONIMAGE FromStream(System.IO.Stream Source)
        {
            ICONIMAGE output = new ICONIMAGE();
            BinaryReader Reader = new BinaryReader(Source);
            output.Read(Reader);
            return output;
        }
        public static ICONIMAGE FromBuffer(byte[] Buffer)
        {
            System.IO.MemoryStream mem = new System.IO.MemoryStream(Buffer);
            return FromStream(mem);
        }

        public void Read(BinaryReader Reader)
        {
            icHeader = new BITMAPINFOHEADER();
            icHeader.Read(Reader);
            icColors = new RGBQUAD[icHeader.biClrUsed != 0 ?
                                    icHeader.biClrUsed :
                                    icHeader.biBitCount <= 8 ?
                                        (uint)(1 << icHeader.biBitCount) : 0];
            for (int i = 0; i < icColors.Length; i++)
            {
                RGBQUAD rgb = new RGBQUAD();
                rgb.Read(Reader);
                icColors[i] = rgb;
            }
            if (icColors.Length == 0) icColors = null;
            int datalenght = (int)(Reader.BaseStream.Length - Reader.BaseStream.Position);
            icXOR = new byte[XorStride * Height];
            Reader.Read(icXOR, 0, icXOR.Length);
            if (icXOR.Length == datalenght)
                icAND = null;
            else
            {
                icAND = new byte[AndStride * Height];
                Reader.Read(icAND, 0, icAND.Length);
            }
        }
        public void Write(BinaryWriter Writer)
        {
            icHeader.Write(Writer);
            if (icColors != null && icColors.Length > 0)
            {
                foreach (RGBQUAD rgb in icColors)
                    rgb.Write(Writer);
            }
            if (icXOR != null)
                Writer.Write(icXOR, 0, icXOR.Length);
            if (icAND != null)
                Writer.Write(icAND, 0, icAND.Length);
        }

        public byte[] GetBytes()
        {
            MemoryStream mem = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mem);
            Write(writer);
            byte[] output = mem.ToArray();
            mem.Dispose();
            return output;
        }

        public Stream CreateBitmapFile()
        {
            MemoryStream Output = new MemoryStream();
            Output.WriteByte(0x42);
            Output.WriteByte(0x4D);

            BinaryWriter writer = new BinaryWriter(Output);

            int totalLenght = (int)(14 + icHeader.biSize +
                (icColors != null ? (int)icColors.Length : (int)0) + icXOR.Length);
            writer.Write((uint)(totalLenght));
            writer.Write((ushort)0);
            writer.Write((ushort)0);
            writer.Write((uint)(14 + icHeader.biSize));
            Write(writer);
            return Output;
        }

    }
}
