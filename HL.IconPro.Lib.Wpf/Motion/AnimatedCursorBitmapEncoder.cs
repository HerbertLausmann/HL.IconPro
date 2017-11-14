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
            header.nPlanes = 1;
            header.iBitCount = 32;
            header.bfAttributes = 1;
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

        public new uint FrameRate
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

        public void SetAuthor(string Author)
        {
            _Author = Author;
        }

        public void SetName(string Name)
        {
            _Name = Name;
        }
    }
}
