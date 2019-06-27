using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
}
