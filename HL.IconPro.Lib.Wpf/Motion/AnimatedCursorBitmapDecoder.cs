using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HL.IconPro.Lib.Core;

namespace HL.IconPro.Lib.Wpf.Motion
{
    /// <summary>
    /// Decodes an Animated Windows Cursor into a collection of BitmapFrames
    /// </summary>
    public class AnimatedCursorBitmapDecoder : AnimatedCursorCodec
    {
        public void Open(Stream Source)
        {
            //Note that, each frame of ani file might be a full (.cur) cursor file or a full (.ico) icon file or even
            //a Bitmap file;
            //I'm guessing that you are luckly enought to be opening a ANI file that has its frames
            //encoded .ico ;)
            Core.Motion.ANI ani = new Core.Motion.ANI();
            ani.Load(Source);
            _Name = ani.INAM;
            _Author = ani.IART;
            _FrameRate = ani.Header.iDispRate;

            foreach (byte[] frame in ani.Frames)
            {
                CursorBitmapDecoder decoder = new CursorBitmapDecoder();
                decoder.Open(new MemoryStream(frame));
                foreach (var f in decoder.Frames)
                {
                    this._Frames.Add(f);
                    var hotspot = decoder.GetHotspot(f);
                    SetHotspot(f, hotspot[0], hotspot[1]);
                }
            }

        }
    }
}
