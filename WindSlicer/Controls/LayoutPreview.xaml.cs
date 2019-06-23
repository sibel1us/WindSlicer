using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
using WindSlicer.Services;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer.Controls
{
    /// <summary>
    /// Interaction logic for LayoutPreview.xaml
    /// </summary>
    public partial class LayoutPreview : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private LayoutModel m_layout = null;
        private SnapAreaModel m_selectedArea = null;
        private double m_aspectRatio = 16d / 9d;

        public LayoutModel Layout
        {
            get => this.m_layout;
            private set
            {
                if (value != this.m_layout)
                {
                    this.m_layout = value;
                    this.OnPropertyChanged(x => x.Layout);
                }
            }
        }
        public SnapAreaModel SelectedArea
        {
            get => this.m_selectedArea;
            private set
            {
                if (value != this.m_selectedArea)
                {
                    this.m_selectedArea = value;
                    this.OnPropertyChanged(x => x.SelectedArea);
                }
            }
        }

        public double AspectRatio
        {
            get => this.m_aspectRatio;
            private set
            {
                if (value > 0.0 && !value.Equals5Digits(this.m_aspectRatio))
                {
                    this.m_aspectRatio = value;
                    this.OnPropertyChanged(x => x.AspectRatio);
                }
            }
        }

        /// <summary>
        /// Scale used for the viewbox
        /// </summary>
        public double PreviewScale = 0.0;

        public LayoutPreview()
        {
            this.InitializeComponent();

            // Initialize event handlers
            this.PropertyChanged += this.LayoutPreview_PropertyChanged;
        }

        protected void OnPropertyChanged<T>(
            Expression<Func<LayoutPreview, T>> expression)
        {
            PropertyChanged?.InvokeFor(expression);
        }

        private void LayoutPreview_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.Layout):
                    this.SelectedArea = this.Layout?.Areas.FirstOrDefault();
                    break;
                default:
                    break;
            }
        }
    }
}
