using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Nuki.Communication.Util;

namespace Nuki.Presentation
{
    public enum BackgoundMode
    {
        None,
        CleanImage,
        BluredImage,
        BluredDarkImage,
    }

    public class ShellViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<MenuItem> menuItems = new ObservableCollection<MenuItem>();
        private MenuItem selectedMenuItem;
        private bool isSplitViewPaneOpen;
        private Visibility m_MenuVisibility = Visibility.Visible;
        private BackgoundMode m_BackgroundMode = BackgoundMode.CleanImage;
        
        public ShellViewModel()
        {
            this.ToggleSplitViewPaneCommand = new Command(() => this.IsSplitViewPaneOpen = !this.IsSplitViewPaneOpen);
        }

        public ICommand ToggleSplitViewPaneCommand { get; private set; }

        public bool IsSplitViewPaneOpen
        {
            get { return this.isSplitViewPaneOpen; }
            set { Set(ref this.isSplitViewPaneOpen, value); }
        }

        public bool MenuEnabled { get { return MenuVisibility == Visibility.Visible; } }
        public Visibility MenuVisibility
        {
            get { return m_MenuVisibility; }
            set
            {
                if (Set(ref this.m_MenuVisibility, value))
                    OnPropertyChanged(nameof(MenuEnabled));
            }
        }

        public BackgoundMode BackgoundMode
        {
            get { return m_BackgroundMode; }
            set { Set(ref m_BackgroundMode, value); }
        }

        public MenuItem SelectedMenuItem
        {
            get { return this.selectedMenuItem; }
            set
            {
                if (value == null)
                {
                    MenuVisibility = Visibility.Collapsed;
                    // auto-close split view pane
                    this.IsSplitViewPaneOpen = false;
                    this.selectedMenuItem = value;
                }
                else
                {
                    MenuVisibility = Visibility.Visible;

                    if (Set(ref this.selectedMenuItem, value))
                    {
                        OnPropertyChanged("SelectedPageType");
                        // auto-close split view pane
                        this.IsSplitViewPaneOpen = false;
                    }

                    else { }
                }
            }
        }
        private Type m_nonMenuPage = null;
        public Type SelectedPageType
        {
            get
            {
                if (this.selectedMenuItem != null) {
                    return this.selectedMenuItem.PageType;
                }
                return m_nonMenuPage;
            }
            set
            {
                
                // select associated menu item
                this.SelectedMenuItem = this.menuItems.FirstOrDefault(m => m.PageType == value);

                if (this.SelectedMenuItem == null)
                    Set(ref m_nonMenuPage, value);
                else
                    m_nonMenuPage = null;
            }
        }

        public ObservableCollection<MenuItem> MenuItems
        {
            get { return this.menuItems; }
        }
    }
}
