using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Nuki.Views
{
    public sealed partial class NukiLockHome : UserControl
    {
        public NukiLockHome()
        {
            this.InitializeComponent();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            lockActionInProgess.Begin();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.LockState))
            {
                switch (ViewModel.LockState)
                {

                    case Communication.API.NukiLockState.Unlocking:

                    case Communication.API.NukiLockState.Locking:

                    case Communication.API.NukiLockState.Unlatching:
                        lockActionInProgess.Begin();
                        break;
                    default:
                        lockActionInProgess.Stop();
                        break;
                }
            }
            else { }
        }
    }
}
