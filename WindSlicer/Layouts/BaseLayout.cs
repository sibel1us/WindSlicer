using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Utilities;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer.Layouts
{
    public abstract class BaseLayout : IWindowLayout
    {
        protected List<List<RectangleF>> items;

        public int Count => this.items.Sum(x => x.Count);
        public int Layers => this.items.Count();

        protected virtual void Validate(RectangleF area, int layer)
        {
            if (area.IsEmpty)
            {
                Error.InvalidOp("Cannot add empty area");
            }

            if (layer < 0 || layer >= this.items.Count)
            {
                Error.OutOfRange(
                    $"Layer index {layer} out of range, must be in 0...{this.items.Count}");
            }

            if (this.items[layer]
                .TryGetFirst(r => r.IntersectsWith(area), out RectangleF old))
            {
                Error.InvalidOp($"New area {area} intersects with {old}");
            }
        }

        public void Add(RectangleF area)
        {
            this.Validate(area, 0);
            this.items[0].Add(area);
        }

        public void Add(RectangleF area, int layer)
        {
            this.Validate(area, layer);
            this.items[layer].Add(area);
        }

        public void AddLayer()
        {
            items.Add(new List<RectangleF>());
        }

        public void AddLayer(params RectangleF[] areas)
        {
            this.items.Add(areas.ToList());
        }

        public void Clear()
        {
            this.items.Clear();
            this.items.Add(new List<RectangleF>());
        }

        public IEnumerable<Rectangle> GetLayout(Rectangle screen)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Rectangle> GetLayout(Rectangle screen, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<Rectangle>> GetLayouts(Rectangle screen)
        {
            throw new NotImplementedException();
        }

        public void Remove(int layer, int index)
        {
            this.items[layer].RemoveAt(index);

            if (this.items[layer].None())
            {
                this.items.RemoveAt(layer);
            }
        }

        public void RemoveLayer(int layer)
        {
            if (this.items.Count == 1 && layer == 1)
            {
                this.Clear();
            }
            else
            {
                this.items.RemoveAt(layer);
            }
        }
    }
}
