using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindSlicer.Win32;

namespace WindSlicer
{
    public partial class LayoutForm : Form
    {
        private Rectangle[] rectangles;
        public IEnumerable<Rectangle> SnapLayout => rectangles.AsEnumerable();

        private readonly Timer fadeTimer = new Timer();

        private readonly Pen pen = new Pen(Brushes.Red, 8f);

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var baseParams = base.CreateParams;

                var exStyle = WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TOOLWINDOW;
                baseParams.ExStyle |= (int)exStyle;

                return baseParams;
            }
        }

        public LayoutForm()
        {
            this.rectangles = new Rectangle[0];
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            this.Opacity = 0.5;
            this.TransparencyKey = Color.Cyan;
            this.BackColor = Color.Cyan;

            this.Bounds = Screen.PrimaryScreen.WorkingArea;

            this.fadeTimer.Interval = 5;
            this.fadeTimer.Tick += (_, args) =>
            {
                if (this.Opacity < 1.0)
                    this.Opacity += 0.1;
                else
                    this.fadeTimer.Stop();
            };

            base.OnLoad(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangles(this.pen, this.rectangles);
            base.OnPaint(e);
        }

        public void SetEnabled(bool enabled)
        {
            if (enabled)
            {
                this.fadeTimer.Start();
            }
            else
            {
                this.Opacity = 0.0;
                this.fadeTimer.Stop();
            }
        }

        public void SetLayout(Rectangle[] areas)
        {
            rectangles = areas;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            pen.Dispose();
            base.OnFormClosed(e);
        }
    }
}
