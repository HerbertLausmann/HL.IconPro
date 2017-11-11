using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace HL.IconPro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex appInstance = new Mutex(true, "HL.IconPro");
        private bool cantStart = false;
        protected override void OnStartup(StartupEventArgs e)
        {
            lock (appInstance)
            {
                if (!appInstance.WaitOne(0, true))
                {
                    MessageBox.Show("Icon Pro is already running!", "Invalid operation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    cantStart = true;
                    Shutdown(0);
                }
            }
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            lock (appInstance)
            {
                if (!cantStart)
                    appInstance.ReleaseMutex();
            }
            base.OnExit(e);
        }
    }
}
