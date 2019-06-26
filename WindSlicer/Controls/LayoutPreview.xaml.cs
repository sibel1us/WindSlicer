using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
using WindSlicer.Utilities.Converters;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer.Controls
{
    /// <summary>
    /// Interaction logic for LayoutPreview.xaml
    /// </summary>
    public partial class LayoutPreview : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const double MAX_WIDTH = 1280.0;
        private const double MAX_HEIGHT = 720.0;
        private const double DEFAULT_ASPECT = MAX_WIDTH / MAX_HEIGHT;

        private LayoutModel m_model = new LayoutModel { Name = "layout name" };
        private SnapAreaModel m_selectedArea = null;
        private bool m_usePixels = false;
        private int m_resX = 0;
        private int m_resY = 0;

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

        [Display(Name = "Use Pixels")]
        public bool UsePixels
        {
            get => this.m_usePixels;
            set
            {
                if (value != this.m_usePixels)
                {
                    this.m_usePixels = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged("");
                }
            }
        }

        [Display(Name = "Resolution Width")]
        public int ResolutionX
        {
            get => this.m_resX;
            set
            {
                if (value > 0 && value != m_resX)
                {
                    this.m_resX = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(CanvasWidth));
                    this.OnPropertyChanged(nameof(CanvasHeight));
                }
            }
        }

        [Display(Name = "Resolution Height")]
        public int ResolutionY
        {
            get => this.m_resY;
            set
            {
                if (value > 0 && value != m_resY)
                {
                    this.m_resY = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(CanvasWidth));
                    this.OnPropertyChanged(nameof(CanvasHeight));
                }
            }
        }

        public double AspectRatio => this.ResolutionX / (double)this.ResolutionY;

        public double CanvasWidth
        {
            get
            {
                if (this.AspectRatio == double.NaN || this.AspectRatio > DEFAULT_ASPECT)
                {
                    return MAX_WIDTH;
                }
                else
                {
                    return MAX_WIDTH / this.AspectRatio;
                }
            }
        }

        public double CanvasHeight
        {
            get
            {
                if (this.AspectRatio == double.NaN || this.AspectRatio <= DEFAULT_ASPECT)
                {
                    return MAX_HEIGHT;
                }
                else
                {
                    return MAX_HEIGHT / this.AspectRatio;
                }
            }
        }

        public LayoutPreview()
        {
            this.InitializeComponent();

            // TODO: taskbar location
            this.ResolutionX = ScreenManager.PrimaryScreen.WorkingArea.Width;
            this.ResolutionY = ScreenManager.PrimaryScreen.WorkingArea.Height;

            // Initialize event handlers
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
            foreach (var item in items)
            {
                this.Container.Children.Add(new SnapAreaPreview
                {
                    Model = item
                });
            }
        }

        private void RemoveItems(IEnumerable<SnapAreaModel> items)
        {
            var controlsToRemove = new List<SnapAreaPreview>();

            foreach (var item in this.Container.Children)
            {
                if (item is SnapAreaPreview snapPreview &&
                    !items.Contains(snapPreview.Model))
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

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var inputs = new Control[]
            {
                this.InputX, this.InputY, this.InputWidth, this.InputHeight
            };

            foreach (var input in inputs)
            {
                input.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
