/*
 *  Icon Pro
 *  Copyright (C) 2017 Herbert Lausmann. All rights reserved.
 *  http://herbertdotlausmann.wordpress.com/
 *  herbert.lausmann@hotmail.com
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *   1. Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *   2. Redistributions in binary form must reproduce the above copyright
 *      notice and this list of conditions.    
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HL.IconPro.Lib.Core
{
    public class IconFile : Container
    {

        #region Procedures
        public override void Write(Stream Output)
        {
            if (_Frames.Count == 0) return;
            _Frames.SortXPCompatible(false);

            BinaryWriter writer = new BinaryWriter(Output);

            writer.Write((ushort)0); // Reserved (must be 0)
            writer.Write((ushort)1); // Resource Type (1 for icons)
            writer.Write((ushort)_Frames.Count); // How many images?

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
                IconFrame frame = _Frames[i] as IconFrame;
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
                writer.Write((ushort)1); // Color Planes
                writer.Write(frame.BitsPerPixel); // Bits per pixel
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
            if (reader.ReadUInt16() != 1) throw new NotSupportedException("This icon file is not supported/valid.");

            ushort framesNumber = reader.ReadUInt16();

            List<int> Lenghts = new List<int>();

            for (int i = 0; i < framesNumber; i++)
            {
                reader.ReadBytes(8); //Read unnecessary data
                uint lenght = reader.ReadUInt32(); //Read the lenght of the bmp data
                uint offset = reader.ReadUInt32(); //Read the offset of the bmp data

                Lenghts.Add((int)lenght);
            }

            for (int i = 0; i < framesNumber; i++)
            {
                byte[] frameData = reader.ReadBytes(Lenghts[i]);
                this._Frames.Add(IconFrame.Create(frameData));
            }
        }
        #endregion

        #region Static
        public static IconFile FromStream(Stream Source)
        {
            IconFile icon = new IconFile();
            icon.Read(Source);
            return icon;
        }
        #endregion

    }
}