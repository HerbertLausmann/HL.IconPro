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
using System.IO;

namespace HL.IconPro.Lib.Core.Motion
{
    /// <summary>
    /// A struct in C/C++, describes a RIFF file.
    /// A riff file is composed of a Root CHUNK. This root has many children chunks.
    /// So, this class can represent a single chunk, its name and its data, our can represent an entire RIFF file.
    /// You can understand this class as a very simple object model (like XML namespace) for riff file format.
    /// </summary>
    public class CHUNK
    {
        public CHUNK()
        {
            children = new List<CHUNK>();
            chunkData = null;
        }
        public string chunkID;
        public uint chunkSize;
        public string fccType;
        public byte[] chunkData;
        public List<CHUNK> children;

        /// <summary>
        /// This method will parse the CHUNK tree of the riff file
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        private static CHUNK[] Parse(BinaryReader Reader)
        {
            List<CHUNK> cks = new List<CHUNK>();
            begin:
            CHUNK ck = new CHUNK();
            try
            {
                ck.chunkID = FromFourCC(Reader.ReadInt32());

                ck.chunkSize = Reader.ReadUInt32();

                if (ck.chunkID == "RIFF" || ck.chunkID == "LIST")
                {
                    byte[] type = new byte[4];
                    ck.fccType = FromFourCC(Reader.ReadInt32());
                    ck.chunkData = new byte[ck.chunkSize - 4];
                    ck.chunkData = new byte[ck.chunkSize];
                    Reader.Read(ck.chunkData, 0, ck.chunkData.Length);
                    ck.children = new List<CHUNK>();
                    ck.children.AddRange(Parse(new BinaryReader(new MemoryStream(ck.chunkData))));
                    cks.Add(ck);
                }
                else
                {
                    ck.fccType = null;
                    ck.chunkData = new byte[ck.chunkSize];
                    Reader.Read(ck.chunkData, 0, ck.chunkData.Length);
                    cks.Add(ck);
                    if (Reader.BaseStream.Position < Reader.BaseStream.Length) goto begin;
                }
            }
            catch { return cks.ToArray(); }
            return cks.ToArray();
        }

        public static CHUNK ParseRIFFFILE(BinaryReader Source)
        {
            return Parse(Source)[0];
        }

        public static string FromFourCC(int FourCC)
        {
            char[] chars = new char[4];
            chars[0] = (char)(FourCC & 0xFF);
            chars[1] = (char)((FourCC >> 8) & 0xFF);
            chars[2] = (char)((FourCC >> 16) & 0xFF);
            chars[3] = (char)((FourCC >> 24) & 0xFF);

            return new string(chars);
        }

        public static int ToFourCC(string FourCC)
        {
            if (FourCC.Length != 4)
            {
                throw new Exception("FourCC strings must be 4 characters long " + FourCC);
            }

            int result = ((int)FourCC[3]) << 24
                        | ((int)FourCC[2]) << 16
                        | ((int)FourCC[1]) << 8
                        | ((int)FourCC[0]);

            return result;
        }

        public static int ToFourCC(char[] FourCC)
        {
            if (FourCC.Length != 4)
            {
                throw new Exception("FourCC char arrays must be 4 characters long " + new string(FourCC));
            }

            int result = ((int)FourCC[3]) << 24
                        | ((int)FourCC[2]) << 16
                        | ((int)FourCC[1]) << 8
                        | ((int)FourCC[0]);

            return result;
        }

        public static int ToFourCC(char c0, char c1, char c2, char c3)
        {
            int result = ((int)c3) << 24
                        | ((int)c2) << 16
                        | ((int)c1) << 8
                        | ((int)c0);

            return result;
        }

        public byte[] GetBytes()
        {
            MemoryStream mem = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mem);
            writer.Write(ToFourCC(chunkID));

            writer.Write(chunkSize);

            if (!string.IsNullOrWhiteSpace(fccType))
            {
                writer.Write(ToFourCC(fccType));
            }
            if (chunkData != null && chunkData.Length > 0)
            {
                writer.Write(chunkData);
            }
            else
            {
                foreach (var ck in children)
                {
                    writer.Write(ck.GetBytes());
                }
            }
            if (chunkSize == 0)
            {
                writer.Seek(4, SeekOrigin.Begin);
                writer.Write((uint)(mem.Length - 8));
            }
            return mem.ToArray();
        }
    }
}

/*
 * I based my code above on the great article series by Jeff Friesen:
 * http://www.informit.com/articles/article.aspx?p=1187852
 * 
 * There he explains very well the structure of ANI files;
 * You can also read this article by entering the Articles folder of this project.
 * As some important knowledge sources can disappear from internet, I've downloaded them all ;)
 */
