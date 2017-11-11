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
using System.IO;
using System.Text;


/// <summary>
/// I rewrote this library to be easier to understand.
/// In fact, cursors and icons files shares the exactly same file structure.
/// So, I wrote classes that are the exact representation of the icon file in C/C++.
/// This way, it's pretty much easier to understand the file structure (That is a little complex).
/// </summary>
namespace HL.IconPro.Lib.Core
{
    /// <summary>
    /// Describes a Icon file or a Cursor File. In fact, both have the same exactly structure.
    /// The difference is the value of ICONDIR.idType;
    /// For icons will be 1 and for cursors will be 2.
    /// </summary>
    public class File
    {
        public File()
        {
            _frames = new FrameCollection();
        }
        private FrameCollection _frames;
        private FileType _type;

        public FrameCollection Frames => _frames;
        /// <summary>
        /// The icons and cursors share the same file structure. The difference comes by signing it in the header of the file!
        /// </summary>
        public FileType Type { get => _type; set { _type = value; } }

        public void Write(Stream Output)
        {
            if (_frames.Count == 0) return;
            BinaryWriter writer = new BinaryWriter(Output);
            _frames.SortXPCompatible(false);

            Frame[] FRAMES = _frames.ToArray();

            ICONDIR dir = new ICONDIR();
            dir.idReserved = (ushort)0;
            dir.idType = (ushort)_type;
            dir.idCount = (ushort)FRAMES.Length;

            dir.Write(writer);
            ushort FileHeaderLength = 6;
            ushort FrameHeaderLength = 16;

            uint FrameDataOffset = FileHeaderLength;
            FrameDataOffset += (uint)(FrameHeaderLength * FRAMES.Length);

            for (int i = 0; i < FRAMES.Length; i++)
            {
                //I spent ONE ENTIRE DAMN DAY with a issue in this code. The fix? I've changed
                //from [i] to [i - 1].
                //A damn "-1" made this thing work!
                //Actually, I'm writting this for myself hahaha
                if (i > 0)
                    FrameDataOffset += (uint)((_frames[i - 1].iconImage == null) ? (uint)FRAMES[i - 1].pngBuffer.Length : FRAMES[i - 1].iconImage.SizeOf);

                var FRAME = FRAMES[i];
                if (FRAME.iconDir == null) FRAME.CreateIconDir();

                if (FRAME.iconImage?.icAND?.Length > 0)
                {
                    //When the ICONIMAGE has a AND mask, we must inform that by
                    //doubling the image height (XOR.Height + AND.Height)
                    FRAME.iconImage.icHeader.biHeight *= 2;
                }

                FRAME.iconDir.dwImageOffset = ((uint)FrameDataOffset);
                FRAME.iconDir.Write(writer);

                FRAMES[i] = FRAME;
            }

            foreach (Frame fr in FRAMES)
            {
                if (fr.type == FrameType.BITMAP)
                    fr.iconImage.Write(writer);
                else
                    writer.Write(fr.pngBuffer);
            }
            _frames.Clear();
            _frames.AddRange(FRAMES);
        }
        public void Read(Stream Source)
        {
            this.Frames.Clear();
            BinaryReader reader = new BinaryReader(Source);
            ICONDIR dir = new ICONDIR();
            dir.Read(reader);
            _type = (FileType)dir.idType;

            for (int i = 0; i < dir.idCount; i++)
            {
                Frame fr = new Frame();
                fr.iconDir = new ICONDIRENTRY();
                fr.iconDir.Read(reader);
                _frames.Add(fr);
            }
            for (int i = 0; i < dir.idCount; i++)
            {
                Frame fr = _frames[i];
                byte[] buffer = new byte[fr.iconDir.dwBytesInRes];
                reader.Read(buffer, 0, buffer.Length);
                fr = Frame.Create(buffer, fr.iconDir);
                _frames.RemoveAt(i);
                _frames.Insert(i, fr);
            }
        }

        public byte[] GetBuffer()
        {
            MemoryStream output = new MemoryStream();
            Write(output);
            return output.ToArray();
        }

        public static File FromBuffer(byte[] source)
        {
            MemoryStream src = new MemoryStream(source);
            src.Position = 0;
            File f = new File();
            f.Read(src);
            return f;
        }
    }

    /// <summary>
    /// The icons and cursors share the same file structure. The difference comes by signing it in the header of the file!
    /// </summary>
    public enum FileType : short
    {
        ICON = 1,
        CURSOR = 2
    };
}
