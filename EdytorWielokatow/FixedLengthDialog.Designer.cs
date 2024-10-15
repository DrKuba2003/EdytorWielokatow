namespace EdytorWielokatow
{
    partial class FixedLengthDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            okBtn = new Button();
            lengthTxb = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // okBtn
            // 
            okBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            okBtn.Location = new Point(92, 161);
            okBtn.Name = "okBtn";
            okBtn.Size = new Size(100, 30);
            okBtn.TabIndex = 0;
            okBtn.Text = "OK";
            okBtn.UseVisualStyleBackColor = true;
            // 
            // lengthTxb
            // 
            lengthTxb.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lengthTxb.Location = new Point(55, 81);
            lengthTxb.Name = "lengthTxb";
            lengthTxb.Size = new Size(168, 27);
            lengthTxb.TabIndex = 1;
            lengthTxb.KeyPress += lengthTxb_KeyPress;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(55, 58);
            label1.Name = "label1";
            label1.Size = new Size(66, 20);
            label1.TabIndex = 2;
            label1.Text = "Długość:";
            // 
            // FixedLengthDialog
            // 
            AcceptButton = okBtn;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = okBtn;
            ClientSize = new Size(282, 203);
            Controls.Add(label1);
            Controls.Add(lengthTxb);
            Controls.Add(okBtn);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FixedLengthDialog";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FixedLengthDialog";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button okBtn;
        private TextBox lengthTxb;
        private Label label1;
    }
}