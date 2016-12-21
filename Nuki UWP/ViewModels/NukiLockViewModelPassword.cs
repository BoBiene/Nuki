using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace Nuki.ViewModels
{
    public partial class NukiLockViewModel
    {

        public bool IsPasswordDialogVisible
        {
            get;
            private set;
        }
        private string m_strUserInputPassword = string.Empty;
        public string UserInputPassword
        {
            get
            {
                return m_strUserInputPassword;
            }
            set
            {
                bool blnEmptyChanged = string.IsNullOrEmpty(m_strUserInputPassword) != string.IsNullOrEmpty(value);
                m_strUserInputPassword = value;

                RaisePropertyChanged();
                if (blnEmptyChanged)
                    PasswordOkAction.RaiseCanExecuteChanged();
            }
        }


        public DelegateCommand m_PasswordOkAction = null;
        public DelegateCommand PasswordOkAction
            => m_PasswordOkAction ?? (m_PasswordOkAction = new DelegateCommand(() =>
            {
                UInt16 securityPin = 0;
                bool blnValid = UInt16.TryParse(UserInputPassword, out securityPin);
                if (blnValid)
                {
                    IsPasswordDialogVisible = false;
                    RaisePropertyChanged(nameof(IsPasswordDialogVisible));
                    m_PasswordResult.SetResult(new PasswordRequestResult(securityPin, blnValid));
                }
                else { }
            }, () => !string.IsNullOrEmpty(UserInputPassword)));

        public DelegateCommand m_PasswordCancelAction = null;
        public DelegateCommand PasswordCancelAction
            => m_PasswordCancelAction ?? (m_PasswordCancelAction = new DelegateCommand(() =>
            {
                IsPasswordDialogVisible = false;
                RaisePropertyChanged(nameof(IsPasswordDialogVisible));
                m_PasswordResult.SetResult(new PasswordRequestResult(0, false));
            }, () => true));

        public struct PasswordRequestResult
        {
            public UInt16 SecurityPIN { get; private set; }
            public bool Successfull { get; private set; }
            public PasswordRequestResult(UInt16 securityPin, bool blnSuccessfull)
            {
                SecurityPIN = securityPin;
                Successfull = blnSuccessfull;
            }
        }

        private TaskCompletionSource<PasswordRequestResult> m_PasswordResult = null;
        public async Task<PasswordRequestResult> RequestPassword()
        {
            var dlg = new Views.dialogRequestPassword();
            var result = await dlg.ShowAsync();
            PasswordRequestResult returnValue = new PasswordRequestResult();
            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                ushort pin = 0;
                if (ushort.TryParse(dlg.UserInputPassword, out pin))
                    returnValue = new PasswordRequestResult(pin, true);

            }
            else
            {

            }

            return returnValue;
            //m_PasswordResult = new TaskCompletionSource<PasswordRequestResult>();
            //UserInputPassword = string.Empty;
            //IsPasswordDialogVisible = true;
            //RaisePropertyChanged(nameof(IsPasswordDialogVisible));
            //return await m_PasswordResult.Task;
        }
    }
}
