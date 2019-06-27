using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Services;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer.Models
{
    public class LayoutModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_name;

        public string Name
        {
            get => m_name;
            set
            {
                if (value != this.m_name)
                {
                    this.m_name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<SnapAreaModel> Areas { get; set; }

        public LayoutModel()
        {
            this.Name = "";
            this.Areas = new ObservableCollection<SnapAreaModel>();
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
