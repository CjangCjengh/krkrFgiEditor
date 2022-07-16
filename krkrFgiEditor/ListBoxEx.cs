using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Permissions;

namespace krkrFgiEditor
{
    public partial class ListBoxEx : ListBox
    {
        private static readonly object EVENT_ITEMSCHANGED = new object();

        public event EventHandler ItemsChanged
        {
            add
            {
                Events.AddHandler(EVENT_ITEMSCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_ITEMSCHANGED, value);
            }
        }

        public ListBoxEx() { }

        protected virtual void OnItemsChanged(EventArgs e)
        {
            ((EventHandler)Events[EVENT_ITEMSCHANGED])?.Invoke(this, e);
        }
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case 384:
                case 385:
                case 386:
                case 388:
                    OnItemsChanged(EventArgs.Empty);
                    break;
            }
        }
    }
}
