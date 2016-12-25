using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Nuki.ViewModels
{
    public abstract class PivotBaseViewModel<T> : ViewModelBase
        where T : PivotBaseViewModel<T>
    {
        private object m_SelectedPivotItem = null;
        public object SelectedPivotItem
        {
            get { return m_SelectedPivotItem; }
            set
            {
                var prev = m_SelectedPivotItem;
                Set(ref m_SelectedPivotItem, value);
                OnNavigatedToPivotItemAsync(prev,value).GetAwaiter();
            }
        }

        private static async Task OnNavigatedToPivotItemAsync(object prevValue, object value, NavigationMode mode = NavigationMode.Refresh, IDictionary<string, object> state = null)
        {
            Part prevViewModel = GetViewModel(prevValue);
            if (prevViewModel != null)
            {
                await prevViewModel.OnNavigatingFromAsync(new NavigatingEventArgs(new DeferralManager()));
            }
            else { }


            Part viewModel = GetViewModel(value);
            if (viewModel != null)
            {
                await viewModel.OnNavigatedToAsync(null, mode, state);
            }
            else { }

            if (prevViewModel != null)
            {
                await prevViewModel.OnNavigatedFromAsync(state, false);
            }
            else { }
        }

        private static Part GetViewModel(object objPivotItem)
        {
            PivotItem pivotItem = objPivotItem as PivotItem;
            if (pivotItem != null)
            {
                UserControl control = pivotItem.Content as UserControl;

                return control?.DataContext as Part;
            }
            else { }

            return null;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            Part viewModel = GetViewModel(SelectedPivotItem);
            if (viewModel != null)
            {
                await viewModel.OnNavigatedFromAsync(pageState,suspending);
            }
            else { }
            await base.OnNavigatedFromAsync(pageState, suspending);
        }
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            Part viewModel = GetViewModel(SelectedPivotItem);
            if (viewModel != null)
            {
                await viewModel.OnNavigatingFromAsync(args);
            }
            else { }
            await base.OnNavigatingFromAsync(args);
        }
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Part viewModel = GetViewModel(SelectedPivotItem);
            if (viewModel != null)
            {
                await viewModel.OnNavigatedToAsync(parameter,mode,state);
            }
            else { }
            
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public PivotBaseViewModel()
        {
            Current = (T)this;
        }


        private static T Current { get; set; }
        public abstract class Part : ViewModelBase
        {
            protected static ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<Part>();
            public T BaseModel { get; private set; }
            public Part()
                : this(PivotBaseViewModel<T>.Current)
            {

            }
            public Part(T baseModel)
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
