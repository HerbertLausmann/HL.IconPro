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
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HL.IconPro.Lib.Wpf
{
    public sealed class EditableBitmapImage
    {
        #region Constructors
        public EditableBitmapImage(BitmapSource Source)
        {
            Initialize(Source);
        }
        public EditableBitmapImage(int PixelWidth, int PixelHeight, double DpiX, double DpiY)
        {
            WriteableBitmap bp = new WriteableBitmap(PixelWidth, PixelHeight, DpiX, DpiY, PixelFormats.Bgra32, null);
            Initialize(bp);
        }
        #endregion

        #region Destructors
        ~EditableBitmapImage()
        {
            _Buffer = null;
            GC.Collect();
        }
        #endregion

        #region Fields
        private BitmapSource _OriginalSource;
        private WriteableBitmap _Source;
        private byte[] _Buffer;
        #endregion

        #region Properties
        public WriteableBitmap Source
        {
            get
            {
                _Source.Lock();
                Marshal.Copy(_Buffer, 0, _Source.BackBuffer, _Buffer.Length);
                _Source.AddDirtyRect(new System.Windows.Int32Rect(0, 0, _Source.PixelWidth, _Source.PixelHeight));
                _Source.Unlock();
                return _Source;
            }
            set
            {
                if (value == null) throw new NullReferenceException("Value can not be null");
                Initialize(value);
            }
        }
        public BitmapSource OriginalSource
        {
            get
            {
                return _OriginalSource;
            }
            set
            {
                if (value == null) throw new NullReferenceException("Value can not be null");
                Initialize(value);
            }
        }
        public byte[] Buffer
        {
            get
            {
                return _Buffer;
            }
            set
            {
                if (value == null) throw new NullReferenceException("Value can not be null");
                if (value.Rank > 1) throw new NullReferenceException("Invalid buffer: Can not be multidimensional");
                if (value.Length != _Buffer.Length) throw new NullReferenceException("Invalid buffer: Can not have a different length from the source");
                _Buffer = value;
            }
        }
        public int PixelWidth
        {
            get { return _OriginalSource.PixelWidth; }
        }
        public int PixelHeight
        {
            get { return _OriginalSource.PixelHeight; }
        }
        public double DpiX
        {
            get { return _OriginalSource.DpiX; }
        }
        public double DpiY
        {
            get { return _OriginalSource.DpiX; }
        }
        public double Width
        {
            get { return _OriginalSource.Width; }
        }
        public double Height
        {
            get { return _OriginalSource.Height; }
        }
        #endregion

        #region Procedures
        private void Initialize(BitmapSource Source)
        {
            _OriginalSource = Source;
            BitmapSource bgra = new FormatConvertedBitmap(Source, PixelFormats.Bgra32, null, 0);
            _Source = new WriteableBitmap(bgra);
            _Buffer = new byte[_Source.BackBufferStride * _Source.PixelHeight];
            Marshal.Copy(_Source.BackBuffer, _Buffer, 0, (_Source.BackBufferStride * _Source.PixelHeight));
        }
        public void SetPixel(int x, int y, Color Color)
        {
            if ((x > _Source.PixelWidth - 1) || (x < 0)) return;
            if ((y > _Source.PixelHeight - 1) || (y < 0)) return;
            if (Color == null) return;

            int loc = (x * 4) + (y * _Source.BackBufferStride);
            _Buffer[loc] = Color.B;
            _Buffer[loc + 1] = Color.G;
            _Buffer[loc + 2] = Color.R;
            _Buffer[loc + 3] = Color.A;
        }

        public Color GetPixel(int x, int y)
        {
            if ((x > _Source.PixelWidth - 1) || (x < 0)) return Color.FromArgb(0, 0, 0, 0);
            if ((y > _Source.PixelHeight - 1) || (y < 0)) return Color.FromArgb(0, 0, 0, 0);

            int loc = (x * 4) + (y * _Source.BackBufferStride);
            return Color.FromArgb(_Buffer[loc + 3], _Buffer[loc + 2], _Buffer[loc + 1], _Buffer[loc]);
        }
        #endregion

        #region Static

        #endregion

        #region Others
        public static implicit operator BitmapSource(EditableBitmapImage Source)
        {
            return Source.Source;
        }
        #endregion
    }
}