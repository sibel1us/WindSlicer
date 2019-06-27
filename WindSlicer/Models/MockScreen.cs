using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer.Models
{
    public class MockScreen : IScreen, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static MockScreen Default = new MockScreen(1920, 1080, AnchorStyles.Bottom, 40);

        public static IEnumerable<AnchorStyles> TaskbarLocations { get; } = new[]
        {
            AnchorStyles.None,
            AnchorStyles.Bottom,
            AnchorStyles.Left,
            AnchorStyles.Right,
            AnchorStyles.Top
        };

        public bool Editable => true;

        private Rectangle m_bounds;
        private AnchorStyles m_taskbarLocation;
        private int m_taskbarSize;

        public int Width
        {
            get => this.Bounds.Width;
            set
            {
                var val = value.Limit(100, 10000);

                if (val != this.Width)
                {
                    this.Bounds = new Rectangle(0, 0, val, this.Height);
                    this.TaskbarSize = this.TaskbarSize;
                }
            }
        }

        public int Height
        {
            get => this.Bounds.Height;
            set
            {
                var val = value.Limit(100, 10000);

                if (val != this.Height)
                {
                    this.Bounds = new Rectangle(0, 0, this.Width, val);
                    this.TaskbarSize = this.TaskbarSize;
                }
            }
        }

        public Rectangle Bounds
        {
            get => this.m_bounds;
            private set
            {
                if (!value.Equals(this.m_bounds))
                {
                    this.m_bounds = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(Width));
                    this.OnPropertyChanged(nameof(Height));
                    this.OnPropertyChanged(nameof(WorkingArea));
                }
            }
        }

        public Rectangle WorkingArea
        {
            get
            {
                var width = this.Bounds.Width;
                var height = this.Bounds.Height;
                var tbs = this.TaskbarSize;

                switch (this.m_taskbarLocation)
                {
                    case AnchorStyles.Bottom:
                        return new Rectangle(0, 0, width, height - this.TaskbarSize);
                    case AnchorStyles.Top:
                        return new Rectangle(0, tbs, width, height - tbs);
                    case AnchorStyles.Left:
                        return new Rectangle(0, 0, width - tbs, height);
                    case AnchorStyles.Right:
                        return new Rectangle(tbs, 0, width - tbs, height);
                    default:
                        return new Rectangle(0, 0, width, height);
                }
            }
        }

        public AnchorStyles TaskbarLocation
        {
            get => this.m_taskbarLocation;
            set
            {
                if (value != this.m_taskbarLocation)
                {
                    this.m_taskbarLocation = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(WorkingArea));
                }
            }
        }

        public int TaskbarSize
        {
            get
            {
                switch (this.m_taskbarLocation)
                {
                    case AnchorStyles.Bottom:
                    case AnchorStyles.Top:
                        return Math.Max(0, Math.Min(this.m_taskbarSize, this.Bounds.Height / 2));
                    case AnchorStyles.Left:
                    case AnchorStyles.Right:
                        return Math.Max(0, Math.Min(this.m_taskbarSize, this.Bounds.Width / 2));
                    default:
                        return 0;
                }
            }
            set
            {
                int val = 0;

                switch (this.m_taskbarLocation)
                {
                    case AnchorStyles.Bottom:
                    case AnchorStyles.Top:
                        val = value.Limit(0, this.Height / 2);
                        break;
                    case AnchorStyles.Left:
                    case AnchorStyles.Right:
                        val = value.Limit(0, this.Width / 2);
                        break;
                }

                if (val != this.m_taskbarSize)
                {
                    this.m_taskbarSize = val;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(WorkingArea));
                }

                if (this.m_taskbarSize == 0)
                {
                    this.TaskbarLocation = AnchorStyles.None;
                }
            }
        }

        public int BitsPerPixel => 0;
        public string DeviceName => "Custom";
        public bool Primary => false;

        public MockScreen(int width, int height)
            : this(width, height, AnchorStyles.Bottom, 40)
        {
        }

        public MockScreen(int width, int height, AnchorStyles taskbarLocation, int taskbarSize)
        {
            this.Bounds = new Rectangle(0, 0, width, height);
            this.TaskbarLocation = taskbarLocation;
            this.TaskbarSize = taskbarSize;
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
