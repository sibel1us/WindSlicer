using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindSlicer.Models;
using WindSlicer.Utilities.Extensions;
using DRectangle = System.Drawing.Rectangle;

namespace WindSlicer.Windows
{
    /// <summary>
    /// Interaction logic for LayoutConfigurationWindow.xaml
    /// </summary>
    public partial class LayoutConfigurationWindow : Window,  INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public LayoutModel SelectedLayout { get; private set; }
        public SnapAreaModel SelectedArea { get; private set; }

        public LayoutConfigurationWindow()
        {
            InitializeComponent();
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
