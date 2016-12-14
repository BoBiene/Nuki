using Newtonsoft.Json;
using Nuki.Communication.Connection;
using Nuki.Services.SettingsServices;
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

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Nuki.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class NukiLock : Page
    {
        public NukiLock()
        {
            this.InitializeComponent();
        }

       

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);

        //    uint uniqueClientID = JsonConvert.DeserializeObject<uint>(e.Parameter.ToString());


        //    ViewModel.NukiConncection = SettingsService.Instance.PairdLocks.Where((l) => l.UniqueClientID.Value == uniqueClientID).FirstOrDefault();
           
        //}
    }
}
