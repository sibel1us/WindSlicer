using System;
using System.Drawing;
using WindSlicer.Commands;

namespace WindSlicer.Services
{
    public interface IIconService : IDisposable
    {
        Icon Default { get; }
        Icon GetIcon(BaseCommand command);
    }
}
