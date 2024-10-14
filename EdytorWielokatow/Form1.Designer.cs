namespace EdytorWielokatow
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Canvas = new PictureBox();
            vertexContextMenu = new ContextMenuStrip(components);
            usunToolStripMenuItem = new ToolStripMenuItem();
            edgeContextMenu = new ContextMenuStrip(components);
            podpodzielToolStripMenuItem = new ToolStripMenuItem();
            pionowaToolStripMenuItem = new ToolStripMenuItem();
            poziomaToolStripMenuItem = new ToolStripMenuItem();
            stalaDlugoscToolStripMenuItem = new ToolStripMenuItem();
            bezToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            vertexContextMenu.SuspendLayout();
            edgeContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // Canvas
            // 
            Canvas.BackColor = SystemColors.Control;
            Canvas.Dock = DockStyle.Fill;
            Canvas.Location = new Point(0, 0);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(1382, 753);
            Canvas.TabIndex = 0;
            Canvas.TabStop = false;
            Canvas.MouseDown += Canvas_MouseDown;
            Canvas.MouseMove += Canvas_MouseMove;
            Canvas.MouseUp += Canvas_MouseUp;
            // 
            // vertexContextMenu
            // 
            vertexContextMenu.ImageScalingSize = new Size(20, 20);
            vertexContextMenu.Items.AddRange(new ToolStripItem[] { usunToolStripMenuItem });
            vertexContextMenu.Name = "contextMenuStrip1";
            vertexContextMenu.Size = new Size(111, 28);
            // 
            // usunToolStripMenuItem
            // 
            usunToolStripMenuItem.Name = "usunToolStripMenuItem";
            usunToolStripMenuItem.Size = new Size(110, 24);
            usunToolStripMenuItem.Text = "Usuń";
            usunToolStripMenuItem.Click += usunToolStripMenuItem_Click;
            // 
            // edgeContextMenu
            // 
            edgeContextMenu.ImageScalingSize = new Size(20, 20);
            edgeContextMenu.Items.AddRange(new ToolStripItem[] { podpodzielToolStripMenuItem, pionowaToolStripMenuItem, poziomaToolStripMenuItem, stalaDlugoscToolStripMenuItem, bezToolStripMenuItem });
            edgeContextMenu.Name = "edgeContextMenu";
            edgeContextMenu.Size = new Size(168, 124);
            // 
            // podpodzielToolStripMenuItem
            // 
            podpodzielToolStripMenuItem.Name = "podpodzielToolStripMenuItem";
            podpodzielToolStripMenuItem.Size = new Size(167, 24);
            podpodzielToolStripMenuItem.Text = "Podpodziel";
            podpodzielToolStripMenuItem.Click += podpodzielToolStripMenuItem_Click;
            // 
            // pionowaToolStripMenuItem
            // 
            pionowaToolStripMenuItem.Name = "pionowaToolStripMenuItem";
            pionowaToolStripMenuItem.Size = new Size(167, 24);
            pionowaToolStripMenuItem.Text = "Pionowa";
            pionowaToolStripMenuItem.Click += pionowaToolStripMenuItem_Click;
            // 
            // poziomaToolStripMenuItem
            // 
            poziomaToolStripMenuItem.Name = "poziomaToolStripMenuItem";
            poziomaToolStripMenuItem.Size = new Size(167, 24);
            poziomaToolStripMenuItem.Text = "Pozioma";
            poziomaToolStripMenuItem.Click += poziomaToolStripMenuItem_Click;
            // 
            // stalaDlugoscToolStripMenuItem
            // 
            stalaDlugoscToolStripMenuItem.Name = "stalaDlugoscToolStripMenuItem";
            stalaDlugoscToolStripMenuItem.Size = new Size(167, 24);
            stalaDlugoscToolStripMenuItem.Text = "Stała długość";
            stalaDlugoscToolStripMenuItem.Click += stalaDlugoscToolStripMenuItem_Click;
            // 
            // bezToolStripMenuItem
            // 
            bezToolStripMenuItem.Name = "bezToolStripMenuItem";
            bezToolStripMenuItem.Size = new Size(167, 24);
            bezToolStripMenuItem.Text = "Bezier";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1382, 753);
            Controls.Add(Canvas);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Edytor wielokątów";
            ((System.ComponentModel.ISupportInitialize)Canvas).EndInit();
            vertexContextMenu.ResumeLayout(false);
            edgeContextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox Canvas;
        private ContextMenuStrip vertexContextMenu;
        private ToolStripMenuItem usunToolStripMenuItem;
        private ContextMenuStrip edgeContextMenu;
        private ToolStripMenuItem podpodzielToolStripMenuItem;
        private ToolStripMenuItem pionowaToolStripMenuItem;
        private ToolStripMenuItem poziomaToolStripMenuItem;
        private ToolStripMenuItem stalaDlugoscToolStripMenuItem;
        private ToolStripMenuItem bezToolStripMenuItem;
    }
}
