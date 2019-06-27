using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindSlicer.Models;

namespace WindSlicer.Services
{
    /// <summary>
    /// Instance class wrapper implementing <see cref="INotifyPropertyChanged"/> for <see
    /// cref="ScreenManager"/>.
    /// </summary>
    public class ScreenService : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IScreen PrimaryScreen => new ScreenWrapper(Screen.PrimaryScreen);

        public IEnumerable<IScreen> Screens
        {
            get
            {
                return Screen.AllScreens.Select(x => new ScreenWrapper(x));
            }
        }

        public ScreenService()
        {
            ScreenManager.ScreensChanged += this.ScreenManager_ScreensChanged;
        }

        private void ScreenManager_ScreensChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged(nameof(PrimaryScreen));
            this.OnPropertyChanged(nameof(Screens));
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            ScreenManager.ScreensChanged -= this.ScreenManager_ScreensChanged;
        }

        private class ScreenWrapper : IScreen
        {
            private readonly Screen screen;

            internal ScreenWrapper(Screen screen)
            {
                this.screen = screen;
            }

            public int BitsPerPixel => this.screen.BitsPerPixel;
            public Rectangle Bounds => this.screen.Bounds;
            public Rectangle WorkingArea => this.screen.WorkingArea;
            public string DeviceName => this.screen.DeviceName;
            public bool Primary => this.screen.Primary;
        }
    }
}
