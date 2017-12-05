using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Reflection;
using HL.MVVM;

namespace HL.IconPro.MVVM.ViewModels
{
    class WelcomeWindowViewModel : HL.MVVM.ViewModelBase
    {
        #region Constructors
        public WelcomeWindowViewModel() { }
        #endregion

        #region Destructors

        #endregion

        #region Fields

        #endregion

        #region Properties
        public string VersionSoftware
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public string VersionWpfCore
        {
            get
            {
                return "WPF LIB VERSION: " + HL.IconPro.Lib.Wpf.Helpers.Version;
            }
        }
        public string VersionCore
        {
            get
            {
                return "CORE LIB VERSION: " + HL.IconPro.Lib.Wpf.Helpers.CoreVersion;
            }
        }
        #endregion

        #region Procedures
        private void FromStaticImage(WelcomeWindow Source)
        {
            entry: Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "PNG/SVG file(*.png;*.svg)|*.png;*.svg";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog(Source) == false) return;
            CreateFromImageViewModel mwvm = null;

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
            mwvm = new CreateFromImageViewModel(frame);
            if (frame.PixelWidth != frame.PixelHeight)
            {
                if (System.Windows.MessageBox.Show("The selected source is not square. Would you like to try another image?",
                     "Invalid Image", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Exclamation) == System.Windows.MessageBoxResult.Yes)
                {
                    goto entry;
                }
            }
            CreateFromImage mw = new CreateFromImage();
            mw.DataContext = mwvm;
            mw.Show();
            Source.Close();
        }
        private void FromFolderSource(WelcomeWindow Source)
        {
            Lib.Wpf.Dialogs.FolderBrowserDialog dialog = new Lib.Wpf.Dialogs.FolderBrowserDialog();
            dialog.Caption = "Select the Source folder:";
            if (dialog.ShowDialog() == false) return;
            string[] pngs = System.IO.Directory.GetFiles(dialog.FolderPath, "*.png");
            MVVM.ViewModels.MainWindowViewModel mwvm = new MainWindowViewModel();
            foreach (string png in pngs)
            {
                BitmapFrame fr = BitmapFrame.Create(new Uri(png));
                fr = BitmapFrame.Create(Lib.Wpf.Helpers.GetResized(fr, fr.PixelWidth));
                mwvm.Frames.Add(new Models.FrameModel(fr, fr.Decoder));
            }
            MainWindow mw = new MainWindow();
            mw.DataContext = mwvm;
            mw.Show();
            Source.Close();
        }
        #endregion

        #region Static

        #endregion

        #region Commands
        public ICommand CreateEmptyCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    ((System.Windows.Window)parameter).Close();
                })));
            }
        }
        public ICommand ExtractIconCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                    {
                        Lib.Wpf.Dialogs.IconBrowserDialog dialog = new Lib.Wpf.Dialogs.IconBrowserDialog();
                        if (dialog.ShowDialog((System.Windows.Window)parameter) == true)
                        {
                            MainWindowViewModel mwvm = new MainWindowViewModel();
                            if (!dialog.FileName.ToLower().EndsWith(".dll") && !dialog.FileName.EndsWith(".exe"))
                            {
                                System.Windows.MessageBox.Show("Please choose icons from EXE and DLL files only!",
                                    "Invalid Icon Source", System.Windows.MessageBoxButton.OK,
                                     System.Windows.MessageBoxImage.Error);
                                return;
                            }
                            System.IO.Stream source = Lib.Wpf.Tools.PEIconExtractor.ExtractIcon(dialog.FileName, dialog.Index);
                            if (source == null)
                            {
                                System.Windows.MessageBox.Show("The selected icon was empty!?",
                                    "Invalid Icon Source", System.Windows.MessageBoxButton.OK,
                                     System.Windows.MessageBoxImage.Error);
                                return;
                            }
                            mwvm.OpenIcon(source);
                            MainWindow mw = new MainWindow();
                            mw.DataContext = mwvm;
                            mw.Show();
                            ((System.Windows.Window)parameter).Close();
                        }
                    })));
            }
        }
        public ICommand CreateFromImageCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                    {
                        //if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        FromStaticImage((WelcomeWindow)parameter);
                        //else
                        //FromFolderSource((WelcomeWindow)parameter);
                    })));
            }
        }
        public ICommand CreateFromFolderCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        FromFolderSource(parameter as WelcomeWindow);
                    else
                    {
                        Lib.Wpf.Dialogs.FolderBrowserDialog folder = new Lib.Wpf.Dialogs.FolderBrowserDialog();
                        if (folder.ShowDialog(parameter as WelcomeWindow) == true)
                        {
                            var frames = System.IO.Directory.GetFiles(folder.FolderPath, "*.png");
                            Lib.Wpf.Motion.AnimatedCursorBitmapEncoder encoder = new Lib.Wpf.Motion.AnimatedCursorBitmapEncoder();
                            encoder.SetAuthor("Herbert Lausmann");
                            encoder.SetName("Animated Windows Cursor Beta");
                            encoder.FrameRate = 3;
                            foreach (string f in frames)
                            {
                                encoder.Frames.Add(BitmapFrame.Create(new Uri(f)));
                            }
                            Microsoft.Win32.SaveFileDialog s = new Microsoft.Win32.SaveFileDialog();
                            s.Filter = "Animated Cursor File (*.ani)|*.ani";
                            if (s.ShowDialog() == true)
                            {
                                System.IO.FileStream fs = new System.IO.FileStream(s.FileName, System.IO.FileMode.Create);
                                encoder.Save(fs);
                                fs.Close();
                            }
                        }
                    }
                })));
            }
        }
        public ICommand OpenFileCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                    {
                        Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                        dialog.Filter = "Icon/Cursor(*.ico, *.cur, *.ani)|*.ico;*.cur;*.ani";
                        dialog.CheckFileExists = true;
                        if (dialog.ShowDialog() == false) return;
                        MainWindowViewModel mwvm = new MainWindowViewModel();
                        System.IO.FileStream fs = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Open);
                        if (dialog.FileName.ToLower().EndsWith(".ico"))
                            mwvm.OpenIcon(fs);
                        else if (dialog.FileName.ToLower().EndsWith(".cur"))
                            mwvm.OpenCursor(fs);
                        else if (dialog.FileName.ToLower().EndsWith(".ani"))
                            mwvm.OpenAnimatedCursor(fs);
                        else return;
                        fs.Close();
                        MainWindow mw = new MainWindow();
                        mw.DataContext = mwvm;
                        mw.Show();
                        ((System.Windows.Window)parameter).Close();
                    })));
            }
        }
        public ICommand NothingCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                    {
                        App.Current.Shutdown();
                    })));
            }
        }

        public ICommand HelpCommand
        {
            get
            {
                return GetCommand(new RelayCommand(new Action<object>((object parameter) =>
                {
                    HelpView view = new HelpView();
                    view.ShowDialog();
                })));
            }
        }
        #endregion
    }
}
