using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PanHG.ViewModel
{
    public class CheckBoxRepos : ViewModelBase
    {
        private string name;
        private bool isChecked;
        private string dateModified;
        private string backgroundColor;
        string tooltip;

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string DateModified
        {
            get
            {
                return dateModified;
            }
            set
            {
                dateModified = value;
                OnPropertyChanged("DateModified");
            }
        }

        public string BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        public string Tooltip
        {
            get
            {
                return tooltip;
            }
            set
            {
                tooltip = value;
                OnPropertyChanged("Tooltip");
            }
        }
    }
}
