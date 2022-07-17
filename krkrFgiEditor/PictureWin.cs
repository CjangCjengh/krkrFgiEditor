using System.Drawing;
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
