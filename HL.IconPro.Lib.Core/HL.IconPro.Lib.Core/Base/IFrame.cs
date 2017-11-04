using System;
using System.Collections.Generic;
using System.Text;

namespace HL.IconPro.Lib.Core
{
    public interface IFrame
    {
        DIB.DIBitmap ToDIB();
        int Width { get; }
        int Height { get; }
        FrameType Type { get; }
        byte[] Buffer { get; }
        ushort BitsPerPixel { get; }
    }
}
