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

        public static CHUNK ParseRIFFFILE(BinaryReader Source)
        {
            CHUNK riff = new CHUNK();
            riff.ParseNode(Source);
            return riff;
        }

        private void ParseNode(BinaryReader Reader)
        {
            chunkID = FromFourCC(Reader.ReadInt32());
            chunkSize = Reader.ReadUInt32();
            //On riff files, only chunks with RIFF and LIST ids can have children;
            //Also, RIFF and LIST chunks contains a second four char ID, the fccType
            if (chunkID == "RIFF" || chunkID == "LIST")
            {
                byte[] type = new byte[4];
                fccType = FromFourCC(Reader.ReadInt32());
                chunkData = new byte[chunkSize - 4];
                Reader.Read(chunkData, 0, chunkData.Length);
                children = new List<CHUNK>();
                children.AddRange(ParseList(new BinaryReader(new MemoryStream(chunkData))));
            }
            else
            {
                fccType = null;
                chunkData = new byte[chunkSize];
                Reader.Read(chunkData, 0, chunkData.Length);
            }
        }

        private List<CHUNK> ParseList(BinaryReader Reader)
        {
            List<CHUNK> cks = new List<CHUNK>();
            try
            {
                while (Reader.BaseStream.CanRead)
                {
                    CHUNK node = new CHUNK();
                    node.ParseNode(Reader);
                    cks.Add(node);
                }
            }
            catch { }
            return cks;
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
