using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows;

namespace HL.IconPro.MVVM.ViewModels
{
    class MainWindowViewModel : CommandableModelBase
    {
        #region Constructors
        public MainWindowViewModel()
        {
            _Informations = new ObservableCollection<Models.IconInformationModel>();
            _Frames = new Models.IconFrameModelObservableCollection();
        }
        #endregion

        #region Destructors

        #endregion

        #region Fields
        private ObservableCollection<Models.IconInformationModel> _Informations;
        private Models.IconFrameModelObservableCollection _Frames;
        private Models.IconFrameModel _SelectedFrame;
        #endregion

        #region Properties
        public ObservableCollection<Models.IconInformationModel> Informations
        {
            get
            {
                return _Informations;
            }
        }
        public Models.IconFrameModelObservableCollection Frames
        {
            get
            {
                return _Frames;
            }
        }
        public Models.IconFrameModel SelectedFrame
        {
            get { return _SelectedFrame; }
            set
            {
                _SelectedFrame = value;
                UpdateInformations();
                OnPropertyChanged("SelectedFrame");
                OnPropertyChanged("Preview");
            }
        }
        public BitmapSource Preview
        {
            get
            {
                if (SelectedFrame == null) return null;
                return (BitmapSource)_SelectedFrame.Frame;
            }
        }
        public bool CanSave
        {
            get
            {
                return _Frames.Count > 0;
            }
        }
        public bool CanExport
        {
            get
            {
                return _Frames.Count > 0;
            }
        }
        public bool CanDeleteFrame
        {
            get
            {
                return SelectedFrame != null;
            }
        }
        public bool CanExportFrame
        {
            get
            {
                return SelectedFrame != null;
            }
        }
        #endregion

        #region Procedures
        public void UpdateInformations()
        {
            try
            {
                _Informations.Clear();
                _Informations.Add(new Models.IconInformationModel("Frame Dimensions",
                                string.Format("{0}x{1}", new object[] { Preview.PixelWidth.ToString(), Preview.PixelHeight.ToString() })));
                long frameSize;
                if (SelectedFrame.Size.Width == 256)
                {
                    System.IO.MemoryStream mem = new System.IO.MemoryStream();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(SelectedFrame.Frame));
                    encoder.Save(mem);
                    frameSize = mem.Length;
                    encoder = null;
                    mem.Close();
                }
                else
                {
                    System.IO.MemoryStream mem = new System.IO.MemoryStream();
                    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(SelectedFrame.Frame));
                    encoder.Save(mem);
                    frameSize = mem.Length;
                    encoder = null;
                    mem.Close();
                }
                frameSize = (long)Math.Round((double)(frameSize / 1000));

                _Informations.Add(new Models.IconInformationModel("Frame Physical Size", frameSize.ToString() + " KB"));
                _Informations.Add(new Models.IconInformationModel("Frame Color Depth", Preview.Format.BitsPerPixel.ToString() + "BPP"));
                _Informations.Add(new Models.IconInformationModel("Frame Decoder", (SelectedFrame.Type == Models.IconFrameModelType.PNG) ? "WIC PNG" : "WIC BMP"));

                _Informations.Add(new Models.IconInformationModel("Icon Size",
                                string.Format("{0}x{1}", new object[] { Frames.Biggest().Size.Width, Frames.Biggest().Size.Width })));
                long iconsize;
                System.IO.MemoryStream iconmem = new System.IO.MemoryStream();
                bool canCompress = Frames.Where(x => x.Type == Models.IconFrameModelType.PNG).ToArray().Length > 0;
                SaveIcon(iconmem, canCompress);
                iconsize = (long)Math.Round((double)(iconmem.Length / 1000));
                iconmem.Close();

