using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;

namespace Nuki.ViewModels
{
    public class NukiLockAdministrationPartViewModel : NukiLockViewModel.Part
    {
        //public NukiLockAdministrationPartViewModel(NukiLockViewModel baseModel)
        //    : base(baseModel)
        //{

        //}
        public NukiLockAdministrationPartViewModel()
        {

        }
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            SendCalibrateCommand.RaiseCanExecuteChanged();
            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        public DelegateCommand m_SendCalibrateCommand = null;
        public DelegateCommand SendCalibrateCommand
            => m_SendCalibrateCommand ?? (m_SendCalibrateCommand = new DelegateCommand(async () =>
            {
                var userPassword = await BaseModel.RequestPassword();
                if (userPassword.Successfull)
                {
                    await BaseModel.NukiConncetion.SendCalibrateRequest(userPassword.SecurityPIN);
                }
                else { }

            }, () => BaseModel.NukiConncetion?.Connected == true));
    }
}
