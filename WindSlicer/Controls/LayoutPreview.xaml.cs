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
using Screen = System.Windows.Forms.Screen;
using AnchorStyles = System.Windows.Forms.AnchorStyles;

namespace WindSlicer.Controls
{
    /// <summary>
    /// Interaction logic for LayoutPreview.xaml
    /// </summary>
    public partial class LayoutPreview : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private LayoutModel m_model = new LayoutModel { Name = "layout name" };
        private SnapAreaModel m_selectedArea = null;
        private IScreen m_selectedScreen = null;
        private bool m_usePixels = true;
        private bool m_showTaskbar = true;

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

        public IScreen SelectedScreen
        {
            get => this.m_selectedScreen;
            set
            {
                if (value != this.m_selectedScreen)
                {
                    this.m_selectedScreen = value ?? this.ScreenList.FirstOrDefault();
                    this.OnPropertyChanged();
                    this.UpdateScreen();
                }
            }
        }

        public MockScreen CustomScreen { get; } = MockScreen.Default;

        public ObservableCollection<IScreen> ScreenList { get; }
            = new ObservableCollection<IScreen>();

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

                    // Force X/Y/Width/Height to update
                    this.OnPropertyChanged("");
                }
            }
        }

        [Display(Name = "Show Taskbar")]
        public bool ShowTaskbar
        {
            get => this.m_showTaskbar;
            set
            {
                if (value != this.m_showTaskbar)
                {
                    this.m_showTaskbar = value;
                    this.OnPropertyChanged();
                    this.UpdateScreen();
                }
            }
        }

        public ScreenService ScreenService { get; } = new ScreenService();

        public LayoutPreview()
        {
            this.InitializeComponent();

            this.ScreenList
                .AddRange(this.ScreenService.Screens)
                .Insert(0, this.CustomScreen);
            this.SelectedScreen = this.CustomScreen;

            // Initialize event handlers
            this.CustomScreen.PropertyChanged += this.CustomScreen_PropertyChanged;
            this.ScreenService.PropertyChanged += this.ScreenService_PropertyChanged;
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

        private void ScreenService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.SelectedScreen = this.ScreenList.FirstOrDefault();

            for (int i = this.ScreenList.Count; i > 0; i--)
            {
                this.ScreenList.RemoveAt(i);
            }
        }

        private void UpdateScreen()
        {
            if (this.SelectedScreen.GetTaskbarLocation() != AnchorStyles.None)
            {
                var bounds = this.SelectedScreen.Bounds;
                var workingArea = this.SelectedScreen.WorkingArea;

                this.TaskBarTop.Height = workingArea.Top - bounds.Top;
                this.TaskBarBottom.Height = bounds.Bottom - workingArea.Bottom;
                this.TaskBarLeft.Width = workingArea.Left - bounds.Left;
                this.TaskBarRight.Width = bounds.Right - workingArea.Right;
            }

            this.OnPropertyChanged("");
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

        private void CustomScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.CustomScreen == this.SelectedScreen)
            {
                this.UpdateScreen();
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

        private void LayoutPreviewControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectedScreen = this.ScreenService.PrimaryScreen;
        }
    }
}
