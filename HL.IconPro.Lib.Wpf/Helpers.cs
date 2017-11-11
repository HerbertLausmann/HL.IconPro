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
using HL.IconPro.Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HL.IconPro.Lib.Wpf
{
    public static class Helpers
    {
        public static BitmapSource Get4BitImage(BitmapSource Source)
        {
            List<Color> palleteColors = new List<Color>(BitmapPalettes.Halftone8Transparent.Colors);
            palleteColors.RemoveAt(15);
            BitmapSource @out = new FormatConvertedBitmap(Source, PixelFormats.Indexed4, new BitmapPalette(palleteColors), 75);
            return @out;
        }

        public static BitmapSource Get8BitImage(BitmapSource Source)
        {
            BitmapSource @out = new FormatConvertedBitmap(Source, PixelFormats.Indexed8, BitmapPalettes.Halftone256Transparent, 75);
            return @out;
        }

        public static BitmapSource GetRGBA32BitImage(BitmapSource Source)
        {
            FormatConvertedBitmap @out = new FormatConvertedBitmap(Source, PixelFormats.Bgra32, null, 0);
            return @out;
        }

        public static BitmapSource GetResized(BitmapSource src, int Size)
        {
            BitmapSource Source = src.Clone();
            BitmapSource backup = Source.Clone();
            try
            {
                TransformedBitmap scaled = new TransformedBitmap();
                scaled.BeginInit();
                scaled.Source = Source;
                double scX = (double)Size / (double)Source.PixelWidth;
                double scy = (double)Size / (double)Source.PixelHeight;
                ScaleTransform tr = new ScaleTransform(scX, scy, Source.Width / 2, Source.Height / 2);
                scaled.Transform = tr;
                scaled.EndInit();
                Source = scaled;
            }
            catch
            {
                Source = backup;
            }
            int stride = ((((Source.Format.BitsPerPixel * Source.PixelWidth) + 31) / 32) * 4);
            byte[] sourceBuffer = new byte[stride * Source.PixelHeight];
            Source.CopyPixels(sourceBuffer, stride, 0);
            BitmapSource bmp = BitmapSource.Create(Source.PixelWidth, Source.PixelHeight, 96, 96, Source.Format, Source.Palette, sourceBuffer, stride);
            return bmp;
        }

        public static BitmapSource AlphaBlend(BitmapSource Source, byte Threshold)
        {
            EditableBitmapImage eb = new EditableBitmapImage(Source);
            System.Windows.Media.Color first = eb.GetPixel(0, 0);
            if (first.A != 255) return eb;
            for (int y = 0; y < Source.PixelHeight; y++)
            {
                for (int x = 0; x < Source.PixelWidth; x++)
                {
                    System.Windows.Media.Color pixel = eb.GetPixel(x, y);
                    if (pixel.B <= Threshold && pixel.G <= Threshold && pixel.R <= Threshold && pixel.A == 255)
                    {
                        eb.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                    }
                }
            }
            return eb;
        }

        public static ushort[] ScaleHotspot(ushort sourceHotspotX, ushort sourceHotspotY, int Width, int Height)
        {
            float ratioX = (float)sourceHotspotX / (float)Width;
            float ratioY = (float)sourceHotspotY / (float)Height;

            float ratio = ratioX < ratioY ? ratioX : ratioY;

            ushort newHeight = Convert.ToUInt16(sourceHotspotX * ratio);
            ushort newWidth = Convert.ToUInt16(sourceHotspotY * ratio);
            return new ushort[2] { newWidth, newHeight };
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref Core.BITMAPINFOHEADER pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        public static string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public static string CoreVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetReferencedAssemblies().
 First(x => x.Name == ("HL.IconPro.Lib.Core")).Version.ToString();
            }
        }

        //  Copyright (c) 2006, Gustavo Franco
        //  Email:  gustavo_franco@hotmail.com
        //  All rights reserved.

        //  Redistribution and use in source and binary forms, with or without modification, 
        //  are permitted provided that the following conditions are met:

        //  Redistributions of source code must retain the above copyright notice, 
        //  this list of conditions and the following disclaimer. 
        //  Redistributions in binary form must reproduce the above copyright notice, 
        //  this list of conditions and the following disclaimer in the documentation 
        //  and/or other materials provided with the distribution. 

        //  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
        //  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
        //  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
        //  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
        //  REMAINS UNCHANGED.
        public static void MASK(int BPP, ref byte[] XOR, ref byte[] AND, int ANDSTRIDE, int XORSTRIDE, int WIDTH, int HEIGHT, RGBQUAD[] SOURCEPALLETE)
        {
            //I took the code bellow from:
            //https://www.codeproject.com/Articles/16178/IconLib-Icons-Unfolded-MultiIcon-and-Windows-Vista

            AND = new byte[Math.Abs(ANDSTRIDE) * HEIGHT];
            //Let extract the AND image from the XOR image
            int strideC = Math.Abs(XORSTRIDE);
            int strideB = Math.Abs(ANDSTRIDE);
            int bpp = BPP;
            int posCY;
            int posCX;
            int posBY;
            int color;
            Color tColor;
            RGBQUAD paletteColor;

            for (int y = 0; y < HEIGHT; y++)
            {
                posBY = strideB * y;
                posCY = strideC * y;
                for (int x = 0; x < WIDTH; x++)
                {
                    switch (bpp)
                    {
                        case 1:
                            AND[(x >> 3) + posCY] = (byte)XOR[(x >> 3) + posCY];
                            break;
                        case 4:
                            color = XOR[(x >> 1) + posCY];
                            paletteColor = SOURCEPALLETE[(x & 1) == 0 ? color >> 4 : color & 0x0F];
                            if (paletteColor.rgbReserved < 255)
                            {
                                AND[(x >> 3) + posBY] |= (byte)(0x80 >> (x & 7));
                                XOR[(x >> 1) + posCY] &= (byte)((x & 1) == 0 ? 0x0F : 0xF0);
                            }
                            break;
                        case 8:
                            color = XOR[x + posCY];
                            paletteColor = SOURCEPALLETE[color];
                            if (paletteColor.rgbReserved < 255)
                            {
                                AND[(x >> 3) + posBY] |= (byte)(0x80 >> (x & 7));
                                XOR[x + posCY] = 0;
                            }
                            break;
                        case 16:
                            throw new NotSupportedException("16 bpp images are not supported for Icons");
                        case 24:
                            posCX = x * 3;
                            tColor = Color.FromArgb(0, XOR[posCX + posCY + 0],
                                                        XOR[posCX + posCY + 1],
                                                        XOR[posCX + posCY + 2]);
                            if (tColor.A < 255)
                                AND[(x >> 3) + posBY] |= (byte)(0x80 >> (x & 7));
                            break;
                        case 32:
                            if (XOR[(x << 2) + posCY + 3] == 0)
                                AND[(x >> 3) + posBY] |= (byte)(0x80 >> (x & 7));


                            //    if (XOR[(x << 2) + posCY + 0] == MASKEDCOLOR.B &&
                            //        XOR[(x << 2) + posCY + 1] == MASKEDCOLOR.G &&
                            //        XOR[(x << 2) + posCY + 2] == MASKEDCOLOR.R)
                            //    {
                            //        AND[(x >> 3) + posBY] |= (byte)(0x80 >> (x & 7));
                            //        XOR[(x << 2) + posCY + 0] = 0;
                            //        XOR[(x << 2) + posCY + 1] = 0;
                            //        XOR[(x << 2) + posCY + 2] = 0;
                            //    }
                            //    else
                            //    {
                            //        XOR[(x << 2) + posCY + 3] = 255;
                            //    }
                            break;
                    }
                }
            }
            //Utilities.FlipYBuffer(AND, WIDTH, HEIGHT, ANDSTRIDE);
        }

        public static bool CompareRGBQUADToColor(RGBQUAD rgbQuad, Color color)
        {
            return rgbQuad.rgbRed == color.R && rgbQuad.rgbGreen == color.G && rgbQuad.rgbBlue == color.B;
        }
    }
}