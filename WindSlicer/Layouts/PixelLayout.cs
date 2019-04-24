using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Utilities;

namespace WindSlicer.Layouts
{
    public class PixelLayout : BaseLayout
    {
        public Rectangle Area { get; }

        public PixelLayout(Rectangle area)
        {
            this.Area = area;
        }

        protected override void Validate(Rectangle area, int layer)
        {
            base.Validate(area, layer);

            if (area.Width > this.Area.Width || area.Height > this.Area.Height)
            {
                Error.InvalidOp(
                    $"Area is larger than the intended area ({area} vs {this.Area})");
            }


        }
    }
}
