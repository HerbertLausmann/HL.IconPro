using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HL.IconPro
{
	/// <summary>
	/// Interaction logic for NewIconWizardWindow.xaml
	/// </summary>
	public partial class WelcomeWindow : Window
	{
		public WelcomeWindow()
		{
			this.InitializeComponent();
			// Insert code required on object creation below this point.
		}
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.LeftCtrl)
                CreateIcon.Content = "CREATE ICON FROM FOLDER SOURCE";
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.LeftCtrl)
                CreateIcon.Content = "CREATE AN ICON FROM IMAGE";
        }
	}
}