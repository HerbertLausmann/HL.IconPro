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
    /// Represents a collection of frames inside of an Icon/Cursor file
    /// </summary>
    public class FrameCollection : List<Frame>
    {
        #region Standard Sorting
        public void SortAscending()
        {
            SortListAscending(this);
        }
        public void SortDescending()
        {
            SortListDescending(this);
        }
        #endregion

        #region Special Sorting
        /*
         * I've done these methods based on icons from various sources that I had opened while developing this API.
         * Right now, I'm just using SortXPCompatible(), but I gonna keep the others by precaution.
         * I'm still not sure about what is the best layout for the Icon Files, but I guess the SortXPCompatible()
         * is the most compatible layout with all the recent Windows Versions (XP, Vista, 7, 8.x, 10)
         */
        public void SortXPCompatible(bool TrueColorFirst)
        {
            List<Frame> frames4Bit = this.Where(x => x.BitsPerPixel == 4).ToList();
            List<Frame> frames8Bit = this.Where(x => x.BitsPerPixel == 8).ToList();
            List<Frame> frames32Bit = this.Where(x => x.BitsPerPixel == 32).ToList();
            if (frames4Bit.Count == 0 && frames8Bit.Count == 0)
            {
                SortDescending();
                return;
            }
            SortListDescending(frames4Bit);
            SortListDescending(frames8Bit);
            SortListDescending(frames32Bit);
            this.Clear();
            foreach (Frame frame in (TrueColorFirst ? frames32Bit : frames4Bit))
            {
                this.Add(frame);
            }
            foreach (Frame frame in frames8Bit)
            {
                this.Add(frame);
            }
            foreach (Frame frame in (TrueColorFirst ? frames4Bit : frames32Bit))
            {
                this.Add(frame);
            }
        }
        public void SortWin7()
        {
            List<Frame> frames4Bit = this.Where(x => x.BitsPerPixel == 4).ToList();
            List<Frame> frames8Bit = this.Where(x => x.BitsPerPixel == 8).ToList();
            List<Frame> frames32Bit = this.Where(x => x.BitsPerPixel == 32).ToList();

            List<Frame> frames32BitStandard = frames32Bit.Where(x => (x.Width == 16) || (x.Width == 32) || (x.Width == 48)).ToList();
            List<Frame> frames32BitOthers = frames32Bit.Where(x => (x.Width != 16) && (x.Width != 32) && (x.Width != 48)).ToList();
            SortListDescending(frames4Bit);
            SortListDescending(frames8Bit);
            SortListDescending(frames32BitStandard);
            SortListDescending(frames32BitOthers);
            this.Clear();
            foreach (Frame frame in frames4Bit)
            {
                this.Add(frame);
            }
            foreach (Frame frame in frames8Bit)
            {
                this.Add(frame);
            }
            foreach (Frame frame in frames32BitStandard)
            {
                this.Add(frame);
            }
            foreach (Frame frame in frames32BitOthers)
            {
                this.Add(frame);
            }
        }
        public void SortWin8xStandard()
        {
            List<Frame> frames4Bit = this.Where(x => x.BitsPerPixel == 4).ToList();
            List<Frame> frames8Bit = this.Where(x => x.BitsPerPixel == 8).ToList();
            List<Frame> frames32Bit = this.Where(x => x.BitsPerPixel == 32).ToList();

            List<Frame> frames32BitStandard = frames32Bit.Where(x => (x.Width == 16) || (x.Width == 32) || (x.Width == 48)).ToList();
            List<Frame> frames32BitOthers = frames32Bit.Where(x => (x.Width != 16) && (x.Width != 32) && (x.Width != 48) && (x.Width != 256)).ToList();
            SortListDescending(frames4Bit);
            SortListDescending(frames8Bit);
            SortListDescending(frames32BitStandard);
            if (frames32Bit.Where(x => x.Width == 256).ToList().Count > 0)
            {
                frames32BitStandard.AddRange(frames32Bit.Where(x => x.Width == 256));
            }
            SortListAscending(frames32BitOthers);
            this.Clear();
            foreach (Frame frame in frames4Bit)
            {
                this.Add(frame);
            }
            foreach (Frame frame in frames8Bit)
            {
                this.Add(frame);
            }
            foreach (Frame frame in frames32BitStandard)
            {
                this.Add(frame);
            }
            foreach (Frame frame in frames32BitOthers)
            {
                this.Add(frame);
            }
        }
        #endregion

        #region Private
        private void SortListDescending(List<Frame> source)
        {
            source.Sort(new Comparison<Frame>((Frame x, Frame y) =>
            {
                if ((x.Width < y.Width)) return 1;
                if ((x.Width == y.Width) && (x.BitsPerPixel < y.BitsPerPixel)) return 1;
                if ((x.Width == y.Width) && (x.BitsPerPixel == y.BitsPerPixel)) return 0;
                if ((x.Width == y.Width) && (x.BitsPerPixel > y.BitsPerPixel)) return -1;
                if ((x.Width > y.Width)) return -1;
                return 0;
            }));
        }
        private void SortListAscending(List<Frame> source)
        {
            source.Sort(new Comparison<Frame>((Frame x, Frame y) =>
            {
                if ((x.Width < y.Width)) return -1;
                if ((x.Width == y.Width) && (x.BitsPerPixel < y.BitsPerPixel)) return -1;
                if ((x.Width == y.Width) && (x.BitsPerPixel == y.BitsPerPixel)) return 0;
                if ((x.Width == y.Width) && (x.BitsPerPixel > y.BitsPerPixel)) return 1;
                if ((x.Width > y.Width)) return 1;
                return 0;
            }));
        }
        #endregion
    }
}