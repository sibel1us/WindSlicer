using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WindSlicer.Utilities.Extensions
{
    public static class GeneralExtensions
    {
        /// <summary>
        /// Returns the double rounded to the nearest integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this double @this)
        {
            return (int)Math.Floor(@this + 0.5);
        }

        /// <summary>
        /// Returns whether the difference between the numbers is less than 0.001.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals3Digits(this double @this, double other)
        {
            return Math.Abs(@this - other) < 0.001;
        }

        /// <summary>
        /// Returns whether the difference between the numbers is less than 0.00001.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals5Digits(this double @this, double other)
        {
            return Math.Abs(@this - other) < 0.00001;
        }

        /// <summary>
        /// Invokes the event for the target of the member expression.
        /// </summary>
        public static void InvokeFor<T, TProp>(
            this PropertyChangedEventHandler handler,
            Expression<Func<T, TProp>> expression)
        {
            if (handler != null)
            {
                var memExp = expression.Body as MemberExpression
                    ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression
                    ?? throw new InvalidCastException("Cannot get property name");

                handler.Invoke(null, new PropertyChangedEventArgs(memExp.Member.Name));
            }
        }
    }
}
