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
using HL.IconPro.Lib.Core.DIB;
using HL.IconPro.Lib.Core;

namespace HL.IconPro.Lib.Wpf
{
    public class IconBitmapEncoder : Codec
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
            IconFile IconFile = new IconFile();
            foreach (BitmapFrame frame in _Frames)
            {
                IconFrame iconFrame = IconFrame.Create(GetBitmapFrameBuffer(frame, UsePngCompression));
                IconFile.Frames.Add(iconFrame);
            }
            IconFile.Write(Output);
        }

        #endregion

        #region Static

        #endregion

        #region Others

        #endregion
    }
}
