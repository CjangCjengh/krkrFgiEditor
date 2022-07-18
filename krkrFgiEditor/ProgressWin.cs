using System.Threading;
using System.Windows.Forms;

namespace krkrFgiEditor
{
    public partial class ProgressWin : Form
    {
        public ProgressWin(int Maximum)
        {
            InitializeComponent();
            progressBar.Maximum = Maximum;
            cts = new CancellationTokenSource();
        }

        public CancellationTokenSource cts;

        public void AddProgress()
        {
            progressBar.Value++;
        }

        private void ProgressWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            cts.Cancel();
        }
    }
}
