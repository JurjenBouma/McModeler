namespace MCModeler
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialogModel = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.translateAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shapeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uvmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialogModel = new System.Windows.Forms.SaveFileDialog();
            this.panelRender = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.panelTop = new System.Windows.Forms.Panel();
            this.treeViewModel = new MCModeler.CustomTreeView();
            this.menuStrip1.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialogModel
            // 
            this.openFileDialogModel.Filter = "JSON|*.json";
            this.openFileDialogModel.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogModel_FileOk);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.translateAxisToolStripMenuItem,
            this.shapeToolStripMenuItem,
            this.rotateToolStripMenuItem,
            this.scaleToolStripMenuItem,
            this.addNewBlockToolStripMenuItem,
            this.removeBlockToolStripMenuItem,
            this.copyElementToolStripMenuItem,
            this.uvmapToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(1264, 84);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.AutoSize = false;
            this.fileToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.fileToolStripMenuItem.Image = global::MCModeler.Properties.Resources.FilesPileModernDetail83x801;
            this.fileToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(94, 80);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.AutoSize = false;
            this.newToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.newToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.newToolStripMenuItem.Image = global::MCModeler.Properties.Resources.NewFileModernDetail49x60;
            this.newToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(184, 66);
            this.newToolStripMenuItem.Text = "New...";
            this.newToolStripMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.openToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.openToolStripMenuItem.Image = global::MCModeler.Properties.Resources.OpenBluePrintModernDetail44x60;
            this.openToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(153, 66);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.saveToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.saveToolStripMenuItem.Image = global::MCModeler.Properties.Resources.SaveScrollsFolderModernDetail57x60;
            this.saveToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(153, 66);
            this.saveToolStripMenuItem.Text = "Save...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Image = global::MCModeler.Properties.Resources.Select57x80;
            this.selectToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(69, 80);
            // 
            // translateAxisToolStripMenuItem
            // 
            this.translateAxisToolStripMenuItem.Image = global::MCModeler.Properties.Resources.AxisMoveBrown92x80;
            this.translateAxisToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.translateAxisToolStripMenuItem.Name = "translateAxisToolStripMenuItem";
            this.translateAxisToolStripMenuItem.Size = new System.Drawing.Size(104, 80);
            this.translateAxisToolStripMenuItem.Click += new System.EventHandler(this.translateAxisToolStripMenuItem_Click);
            // 
            // shapeToolStripMenuItem
            // 
            this.shapeToolStripMenuItem.Image = global::MCModeler.Properties.Resources.Shape73x80;
            this.shapeToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.shapeToolStripMenuItem.Name = "shapeToolStripMenuItem";
            this.shapeToolStripMenuItem.Size = new System.Drawing.Size(85, 80);
            this.shapeToolStripMenuItem.Click += new System.EventHandler(this.shapeToolStripMenuItem_Click);
            // 
            // rotateToolStripMenuItem
            // 
            this.rotateToolStripMenuItem.Image = global::MCModeler.Properties.Resources.Rotate82x80;
            this.rotateToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
            this.rotateToolStripMenuItem.Size = new System.Drawing.Size(94, 80);
            // 
            // scaleToolStripMenuItem
            // 
            this.scaleToolStripMenuItem.Image = global::MCModeler.Properties.Resources.Scale2_78x80;
            this.scaleToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.scaleToolStripMenuItem.Name = "scaleToolStripMenuItem";
            this.scaleToolStripMenuItem.Size = new System.Drawing.Size(90, 80);
            // 
            // addNewBlockToolStripMenuItem
            // 
            this.addNewBlockToolStripMenuItem.Image = global::MCModeler.Properties.Resources.NewElementGrass97x80;
            this.addNewBlockToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.addNewBlockToolStripMenuItem.Name = "addNewBlockToolStripMenuItem";
            this.addNewBlockToolStripMenuItem.Size = new System.Drawing.Size(109, 80);
            // 
            // removeBlockToolStripMenuItem
            // 
            this.removeBlockToolStripMenuItem.Image = global::MCModeler.Properties.Resources.RemoveElementGrass99x80Cut;
            this.removeBlockToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.removeBlockToolStripMenuItem.Name = "removeBlockToolStripMenuItem";
            this.removeBlockToolStripMenuItem.Size = new System.Drawing.Size(111, 80);
            // 
            // copyElementToolStripMenuItem
            // 
            this.copyElementToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyElementToolStripMenuItem.Image")));
            this.copyElementToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.copyElementToolStripMenuItem.Name = "copyElementToolStripMenuItem";
            this.copyElementToolStripMenuItem.Size = new System.Drawing.Size(134, 80);
            // 
            // uvmapToolStripMenuItem
            // 
            this.uvmapToolStripMenuItem.Image = global::MCModeler.Properties.Resources.TextureButtonRedPaper155x80;
            this.uvmapToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.uvmapToolStripMenuItem.Name = "uvmapToolStripMenuItem";
            this.uvmapToolStripMenuItem.Size = new System.Drawing.Size(167, 80);
            // 
            // saveFileDialogModel
            // 
            this.saveFileDialogModel.DefaultExt = "json";
            this.saveFileDialogModel.Filter = "JSON|*.json";
            this.saveFileDialogModel.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogModel_FileOk);
            // 
            // panelRender
            // 
            this.panelRender.AllowDrop = true;
            this.panelRender.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelRender.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRender.Location = new System.Drawing.Point(0, 84);
            this.panelRender.Name = "panelRender";
            this.panelRender.Size = new System.Drawing.Size(1062, 645);
            this.panelRender.TabIndex = 2;
            this.panelRender.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelRender_DragDrop);
            this.panelRender.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelRender_DragEnter);
            this.panelRender.Enter += new System.EventHandler(this.panelRender_Enter);
            this.panelRender.Leave += new System.EventHandler(this.panelRender_Leave);
            this.panelRender.MouseEnter += new System.EventHandler(this.panelRender_MouseEnter);
            this.panelRender.MouseLeave += new System.EventHandler(this.panelRender_MouseLeave);
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panelRight.Controls.Add(this.treeViewModel);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(1062, 84);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(202, 645);
            this.panelRight.TabIndex = 3;
            this.panelRight.MouseEnter += new System.EventHandler(this.panelLeft_MouseEnter);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "TreeViewIconBlack.png");
            this.imageList1.Images.SetKeyName(1, "TreeViewIconBlue.png");
            this.imageList1.Images.SetKeyName(2, "TreeViewIconGreen.png");
            this.imageList1.Images.SetKeyName(3, "TreeViewIconRed.png");
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.menuStrip1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1264, 84);
            this.panelTop.TabIndex = 4;
            // 
            // treeViewModel
            // 
            this.treeViewModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.treeViewModel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewModel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.treeViewModel.HideSelection = false;
            this.treeViewModel.Location = new System.Drawing.Point(0, 0);
            this.treeViewModel.Name = "treeViewModel";
            this.treeViewModel.Size = new System.Drawing.Size(202, 645);
            this.treeViewModel.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 729);
            this.Controls.Add(this.panelRender);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelTop);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Mc Modeler";
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.Move += new System.EventHandler(this.Form1_Move);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialogModel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialogModel;
        private System.Windows.Forms.Panel panelRight;
        public System.Windows.Forms.Panel panelRender;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private CustomTreeView treeViewModel;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem translateAxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shapeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeBlockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uvmapToolStripMenuItem;
    }
}

