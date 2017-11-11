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

namespace HL.IconPro.Lib.Core
{
    /// <summary>
    /// Some useful methods
    /// </summary>
    public static class Utilities
    {
        /*
         * The methods bellow will multiply/divide by 2 the height of a BITMAPINFOHEADER byte buffer
         * I no longer use these. In fact, I changed to something way better.
         * But I'm keeping them by now.
         */
        public static byte[] TwiceDIBHeight(byte[] buffer)
        {
            int height = BitConverter.ToInt32(buffer, 8);
            height *= 2;
            byte[] twicedHeight = BitConverter.GetBytes(height);
            buffer[8] = twicedHeight[0];
            buffer[9] = twicedHeight[1];
            buffer[10] = twicedHeight[2];
            buffer[11] = twicedHeight[3];
            return buffer;
        }
        public static byte[] UnTwiceDIBHeight(byte[] buffer)
        {
            int height = BitConverter.ToInt32(buffer, 8);
            height /= 2;
            byte[] twicedHeight = BitConverter.GetBytes(height);
            buffer[8] = twicedHeight[0];
            buffer[9] = twicedHeight[1];
            buffer[10] = twicedHeight[2];
            buffer[11] = twicedHeight[3];
            return buffer;
        }

        /// <summary>
        ///  Just for information, in PNG files, the Width and Height are stored in BigEndian byte order
        ///  That means, the inverted byte order we are usual to work with.
        ///   That's why I built the method GetBigEndianUInt
        /// </summary>
        /// <param name="source">Source byte array in big endian byte order</param>
        /// <param name="index">the index where the UInt begins</param>
        /// <returns>RReturns a UInt in the little endian byte order, the usual one</returns>
        public static uint GetBigEndianUInt(byte[] source, int index)
        {
            byte[] uInteger = new byte[4] { source[index + 3], source[index + 2], source[index + 1], source[index] };
            return BitConverter.ToUInt32(uInteger, 0);
        }

        /// <summary>
        /// From the XOR image viewpoint, the AND mask must be flipped vertically. Don't ask why..
        /// Actually, I'm no longer using this function. I found a better way to build a AND mask that doesn't needs this method
        /// But it might be useful in the future, so I will keep it here for now!
        /// </summary>
        /// <param name="Buffer">The AND bitmap mask pixel array</param>
        /// <param name="Width">The width of the bitmap</param>
        /// <param name="Height">The height of the bitmap</param>
        /// <param name="Stride">The stride of the bitmap</param>
        /// <returns>Returns the AND mask pixels flipped vertically</returns>
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

    }
}
