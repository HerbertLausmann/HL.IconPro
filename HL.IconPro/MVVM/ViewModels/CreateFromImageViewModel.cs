using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using HL.MVVM;


namespace HL.IconPro.MVVM.ViewModels
{
    class CreateFromImageViewModel : HL.MVVM.ViewModelBase
    {
        #region Constructors
        public CreateFromImageViewModel(BitmapFrame bp)
        {
            _Sizes = new System.Collections.ObjectModel.ObservableCollection<Models.FrameSizeModel>();
            _BitDepths = new Dictionary<int, string>();
            _Image = bp;
            foreach (int size in IconSizes)
            {
                if (size > bp.PixelWidth) break;
                Models.FrameSizeModel fsm = new Models.FrameSizeModel(new System.Windows.Size(size, size));
                fsm.PropertyChanged += fsm_PropertyChanged;
                _Sizes.Add(fsm);
            }
            bool bgr32 = (_Image.Format.BitsPerPixel > 8);
            bool indexed8 = (_Image.Format.BitsPerPixel > 4);
            bool indexed4 = (_Image.Format.BitsPerPixel > 1);
            if (bgr32)
            {
                _BitDepths.Add(32, "32 BPP");
                _BitDepths.Add(8, "8 BPP");
                _BitDepths.Add(4, "4 BPP");
                _BitDepths.Add((32 + 8 + 4), "32 BPP + 8 BPP + 4 BPP");
                _BitDepths.Add((32 + 8), "8 BPP + 4 BPP");
            }
            else if (!bgr32 && indexed8)
            {
                _BitDepths.Add(8, "8 BPP");
                _BitDepths.Add(4, "4 BPP");
                _BitDepths.Add((32 + 8), "8 BPP + 4 BPP");
            }
            else
            {
                _BitDepths.Add(4, "4 BPP");
            }
            SelectedBitDepth = 0;
        }

        public CreateFromImageViewModel() { }
        #endregion

        #region Destructors

        #endregion

        #region Fields
        private BitmapFrame _Image;
        private System.Collections.ObjectModel.ObservableCollection<Models.FrameSizeModel> _Sizes;
        private int _SelectedBitDepth;
        private Dictionary<int, string> _BitDepths;
        #endregion

        #region Properties
        public BitmapSource Image
        {
            get
            {
                if (_Sizes == null) return _Image;
                if (_Sizes.Count == 0) return _Image;

                if (_Sizes.Count == 0) return null;
                Models.FrameSizeModel biggest = _Sizes[0];
                foreach (Models.FrameSizeModel item in _Sizes)
                {
                    if (item.GetSize().Width > biggest.GetSize().Width && item.Checked)
                    {
                        biggest = item;
                    }
                }
                return IconPro.Lib.Wpf.Helpers.GetResized(_Image, (int)biggest.GetSize().Width);
            }
            set
            {
                _Image = BitmapFrame.Create(value);
                OnPropertyChanged("Image");
            }
        }
        public System.Collections.ObjectModel.ObservableCollection<Models.FrameSizeModel> Sizes
        {
            get { return _Sizes; }
        }
        public System.Collections.ICollection BitDepths
        {
            get
            {
                if (_BitDepths == null || _BitDepths.Values == null) return null;
                return _BitDepths.Values;
            }
        }
        public int SelectedBitDepth
        {
            get { return _SelectedBitDepth; }
            set
            {
                SetField(ref _SelectedBitDepth, value);
                if (_BitDepths.ElementAt(value).Key == 32)
                {
                    foreach (Models.FrameSizeModel size in _Sizes.Where(x => x.GetSize().Width > 16))
                    {
                        _Sizes[_Sizes.IndexOf(size)].Enabled = true;
                    }
                }
                else if (_BitDepths.ElementAt(value).Key == 8)
                {
                    foreach (Models.FrameSizeModel size in _Sizes.Where(x => x.GetSize().Width > 16))
                    {
                        _Sizes[_Sizes.IndexOf(size)].Enabled = true;
                    }
                    // foreach (Models.FrameSizeModel size in _Sizes.Where(x => x.GetSize().Width > 48))
                    // {
                    //     _Sizes[_Sizes.IndexOf(size)].Enabled = false;
                    //     _Sizes[_Sizes.IndexOf(size)].Checked = false;
                    // }
                }
                else if (_BitDepths.ElementAt(value).Key == 4)
                {
                    foreach (Models.FrameSizeModel size in _Sizes.Where(x => x.GetSize().Width > 16))
                    {
                        _Sizes[_Sizes.IndexOf(size)].Enabled = true;
                    }
                    //foreach (Models.FrameSizeModel size in _Sizes.Where(x => x.GetSize().Width > 48))
                    //{
                    //    _Sizes[_Sizes.IndexOf(size)].Enabled = false;
                    //    _Sizes[_Sizes.IndexOf(size)].Checked = false;
                    //}
                }
                else if (_BitDepths.ElementAt(value).Key == (32 + 8 + 4))
                {
                    foreach (Models.FrameSizeModel size in _Sizes.Where(x => x.GetSize().Width > 16))
                    {
                        _Sizes[_Sizes.IndexOf(size)].Enabled = true;
                    }
                }
                else if (_BitDepths.ElementAt(value).Key == (8 + 4))
                {
                    foreach (Models.FrameSizeModel size in _Sizes.Where(x => x.GetSize().Width > 16))
                    {
                        _Sizes[_Sizes.IndexOf(size)].Enabled = true;
                    }
                    // foreach (Models.FrameSizeModel size in _Sizes.Where(x => x.GetSize().Width > 48))
                    // {
                    //     _Sizes[_Sizes.IndexOf(size)].Enabled = false;
                    //     _Sizes[_Sizes.IndexOf(size)].Checked = false;
                    // }
                }
            }
        }
        #endregion

