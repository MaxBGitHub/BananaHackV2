using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2.UI
{
    internal class ClipboardChangeEventArgs : EventArgs
    {
        public readonly System.Windows.Forms.IDataObject DataObject;
        public ClipboardChangeEventArgs(System.Windows.Forms.IDataObject dataObject)
        {
            this.DataObject = dataObject;
        }
    }
}
