using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Models
{
    public interface IScreen
    {
        int BitsPerPixel { get; }
        Rectangle Bounds { get; }
        Rectangle WorkingArea { get; }
        string DeviceName { get; }
        bool Primary { get; }
    }

    public class MockScreen : IScreen
    {
        private Rectangle bounds;

        public int BitsPerPixel => 0;
        public Rectangle Bounds => this.bounds;
        public Rectangle WorkingArea => this.bounds;
        public string DeviceName => "";
        public bool Primary => false;

        public MockScreen(int width, int height)
        {
            this.bounds = new Rectangle(0, 0, width, height);
        }
    }
}
