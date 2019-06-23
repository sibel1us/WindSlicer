using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WindSlicer.Utilities.Extensions;
using System.ComponentModel.DataAnnotations;

namespace WindSlicer.Models
{
    public class SnapAreaModel : INotifyPropertyChanged
    {
        private const double MIN_VALUE = 0.0;
        private const double MAX_VALUE = 0.0;

        public event PropertyChangedEventHandler PropertyChanged;

        private double m_x;
        private double m_y;
        private double m_width;
        private double m_height;

        [Range(MIN_VALUE, MAX_VALUE)]
        public double X
        {
            get => this.m_x;
            set
            {
                if (!value.Equals5Digits(this.m_x))
                {
                    this.m_x = Constrain(value);
                    OnPropertyChanged(x => x.X);
                }
            }
        }

        [Range(MIN_VALUE, MAX_VALUE)]
        public double Y
        {
            get => this.m_y;
            set
            {
                if (!value.Equals5Digits(this.m_y))
                {
                    this.m_y = Constrain(value);
                    OnPropertyChanged(x => x.Y);
                }
            }
        }

        [Range(MIN_VALUE, MAX_VALUE)]
        public double Width
        {
            get => this.m_width;
            set
            {
                if (!value.Equals5Digits(this.m_width))
                {
                    this.m_width = Constrain(value);
                    OnPropertyChanged(x => x.Width);
                }
            }
        }

        [Range(MIN_VALUE, MAX_VALUE)]
        public double Height
        {
            get => this.m_height;
            set
            {
                if (!value.Equals5Digits(this.m_height))
                {
                    this.m_height = Constrain(value);
                    OnPropertyChanged(x => x.Height);
                }
            }
        }

        /// <summary>
        /// Initializes a new model for a snap area.
        /// </summary>
        public SnapAreaModel()
        {
            this.m_x = 0.0;
            this.m_y = 0.0;
            this.m_width = 1.0;
            this.m_height = 1.0;
        }

        /// <summary>
        /// Gets the bounds of the area on the parameter screen.
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        public Rectangle GetBoundsFor(Rectangle screen)
        {
            return new Rectangle(
                screen.X + (screen.X * this.X).Round(),
                screen.Y + (screen.Y * this.Y).Round(),
                (screen.Width * this.Width).Round(),
                (screen.Height * this.Height).Round());
        }

        /// <summary>
        /// Sets the area based on the ratio of the parameter screen and area.
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="area"></param>
        public void SetBoundsFor(Rectangle screen, Rectangle area)
        {
            if (screen.IsEmpty || area.IsEmpty || !screen.IntersectsWith(area))
                return;

            this.X = (area.X - screen.X) / (double)screen.Width;
            this.Y = (area.Y - screen.Y) / (double)screen.Height;
            this.Width = area.Width / (double)screen.Width;
            this.Height = area.Height / (double)screen.Height;
        }

        protected void OnPropertyChanged<T>(Expression<Func<SnapAreaModel, T>> expression)
        {
            PropertyChanged.InvokeFor(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private double Constrain(double d) => (d < MIN_VALUE)
            ? MIN_VALUE
            : (d > MAX_VALUE) ? MAX_VALUE : d;
    }
}
