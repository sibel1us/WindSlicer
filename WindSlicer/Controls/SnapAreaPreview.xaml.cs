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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindSlicer.Models;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer.Controls
{
    /// <summary>
    /// Interaction logic for SnapAreaPreview.xaml
    /// </summary>
    public partial class SnapAreaPreview : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SnapAreaModel m_model;

        public SnapAreaModel Model
        {
            get => this.m_model;
            set
            {
                if (value != this.m_model)
                {
                    this.m_model = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Rectangle RootRectangle => this.Root;
        public LayoutPreview Layout => this.FindParent<LayoutPreview>();

        public SnapAreaPreview()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //OnPropertyChanged(x => x.Container);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            base.OnRender(drawingContext);
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled && this.Layout != null)
            {
                if (this.Layout.SelectedArea != this.Model)
                {
                    this.Layout.SelectedArea = this.Model;
                    e.Handled = true;
                }
            }
        }
    }
}
