using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        bool Editable { get; }

        [Display(Name = "Width")]
        int Width { get; set; }

        [Display(Name = "Height")]
        int Height { get; set; }

        [Display(Name = "Taskbar")]
        AnchorStyles TaskbarLocation { get; set; }

        [Display(Name = "Taskbar Size")]
        int TaskbarSize { get; set; }
    }
}
