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
            BitmapSource @out = new FormatConvertedBitmap(Source, PixelFormats.Indexed8, BitmapPalettes.Halftone252Transparent, 75);
            return @out;
        }

        public static BitmapSource GetRGBA32BitImage(BitmapSource Source)
        {
            FormatConvertedBitmap @out = new FormatConvertedBitmap(Source, PixelFormats.Bgra32, null, 0);
            return @out;
        }

        public static BitmapSource GetResized(BitmapSource Source, int Size)
        {
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
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref Core.DIB.BITMAPINFOHEADER pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        public static string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public static string CoreVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetReferencedAssemblies().
 First(x => x.Name == ("HL.IconPro.Lib.Core")).Version.ToString();
            }
        }
    }
}