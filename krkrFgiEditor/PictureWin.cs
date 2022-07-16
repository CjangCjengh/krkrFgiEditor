using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace krkrFgiEditor
{
    public partial class PictureWin : Form
    {
        public PictureWin(Image image)
        {
            InitializeComponent();
            fgiBox.Image = Program.ResizeImage(image, fgiBox.Width, fgiBox.Height);
        }
    }
}
