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

namespace HL.IconPro.Lib.Core
{
    /// <summary>
    /// A icon/cursor frame might come as a Bitmap Image (with a mask to give the transparency), 
    /// or a PNG image (only acceptable for the 256x256 size)
    /// </summary>
    public enum FrameType
    {
        PNG,
        BITMAP
    };
}
