using Nuki.ViewModels;
using Nuki.Views;
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
    public sealed partial class Setup06LockNamePage : Page
    {
        public Setup06LockNamePage()
        {
            SetupNewLock.Current.ViewModel.BackgoundMode = BackgoundMode.BluredImage;
            this.InitializeComponent();
        }

        public SetupNewLockViewModel ViewModel => SetupNewLock.Current.ViewModel;

   
    }
}
