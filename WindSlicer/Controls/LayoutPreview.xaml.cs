using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private double m_aspectRatio = 16d / 9d;
        private LayoutModel m_model = new LayoutModel
        {
            Name = "Nibba"
        };

        public LayoutModel Model
        {
            get => this.m_model ?? (this.m_model = new LayoutModel());
            set
            {
                if (value != this.m_model)
                {
                    this.m_model = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private SnapAreaModel m_selectedArea = null;
        public SnapAreaModel SelectedArea
        {
            get => this.m_selectedArea;
            set
            {
                if (value != this.m_selectedArea)
                {
                    this.m_selectedArea = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Canvas AreaContainer => this.Container;
        public ListBox AreaListBox => this.ItemListBox;

        public double AspectRatio
        {
            get => this.m_aspectRatio;
            private set
            {
                if (value > 0.0 && !value.Equals5Digits(this.m_aspectRatio))
                {
                    this.m_aspectRatio = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public double CanvasWidth
        {
            get
            {
                var width = 640.0;
                return width;
            }
        }

        public double CanvasHeight
        {
            get
            {
                var height = 480.0;
                return height;
            }
        }

        public LayoutPreview()
        {
            this.InitializeComponent();

            // Initialize event handlers
            this.PropertyChanged += this.LayoutPreview_PropertyChanged;
            this.Model.Areas.CollectionChanged += this.Areas_CollectionChanged;

            this.Model.Areas.Add(new SnapAreaModel
            {
                Width = 0.25,
                Height = 0.25,
                X = 0.11,
                Y = 0.11
            });

            this.Model.Areas.Add(new SnapAreaModel
            {
                Width = 0.2,
                Height = 0.3,
                X = 0.75,
                Y = 0.65
            });
        }

        private void AddItems(IEnumerable<SnapAreaModel> items)
        {
            SnapAreaPreview p = null;
            foreach (var item in items)
            {
                p = new SnapAreaPreview
                {
                    Model = item
                };
                this.Container.Children.Add(p);
            }
        }

        private void RemoveItems(IEnumerable<SnapAreaModel> items)
        {
            var controlsToRemove = new List<SnapAreaPreview>();

            foreach (var item in this.Container.Children)
            {
                if (item is SnapAreaPreview snapPreview &&
                    items.Contains(snapPreview.Model))
                {
                    controlsToRemove.Add(snapPreview);
                }
            }

            foreach (var control in controlsToRemove)
            {
                this.Container.Children.Remove(control);
            }
        }

        private void ClearItems()
        {
            this.Container.Children.Clear();
        }

        private void Areas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddItems(e.NewItems.OfType<SnapAreaModel>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveItems(e.NewItems.OfType<SnapAreaModel>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ClearItems();
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    this.ClearItems();
                    this.AddItems(e.NewItems.OfType<SnapAreaModel>());
                    break;
            }
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LayoutPreview_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.Model):
                    break;
                case nameof(this.AspectRatio):
                    this.OnPropertyChanged(nameof(CanvasWidth));
                    this.OnPropertyChanged(nameof(CanvasHeight));
                    break;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
