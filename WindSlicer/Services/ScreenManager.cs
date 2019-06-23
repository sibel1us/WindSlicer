using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WindSlicer.Services
{
    public static class ScreenManager
    {
        public static event EventHandler ScreensChanged;

        private static Screen m_primary = Screen.PrimaryScreen;
        private static Screen[] m_screens = Screen.AllScreens;

        public static Screen PrimaryScreen => m_primary;
        public static IEnumerable<Screen> Screens => m_screens;

        private static readonly object subLock = new object();
        private static bool isSubscribed = false;

        public static void Subscribe()
        {
            if (!isSubscribed)
            {
                lock (subLock)
                {
                    if (!isSubscribed)
                    {
                        SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
                        DisplaySettingsChanged(null, null);
                        isSubscribed = true;
                    }
                }
            }
        }

        public static void Unsubscribe()
        {
            if (isSubscribed)
            {
                lock (subLock)
                {
                    if (isSubscribed)
                    {
                        SystemEvents.DisplaySettingsChanged -= DisplaySettingsChanged;
                        isSubscribed = false;
                    }
                }
            }
        }

        private static void DisplaySettingsChanged(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                RaiseIfScreensChanged);
        }

        private static void RaiseIfScreensChanged()
        {
            bool changed = false;

            var newPrimary = Screen.PrimaryScreen;
            var newScreens = Screen.AllScreens;

            if (m_screens.Length != newScreens.Length)
            {
                changed = true;
            }
            else if (!Compare(m_primary, newPrimary))
            {
                changed = true;
            }
            else
            {
                for (int i = 0; i < m_screens.Length; i++)
                {
                    if (!Compare(m_screens[i], newScreens[i]))
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (changed)
            {
                m_primary = newPrimary;
                m_screens = newScreens;
                ScreensChanged?.Invoke(null, null);
            }
        }

        private static bool Compare(Screen a, Screen b)
        {
            return a.Primary == b.Primary
                && a.Bounds.Equals(b.Bounds)
                && a.WorkingArea.Equals(b.WorkingArea)
                && a.BitsPerPixel == b.BitsPerPixel;
        }
    }
}
