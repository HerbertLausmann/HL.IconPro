using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HL.IconPro.Lib.Core
{
    public class CursorFile : Container
    {
        public override void Write(Stream Output)
        {
            if (_Frames.Count == 0) return;
            _Frames.SortXPCompatible(false);

            BinaryWriter writer = new BinaryWriter(Output);

            writer.Write((ushort)0); //Reserved (must be 0)
            writer.Write((ushort)2); //Resource Type (2 for cursors)
            writer.Write((ushort)_Frames.Count); //How many images?

            byte[][] frames = new byte[_Frames.Count][];
            for (int i = 0; i < _Frames.Count; i++)
            {
                if (_Frames[i].Type == FrameType.DIB)
                {
                    frames[i] = _Frames[i].Buffer;
                }
                else
                    frames[i] = _Frames[i].Buffer;
            }

            ushort FileHeaderLength = 6;
            ushort FrameHeaderLength = 16;

            uint FrameDataOffset = FileHeaderLength;
            FrameDataOffset += (uint)(FrameHeaderLength * _Frames.Count);

            for (int i = 0; i < _Frames.Count; i++)
            {
                CursorFrame frame = _Frames[i] as CursorFrame;
                if (i > 0)
                {
                    FrameDataOffset += Convert.ToUInt32(frames[i - 1].Length);
                }
                // Width, in pixels, of the image
                if (frame.Width == 256)
                    writer.Write((byte)0);
                else
                    writer.Write((byte)frame.Width);
                // Height, in pixels, of the image
                if (frame.Height == 256)
                    writer.Write((byte)0);
                else
                    writer.Write((byte)frame.Height);
                // Number of colors in image (0 if >=8bpp)
                if (frame.BitsPerPixel == 4)
                    writer.Write((byte)16);
                else
                    writer.Write((byte)0);

                writer.Write((byte)0); // Reserved ( must be 0)
                writer.Write(frame.HotspotX); // Hotspot x position
                writer.Write(frame.HotspotY); // Hotspot y position
                writer.Write((uint)frames[i].Length); // How many bytes in this resource?
                writer.Write((uint)FrameDataOffset); // Where in the file is this image?
            }

            foreach (byte[] frame in frames)
            {
                writer.Write(frame);
            }
        }
        public override void Read(Stream Source)
        {
            this._Frames.Clear();
            BinaryReader reader = new BinaryReader(Source);
            if (reader.ReadUInt16() != 0) throw new NotSupportedException("This icon file is not supported/valid.");
            if (reader.ReadUInt16() != 2) throw new NotSupportedException("This icon file is not supported/valid.");

            ushort framesNumber = reader.ReadUInt16();

            List<int> Lenghts = new List<int>();
            List<ushort[]> Hotspots = new List<ushort[]>();

            for (int i = 0; i < framesNumber; i++)
            {
                reader.ReadBytes(4); //Read unnecessary data
                ushort x, y;

                x = reader.ReadUInt16();
                y = reader.ReadUInt16();
                Hotspots.Add(new ushort[2] { x, y });

                uint lenght = reader.ReadUInt32(); //Read the lenght of the bmp data
                uint offset = reader.ReadUInt32(); //Read the offset of the bmp data

                Lenghts.Add((int)lenght);
            }

            for (int i = 0; i < framesNumber; i++)
            {
                byte[] frameData = reader.ReadBytes(Lenghts[i]);
                this._Frames.Add(CursorFrame.Create(frameData, Hotspots[i][0], Hotspots[i][1]));
            }
        }
        public static CursorFile FromStream(Stream Source)
        {
            CursorFile icon = new CursorFile();
            icon.Read(Source);
            return icon;
        }
    }
}
