using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Utilities;

namespace WindSlicer.Layouts
{
    public class RelativeLayout : BaseLayout
    {
        protected override void Validate(RectangleF area, int layer)
        {
            if (new[] { area.X, area.Y, area.Width, area.Height }
                .Any(f => f > 1.0 || f < 0.0))
            {
                Error.InvalidOp($"Area dimensions must be in 0.0...1.0 ({area})");
            }
        }

        public PixelLayout ToPixelLayout(Rectangle area)
        {
            var layout = new PixelLayout(area);

            foreach (var layer in this.items)
            {
                layout.AddLayer(layer.Select(old => new RectangleF(
                    (float)Math.Round(old.X * area.Width),
                    (float)Math.Round(old.Y * area.Height),
                    (float)Math.Round(old.Width * area.Width),
                    (float)Math.Round(old.Height * area.Height)))
                    .ToArray());
            }

            return layout;
        }
    }
}
