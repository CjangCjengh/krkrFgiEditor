using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;

namespace krkrFgiEditor
{
    public partial class ComboBoxEx : ComboBox
    {
        private static readonly object EVENT_HOVEREDINDEXCHANGED = new object();

        private int hoveredIndex = -1;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HoveredIndex
        {
            get
            {
                return hoveredIndex;
            }
            private set
            {
                if (HoveredIndex != value)
                {
                    hoveredIndex = value;
                    OnHoveredIndexChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler HoveredIndexChanged
        {
            add
            {
                Events.AddHandler(EVENT_HOVEREDINDEXCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_HOVEREDINDEXCHANGED, value);
            }
        }

        public ComboBoxEx() { }

        protected virtual void OnHoveredIndexChanged(EventArgs e)
        {
            ((EventHandler)Events[EVENT_HOVEREDINDEXCHANGED])?.Invoke(this, e);
        }
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case 327:
                    HoveredIndex = m.Result.ToInt32();
                    break;
                case 8465:
                    if (m.WParam.ToInt32() >> 16 == 8)
                        HoveredIndex = -1;
                    break;
            }
        }
    }
}
