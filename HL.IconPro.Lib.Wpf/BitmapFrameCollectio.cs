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
using System.Windows.Media.Imaging;

namespace HL.IconPro.Lib.Wpf
{
    public class BitmapFrameCollection : List<BitmapFrame>
    {

        public new void Add(BitmapFrame item)
        {
            if (item == null)
                throw new NullReferenceException("BitmapFrame cannot be null");
            if (item.PixelWidth > 256)
                throw new InvalidOperationException("The width of the frame cannot be greater than 256");
            if (item.PixelHeight > 256)
                throw new InvalidOperationException("The height of the frame cannot be greater than 256");
            if (item.PixelWidth < 1)
                throw new InvalidOperationException("The frame width cannot be less than 1");
            if (item.PixelHeight < 1)
                throw new InvalidOperationException("The frame height cannot be less than 1");
            if (item.PixelWidth != item.PixelHeight)
                throw new InvalidOperationException("BitmapFrame must be square");
            base.Add(item);
        }

        public new void Insert(int Index, BitmapFrame Item)
        {
            if (Item == null)
                throw new NullReferenceException("BitmapFrame cannot be null");
            if (Item.PixelWidth > 256)
                throw new InvalidOperationException("The width of the frame cannot be greater than 256");
            if (Item.PixelHeight > 256)
                throw new InvalidOperationException("The height of the frame cannot be greater than 256");
            if (Item.PixelWidth < 1)
                throw new InvalidOperationException("The frame width cannot be less than 1");
            if (Item.PixelHeight < 1)
                throw new InvalidOperationException("The frame height cannot be less than 1");
            if (Item.PixelWidth != Item.PixelHeight)
                throw new InvalidOperationException("BitmapFrame must be square");
            base.Insert(Index, Item);
        }

        public void SortDescending()
        {
            this.Sort(new Comparison<BitmapFrame>((BitmapFrame x, BitmapFrame y) =>
            {
                if (x.PixelWidth > y.PixelWidth)
                {
                    return -1;
                }
                else if (x.PixelWidth < y.PixelWidth)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }));
        }

        public void SortAscending()
        {
            this.Sort(new Comparison<BitmapFrame>((BitmapFrame x, BitmapFrame y) =>
            {
                if (x.PixelWidth > y.PixelWidth)
                {
                    return 1;
                }
                else if (x.PixelWidth < y.PixelWidth)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }));
        }

    }
}
