using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HL.IconPro.MVVM.ViewModels
{
    class CreateFrameViewModel : CommandableModelBase
    {
        #region Constructors
        public CreateFrameViewModel()
        {
            _SupportedSizes = new Dictionary<int, string>();
            _BitDepths = new Dictionary<int, string>();
            _Source = null;
        }
        public CreateFrameViewModel(BitmapFrame Source)
        {
            _SupportedSizes = new Dictionary<int, string>();
            _BitDepths = new Dictionary<int, string>();
            _Source = Source;
            foreach (int size in CreateIconFromImageViewModel.IconSizes)
            {
                if (size > Source.PixelWidth) break;
                _SupportedSizes.Add(size, string.Format("{0} x {0}", size));
            }
            _SupportedSizes = _SupportedSizes.Reverse().ToDictionary(x => x.Key, x => x.Value);
            if (Source.Format.BitsPerPixel >= 24)
                _BitDepths.Add(32, "32 BPP");
            if (Source.Format.BitsPerPixel >= 8)
                _BitDepths.Add(8, "8 BPP");
            if (Source.Format.BitsPerPixel >= 4)
                _BitDepths.Add(4, "4 BPP");
        }
        #endregion

        #region Destructors

        #endregion

        #region Fields
        private Dictionary<int, string> _SupportedSizes;
        private int _SelectedSizeIndex;
        private Dictionary<int, string> _BitDepths;
        private int _SelectedBitDepthIndex;
        private BitmapSource _Source;

        private BitmapFrame _Result;
        #endregion

        #region Properties
        public ICollection SupportedSizes
        {
            get { return _SupportedSizes.Values; }
        }
        public ICollection BitDepths
        {
            get { return _BitDepths.Values; }
        }

        public int SelectedSizeIndex
        {
            get { return _SelectedSizeIndex; }
            set
            {
                _SelectedSizeIndex = value;
                OnPropertyChanged("SelectedSizeIndex");
                OnPropertyChanged("Preview");
            }
        }
        public int SelectedBitDepthIndex
        {
            get { return _SelectedBitDepthIndex; }
            set
            {
                _SelectedBitDepthIndex = value;
                OnPropertyChanged("SelectedBitDepthIndex");
                OnPropertyChanged("Preview");
            }
        }

        public BitmapSource Preview
        {
            get
            {
                if (_Source == null) return null;
                BitmapSource img = Lib.Wpf.Helpers.GetResized(_Source, _SupportedSizes.ElementAt(_SelectedSizeIndex).Key);
                switch (_BitDepths.ElementAt(_SelectedBitDepthIndex).Key)
                {
                    case 8:
                        img = Lib.Wpf.Helpers.Get8BitImage(img);
                        break;
                    case 4:
                        img = Lib.Wpf.Helpers.Get4BitImage(img);
                        break;
                    default:
                        break;
                }
                return img;
            }
        }

        public BitmapFrame Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
            }
        }
        #endregion

        #region Procedures

        #endregion

        #region Static

        #endregion

        #region Commands
        public ICommand CreateCommand
        {
            get
            {
                return GetCommand("Create", new Command((object parameter) =>
                    {
                        lock (this)
                        {
                            Result = BitmapFrame.Create(Preview);
                        }
                        ((Window)parameter).Close();
                    }));
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return GetCommand("Cancel", new Command((object parameter) =>
                {
                    lock (this)
                    {
                        Result = null;
                    }
                     ((Window)parameter).Close();
                }));
            }
        }
        #endregion
    }
}
