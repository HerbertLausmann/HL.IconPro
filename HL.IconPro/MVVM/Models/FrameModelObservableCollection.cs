using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL.MVVM;

namespace HL.IconPro.MVVM.Models
{
    class FrameModelObservableCollection : System.Collections.ObjectModel.ObservableCollection<FrameModel>
    {
        public new void Add(FrameModel Item)
        {
            Comparison<FrameModel> comparer = new Comparison<FrameModel>((FrameModel x, FrameModel y) =>
            {
                if ((x.Size.Width < y.Size.Width && x.PixelFormat.BitsPerPixel == y.PixelFormat.BitsPerPixel)) return -1;
                if ((x.Size.Width == y.Size.Width) && (x.PixelFormat.BitsPerPixel < y.PixelFormat.BitsPerPixel)) return -1;
                if ((x.Size.Width == y.Size.Width) && (x.PixelFormat.BitsPerPixel == y.PixelFormat.BitsPerPixel)) return 0;
                if ((x.Size.Width == y.Size.Width) && (x.PixelFormat.BitsPerPixel > y.PixelFormat.BitsPerPixel)) return 1;
                if ((x.Size.Width > y.Size.Width)) return 1;
                return 0;
            });
            int i;
            for (i = 0; i < base.Count; i++)
            {
                if (comparer.Invoke(base[i], Item) == -1)
                    break;
            }
            base.Insert(i, Item);
        }

        public void AddWithoutSort(FrameModel Item)
        {
            base.Add(Item);
        }

        public FrameModel Biggest()
        {
            if (Count == 0) return null;
            FrameModel biggest = base[0];
            foreach (FrameModel item in (FrameModelObservableCollection)this)
            {
                if (item.Size.Width > biggest.Size.Width)
                {
                    biggest = item;
                }
            }
            return biggest;
        }

    }
}
