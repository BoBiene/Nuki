using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Presentation
{
    public class MenuItem : NotifyPropertyChanged
    {
        private char icon;
        private string title;
        private Type pageType;


        public char Icon
        {
            get { return this.icon; }
            set { Set(ref this.icon, value); }
        }

        public int LeftMargin { get; set; } = 0;

        public string Margin
        {
            get { return string.Format("{0},0,0,0", LeftMargin); }
        }
            
        public string Title
        {
            get { return this.title; }
            set { Set(ref this.title, value); }
        }

        public Type PageType
        {
            get { return this.pageType; }
            set { Set(ref this.pageType, value); }
        }
    }
}
