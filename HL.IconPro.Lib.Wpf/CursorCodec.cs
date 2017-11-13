using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace HL.IconPro.Lib.Wpf
{
    public abstract class CursorCodec : Codec
    {
        public CursorCodec()
        {
            _Hotspots = new Dictionary<BitmapFrame, ushort[]>();
        }
        private Dictionary<BitmapFrame, ushort[]> _Hotspots;

        public ushort[] GetHotspot(BitmapFrame Frame)
        {
            if (!base.Frames.Contains(Frame)) throw new InvalidOperationException("Cant get hotspot for a inexistent Frame in Frames collection");
            bool sucess = _Hotspots.TryGetValue(Frame, out ushort[] hotspot);
            if (!sucess) hotspot = new ushort[2] { 0, 0 };
            return hotspot;
        }

        public void SetHotspot(BitmapFrame Frame, ushort X, ushort Y)
        {
            if (!base.Frames.Contains(Frame)) throw new InvalidOperationException("Cant set hotspot for a inexistent Frame in Frames collection");
            _Hotspots.Add(Frame, new ushort[2] { X, Y });
        }

    }
}
