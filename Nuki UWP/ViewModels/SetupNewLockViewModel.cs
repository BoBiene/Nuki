using Nuki.Pages.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Nuki.Communication.Connection;
using Windows.UI.Xaml.Controls;
using Nuki.Views.Setup;
using Nuki.Services.SettingsServices;
using Nuki.Views;
using Nuki.Communication.Connection.Bluetooth;

namespace Nuki.ViewModels
{
    public enum BackgoundMode
    {
        None,
        CleanImage,
        BluredImage,
        BluredDarkImage,
    }

    public class SetupNewLockViewModel : ViewModelBase
    {
        private static readonly LinkedList<Type> m_Pages = new LinkedList<Type>(new Type[]
        {
            typeof(Setup01LandingPage),
            typeof(Setup02ConfirmInstallation),
            typeof(Setup03NamePage),
            typeof(Setup04PairDevice),
            typeof(Setup05IconPage),
            typeof(Setup06LockNamePage),
            typeof(SetupSuccessfull)
        });
        private BackgoundMode m_BackgroundMode = BackgoundMode.CleanImage;
        private LinkedListNode<Type> m_SelectedPage = m_Pages.First;
        private Symbol m_Icon = Symbol.Home;
        string m_MyName = "nuki";
        string m_MyLockName = "nuki";

        public BackgoundMode BackgoundMode
        {
            get { return m_BackgroundMode; }
            set { Set(ref m_BackgroundMode, value); }
        }
        public Type SelectedPageType { get { return m_SelectedPage.Value; } }
        public string MyName { get { return m_MyName; } set { Set(ref m_MyName, value); } }
        public string MyLockName { get { return m_MyLockName; } set { Set(ref m_MyLockName, value); } }
        public Symbol Icon { get { return m_Icon; } set { Set(ref m_Icon, value); } }

        public BluetoothPairResult PairResult { get; internal set; }

        public void GotoNextPage()
        {
            m_SelectedPage = m_SelectedPage.Next;
            RaisePropertyChanged(nameof(SelectedPageType));
        }

        public void Cancel()
        {

        }

        public void CompleteSetup()
        {
            PairResult.ConnectionInfo.Icon = m_Icon;
            PairResult.ConnectionInfo.ConnectionName = MyLockName;
            foreach (var lockToReplace in SettingsService.Instance.PairdLocks
                .Where((l) => l.UniqueClientID == PairResult.ConnectionInfo.UniqueClientID).ToList())
                SettingsService.Instance.PairdLocks.Remove(lockToReplace);
            SettingsService.Instance.PairdLocks.Add(PairResult.ConnectionInfo);
            NavigationService?.Navigate(typeof(NukiLock), PairResult.ConnectionInfo.UniqueClientID.Value);
        }
    }
}
    