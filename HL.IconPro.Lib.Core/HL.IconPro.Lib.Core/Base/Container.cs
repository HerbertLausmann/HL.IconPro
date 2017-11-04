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
using System.Linq;
using System.Text;

namespace HL.IconPro.Lib.Core
{
    public abstract class Container : IContainer
    {
        #region Constructors
        public Container() { _Frames = new FrameCollection(); }
        #endregion

        #region Destructors

        #endregion

         #region Fields
        protected FrameCollection _Frames;
        #endregion

        #region Properties
        public FrameCollection Frames
        {
            get
            {
                return _Frames;
            }
        }
        #endregion

        #region Procedures
        public abstract void Write(Stream Output);
        public abstract void Read(Stream Source);
        #endregion

        #region Static

        #endregion

        #region Others

        #endregion
    }
}
