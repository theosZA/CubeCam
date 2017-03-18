using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CubeCam
{
    public partial class ManualScrambleDialog : Form
    {
        public string[] Scrambles => Regex.Split(scrambleInput.Text, @"\r?\n|\r");

        public ManualScrambleDialog()
        {
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
