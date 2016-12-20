using Nuki.Communication.Connection;
using Nuki.Services.SettingsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Template10.Common;
using Template10.Services.NavigationService;

namespace Nuki.ViewModels
{
    public partial class NukiLockViewModel : ViewModelBase
    {
        private NukiConnectionBinding m_NukiConnectionBinding = null;
        private Visibility m_ProgressbarVisibility = Visibility.Collapsed;
        private object m_SelectedPivotItem = null;
        //public NukiLockAdministrationPartViewModel AdministrationViewModel { get; private set; }
        //public NukiLockHomePartViewModel HomeViewModel { get; private set; }
        //public NukiLockSettingsPartViewModel SettingsViewModel { get; private set; }
        //public NukiLockStatusPartViewModel StatusViewModel { get; private set; }


        public string SelectedLock
        {
            get { return NukiConncection?.ConnectionName ?? string.Empty; }

        }


        public Visibility ProgressbarVisibility => m_ProgressbarVisibility;

        public void ShowProgressbar(bool blnVisibility)
        {
            if (blnVisibility)
                m_ProgressbarVisibility = Visibility.Visible;
            else
                m_ProgressbarVisibility = Visibility.Collapsed;

            RaisePropertyChanged(nameof(ProgressbarVisibility));
        }

        public object SelectedPivotItem {
            get { return m_SelectedPivotItem; }
            set
            {
                Set(ref m_SelectedPivotItem, value);
                OnNavigatedToPivotItemAsync(value).GetAwaiter();
            }
        }

        private static async Task OnNavigatedToPivotItemAsync(object value, NavigationMode mode = NavigationMode.Refresh, IDictionary<string, object> state = null)
        {
            PivotItem pivotItem = value as PivotItem;
            if (pivotItem != null)
            {
                UserControl control = pivotItem.Content as UserControl;

                Part viewModel = control?.DataContext as Part;
                if (viewModel != null)
                {
                    await viewModel.OnNavigatedToAsync(null, mode, state);
                }
                else { }
            }
            else { }
        }

        public BluetoothConnection BluetoothConnection { get; private set; }

        public NukiConnectionBinding NukiConncection
        {
            get { return m_NukiConnectionBinding; }
            set
            {
                Set(ref m_NukiConnectionBinding, value);
                RaisePropertyChanged(nameof(SelectedLock));
            
            }
        }


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            NukiConncection = SettingsService.Instance.PairdLocks.Where((l) => l.UniqueClientID.Value == parameter as uint?).FirstOrDefault();

            var connectResult = await NukiConnectionFactory.TryConnect(NukiConncection, (action) => Dispatcher.DispatchAsync(action).AsAsyncAction());

            BluetoothConnection = connectResult.Connection;
            RaisePropertyChanged(nameof(BluetoothConnection));

            await OnNavigatedToPivotItemAsync(SelectedPivotItem,mode,state);
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public NukiLockViewModel()
        {
            Current = this;
            //AdministrationViewModel = new NukiLockAdministrationPartViewModel(this);
            //HomeViewModel = new NukiLockHomePartViewModel(this);
            //SettingsViewModel = new NukiLockSettingsPartViewModel(this);
            //StatusViewModel = new NukiLockStatusPartViewModel(this);
        }

        private static NukiLockViewModel Current { get; set; }
        public abstract class Part : ViewModelBase
        {
            public NukiLockViewModel BaseModel { get; private set; }
            public Part()
                :this(NukiLockViewModel.Current)
            {

            }
            public Part(NukiLockViewModel baseModel)
                : base()
            {
                BaseModel = baseModel;
             
            }

            public override IStateItems SessionState
            {
                get
                {
                    return base.SessionState ?? BaseModel.SessionState;
                }

                set
                {
                    base.SessionState = value;
                }
            }

            public override INavigationService NavigationService
            {
                get
                {
                    return base.NavigationService ?? BaseModel.NavigationService;
                }

                set
                {
                    base.NavigationService = value;
                }
            }
            public override IDispatcherWrapper Dispatcher
            {
                get
                {
                    return base.Dispatcher ?? BaseModel.Dispatcher;
                }

                set
                {
                    base.Dispatcher = value;
                }
            }
        }
    }
}
