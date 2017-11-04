using HL.IconPro.Lib.Core;
using HL.IconPro.Lib.Core.DIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HL.IconPro.Lib.Wpf
{
    public class CursorBitmapDecoder : Codec
    {
        public void Open(System.IO.Stream Source)
        {
            CursorFile iconFile = CursorFile.FromStream(Source);
            foreach (Core.CursorFrame cursorFrame in iconFile.Frames)
            {
                System.IO.MemoryStream frameStream = new System.IO.MemoryStream(cursorFrame.Buffer);
                System.Windows.Media.Imaging.BitmapFrame frame = null;
                if (cursorFrame.Type == Core.FrameType.DIB)
                {
                    DIBitmap dib = DIBitmap.FromStream(frameStream);
                    if (dib.icHeader.biBitCount == 32)
                    {
                        byte[] pixels = dib.icXOR;
                        pixels = DIBitmap.FlipYBuffer(pixels, dib.Width, dib.Height, dib.XorStride);
                        BitmapSource bp = BitmapSource.Create(dib.Width, dib.Height, 96, 96, PixelFormats.Bgra32, null, pixels, dib.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (dib.icHeader.biBitCount == 24)
                    {
                        byte[] pixels = dib.icXOR;
                        pixels = DIBitmap.FlipYBuffer(pixels, dib.Width, dib.Height, dib.XorStride);
                        BitmapSource bp = BitmapSource.Create(dib.Width, dib.Height, 96, 96, PixelFormats.Bgr24, null, pixels, dib.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (dib.icHeader.biBitCount == 8)
                    {
                        byte[] pixels = dib.icXOR;
                        pixels = DIBitmap.FlipYBuffer(pixels, dib.Width, dib.Height, dib.XorStride);
                        BitmapSource bp = BitmapSource.Create(dib.Width, dib.Height, 96, 96, PixelFormats.Indexed8, GetPalette(dib.icColors, true), pixels, dib.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (dib.icHeader.biBitCount == 4)
                    {
                        byte[] pixels = dib.icXOR;
                        pixels = DIBitmap.FlipYBuffer(pixels, dib.Width, dib.Height, dib.XorStride);
                        BitmapSource bp = BitmapSource.Create(dib.Width, dib.Height, 96, 96, PixelFormats.Indexed4, GetPalette(dib.icColors, true), pixels, dib.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else
                    {
                        try
                        {
                            byte[] pixels = dib.icXOR;
                            pixels = DIBitmap.FlipYBuffer(pixels, dib.Width, dib.Height, dib.XorStride);
                            BitmapSource bp = BitmapSource.Create(dib.Width, dib.Height, 96, 96, PixelFormats.Indexed1, GetPalette(dib.icColors, true), pixels, dib.XorStride);
                            frame = BitmapFrame.Create(bp);
                        }
                        catch { continue; }
                    }

                    if (dib.icAND != null)
                    {
                        RGBQUAD[] palette = new RGBQUAD[2] { RGBQUAD.FromRGBA(0, 0, 0, 0), RGBQUAD.FromRGBA(255, 255, 255, 255) };
                        byte[] pixels = dib.icAND;
                        pixels = DIBitmap.FlipYBuffer(pixels, dib.Width, dib.Height, dib.AndStride);
                        BitmapSource AND = BitmapSource.Create(dib.Width, dib.Height,
                            96, 96, PixelFormats.Indexed1, GetPalette(palette, true), pixels, dib.AndStride);

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
                        PngBitmapDecoder decoder = new PngBitmapDecoder(frameStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        frame = decoder.Frames[0];
                    }
                    catch
                    {
                        frame = BitmapFrame.Create(CreateEmptyBitmap(cursorFrame.Width));
                    }
                }
                _Frames.Add(frame);
            }
        }
    }
}
