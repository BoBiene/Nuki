using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Nuki.Communication.API;

namespace Nuki.Converter
{
    public class LockStateConverter : IValueConverter
    {
        public NukiLockState LockState { get; set; }
        public bool Inverted { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool blnRet = false;
            NukiLockState state;
            if (Enum.TryParse<NukiLockState>(value?.ToString(), out state))
            {
                blnRet = state == LockState;
                
            }
            else { }

            if (Inverted)
                blnRet = !blnRet;
            return blnRet;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
