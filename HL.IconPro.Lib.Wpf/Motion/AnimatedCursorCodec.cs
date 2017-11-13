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

namespace HL.IconPro.Lib.Wpf.Motion
{
    /// <summary>
    /// Base class for Animated Cursors Codecs. Adds support for the Animated Cursor Metadata
    /// </summary>
    public class AnimatedCursorCodec : CursorCodec
    {
        protected string _Name;
        protected string _Author;
        protected ushort _FrameRate;

        /// <summary>
        /// The cursor's name, if available
        /// </summary>
        public string Name => _Name;

        /// <summary>
        /// The name of the author, if available
        /// </summary>
        public string Author => _Author;

        /// <summary>
        /// Default frame display rate (measured in 1/60th-of-a-second units)
        /// </summary>
        public ushort FrameRate => _FrameRate;
    }
}
