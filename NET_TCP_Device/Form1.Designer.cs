namespace NET_TCP_Device
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPictureAsJPGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultTableView = new NET_TCP_Device.ResultTableViewClass();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontToolStripMenuItem,
            this.exportPictureAsJPGToolStripMenuItem,
            this.exportPictureToolStripMenuItem,
            this.toolStripMenuItem1,
            this.fullScreenToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(281, 106);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.fontToolStripMenuItem.Text = "Font";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // exportPictureAsJPGToolStripMenuItem
            // 
            this.exportPictureAsJPGToolStripMenuItem.Name = "exportPictureAsJPGToolStripMenuItem";
            this.exportPictureAsJPGToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.exportPictureAsJPGToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.exportPictureAsJPGToolStripMenuItem.Text = "Export Picture As";
            this.exportPictureAsJPGToolStripMenuItem.Click += new System.EventHandler(this.exportPictureAsJPGToolStripMenuItem_Click);
            // 
            // exportPictureToolStripMenuItem
            // 
            this.exportPictureToolStripMenuItem.Name = "exportPictureToolStripMenuItem";
            this.exportPictureToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.exportPictureToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.exportPictureToolStripMenuItem.Text = "Export Picture";
            this.exportPictureToolStripMenuItem.Click += new System.EventHandler(this.exportPictureToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(277, 6);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.CheckOnClick = true;
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Space)));
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(280, 24);
            this.fullScreenToolStripMenuItem.Text = "Full Screen";
            this.fullScreenToolStripMenuItem.CheckedChanged += new System.EventHandler(this.fullScreenToolStripMenuItem_CheckedChanged);
            // 
            // resultTableView
            // 
            this.resultTableView.AllTextFont = new System.Drawing.Font("Times New Roman", 16F);
            this.resultTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultTableView.Location = new System.Drawing.Point(0, 0);
            this.resultTableView.Name = "resultTableView";
            this.resultTableView.Size = new System.Drawing.Size(691, 443);
            this.resultTableView.TabIndex = 0;
            this.resultTableView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.resultTableView_MouseDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 443);
            this.Controls.Add(this.resultTableView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "QUIZ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ResultTableViewClass resultTableView;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportPictureAsJPGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fullScreenToolStripMenuItem;




    }
}

