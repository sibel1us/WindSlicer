﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindSlicer.Models;

namespace WindSlicer.Utilities.Extensions
{
    public static class GeneralExtensions
    {
        public static bool Has<TEnum>(this TEnum @this, TEnum flag)
            where TEnum : Enum
        {
            return @this.HasFlag(flag);
        }

        public static int Limit(this int @this, int min, int max)
        {
            return Math.Min(max, Math.Max(min, @this));
        }

        public static double Limit(this double @this, double min, double max)
        {
            return Math.Min(max, Math.Max(min, @this));
        }

        /// <summary>
        /// Returns the double rounded to the nearest integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this double @this)
        {
            return (int)(@this + 0.5);
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

        public static Rectangle GetTaskbar(this Screen @this)
        {
            if (@this == null)
                throw new NullReferenceException(nameof(@this));

            return Rectangle.Union(@this.Bounds, @this.WorkingArea);
        }

        public static AnchorStyles GetTaskbarLocation(this IScreen @this)
        {
            return InternalGetTaskbarLocation(@this.Bounds, @this.WorkingArea);
        }

        public static AnchorStyles GetTaskbarLocation(this Screen @this)
        {
            return InternalGetTaskbarLocation(@this.Bounds, @this.WorkingArea);
        }

        public static int GetTaskbarSize(this Screen @this)
        {
            var bounds = @this.Bounds;
            var workingArea = @this.WorkingArea;

            switch (GetTaskbarLocation(@this))
            {
                case AnchorStyles.Top:
                    return workingArea.Top - bounds.Top;
                case AnchorStyles.Bottom:
                    return bounds.Bottom - workingArea.Bottom;
                case AnchorStyles.Left:
                    return workingArea.Left - bounds.Left;
                case AnchorStyles.Right:
                    return bounds.Right - workingArea.Right;
                default:
                    return 0;
            }
        }

        private static AnchorStyles InternalGetTaskbarLocation(
            Rectangle bounds,
            Rectangle workingArea)
        {
            var location = AnchorStyles.None;

            if (workingArea.Height < bounds.Height)
            {
                if (workingArea.Top > bounds.Top)
                {
                    location |= AnchorStyles.Top;
                }
                if (bounds.Bottom > workingArea.Bottom)
                {
                    location |= AnchorStyles.Bottom;
                }
            }

            if (workingArea.Width < bounds.Width)
            {
                if (workingArea.Left > bounds.Left)
                {
                    location |= AnchorStyles.Left;
                }
                if (bounds.Right > workingArea.Right)
                {
                    location |= AnchorStyles.Right;
                }
            }

            return location;
        }
    }
}
