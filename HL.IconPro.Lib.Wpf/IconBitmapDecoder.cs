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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HL.IconPro.Lib.Core;

namespace HL.IconPro.Lib.Wpf
{
    public class IconBitmapDecoder : Codec
    {
        public void Open(System.IO.Stream Source)
        {
            File iconFile = new File();
            iconFile.Read(Source);
            foreach (Frame iconFrame in iconFile.Frames)
            {
                System.Windows.Media.Imaging.BitmapFrame frame = null;
                if (iconFrame.type == FrameType.BITMAP)
                {
                    if (iconFrame.iconImage.icHeader.biBitCount == 32)
                    {
                        byte[] pixels = iconFrame.iconImage.icXOR;
                        pixels = Utilities.FlipYBuffer(pixels, iconFrame.Width, iconFrame.Height, iconFrame.iconImage.XorStride);
                        BitmapSource bp = BitmapSource.Create(iconFrame.Width, iconFrame.Height, 96, 96, PixelFormats.Bgra32, null, pixels, iconFrame.iconImage.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (iconFrame.iconImage.icHeader.biBitCount == 24)
                    {
                        byte[] pixels = iconFrame.iconImage.icXOR;
                        pixels = Utilities.FlipYBuffer(pixels, iconFrame.Width, iconFrame.Height, iconFrame.iconImage.XorStride);
                        BitmapSource bp = BitmapSource.Create(iconFrame.Width, iconFrame.Height, 96, 96, PixelFormats.Bgr24, null, pixels, iconFrame.iconImage.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (iconFrame.iconImage.icHeader.biBitCount == 8)
                    {
                        byte[] pixels = iconFrame.iconImage.icXOR;
                        pixels = Utilities.FlipYBuffer(pixels, iconFrame.Width, iconFrame.Height, iconFrame.iconImage.XorStride);
                        BitmapSource bp = BitmapSource.Create(iconFrame.Width, iconFrame.Height, 96, 96, PixelFormats.Indexed8, GetPalette(iconFrame.iconImage.icColors, true), pixels, iconFrame.iconImage.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (iconFrame.iconImage.icHeader.biBitCount == 4)
                    {
                        byte[] pixels = iconFrame.iconImage.icXOR;
                        pixels = Utilities.FlipYBuffer(pixels, iconFrame.Width, iconFrame.Height, iconFrame.iconImage.XorStride);
                        BitmapSource bp = BitmapSource.Create(iconFrame.Width, iconFrame.Height, 96, 96, PixelFormats.Indexed4, GetPalette(iconFrame.iconImage.icColors, true), pixels, iconFrame.iconImage.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else
                    {
                        try
                        {
                            byte[] pixels = iconFrame.iconImage.icXOR;
                            pixels = Utilities.FlipYBuffer(pixels, iconFrame.Width, iconFrame.Height, iconFrame.iconImage.XorStride);
                            BitmapSource bp = BitmapSource.Create(iconFrame.Width, iconFrame.Height, 96, 96, PixelFormats.Indexed1, GetPalette(iconFrame.iconImage.icColors, true), pixels, iconFrame.iconImage.XorStride);
                            frame = BitmapFrame.Create(bp);
                        }
                        catch { continue; }
                    }

                    if (iconFrame.iconImage.icAND?.Length > 0)
                    {
                        RGBQUAD[] palette = new RGBQUAD[2] { RGBQUAD.FromRGBA(0, 0, 0, 0), RGBQUAD.FromRGBA(255, 255, 255, 255) };
                        byte[] pixels = iconFrame.iconImage.icAND;
                        pixels = Utilities.FlipYBuffer(pixels, iconFrame.Width, iconFrame.Height, iconFrame.iconImage.AndStride);
                        BitmapSource AND = BitmapSource.Create(iconFrame.Width, iconFrame.Height,
                            96, 96, PixelFormats.Indexed1, GetPalette(palette, true), pixels, iconFrame.iconImage.AndStride);

                        EditableBitmapImage editableXOR = new EditableBitmapImage(frame);
                        EditableBitmapImage editableAND = new EditableBitmapImage(AND);
                        for (int x = 0; x < editableXOR.PixelWidth; x++)
                        {
                            for (int y = 0; y < editableXOR.PixelHeight; y++)
                            {
                                Color px = editableAND.GetPixel(x, y);
                                if (px == Colors.White)
                                {
                                    Color c = editableXOR.GetPixel(x, y);
                                    c.A = 0;
                                    editableXOR.SetPixel(x, y, c);
                                }
                            }
                        }
                        if (frame.Format.BitsPerPixel == 32)
                        {
                            // Do nothing
                        }
                        else if (frame.Format.BitsPerPixel == 24)
                        {
                            frame = BitmapFrame.Create(editableXOR);
                        }
                        else if (frame.Format.BitsPerPixel == 8)
                        {
                            BitmapSource s = Helpers.Get8BitImage(editableXOR);
                            frame = BitmapFrame.Create(s);
                        }
                        else if (frame.Format.BitsPerPixel == 4)
                        {
                            BitmapSource s = Helpers.Get4BitImage(editableXOR);
                            frame = BitmapFrame.Create(s);
                        }
                    }
                }
                else
                {
                    try
                    {
                        PngBitmapDecoder decoder = new PngBitmapDecoder(new System.IO.MemoryStream(iconFrame.pngBuffer), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        frame = decoder.Frames[0];
                    }
                    catch
                    {
                        frame = BitmapFrame.Create(CreateEmptyBitmap(iconFrame.Width));
                    }
                }
                _Frames.Add(frame);
            }
        }
    }
}
