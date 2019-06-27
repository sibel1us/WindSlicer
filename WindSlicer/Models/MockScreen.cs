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
    public class MockScreen : IScreen
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

        public Rectangle Bounds { get; }
        public Rectangle WorkingArea { get; }
        public int BitsPerPixel => 0;
        public string DeviceName => "Custom";
        public bool Primary => false;

        public MockScreen(int width, int height)
            : this(width, height, AnchorStyles.Bottom, 40)
        {
        }

        public MockScreen(
            int width,
            int height,
            AnchorStyles taskbarLocation,
            int taskbarSize)
        {
            this.Bounds = new Rectangle(0, 0, width, height);

            var tbw = Math.Max(0, Math.Min(taskbarSize, width / 2));
            var tbh = Math.Max(0, Math.Min(taskbarSize, height / 2));

            switch (taskbarLocation)
            {
                case AnchorStyles.Bottom:
                    this.WorkingArea = new Rectangle(0, 0, width, height - tbh);
                    break;
                case AnchorStyles.Top:
                    this.WorkingArea = new Rectangle(0, tbh, width, height - tbh);
                    break;
                case AnchorStyles.Left:
                    this.WorkingArea = new Rectangle(0, 0, width - tbw, height);
                    break;
                case AnchorStyles.Right:
                    this.WorkingArea = new Rectangle(tbw, 0, width - tbw, height);
                    break;
                case AnchorStyles.None:
                default:
                    this.WorkingArea = new Rectangle(0, 0, width, height);
                    break;
            }
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
