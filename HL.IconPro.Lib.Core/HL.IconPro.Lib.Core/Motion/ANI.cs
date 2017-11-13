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
using System.Text;
using System.IO;
using System.Linq;

namespace HL.IconPro.Lib.Core.Motion
{
    /// <summary>
    /// Describes an animated cursor file, that is, actually, a RIFF file.
    /// </summary>
    public class ANI
    {

        public ANI()
        {
            _Frames = new List<byte[]>();
        }

        private List<byte[]> _Frames;
        private string _IART;
        private string _INAM;
        private ANIHEADER _Header;

        /// <summary>
        /// The frames of this Animated Cursor. They might be a Icon file or a Cursor File or even a Bitmap
        /// </summary>
        public List<byte[]> Frames => _Frames;

        /// <summary>
        /// Gets ou sets a header for this animated cursor
        /// </summary>
        public ANIHEADER Header { get => _Header; set => _Header = value; }

        public string IART { get => _IART; set => _IART = value; }
        public string INAM { get => _INAM; set => _INAM = value; }

        /// <summary>
        /// Loads an animated cursor file
        /// </summary>
        /// <param name="source"></param>
        public void Load(Stream source)
        {
            _Frames = new List<byte[]>();
            BinaryReader reader = new BinaryReader(source);
            CHUNK riffFile = CHUNK.ParseRIFFFILE(reader);

            var header = riffFile.children.First(x => x.chunkID == "anih");

            _Header = ANIHEADER.FromBuffer(header.chunkData);

            try
            {
                var artist = riffFile.children.First(x => x.chunkID == "IART");
                if (artist != null) _IART = Encoding.Unicode.GetString(artist.chunkData);
                var name = riffFile.children.First(x => x.chunkID == "INAM");
                if (name != null) _INAM = Encoding.Unicode.GetString(name.chunkData);
            }
            catch { }
            var frames = riffFile.children.First(x => x.chunkID == "LIST" && x.fccType == "fram");
            foreach (CHUNK frame in frames.children)
            {
                _Frames.Add(frame.chunkData);
            }
        }

        /// <summary>
        /// Fully implemented, but still not tested.
        /// </summary>
        /// <param name="Output"></param>
        public void Write(Stream Output)
        {
            CHUNK Riff = new CHUNK();
            Riff.chunkID = "RIFF";
            Riff.fccType = "ACON";

            if (!string.IsNullOrWhiteSpace(IART) && !string.IsNullOrWhiteSpace(INAM))
            {
                CHUNK info = new CHUNK();
                info.chunkID = "LIST";
                info.fccType = "INFO";

                CHUNK inam = new CHUNK(); CHUNK iart = new CHUNK();
                inam.chunkID = "INAM";
                inam.chunkData = Encoding.Unicode.GetBytes(_INAM);

                iart.chunkID = "IART";
                iart.chunkData = Encoding.Unicode.GetBytes(_IART);
                info.children.Add(inam);
                info.children.Add(iart);
                Riff.children.Add(info);
            }
            CHUNK header = new CHUNK();
            header.chunkID = "anih";
            header.chunkData = _Header.GetBuffer();
            Riff.children.Add(header);

            CHUNK frames = new CHUNK();
            frames.chunkID = "LIST";
            frames.fccType = "fram";

            foreach (byte[] frame in _Frames)
            {
                CHUNK fr = new CHUNK();
                fr.chunkID = "icon";
                fr.chunkData = frame;
                frames.children.Add(fr);
            }

            Riff.children.Add(frames);

            byte[] buffer = Riff.GetBytes();
            Output.Write(buffer, 0, buffer.Length);
        }
    }
}

