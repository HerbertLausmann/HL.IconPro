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
    public static class Utilities
    {
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

        public static uint GetBigEndianUInt(byte[] source, int index)
        {
            byte[] uInteger = new byte[4] { source[index + 3], source[index + 2], source[index + 1], source[index] };
            return BitConverter.ToUInt32(uInteger, 0);
        }
    }
}
