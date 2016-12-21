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

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await OnNavigatedToPivotItemAsync(SelectedPivotItem, mode, state);
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public PivotBaseViewModel()
        {
            Current = (T)this;
        }


        private static T Current { get; set; }
        public abstract class Part : ViewModelBase
        {
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
