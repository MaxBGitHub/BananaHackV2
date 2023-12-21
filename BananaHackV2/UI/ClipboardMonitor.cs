using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;

namespace BananaHackV2.UI
{
    [DefaultEvent("ClipboardChanged")]
    internal class ClipboardMonitor : Control
    {        
        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardViewer(
            IntPtr hWndNewViewer
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ChangeClipboardChain(
            IntPtr hWndRemove, 
            IntPtr hWndNewNext
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(
            IntPtr hWnd, 
            int wMsg, 
            IntPtr wParam, 
            IntPtr lParam
        );


        private const int WM_DRAWCLIPBOARD = 0x0308;
        private const int WM_CHANGECBCHAIN = 0x030d;

        private const int CLIPTIMEOUT = 200;

        private IntPtr _nextClipViewer;
        private DateTime _lastClipTime;


        private event EventHandler<ClipboardChangeEventArgs> onClipboardChanged;
        public event EventHandler<ClipboardChangeEventArgs> ClipboardChanged
        {
            add {
                onClipboardChanged += value;
            }
            remove {
                onClipboardChanged -= value;
            }
        }

        protected virtual void OnClipboardChanged()
        {
            try {                
                IDataObject dataObject = Clipboard.GetDataObject();
                onClipboardChanged?.Invoke(this, new ClipboardChangeEventArgs(dataObject));                
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        DateTime currentTicks = DateTime.Now;
                        long diff = (int)Math.Ceiling((currentTicks - _lastClipTime).TotalMilliseconds);
                        _lastClipTime = DateTime.Now;
                        if (diff >= CLIPTIMEOUT) {
                            OnClipboardChanged();
                        }
                        SendMessage(_nextClipViewer, m.Msg, m.WParam, m.LParam);                        
                        break;
                    }
                case WM_CHANGECBCHAIN:
                    {
                        if (m.WParam == _nextClipViewer) {
                            _nextClipViewer = m.LParam;
                        }
                        else {
                            SendMessage(_nextClipViewer, m.Msg, m.WParam, m.LParam);
                        }
                        break;
                    }
                default:
                    base.WndProc(ref m);
                    break;
            }
        }


        protected override void Dispose(bool disposing)
        {
            ChangeClipboardChain(this.Handle, _nextClipViewer);
            base.Dispose(disposing);
        }


        public ClipboardMonitor()
        {
            this.Visible = false;
            this.Size = new System.Drawing.Size(16, 16);
            this.BackColor = System.Drawing.Color.Red;
            _nextClipViewer = SetClipboardViewer(this.Handle);
        }
    }
}
