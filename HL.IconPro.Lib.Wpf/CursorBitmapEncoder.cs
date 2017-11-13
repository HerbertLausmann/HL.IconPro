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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HL.IconPro.Lib.Core;

namespace HL.IconPro.Lib.Wpf
{
    public class CursorBitmapEncoder : CursorCodec
    {
        #region Constructors

        #endregion

        #region Destructors

        #endregion

        #region Fields
        private bool _UsePngCompression = true;
        #endregion

        #region Properties
        public bool UsePngCompression
        {
            get { return _UsePngCompression; }
            set { _UsePngCompression = value; }
        }
        #endregion

        #region Procedures

        public void Save(System.IO.Stream Output)
        {
            File CursorFile = new File();
            CursorFile.Type = FileType.CURSOR;
            foreach (BitmapFrame frame in _Frames)
            {
                ushort[] hotspot = GetHotspot(frame);
                Frame CursorFrame = Frame.Create(GetImageFrameBuffer(frame, UsePngCompression));
                CursorFrame.CreateIconDir();
                CursorFrame.iconDir.wPlanes = hotspot[0];
                CursorFrame.iconDir.wBitCount = hotspot[1];
                if (CursorFrame.iconImage != null)
                {
                    CursorFrame.iconImage.icAND = new byte[0];
                    Helpers.MASK(CursorFrame.BitsPerPixel, ref CursorFrame.iconImage.icXOR,
                        ref CursorFrame.iconImage.icAND, CursorFrame.iconImage.AndStride,
                        CursorFrame.iconImage.XorStride, CursorFrame.Width, CursorFrame.Height,
                        CursorFrame.iconImage.icColors);
                }
                CursorFile.Frames.Add(CursorFrame);
            }
            CursorFile.Write(Output);
        }

        #endregion

        #region Static

        #endregion

        #region Others

        #endregion
    }
}
