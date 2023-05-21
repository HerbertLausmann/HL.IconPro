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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HL.IconPro.Lib.Wpf
{
    public abstract class Codec
    {
        #region Constructors
        public Codec() { _Frames = new BitmapFrameCollection(); }
        #endregion

        #region Destructors

        #endregion

        #region Fields
        protected BitmapFrameCollection _Frames;
        #endregion

        #region Properties
        public ICollection<BitmapFrame> Frames
        {
            get { return _Frames; }
        }
        #endregion

        #region Procedures
        /// <summary>
        /// Obsolete, but I will keep it for now. Use Helpers.MASK(...)
        /// </summary>
        /// <param name="bp"></param>
        /// <returns></returns>
        protected byte[] CreateMask(BitmapSource bp)
        {
            var output = new FormatConvertedBitmap(bp, PixelFormats.Indexed1, null, 80);
            WriteableBitmap w = new WriteableBitmap(output);
            byte[] buffer = new byte[w.BackBufferStride * w.PixelHeight];
            System.Runtime.InteropServices.Marshal.Copy(w.BackBuffer, buffer, 0, w.BackBufferStride * w.PixelHeight);
            return HL.IconPro.Lib.Core.Utilities.FlipYBuffer(buffer, w.PixelWidth, w.PixelHeight, w.BackBufferStride);
        }
        protected byte[] GetImageFrameBuffer(BitmapFrame Source, bool UsePngCompression)
        {
            if (UsePngCompression && (Source.PixelWidth == 256) && Source.Format.BitsPerPixel >= 24)
            {
                PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(Source);
                System.IO.MemoryStream frameBuffer = new System.IO.MemoryStream();
                pngEncoder.Save(frameBuffer);
                pngEncoder = null;
                frameBuffer.Position = 0;
                return frameBuffer.ToArray();
            }
            else
            {
                BmpBitmapEncoder bmpEncoder = new BmpBitmapEncoder();
                bmpEncoder.Frames.Add(Source);
                System.IO.MemoryStream frameBuffer = new System.IO.MemoryStream();
                bmpEncoder.Save(frameBuffer);
                bmpEncoder = null;
                return frameBuffer.ToArray();
            }
        }
        protected BitmapPalette GetPalette(Core.RGBQUAD[] ColorTable, bool opaque)
        {
            List<Color> palette = new List<Color>(ColorTable.Length);
            foreach (Core.RGBQUAD rgb in ColorTable)
            {
                palette.Add(CreateColor(rgb, opaque));
            }

            return new BitmapPalette(palette);
        }
        protected Color CreateColor(Core.RGBQUAD rgb, bool opaque)
        {
            byte a = rgb.rgbReserved;
            byte r = rgb.rgbRed;
            byte g = rgb.rgbGreen;
            byte b = rgb.rgbBlue;
            if (opaque)
                a = 255;
            return Color.FromArgb(a, r, g, b);
        }
        protected BitmapSource CreateEmptyBitmap(int size)
        {
            int stride = size / 8;
            byte[] pixels = new byte[size * stride];

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);
            BitmapPalette myPalette = new BitmapPalette(colors);

            // Creates a new empty image with the pre-defined palette
            BitmapSource image = BitmapSource.Create(
                                                     size, size,
                                                     96, 96,
                                                     PixelFormats.Indexed1,
                                                     myPalette,
                                                     pixels,
                                                     stride);
            return image;
        }
        #endregion

        #region Static

        #endregion

        #region Others

        #endregion
    }
}
