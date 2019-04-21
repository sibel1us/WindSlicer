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
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Opacity = .5;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Left = Screen.PrimaryScreen.WorkingArea.Bottom;
            this.Bounds = Screen.PrimaryScreen.WorkingArea;
            this.Top = Screen.PrimaryScreen.WorkingArea.Top;
            base.OnLoad(e);
        }
    }
}
