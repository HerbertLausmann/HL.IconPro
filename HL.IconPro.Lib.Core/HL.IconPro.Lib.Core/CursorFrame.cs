using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HL.IconPro.Lib.Core
{
    public class CursorFrame : IFrame
    {
        #region Constructors
        private CursorFrame(byte[] buffer, ushort HotspotX, ushort HotspotY)
        {
            _HotspotX = HotspotX;
            _HotspotY = HotspotY;
            ReadBuffer(buffer);
        }
        private CursorFrame() { }
        #endregion

        #region Destructors

        #endregion

        #region Fields
        private byte[] _buffer;
        private FrameType _type;
        private ushort _HotspotX, _HotspotY;
        #endregion

        #region Properties
        public int Width
        {
            get
            {
                if (_type == FrameType.DIB)
                    return BitConverter.ToInt32(_buffer, 4);
                else
                    return (int)Utilities.GetBigEndianUInt(_buffer, 16);
            }
        }
        public int Height
        {
            get
            {
                if (_type == FrameType.DIB)
                {
                    int height = BitConverter.ToInt32(_buffer, 8);
                    height = ((height == (Width * 2)) ? (height / 2) : height);
                    return height;
                }
                else
                    return (int)Utilities.GetBigEndianUInt(_buffer, 20);
            }
        }

        public ushort BitsPerPixel
        {
            get
            {
                if (_type == FrameType.DIB)
                    return BitConverter.ToUInt16(_buffer, 14);
                else
                {
                    byte bitdepth = _buffer[24];
                    byte colorType = _buffer[25];
                    if (colorType == 6)
                        return 32;
                    else if (colorType == 2)
                        return 24;
                    else if (colorType == 3)
                        return 4;
                    else
                        return 8;
                }
            }
        }

        public FrameType Type
        {
            get
            {
                return _type;
            }
        }
        public byte[] Buffer
        {
            get
            {
                return _buffer;
            }
        }
        public ushort HotspotX { get => _HotspotX; set => _HotspotX = value; }
        public ushort HotspotY { get => _HotspotY; set => _HotspotY = value; }
        #endregion

        #region Procedures
        private void ReadBuffer(byte[] buffer)
        {
            MemoryStream memBuffer = new MemoryStream(buffer);
            BinaryReader discover = new BinaryReader(memBuffer);
            if (discover.ReadByte() == 137) //137 or 89 in hex, is the first byte of a PNG file
            {
                byte p = discover.ReadByte();
                byte n = discover.ReadByte();
                byte g = discover.ReadByte();
                if ((p == 80) && (n == 78) && (g == 71)) //In ASCII, the bytes 80 78 71 means PNG
                {
                    memBuffer.Position = 0;
                    ReadPNGBuffer(memBuffer);
                    return;
                }
            }
            memBuffer.Position = 0;
            ReadBMPBuffer(memBuffer);
        }
        private void ReadPNGBuffer(MemoryStream buffer)
        {
            _type = FrameType.PNG;
            _buffer = buffer.ToArray();
        }
        private void ReadBMPBuffer(MemoryStream buffer)
        {
            _type = FrameType.DIB;
            buffer.Position = 0;
            BinaryReader reader = new BinaryReader(buffer);
            byte firstByte = reader.ReadByte();
            byte secondByte = reader.ReadByte();

            if ((firstByte == 0x42) && (secondByte == 0x4D)) //In BMP file header, the first two bytes represents, in ASCII code, BM
            {
                buffer.Position = 0;
                _buffer = new byte[(buffer.Length - 14)];
                buffer.Seek(14, SeekOrigin.Begin);
                buffer.Read(_buffer, 0, (int)(buffer.Length - 14));
            }
            else
            {
                buffer.Position = 0;
                _buffer = new byte[buffer.Length];
                buffer.Read(_buffer, 0, (int)buffer.Length);
            }
        }

        public DIB.DIBitmap ToDIB()
        {
            if (Type == FrameType.DIB)
            {
                return DIB.DIBitmap.FromBuffer(_buffer);
            }
            return null;
        }
        #endregion

        #region Static
        public static CursorFrame Create(byte[] buffer, ushort HotspotX, ushort HotspotY)
        {
            return new CursorFrame(buffer, HotspotX,HotspotY);
        }
        public static CursorFrame FromDIB(DIB.DIBitmap dib, ushort HotspotX, ushort HotspotY)
        {
            CursorFrame frame = new CursorFrame();
            frame._buffer = dib.GetBytes();
            frame.HotspotX = HotspotX;
            frame.HotspotY = HotspotY;
            return frame;
        }
        #endregion

        #region Others

        #endregion
    }
}
