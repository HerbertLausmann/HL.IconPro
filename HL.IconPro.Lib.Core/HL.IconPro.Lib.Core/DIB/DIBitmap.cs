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
using System.Linq;
using System.Text;
using System.IO;

namespace HL.IconPro.Lib.Core.DIB
{
    public sealed class DIBitmap
    {
        #region Constructors

        #endregion

        #region Destructors

        #endregion

        #region Fields
        public BITMAPINFOHEADER icHeader;// DIB header
        public RGBQUAD[] icColors;       // Color table
        public byte[] icXOR;             // DIB bits for XOR mask
        public byte[] icAND;             // DIB bits for AND mask
        #endregion

        #region Properties
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
            get { return icHeader.biHeight == (icHeader.biWidth * 2) ? (icHeader.biHeight /= 2) : icHeader.biHeight; }
        }

        public int BitsPerPixel
        {
            get { return icHeader.biBitCount; }
        }
        #endregion

        #region Procedures
        public void Read(Stream Source)
        {
            icHeader = new BITMAPINFOHEADER();
            icHeader.Read(Source);
            icColors = new RGBQUAD[icHeader.biClrUsed != 0 ?
                                    icHeader.biClrUsed :
                                    icHeader.biBitCount <= 8 ?
                                        (uint)(1 << icHeader.biBitCount) : 0];
            for (int i = 0; i < icColors.Length; i++)
            {
                RGBQUAD rgb = new RGBQUAD();
                rgb.Read(Source);
                icColors[i] = rgb;
            }
            if (icColors.Length == 0) icColors = null;
            int datalenght = (int)(Source.Length - Source.Position);
            icXOR = new byte[XorStride * Height];
            Source.Read(icXOR, 0, icXOR.Length);
            if (icXOR.Length == datalenght)
                icAND = null;
            else
            {
                icAND = new byte[AndStride * Height];
                Source.Read(icAND, 0, icAND.Length);
            }
            #region ObseleteButFunctional
            //byte[] icData = new byte[Source.Length - Source.Position];
            //Source.Read(icData, 0, (int)(Source.Length - Source.Position));
            //icXOR = new byte[XorStride * Height];
            //Array.Copy(icData, icXOR, icXOR.Length);
            //if (icXOR.Length < icData.Length)
            //{
            //    icAND = new byte[icData.Length - icXOR.Length];
            //    Array.ConstrainedCopy(icData, icXOR.Length, icAND, 0, icAND.Length);
            //}
            //else
            //    icAND = null;
            #endregion
        }
        public void Write(Stream Output)
        {
            icHeader.Write(Output);
            if (icColors != null && icColors.Length > 0)
            {
                foreach (RGBQUAD rgb in icColors)
                    rgb.Write(Output);
            }
            if (icXOR != null)
                Output.Write(icXOR, 0, icXOR.Length);
            if (icAND != null)
                Output.Write(icAND, 0, icAND.Length);
        }

        public byte[] GetBytes()
        {
            MemoryStream mem = new MemoryStream();
            Write(mem);
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
            Write(Output);
            return Output;
        }

        #endregion

        #region Static
        public static DIBitmap FromStream(Stream Source)
        {
            DIBitmap output = new DIBitmap();
            output.Read(Source);
            return output;
        }
        public static DIBitmap FromBuffer(byte[] Buffer)
        {
            MemoryStream mem = new MemoryStream(Buffer);
            return FromStream(mem);
        }
        public static DIBitmap FromBitmapFile(Stream Source)
        {
            Source.Position = 14;
            return FromStream(Source);
        }
        public static byte[] FlipYBuffer(byte[] Buffer, int Width, int Height, int Stride)
        {
            byte[] output = new byte[Buffer.Length];
            int xCounter = 0;
            int yCounter = 1;
            for (int y = Height; y > 0; y--)
            {
                for (int x = ((Buffer.Length) - (Stride * yCounter)); x < (Stride * y); x++)
                {
                    output[xCounter] = Buffer[x];
                    xCounter += 1;
                }
                yCounter += 1;
            }
            return output;
        }
        #endregion

        #region Others

        #endregion
    }
}