using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.IconPro.MVVM.Models
{
    class IconFrameModelObservableCollection : System.Collections.ObjectModel.ObservableCollection<IconFrameModel>
    {
        public new void Add(IconFrameModel Item)
        {
            Comparison<IconFrameModel> comparer = new Comparison<IconFrameModel>((IconFrameModel x, IconFrameModel y) =>
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

        public void AddWithoutSort(IconFrameModel Item)
        {
            base.Add(Item);
        }

        public IconFrameModel Biggest()
        {
            if (Count == 0) return null;
            IconFrameModel biggest = base[0];
            foreach (IconFrameModel item in (IconFrameModelObservableCollection)this)
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
