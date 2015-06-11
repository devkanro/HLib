using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLib.Controls.Primitives
{
    public interface IVirtualizingItem
    {
        bool IsVirtualized { get; }

        bool CanVirtualize { get; }

        bool CanRealize { get; }

        void Virtualize();

        void Realize();
    }
}
