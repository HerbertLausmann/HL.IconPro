using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HL.IconPro.Lib.Core;

namespace HL.IconPro.Lib.Wpf.Motion
{
    public class AnimatedCursorBitmapDecoder : Codec
    {
        public void Open(Stream Source)
        {
            //Note that, each frame of ani file might be a full (.cur) cursor file or a full (.ico) icon file or even
            //a Bitmap file;
            //I'm guessing that you are luckly enought to be opening a ANI file that has its frames
            //encoded .ico ;)
            Core.Motion.ANI ani = new Core.Motion.ANI();
            ani.Load(Source);
            foreach (byte[] frame in ani.Frames)
            {
                IconBitmapDecoder decoder = new IconBitmapDecoder();
                decoder.Open(new MemoryStream(frame));
                foreach (var f in decoder.Frames)
                    this._Frames.Add(f);
            }
        }
    }
}
