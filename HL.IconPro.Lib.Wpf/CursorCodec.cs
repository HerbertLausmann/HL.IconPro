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
using System.Windows.Media.Imaging;
using System.Windows;

namespace HL.IconPro.Lib.Wpf
{
    public abstract class CursorCodec : Codec
    {
        public CursorCodec()
        {
            _Hotspots = new Dictionary<BitmapFrame, ushort[]>();
        }
        private Dictionary<BitmapFrame, ushort[]> _Hotspots;

        public ushort[] GetHotspot(BitmapFrame Frame)
        {
            if (!base.Frames.Contains(Frame)) throw new InvalidOperationException("Cant get hotspot for a inexistent Frame in Frames collection");
            bool sucess = _Hotspots.TryGetValue(Frame, out ushort[] hotspot);
            if (!sucess) hotspot = new ushort[2] { 0, 0 };
            return hotspot;
        }

        public void SetHotspot(BitmapFrame Frame, ushort X, ushort Y)
        {
            if (!base.Frames.Contains(Frame)) throw new InvalidOperationException("Cant set hotspot for a inexistent Frame in Frames collection");
            _Hotspots.Add(Frame, new ushort[2] { X, Y });
        }

    }
}
