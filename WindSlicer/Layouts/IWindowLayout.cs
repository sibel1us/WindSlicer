using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindSlicer.Layouts
{
    public interface IWindowLayout
    {
        int Count { get; }
        int Layers { get; }

        void Add(Rectangle area);
        void Add(Rectangle area, int layer);
        void AddLayer();
        void AddLayer(params Rectangle[] areas);

        void Remove(int layer, int index);
        void RemoveLayer(int layer);
        void Clear();

        IEnumerable<Rectangle> GetLayout(Rectangle screen);
        IEnumerable<Rectangle> GetLayout(Rectangle screen, int index);
        IEnumerable<IEnumerable<Rectangle>> GetLayouts(Rectangle screen);
    }
}
