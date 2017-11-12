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
        //For now, there is the need to implement methods and properties to get some metadata of the file like:
        //The frame rate, size and etc
        //The name and author;
        //The hotspots...
        //There are still some CHUNKS in the ANI file that I'm ignoring for now.
        //For example, there is a chunk for the individual frame rate of each frame;
        //There is a chunk data contains the sequence of the animation, for example 00:00{ frame1, frame4, frame2, frame3 ... }01:00
        //So, these things are missing.

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
