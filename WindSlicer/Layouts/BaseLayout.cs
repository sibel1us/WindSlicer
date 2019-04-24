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
        protected List<List<Rectangle>> items;

        public int Count => this.items.Sum(x => x.Count);
        public int Layers => this.items.Count();

        protected virtual void Validate(Rectangle area, int layer)
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

            if (this.items[layer].Any(r => r.IntersectsWith(area)))
            {
                Error.InvalidOp($"Space is already reserved, cannot add {area}");
            }
        }

        public void Add(Rectangle area)
        {
            this.Validate(area, 0);
            this.items[0].Add(area);
        }

        public void Add(Rectangle area, int layer)
        {
            this.Validate(area, layer);
            this.items[layer].Add(area);
        }

        public void AddLayer()
        {
            items.Add(new List<Rectangle>());
        }

        public void AddLayer(params Rectangle[] areas)
        {
            this.items.Add(areas.ToList());
        }

        public void Clear()
        {
            this.items.Clear();
            this.items.Add(new List<Rectangle>());
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
