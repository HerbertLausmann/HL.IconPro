using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace HL.IconPro.Lib.Wpf.Motion
{
    /// <summary>
    /// Encodes a sequence of BitmapFrames into an Animated Windows Cursor
    /// </summary>
    public class AnimatedCursorBitmapEncoder : AnimatedCursorCodec
    {
        public void Save(Stream Output)
        {
            Core.Motion.ANI ANI = new Core.Motion.ANI();

            var header = new Core.Motion.ANIHEADER();
            header.iDispRate = FrameRate;
            header.nFrames = (ushort)this.Frames.Count;
            header.nSteps = (ushort)this.Frames.Count;
            header.bfAttributes = 0;
            ANI.Header = header;

            ANI.IART = Author;
            ANI.INAM = Name;

            foreach (BitmapFrame frame in Frames)
            {
                CursorBitmapEncoder enc = new CursorBitmapEncoder();
                enc.Frames.Add(frame);
                enc.SetHotspot(frame, GetHotspot(frame)[0], GetHotspot(frame)[1]);
                MemoryStream encStream = new MemoryStream();
                enc.Save(encStream);
                ANI.Frames.Add(encStream.ToArray());
             }
            ANI.Write(Output);
        }

        public new ushort FrameRate
        {
            get
            {
                return _FrameRate;
            }
            set
            {
                _FrameRate = value;
            }
        }
    }
}