        #region Procedures
        private void fsm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Checked")
                OnPropertyChanged("Image");
        }
        #endregion

        #region Static
        /// <summary>
        /// After opening some windows icons I've realized that they've included also the 20x20 and 40x40 frame sizes.
        /// I have no ideia about why, but, I decided to include these sizes as well.
        /// The constant bellow is a global constant
        /// </summary>
        // public static readonly int[] IconSizes = new int[] { 16, 24, 32, 48, 64, 72, 96, 128, 256 };
        public static readonly int[] IconSizes = new int[] { 16, 20, 24, 32, 40, 48, 64, 72, 96, 128, 256 };
        #endregion

        #region Commands
        public ICommand CreateCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                 {
                     MVVM.ViewModels.MainWindowViewModel mwvm = new MainWindowViewModel();
                     foreach (Models.FrameSizeModel size in Sizes)
                     {
                         if (size.Checked == true)
                         {
                             BitmapSource img = IconPro.Lib.Wpf.Helpers.GetResized(_Image, (int)size.GetSize().Width);
                             if (_BitDepths.ElementAt(SelectedBitDepth).Value.Contains("4 BPP"))
                             {
                                 if (img.Format.BitsPerPixel == 4)
                                 {
                                     mwvm.Frames.Add(new Models.FrameModel(BitmapFrame.Create(img), _Image.Decoder));
                                 }
                                 else
                                 {
                                     BitmapSource img4bit = IconPro.Lib.Wpf.Helpers.Get4BitImage(img);
                                     mwvm.Frames.Add(new Models.FrameModel(BitmapFrame.Create(img4bit), _Image.Decoder));
                                 }
                             }
                             if (_BitDepths.ElementAt(SelectedBitDepth).Value.Contains("8 BPP"))
                             {
                                 if (img.Format.BitsPerPixel == 8)
                                 {
                                     mwvm.Frames.Add(new Models.FrameModel(BitmapFrame.Create(img), _Image.Decoder));
                                 }
                                 else
                                 {
                                     BitmapSource img8bit = IconPro.Lib.Wpf.Helpers.Get8BitImage(img);
                                     mwvm.Frames.Add(new Models.FrameModel(BitmapFrame.Create(img8bit), _Image.Decoder));
                                 }
                             }
                             if (_BitDepths.ElementAt(SelectedBitDepth).Value.Contains("32 BPP"))
                                 mwvm.Frames.Add(new Models.FrameModel(BitmapFrame.Create(IconPro.Lib.Wpf.Helpers.GetRGBA32BitImage(img)), _Image.Decoder));
                         }
                     }
                     MainWindow mw = new MainWindow();
                     mw.DataContext = mwvm;
                     mw.Show();
                     ((System.Windows.Window)parameter).Close();
                 })));
            }
        }
        public ICommand CancelCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                 {
                     WelcomeWindow ww = new WelcomeWindow();
                     ww.Show();
                     ((System.Windows.Window)parameter).Close();
                 })));
            }
        }
        #endregion
    }
}
