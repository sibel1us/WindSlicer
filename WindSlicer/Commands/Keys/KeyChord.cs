using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace WindSlicer.Commands.Keys
{
    public sealed class KeyChord : IKeyPress, IComparable, IComparable<IKeyPress>, IEquatable<IKeyPress>
    {
        public KeyPress First { get; }
        public KeyPress Second { get; }

        /// <summary>
        /// Initializes a two key chord with no modifier keys.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public KeyChord(Key first, Key second)
            : this(new KeyPress(first), new KeyPress(second)) { }

        /// <summary>
        /// Initializes a two key chord with a shared modifier key.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="commonModifier">Common modifier to use for both parts of the chord</param>
        public KeyChord(Key first, Key second, ModifierKeys commonModifier)
            : this(new KeyPress(first, commonModifier), new KeyPress(second, commonModifier)) { }

        public KeyChord(KeyPress root, KeyPress second)
        {
            this.First = root;
            this.Second = second;
        }

        /// <summary>
        /// Checks whether the parameter keypress can be used to trigger this instance.
        /// </summary>
        /// <param name="other">User keypress</param>
        public bool IsMatch(IKeyPress other)
        {
            if (other is KeyPress keyPress)
                return keyPress.Equals(First);

            return
                other is KeyChord chord &&
                First.Equals(chord.First) &&
                Second.Equals(chord.Second);
        }

        public override bool Equals(object obj)
        {
            if (obj is IKeyPress kp)
                return Equals(kp);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = -545715949;
            hashCode = hashCode * -1521134295 + EqualityComparer<KeyPress>.Default.GetHashCode(First);
            hashCode = hashCode * -1521134295 + EqualityComparer<KeyPress>.Default.GetHashCode(Second);
            return hashCode;
        }

        public override string ToString() => $"{First}, {Second}";

        public bool Equals(IKeyPress other)
        {
            return other is KeyChord keyChord && GetHashCode() == keyChord.GetHashCode();
        }

        public int CompareTo(IKeyPress other)
        {
            if (other is KeyChord keyChord)
            {
                var first = First.CompareTo(keyChord.First);
                return (first == 0) ? Second.CompareTo(keyChord.Second) : first;
            }

            return First.CompareTo(other as KeyPress);
        }

        public int CompareTo(object obj)
        {
            if (obj is IKeyPress kp) return CompareTo(kp);
            throw new InvalidOperationException($"Cannot compare to unknown type {obj?.GetType()}");
        }
    }
}
