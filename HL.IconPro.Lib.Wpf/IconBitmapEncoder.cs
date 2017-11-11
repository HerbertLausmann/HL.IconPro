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
            File IconFile = new File();
            IconFile.Type = FileType.ICON;
            foreach (BitmapFrame frame in _Frames)
            {
                Frame iconFrame = Frame.Create(GetImageFrameBuffer(frame, UsePngCompression));
                if (iconFrame.iconImage != null)
                {
                    iconFrame.iconImage.icAND = new byte[0];

                    //This line will generate an AND mask and link it to the XOR bitmap
                    Helpers.MASK(iconFrame.BitsPerPixel, ref iconFrame.iconImage.icXOR,
                        ref iconFrame.iconImage.icAND, iconFrame.iconImage.AndStride,
                        iconFrame.iconImage.XorStride, iconFrame.Width, iconFrame.Height,
                        iconFrame.iconImage.icColors);

                    //Most all the Windows Icons that I've opened
                    //had these values setted to 0!
                    //Actually, I have no ideia about why, but I doing this as well:
                    iconFrame.iconImage.icHeader.biClrImportant = 0;
                    iconFrame.iconImage.icHeader.biClrUsed = 0;
                    iconFrame.iconImage.icHeader.biXPelsPerMeter = 0;
                    iconFrame.iconImage.icHeader.biYPelsPerMeter = 0;
                }
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
