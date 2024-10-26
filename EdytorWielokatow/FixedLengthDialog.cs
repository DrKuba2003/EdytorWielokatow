namespace EdytorWielokatow
{
    public partial class FixedLengthDialog : Form
    {
        public FixedLengthDialog()
        {
            InitializeComponent();
        }

        public int Show(double length)
        {
            lengthTxb.Text = ((int)length).ToString();

            ShowDialog();

            return int.Parse(lengthTxb.Text);
        }

        private void lengthTxb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
                e.Handled = true;
        }
    }
}
