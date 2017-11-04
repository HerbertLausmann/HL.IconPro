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
    public struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public RGBQUAD[] bmiColors;

        public void Read(Stream Source)
        {
            bmiHeader = new BITMAPINFOHEADER();
            bmiHeader.Read(Source);
            bmiColors = new RGBQUAD[bmiHeader.biClrUsed != 0 ?
                                    bmiHeader.biClrUsed :
                                    bmiHeader.biBitCount <= 8 ?
                                        (uint)(1 << bmiHeader.biBitCount) : 0];
            for (int i = 0; i < bmiColors.Length; i++)
            {
                RGBQUAD rgb = new RGBQUAD();
                rgb.Read(Source);
                bmiColors[i] = rgb;
            }
            if (bmiColors.Length == 0) bmiColors = null;
        }
        public void Write(Stream Output)
        {
            bmiHeader.Write(Output);
            if (bmiColors != null && bmiColors.Length > 0)
            {
                foreach (RGBQUAD rgb in bmiColors)
                    rgb.Write(Output);
            }
        }
    }
}
