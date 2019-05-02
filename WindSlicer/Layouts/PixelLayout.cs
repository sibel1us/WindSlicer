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

        protected override void Validate(RectangleF area, int layer)
        {
            base.Validate(area, layer);

            if (area.Width > this.Area.Width || area.Height > this.Area.Height)
            {
                Error.InvalidOp(
                    $"Area is larger than the intended area ({area} vs {this.Area})");
            }


        }

        public override IEnumerable<Rectangle> GetLayout(Rectangle screen, int index)
        {
            if (!screen.Equals(this.Area))
                return new Rectangle[0];

            return this.items.ElementAt(index).Select(r => Rectangle.Round(r));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newArea"></param>
        /// <returns></returns>
        public PixelLayout Transform(Rectangle newArea)
        {
            var layout = new PixelLayout(newArea);

            var xMult = newArea.Width / this.Area.Width;
            var yMult = newArea.Height / this.Area.Height;

            foreach (var layer in this.items)
            {
                layout.AddLayer(layer.Select(old => new RectangleF(
                    old.X * xMult,
                    old.Y * yMult,
                    old.Width * xMult,
                    old.Height * yMult))
                    .ToArray());
            }

            return layout;
        }

        public RelativeLayout ToRelativeLayout()
        {
            var layout = new RelativeLayout();

            foreach (var layer in this.items)
            {
                layout.AddLayer(layer.Select(old => new RectangleF(
                    old.X / this.Area.Width,
                    old.Y / this.Area.Height,
                    old.Width / this.Area.Width,
                    old.Height / this.Area.Height))
                    .ToArray());
            }

            return layout;
        }
    }
}
