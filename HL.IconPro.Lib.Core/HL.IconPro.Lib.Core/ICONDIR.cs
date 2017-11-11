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

namespace HL.IconPro.Lib.Core
{
    /// <summary>
    /// A struct in C/C++, this class represents the file header of an icon/cursor file
    /// </summary>
    public class ICONDIR
    {
        public ushort idReserved;   // Reserved (must be 0)
        /// <summary>
        /// 1 for icons and 2 for static cursors
        /// </summary>
        public ushort idType;       // Resource Type (1 for icons)
        public ushort idCount;      // How many images?
        
        public void Read(System.IO.BinaryReader Reader)
        {
            idReserved = Reader.ReadUInt16();
            idType = Reader.ReadUInt16();
            idCount = Reader.ReadUInt16();
        }

        public void Write(System.IO.BinaryWriter Writer)
        {
            Writer.Write(idReserved);
            Writer.Write(idType);
            Writer.Write(idCount);
        }
    }
}
