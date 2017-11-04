using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HL.IconPro.Lib.Wpf.Tools
{
    public static class SVGConverter
    {
        public static BitmapSource GetBitmapSource(Stream Source, float overAllSize = 256.0f)
        {
            Svg.SvgDocument doc = Svg.SvgDocument.Open<Svg.SvgDocument>(Source);

            float w = doc.GetDimensions().Width;
            float h = doc.GetDimensions().Height;

            float ratioX = overAllSize / w;
            float ratioY = overAllSize / h;

            float ratio = ratioX < ratioY ? ratioX : ratioY;

            int newHeight = Convert.ToInt32(h * ratio);
            int newWidth = Convert.ToInt32(w * ratio);

            if (doc.Width.Type != Svg.SvgUnitType.Percentage && doc.Height.Type != Svg.SvgUnitType.Percentage)
                doc.Transforms.Add(new Svg.Transforms.SvgScale(ratio));

            Bitmap HBITMAP = doc.Draw(newWidth, newHeight);

            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(HBITMAP.GetHbitmap(), IntPtr.Zero,
     System.Windows.Int32Rect.Empty,
     BitmapSizeOptions.FromEmptyOptions());
            return bs;
        }
    }
}
