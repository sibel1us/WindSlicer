using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WindSlicer.Commands.Keys
{
    /// <summary>
    /// Single key press.
    /// </summary>
    public sealed class KeyPress : IKeyPress, IComparable, IComparable<IKeyPress>, IEquatable<IKeyPress>
    {
        public Key Key { get; }
        public ModifierKeys ModifierKeys { get; }

        /// <summary>
        /// Initializes a new key press with no modifier keys.
        /// </summary>
        /// <param name="key"></param>
        public KeyPress(Key key) : this(key, ModifierKeys.None) { }

        /// <summary>
        /// Initializes a new key press.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifierKeys"></param>
        public KeyPress(Key key, ModifierKeys modifierKeys)
        {
            this.Key = key;
            this.ModifierKeys = modifierKeys;
        }

        /// <summary>
        /// Checks whether the parameter keypress can be used to trigger this instance.
        /// </summary>
        /// <param name="other">User keypress</param>
        public bool IsMatch(IKeyPress other)
        {
            return other is KeyPress keyPress &&
                Key == keyPress.Key &&
                ModifierKeys == keyPress.ModifierKeys;
        }

        public override bool Equals(object obj)
        {
            if (obj is IKeyPress kp)
                return Equals(kp);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 2115199386;
            hashCode = hashCode * -1521134295 + Key.GetHashCode();
            hashCode = hashCode * -1521134295 + ModifierKeys.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return ModifierKeys == ModifierKeys.None
                ? $"[{Key}]"
                : $"[{ModifierKeys} + {Key}]";
        }

        public static bool Equal(KeyPress first, KeyPress second)
        {
            return (first?.GetHashCode() ?? 0) == (second?.GetHashCode() ?? 0);
        }

        public bool Equals(IKeyPress other)
        {
            return other is KeyPress keyPress && GetHashCode() == keyPress.GetHashCode();
        }

        public int CompareTo(IKeyPress other)
        {
            if (other is KeyChord keyChord)
            {
                return -keyChord.CompareTo(this);
            }
            if (other is KeyPress keyPress)
            {
                var modifiers = ModifierKeys.CompareTo(keyPress.ModifierKeys);
                return (modifiers == 0) ? Key.CompareTo(keyPress.Key) : modifiers;
            }

            return -1; // Unknown type
        }

        public int CompareTo(object obj)
        {
            if (obj is IKeyPress kp) return CompareTo(kp);
            throw new InvalidOperationException($"Cannot compare to unknown type {obj?.GetType()}");
        }
    }
}
