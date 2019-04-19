using System;

namespace WindSlicer.Commands.Keys
{
    /// <summary>
    /// Interface for composite key bindings.
    /// </summary>
    public interface IKeyPress : IComparable, IComparable<IKeyPress>, IEquatable<IKeyPress>
    {
        bool IsMatch(IKeyPress other);
    }
}
