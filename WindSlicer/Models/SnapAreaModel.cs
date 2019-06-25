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
using System.Runtime.CompilerServices;

namespace WindSlicer.Models
{
    public class SnapAreaModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private const double MIN_VALUE = 0.0;
        private const double MAX_VALUE = 1.0;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayString => this.ToString();

        public string Error { get => null; }

        public string this[string columnName]
        {
            get
            {
                bool errored = false;

                switch (columnName)
                {
                    case nameof(this.X):
                        errored = this.IsInvalid(this.X);
                        break;
                    case nameof(this.Y):
                        errored = this.IsInvalid(this.Y);
                        break;
                    case nameof(this.Width):
                        errored = this.IsInvalid(this.Width);
                        break;
                    case nameof(this.Height):
                        errored = this.IsInvalid(this.Height);
                        break;
                }

                if (errored)
                {
                    return $"Value must be between {MIN_VALUE} and {MAX_VALUE}";
                }

                switch (columnName)
                {
                    case nameof(this.X):
                    case nameof(this.Width):
                        if ((this.X + this.Width) > MAX_VALUE)
                        {
                            return $"Left and Width combined cannot exceed {MAX_VALUE}";
                        }
                        break;
                    case nameof(this.Y):
                    case nameof(this.Height):
                        if ((this.Y + this.Height) > MAX_VALUE)
                        {
                            return $"Top and Height combined cannot exceed {MAX_VALUE}";
                        }
                        break;
                }

                return null;
            }
        }

        private double m_x;
        private double m_y;
        private double m_width;
        private double m_height;

        [Display(Name = "Left")]
        [Range(MIN_VALUE, MAX_VALUE)]
        public double X
        {
            get => this.m_x;
            set
            {
                if (!value.Equals5Digits(this.m_x))
                {
                    this.m_x = value;
                    this.UpdatedXAxis();
                }
            }
        }

        [Display(Name = "Top")]
        [Range(MIN_VALUE, MAX_VALUE)]
        public double Y
        {
            get => this.m_y;
            set
            {
                if (!value.Equals5Digits(this.m_y))
                {
                    this.m_y = value;
                    this.UpdatedYAxis();
                }
            }
        }

        [Display(Name = "Width")]
        [Range(MIN_VALUE, MAX_VALUE)]
        public double Width
        {
            get => this.m_width;
            set
            {
                if (!value.Equals5Digits(this.m_width))
                {
                    this.m_width = value;
                    this.UpdatedXAxis();
                }
            }
        }

        [Display(Name = "Height")]
        [Range(MIN_VALUE, MAX_VALUE)]
        public double Height
        {
            get => this.m_height;
            set
            {
                if (!value.Equals5Digits(this.m_height))
                {
                    this.m_height = value;
                    this.UpdatedYAxis();
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

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            string td(double d)
            {
                return d.ToString("0.000");
            }

            return $"X:{td(this.X)}, Y:{td(this.Y)}, W:{td(this.Width)}, H:{td(this.Height)}";
        }

        private void UpdatedXAxis()
        {
            this.OnPropertyChanged(nameof(X));
            this.OnPropertyChanged(nameof(Width));
            this.OnPropertyChanged(nameof(DisplayString));
        }

        private void UpdatedYAxis()
        {
            this.OnPropertyChanged(nameof(Y));
            this.OnPropertyChanged(nameof(Height));
            this.OnPropertyChanged(nameof(DisplayString));
        }

        private double Constrain(double value) => Math.Max(MIN_VALUE, Math.Min(MIN_VALUE, value));

        private bool IsInvalid(double value) => value < MIN_VALUE || value > MAX_VALUE;
    }
}
