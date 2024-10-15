using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdytorWielokatow
{
    public partial class FixedLengthDialog : Form
    {
        public FixedLengthDialog()
        {
            InitializeComponent();
        }

        public double Show(double length)
        {
            lengthTxb.Text = length.ToString();
            
            ShowDialog();

            return Double.Parse(lengthTxb.Text);
        }

        private void lengthTxb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
                e.Handled = true;
        }
    }
}
