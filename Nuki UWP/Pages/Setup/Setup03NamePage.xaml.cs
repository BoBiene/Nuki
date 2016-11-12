using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Nuki.Pages.Setup
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Setup03NamePage : Page
    {
        public Setup03NamePage()
        {
            Shell.Current.ViewModel.BackgoundMode = Presentation.BackgoundMode.BluredImage;
            this.InitializeComponent();
        }



        private void Weiter_Click(object sender, RoutedEventArgs e)
        {
            Shell.Current.ViewModel.SelectedPageType = typeof(Setup04PairDevice);
        }
    }
}
