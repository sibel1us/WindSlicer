using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WindSlicer.Commands;
using WindSlicer.Commands.General;
using WindSlicer.Commands.Keys;
using WindSlicer.Commands.Window;
using WindSlicer.Win32;

namespace WindSlicer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly KeyboardHook hook;

        public bool Minimized => this.WindowState == WindowState.Minimized;

        public System.Windows.Forms.NotifyIcon TrayIcon { get; private set; }
        public CommandWindow CmdWindow { get; private set; }

        public Dictionary<IKeyPress, BaseCommand> Commands { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();

            this.hook = new KeyboardHook();
            this.InitHotkeys();

            //InitTrayIcon();

            this.CmdWindow = new CommandWindow();

            this.Hide();
        }

        private void InitHotkeys()
        {
            this.hook.RegisterHotKey(ModifierKeys.Control, System.Windows.Forms.Keys.OemBackslash);
            this.hook.KeyPressed += (_, args) =>
            {
                if (args.Modifier == ModifierKeys.Control)
                {
                    this.ShowSnapWindow();
                }
            };
        }

        private void InitTrayIcon()
        {
            this.TrayIcon = new System.Windows.Forms.NotifyIcon
            {
                //Icon = Properties.Resources.TileIcon,
                Visible = true
            };

            this.TrayIcon.Click += (sender, e) =>
            {
                this.Show();
            };
        }

        private void ShowSnapWindow()
        {
            if (this.CmdWindow.IsVisible)
            {
                Console.WriteLine("Snapwindow already visible");
                this.CmdWindow.Activate();
                return;
            }

            var dict = new Dictionary<Key, Rectangle>
            {
                { Key.D1, new Rectangle(0, 0, 5, 8) },
                { Key.D2, new Rectangle(5, 0, 3, 8) },
                { Key.D3, new Rectangle(5, 0, 3, 4) },
                { Key.D4, new Rectangle(5, 4, 3, 4) },
                { Key.D5, new Rectangle(2, 1, 4, 6) },
                { Key.Z, new Rectangle(1, 1, 3, 6) },
                { Key.X, new Rectangle(4, 1, 3, 6) }
            };

            var mon = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

            int f(int scr, int mul)
            {
                if (mul == 8)
                    return scr;

                return (int)Math.Round(scr * mul / 8d);
            }

            bool snapsDefined = (this.Commands != null);

            if (!snapsDefined)
            {
                this.Commands = new Dictionary<IKeyPress, BaseCommand>();

                foreach (var kvp in dict)
                {
                    this.Commands.Add(new KeyPress(kvp.Key), new SnapWindowCommand(
                    f(mon.Width, kvp.Value.X),
                    f(mon.Height, kvp.Value.Y),
                    f(mon.Width, kvp.Value.Width),
                    f(mon.Height, kvp.Value.Height)));
                }

                string locSkype = @"C:\Program Files\Microsoft Office\Office16\lync.exe";
                string locOutlook = @"C:\Program Files\Microsoft Office\Office16\OUTLOOK.EXE";
                string locNpp = @"C:\Program Files\Notepad++\notepad++.exe";

                this.Commands.Add(new KeyPress(Key.M), new MaximizeWindowCommand());
                this.Commands.Add(new KeyPress(Key.D0), new MinimizeWindowCommand());
                this.Commands.Add(new KeyChord(Key.K, Key.M), new MaximizeWindowCommand());
                this.Commands.Add(new KeyChord(Key.K, Key.N), new MinimizeWindowCommand());
                this.Commands.Add(new KeyPress(Key.S), new ApplicationCommand(locSkype) { WindowClassName = "CommunicatorMainWindowClass" });
                this.Commands.Add(new KeyPress(Key.O), new ApplicationCommand(locOutlook));
                this.Commands.Add(new KeyPress(Key.N), new ApplicationCommand(locNpp));
                this.Commands.Add(new KeyPress(Key.L), new SpecialFolderCommand(Environment.SpecialFolder.MyComputer));
                this.Commands.Add(new KeyPress(Key.D), new SpecialFolderCommand(Environment.SpecialFolder.MyDocuments));
                this.Commands.Add(new KeyPress(Key.P), new SpecialFolderCommand(Environment.SpecialFolder.MyPictures));
            }

            this.CmdWindow.Enable(NativeMethods.GetForegroundWindow(), snapsDefined ? null : this.Commands);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            //this.TrayIcon.Visible = minimized;
            this.ShowInTaskbar = this.Minimized;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                this.hook.Dispose();
            }
        }
    }
}