                _Informations.Add(new Models.IconInformationModel("Icon Physical Size", iconsize.ToString() + " KB"));
            }
            catch { }
        }
        public void SaveIcon(System.IO.Stream Output, bool compression)
        {
            IconPro.Lib.Wpf.IconBitmapEncoder encoder = new Lib.Wpf.IconBitmapEncoder();
            encoder.UsePngCompression = compression;
            foreach (Models.IconFrameModel fr in _Frames)
            {
                encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(fr.Frame));
            }
            encoder.Save(Output);
            encoder = null;
        }
        public void OpenIcon(System.IO.Stream Source)
        {
            IconPro.Lib.Wpf.IconBitmapDecoder decoder = new Lib.Wpf.IconBitmapDecoder();
            decoder.Open(Source);

            // System.Windows.Media.Imaging.IconBitmapDecoder decoder = new
            //   System.Windows.Media.Imaging.IconBitmapDecoder(Source, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            foreach (BitmapFrame bp in decoder.Frames)
            {
                _Frames.AddWithoutSort(new Models.IconFrameModel(bp, bp.Decoder));
            }
            decoder = null;
            if (_Frames?.Count > 0) SelectedFrame = _Frames[0];
        }
        public void OpenCursor(System.IO.Stream Source)
        {
            IconPro.Lib.Wpf.CursorBitmapDecoder decoder = new Lib.Wpf.CursorBitmapDecoder();
            decoder.Open(Source);
            foreach (BitmapFrame bp in decoder.Frames)
            {
                _Frames.AddWithoutSort(new Models.IconFrameModel(bp, bp.Decoder));
            }
            decoder = null;
            if (_Frames?.Count > 0) SelectedFrame = _Frames[0];
        }
        public void OpenAnimatedCursor(System.IO.Stream Source)
        {
            var decoder = new IconPro.Lib.Wpf.Motion.AnimatedCursorBitmapDecoder();
            decoder.Open(Source);
            foreach (BitmapFrame bp in decoder.Frames)
            {
                _Frames.AddWithoutSort(new Models.IconFrameModel(bp, bp.Decoder));
            }
            decoder = null;
            if (_Frames?.Count > 0) SelectedFrame = _Frames[0];
        }
        protected override void OnPropertyChanged(string Name)
        {
            base.OnPropertyChanged(Name);
            base.OnPropertyChanged("CanSave");
            base.OnPropertyChanged("CanExport");
            base.OnPropertyChanged("CanDeleteFrame");
            base.OnPropertyChanged("CanExportFrame");
        }
        #endregion

        #region Static

        #endregion

        #region Commands
        public ICommand HomeCommand
        {
            get
            {
                return GetCommand("Home", new Command(new Action<object>((object parameter) =>
                {
                    _Frames.Clear();
                    _Informations.Clear();
                    SelectedFrame = null;
                    WelcomeWindow ww = new WelcomeWindow();
                    ww.Show();
                    ((MainWindow)parameter).Close();
                })));
            }
        }
        public ICommand OpenCommand
        {
            get
            {
                return GetCommand("Open", new Command(new Action<object>((object parameter) =>
                     {
                         _Frames.Clear();
                         _Informations.Clear();
                         SelectedFrame = null;
                         Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                         dialog.Filter = "Icon/Cursor(*.ico, *.cur)|*.ico;*.cur";
                         dialog.CheckFileExists = true;
                         if (dialog.ShowDialog() == false) return;
                         System.IO.FileStream fs = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Open);
                         if (dialog.FileName.ToLower().EndsWith(".ico"))
                             OpenIcon(fs);
                         else
                             OpenCursor(fs);
                         fs.Close();
                     })));
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return GetCommand("Save", new Command(new Action<object>((object parameter) =>
                     {
                         Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                         bool canCompress = Frames.Where(x => x.Size.Width == 256).ToArray().Length > 0;
                         if (canCompress)
                             dialog.Filter = "Compressed Icon File (*.ico)|*.ico|Icon File (*.ico)|*.ico";
                         else
                             dialog.Filter = "Icon File (*.ico)|*.ico";
                         dialog.AddExtension = true;
                         dialog.CheckPathExists = true;
                         if (dialog.ShowDialog() == false) return;
                         System.IO.FileStream fs = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Create);
                         bool compression = (canCompress && dialog.FilterIndex == 1);
                         SaveIcon(fs, compression);
                         fs.Close();
                     })));
            }
        }
        public ICommand ExportCommand
        {
            get
            {
                return GetCommand("Export", new Command(new Action<object>((object parameter) =>
                     {
                         var res = System.Windows.MessageBox.Show("You are going to export all the frames of this icon file as PNG images. That means, maybe, dozens of images. Are you sure? To export an icon file, use the SAVE option!",
                             "Are you sure?", System.Windows.MessageBoxButton.YesNo);
                         if (res == System.Windows.MessageBoxResult.No) return;
                         Lib.Wpf.Dialogs.FolderBrowserDialog dialog = new Lib.Wpf.Dialogs.FolderBrowserDialog();
                         dialog.Caption = "Select de output export folder:";
                         if (dialog.ShowDialog(parameter as System.Windows.Window) == true)
                         {
                             string exportFolder = dialog.FolderPath;
                             if (!System.IO.Directory.Exists(exportFolder))
                             {
                                 System.Windows.MessageBox.Show("The selected path doesn't exists",
                                     "Invalid folder path", System.Windows.MessageBoxButton.OK,
                                     System.Windows.MessageBoxImage.Error);
                                 return;
                             }
                             foreach (HL.IconPro.MVVM.Models.IconFrameModel item in _Frames)
                             {
                                 string itemPath =
                                     string.Format("{0}\\{1}x{1} - {2}BPP.png",
                                     exportFolder, item.Size.Width.ToString(),
                                         item.PixelFormat.BitsPerPixel.ToString());
                                 if (System.IO.File.Exists(itemPath))
                                 {
                                     for (int i = 0; i < 99; i++)
                                     {
                                         itemPath =
                                     string.Format("{0}\\{1}x{1} - {2}BPP ({3}).png",
                                     exportFolder, item.Size.Width.ToString(),
                                         item.PixelFormat.BitsPerPixel.ToString(), i);
                                         if (!System.IO.File.Exists(itemPath)) break;
                                     }
                                 }
                                 PngBitmapEncoder encoder = new PngBitmapEncoder();
                                 encoder.Frames.Add(BitmapFrame.Create(item.Frame));
                                 System.IO.FileStream itemStream = new System.IO.FileStream(itemPath, System.IO.FileMode.Create);
                                 encoder.Save(itemStream);
                                 itemStream.Close();
                                 encoder = null;
                             }
                             System.Diagnostics.Process.Start("explorer.exe", exportFolder);
                         }
                     })));
            }
        }
        public ICommand DeleteFrameCommand
        {
            get
            {
                return GetCommand("DeleteFrame", new Command(new Action<object>((object parameter) =>
                     {
                         _Frames.Remove(SelectedFrame);
                     })));
            }
        }
        public ICommand CreateFrameCommand
        {
            get
            {
                return GetCommand("CreateFrame", new Command(new Action<object>((object parameter) =>
                     {
                         entry: Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                         dialog.Filter = "PNG/SVG file(*.png;*.svg)|*.png;*.svg";
                         dialog.CheckFileExists = true;
                         if (dialog.ShowDialog(parameter as Window) == false) return;

                         System.IO.FileStream fs = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Open);
                         BitmapFrame frame = null;

                         if (dialog.FileName.ToLower().EndsWith(".png"))
                         {
                             BitmapDecoder dec =
                                             BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                             frame = dec.Frames[0];
                         }
                         else
                         {
                             frame = BitmapFrame.Create(Lib.Wpf.Tools.SVGConverter.GetBitmapSource(fs));
                         }
                         fs.Close();
                         if (frame.PixelWidth != frame.PixelHeight)
                         {
                             if (System.Windows.MessageBox.Show("The selected source is not square. Would you like to try another image?",
                                  "Invalid Image", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Exclamation) == System.Windows.MessageBoxResult.Yes)
                             {
                                 goto entry;
                             }
                         }
                         else
                         {
                             ViewModels.CreateFrameViewModel VM = new CreateFrameViewModel(frame);
                             HL.IconPro.CreateFrame window = new CreateFrame();
                             window.DataContext = VM;
                             window.ShowDialog();
                             if (VM.Result != null)
                                 _Frames.Add(new Models.IconFrameModel(VM.Result, VM.Result.Decoder));
                         }
                     })));
            }
        }
        public ICommand ExportFrameCommand
        {
            get
            {
                return GetCommand("ExportFrame", new Command(new Action<object>((object parameter) =>
                     {
                         Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                         dialog.Filter = "Portable Network Graphics (*.png)|*.png";
                         dialog.AddExtension = true;
                         dialog.CheckPathExists = true;
                         if (dialog.ShowDialog() == false) return;
                         System.IO.FileStream fs = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Create);
                         PngBitmapEncoder encoder = new PngBitmapEncoder();
                         encoder.Frames.Add(BitmapFrame.Create(SelectedFrame.Frame));
                         encoder.Save(fs);
                         encoder = null;
                         fs.Close();
                     })));
            }
        }
        #endregion
    }
}