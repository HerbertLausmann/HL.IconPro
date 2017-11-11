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
    public class CursorBitmapDecoder : Codec
    {
        public void Open(System.IO.Stream Source)
        {
            File CursorFile = new File();
            CursorFile.Read(Source);
            if (CursorFile.Type != FileType.CURSOR) throw new InvalidOperationException();
            foreach (Frame CursorFrame in CursorFile.Frames)
            {
                System.Windows.Media.Imaging.BitmapFrame frame = null;
                if (CursorFrame.type == FrameType.BITMAP)
                {
                    if (CursorFrame.iconImage.icHeader.biBitCount == 32)
                    {
                        byte[] pixels = CursorFrame.iconImage.icXOR;
                        pixels = Utilities.FlipYBuffer(pixels, CursorFrame.Width, CursorFrame.Height, CursorFrame.iconImage.XorStride);
                        BitmapSource bp = BitmapSource.Create(CursorFrame.Width, CursorFrame.Height, 96, 96, PixelFormats.Bgra32, null, pixels, CursorFrame.iconImage.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (CursorFrame.iconImage.icHeader.biBitCount == 24)
                    {
                        byte[] pixels = CursorFrame.iconImage.icXOR;
                        pixels = Utilities.FlipYBuffer(pixels, CursorFrame.Width, CursorFrame.Height, CursorFrame.iconImage.XorStride);
                        BitmapSource bp = BitmapSource.Create(CursorFrame.Width, CursorFrame.Height, 96, 96, PixelFormats.Bgra32, null, pixels, CursorFrame.iconImage.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (CursorFrame.iconImage.icHeader.biBitCount == 8)
                    {
                        byte[] pixels = CursorFrame.iconImage.icXOR;
                        pixels = Utilities.FlipYBuffer(pixels, CursorFrame.Width, CursorFrame.Height, CursorFrame.iconImage.XorStride);
                        BitmapSource bp = BitmapSource.Create(CursorFrame.Width, CursorFrame.Height, 96, 96, PixelFormats.Indexed8, GetPalette(CursorFrame.iconImage.icColors, true), pixels, CursorFrame.iconImage.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else if (CursorFrame.iconImage.icHeader.biBitCount == 4)
                    {
                        byte[] pixels = CursorFrame.iconImage.icXOR;
                        pixels = Utilities.FlipYBuffer(pixels, CursorFrame.Width, CursorFrame.Height, CursorFrame.iconImage.XorStride);
                        BitmapSource bp = BitmapSource.Create(CursorFrame.Width, CursorFrame.Height, 96, 96, PixelFormats.Indexed4, GetPalette(CursorFrame.iconImage.icColors, true), pixels, CursorFrame.iconImage.XorStride);
                        frame = BitmapFrame.Create(bp);
                    }
                    else
                    {
                        try
                        {
                            byte[] pixels = CursorFrame.iconImage.icXOR;
                            pixels = Utilities.FlipYBuffer(pixels, CursorFrame.Width, CursorFrame.Height, CursorFrame.iconImage.XorStride);
                            BitmapSource bp = BitmapSource.Create(CursorFrame.Width, CursorFrame.Height, 96, 96, PixelFormats.Indexed1, GetPalette(CursorFrame.iconImage.icColors, true), pixels, CursorFrame.iconImage.XorStride);
                            frame = BitmapFrame.Create(bp);
                        }
                        catch { continue; }
                    }

                    if (CursorFrame.iconImage.icAND?.Length > 0)
                    {
                        RGBQUAD[] palette = new RGBQUAD[2] { RGBQUAD.FromRGBA(0, 0, 0, 0), RGBQUAD.FromRGBA(255, 255, 255, 255) };
                        byte[] pixels = CursorFrame.iconImage.icAND;
                        pixels = Utilities.FlipYBuffer(pixels, CursorFrame.Width, CursorFrame.Height, CursorFrame.iconImage.AndStride);
                        BitmapSource AND = BitmapSource.Create(CursorFrame.Width, CursorFrame.Height,
                            96, 96, PixelFormats.Indexed1, GetPalette(palette, true), pixels, CursorFrame.iconImage.AndStride);

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
                        PngBitmapDecoder decoder = new PngBitmapDecoder(new System.IO.MemoryStream(CursorFrame.pngBuffer), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        frame = decoder.Frames[0];
                    }
                    catch
                    {
                        frame = BitmapFrame.Create(CreateEmptyBitmap(CursorFrame.Width));
                    }
                }
                _Frames.Add(frame);
            }
        }
    }
}
