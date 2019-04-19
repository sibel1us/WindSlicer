using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindSlicer.Commands;
using WindSlicer.Commands.Keys;
using WindSlicer.Utilities.Extensions;

namespace WindSlicer
{
    using KeyCommandMap = ObservableConcurrentDictionary<IKeyPress, BaseCommand>;

    /// <summary>
    /// Interaction logic for SnapWindow.xaml
    /// </summary>
    public partial class CommandWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IntPtr _activeHwnd = IntPtr.Zero;
        private KeyPress _chordPrimer = null;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr ActiveHwnd
        {
            get => _activeHwnd;
            set
            {
                if (value != _activeHwnd)
                {
                    _activeHwnd = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveHwnd)));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public KeyPress ChordPrimer
        {
            get => _chordPrimer;
            set
            {
                if (!KeyPress.Equal(value, _chordPrimer))
                {
                    _chordPrimer = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChordPrimer)));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public KeyCommandMap Commands { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private readonly HashSet<Key> _modifiers = new HashSet<Key>
        {
            Key.LeftCtrl, Key.LeftShift, Key.LeftAlt,
            Key.RightAlt, Key.RightShift, Key.RightAlt,
            Key.LWin, Key.RWin, Key.System
        };

        /// <summary>
        /// 
        /// </summary>
        public CommandWindow()
        {
            Commands = new KeyCommandMap();

            InitializeComponent();


            PropertyChanged += Window_PropertyChanged;
            Commands.CollectionChanged += Commands_CollectionChanged;
        }

        private void Commands_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public void Enable(IntPtr target, IDictionary<IKeyPress, BaseCommand> commands = null)
        {
            if (commands != null)
            {
                this.Commands.RemoveAll();
                this.Commands.AddRange(commands);
            }

            // No window / not an open window (might be Desktop etc.)
            if (!this.IsVisible)
            {
                this.Show();
            }

            this.ActiveHwnd = target;

            this.Activate();
            this.Focus();
        }

        // TODO: refactor this dumb method
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            void Done()
            {
                e.Handled = true;
                this.ChordPrimer = null;
                this.Hide();
            }

            // Don't trigger for only modifiers
            if (_modifiers.Contains(e.Key))
            {
                e.Handled = true;
                return;
            }

            // Escape key always cancels
            if (e.Key == Key.Escape)
            {
                Done();
                return;
            }

            IKeyPress keyPress = new KeyPress(e.Key, Keyboard.Modifiers);

            if (ChordPrimer != null)
            {
                keyPress = new KeyChord(ChordPrimer, (KeyPress)keyPress);
            }

            // Not found
            if (Commands.Keys.None(kp => kp.IsMatch(keyPress)))
            {
                Done();
                return;
            }

            var match = Commands.First(kvp => kvp.Key.IsMatch(keyPress));

            // Prime chord
            if (ChordPrimer == null && match.Key is KeyChord chord)
            {
                ChordPrimer = chord.First;
                e.Handled = true;
                return;
            }

            ICommand command = match.Value;

            if (command is WindowCommand windowCommand)
            {
                // TODO: error logging
                if (windowCommand.CanExecute(ActiveHwnd))
                {
                    windowCommand.Execute(ActiveHwnd);
                }
            }
            else
            {
                if (command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }

            Done();
        }

        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //if (e.NewFocus is Button)
            //    return;
            //this.Disable();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Disable();
        }

        private void Disable()
        {
            if (this.IsVisible)
            {
                Console.WriteLine("Lost focus");
                this.Hide();
                this.ActiveHwnd = IntPtr.Zero;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Window_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ActiveHwnd))
            {
                foreach (var wndCmd in Commands.Values.Where(cmd => cmd is WindowCommand))
                {
                    wndCmd.RaiseCanExecuteChanged();
                }
            }
        }
    }
}
