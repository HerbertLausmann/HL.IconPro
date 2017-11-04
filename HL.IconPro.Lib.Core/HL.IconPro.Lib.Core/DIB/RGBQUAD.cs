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
    public struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;

        public void Read(Stream Source)
        {
            rgbBlue = (byte)Source.ReadByte();
            rgbGreen = (byte)Source.ReadByte();
            rgbRed = (byte)Source.ReadByte();
            rgbReserved = (byte)Source.ReadByte();
        }
        public void Write(Stream Output)
        {
            Output.WriteByte(rgbBlue);
            Output.WriteByte(rgbGreen);
            Output.WriteByte(rgbRed);
            Output.WriteByte(rgbReserved);
        }

        public void SetTransparency(bool Transparent)
        {
            if (Transparent) rgbReserved = 0;
            else rgbReserved = 255;
        }

        public static RGBQUAD FromRGB(byte r, byte g, byte b)
        {
            RGBQUAD output = new RGBQUAD();
            output.rgbRed = r;
            output.rgbGreen = g;
            output.rgbBlue = b;
            output.rgbReserved = 255;
            return output;
        }
        public static RGBQUAD FromRGBA(byte r, byte g, byte b, byte a)
        {
            RGBQUAD output = FromRGB(r, g, b);
            output.rgbReserved = a;
            return output;
        }
    }
}
