using Nuki.ViewModels;
using Nuki.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Nuki.Pages.Setup
{

    public class IconItem
    {
        public Symbol Icon { get; set; }
        public IconItem(Symbol icon)
        {
            Icon = icon;
        }

    }

    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Setup05IconPage : Page

    {
        private ObservableCollection<IconItem> m_IconList = new ObservableCollection<IconItem>();
        public Setup05IconPage()
        {

            m_IconList.Add(new IconItem(Symbol.Home));
            m_IconList.Add(new IconItem(Symbol.SolidStar));
            m_IconList.Add(new IconItem(Symbol.OutlineStar));
            m_IconList.Add(new IconItem(Symbol.Tag));
            m_IconList.Add(new IconItem(Symbol.Street));
            m_IconList.Add(new IconItem(Symbol.Like));
            m_IconList.Add(new IconItem(Symbol.Dislike));
            m_IconList.Add(new IconItem(Symbol.Permissions));
            m_IconList.Add(new IconItem(Symbol.Audio));
            m_IconList.Add(new IconItem(Symbol.Contact));
            m_IconList.Add(new IconItem(Symbol.Emoji));
            m_IconList.Add(new IconItem(Symbol.Repair));
            m_IconList.Add(new IconItem(Symbol.Shop));
            m_IconList.Add(new IconItem(Symbol.Phone));
            m_IconList.Add(new IconItem(Symbol.Flag));
            m_IconList.Add(new IconItem(Symbol.World));
            m_IconList.Add(new IconItem(Symbol.People));
            m_IconList.Add(new IconItem(Symbol.Clock));
            m_IconList.Add(new IconItem(Symbol.Emoji2));
            m_IconList.Add(new IconItem(Symbol.Mail));
            m_IconList.Add(new IconItem(Symbol.Camera));
            SetupNewLock.Current.ViewModel.BackgoundMode = BackgoundMode.BluredDarkImage;
            this.InitializeComponent();
        }

        public object SelectedIcon
        {
            get { return ViewModel.Icon; }
            set
            {
                if (value is Symbol)
                    ViewModel.Icon = (Symbol)value;
                else if (value is IconItem)
                    ViewModel.Icon = ((IconItem)value).Icon;
            }
        }
        public SetupNewLockViewModel ViewModel => SetupNewLock.Current.ViewModel;
        public ObservableCollection<IconItem> IconList
        {
            get
            {
                return m_IconList;
            }
            set
            {
                if (m_IconList == value)
                    return;
                m_IconList = value;
            }
        }
    }
}
