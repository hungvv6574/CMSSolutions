using System;
using System.Windows.Forms;

namespace CMSSolutions.GTools.Common.Dialogs
{
    public partial class InputDialog : Form
    {
        public string LabelText
        {
            get { return lblInput.Text; }
            set { lblInput.Text = value; }
        }

        public string UserInput
        {
            get { return txtInput.Text; }
            set { txtInput.Text = value; }
        }

        public InputDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
