namespace BananaHackV2.UI
{
    partial class WndMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WndMain));
            this.toolStrip_StatusBar = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip_WindowMenu = new System.Windows.Forms.ToolStrip();
            this.imageList_TabHeader = new System.Windows.Forms.ImageList(this.components);
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.tabPage_MainWindow_Settings = new System.Windows.Forms.TabPage();
            this.tabPage_MainWindow_Log = new System.Windows.Forms.TabPage();
            this.tabPage_MainWindow_ShiftProcessing = new System.Windows.Forms.TabPage();
            this.tabPage_MainWindow_ImageUpload = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.flowLayout_Images = new System.Windows.Forms.FlowLayoutPanel();
            this.tabctl_MainWindow = new System.Windows.Forms.TabControl();
            this.uploadButton_OcrImage = new BananaHackV2.UI.Components.UploadButton();
            this.shiftOverview1 = new BananaHackV2.UI.Components.ShiftOverview();
            this.toolStrip_StatusBar.SuspendLayout();
            this.tabPage_MainWindow_ShiftProcessing.SuspendLayout();
            this.tabPage_MainWindow_ImageUpload.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabctl_MainWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip_StatusBar
            // 
            this.toolStrip_StatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip_StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.toolStripProgressBar1,
            this.toolStripLabel2});
            this.toolStrip_StatusBar.Location = new System.Drawing.Point(0, 544);
            this.toolStrip_StatusBar.Name = "toolStrip_StatusBar";
            this.toolStrip_StatusBar.Size = new System.Drawing.Size(1014, 25);
            this.toolStrip_StatusBar.TabIndex = 0;
            this.toolStrip_StatusBar.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(86, 22);
            this.toolStripLabel1.Text = "toolStripLabel1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 22);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(86, 22);
            this.toolStripLabel2.Text = "toolStripLabel2";
            // 
            // toolStrip_WindowMenu
            // 
            this.toolStrip_WindowMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_WindowMenu.Name = "toolStrip_WindowMenu";
            this.toolStrip_WindowMenu.Size = new System.Drawing.Size(1014, 25);
            this.toolStrip_WindowMenu.TabIndex = 1;
            this.toolStrip_WindowMenu.Text = "toolStrip1";
            // 
            // imageList_TabHeader
            // 
            this.imageList_TabHeader.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_TabHeader.ImageStream")));
            this.imageList_TabHeader.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_TabHeader.Images.SetKeyName(0, "Timer_16x.png");
            this.imageList_TabHeader.Images.SetKeyName(1, "NewCatalog_16x.png");
            this.imageList_TabHeader.Images.SetKeyName(2, "Settings_16x.png");
            this.imageList_TabHeader.Images.SetKeyName(3, "Camera_16x.png");
            // 
            // tabPage_MainWindow_Settings
            // 
            this.tabPage_MainWindow_Settings.ImageIndex = 2;
            this.tabPage_MainWindow_Settings.Location = new System.Drawing.Point(4, 26);
            this.tabPage_MainWindow_Settings.Name = "tabPage_MainWindow_Settings";
            this.tabPage_MainWindow_Settings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MainWindow_Settings.Size = new System.Drawing.Size(1006, 489);
            this.tabPage_MainWindow_Settings.TabIndex = 2;
            this.tabPage_MainWindow_Settings.Text = "Einstellungen";
            this.tabPage_MainWindow_Settings.UseVisualStyleBackColor = true;
            // 
            // tabPage_MainWindow_Log
            // 
            this.tabPage_MainWindow_Log.ImageIndex = 1;
            this.tabPage_MainWindow_Log.Location = new System.Drawing.Point(4, 26);
            this.tabPage_MainWindow_Log.Name = "tabPage_MainWindow_Log";
            this.tabPage_MainWindow_Log.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MainWindow_Log.Size = new System.Drawing.Size(1006, 489);
            this.tabPage_MainWindow_Log.TabIndex = 1;
            this.tabPage_MainWindow_Log.Text = "Log";
            this.tabPage_MainWindow_Log.UseVisualStyleBackColor = true;
            // 
            // tabPage_MainWindow_ShiftProcessing
            // 
            this.tabPage_MainWindow_ShiftProcessing.Controls.Add(this.shiftOverview1);
            this.tabPage_MainWindow_ShiftProcessing.ImageIndex = 0;
            this.tabPage_MainWindow_ShiftProcessing.Location = new System.Drawing.Point(4, 26);
            this.tabPage_MainWindow_ShiftProcessing.Name = "tabPage_MainWindow_ShiftProcessing";
            this.tabPage_MainWindow_ShiftProcessing.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MainWindow_ShiftProcessing.Size = new System.Drawing.Size(1006, 489);
            this.tabPage_MainWindow_ShiftProcessing.TabIndex = 0;
            this.tabPage_MainWindow_ShiftProcessing.Text = "Schicht Erkennung";
            this.tabPage_MainWindow_ShiftProcessing.UseVisualStyleBackColor = true;
            // 
            // tabPage_MainWindow_ImageUpload
            // 
            this.tabPage_MainWindow_ImageUpload.Controls.Add(this.splitContainer1);
            this.tabPage_MainWindow_ImageUpload.ImageIndex = 3;
            this.tabPage_MainWindow_ImageUpload.Location = new System.Drawing.Point(4, 26);
            this.tabPage_MainWindow_ImageUpload.Name = "tabPage_MainWindow_ImageUpload";
            this.tabPage_MainWindow_ImageUpload.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MainWindow_ImageUpload.Size = new System.Drawing.Size(1006, 489);
            this.tabPage_MainWindow_ImageUpload.TabIndex = 3;
            this.tabPage_MainWindow_ImageUpload.Text = "Bilder hochladen";
            this.tabPage_MainWindow_ImageUpload.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.uploadButton_OcrImage);
            this.splitContainer1.Panel1.SizeChanged += new System.EventHandler(this.splitContainer1_Panel1_SizeChanged);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.flowLayout_Images);
            this.splitContainer1.Size = new System.Drawing.Size(1000, 483);
            this.splitContainer1.SplitterDistance = 164;
            this.splitContainer1.TabIndex = 0;
            // 
            // flowLayout_Images
            // 
            this.flowLayout_Images.AutoScroll = true;
            this.flowLayout_Images.BackColor = System.Drawing.Color.DarkGray;
            this.flowLayout_Images.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayout_Images.Location = new System.Drawing.Point(0, 0);
            this.flowLayout_Images.Name = "flowLayout_Images";
            this.flowLayout_Images.Size = new System.Drawing.Size(1000, 315);
            this.flowLayout_Images.TabIndex = 0;
            // 
            // tabctl_MainWindow
            // 
            this.tabctl_MainWindow.Controls.Add(this.tabPage_MainWindow_ImageUpload);
            this.tabctl_MainWindow.Controls.Add(this.tabPage_MainWindow_ShiftProcessing);
            this.tabctl_MainWindow.Controls.Add(this.tabPage_MainWindow_Log);
            this.tabctl_MainWindow.Controls.Add(this.tabPage_MainWindow_Settings);
            this.tabctl_MainWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabctl_MainWindow.ImageList = this.imageList_TabHeader;
            this.tabctl_MainWindow.Location = new System.Drawing.Point(0, 25);
            this.tabctl_MainWindow.Name = "tabctl_MainWindow";
            this.tabctl_MainWindow.SelectedIndex = 0;
            this.tabctl_MainWindow.Size = new System.Drawing.Size(1014, 519);
            this.tabctl_MainWindow.TabIndex = 0;
            // 
            // uploadButton_OcrImage
            // 
            this.uploadButton_OcrImage.BackColor = System.Drawing.SystemColors.Control;
            this.uploadButton_OcrImage.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uploadButton_OcrImage.Location = new System.Drawing.Point(381, 70);
            this.uploadButton_OcrImage.Name = "uploadButton_OcrImage";
            this.uploadButton_OcrImage.Size = new System.Drawing.Size(243, 42);
            this.uploadButton_OcrImage.TabIndex = 1;
            // 
            // shiftOverview1
            // 
            this.shiftOverview1.Location = new System.Drawing.Point(3, 6);
            this.shiftOverview1.Name = "shiftOverview1";
            this.shiftOverview1.Size = new System.Drawing.Size(995, 226);
            this.shiftOverview1.TabIndex = 0;
            // 
            // WndMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1014, 569);
            this.Controls.Add(this.tabctl_MainWindow);
            this.Controls.Add(this.toolStrip_WindowMenu);
            this.Controls.Add(this.toolStrip_StatusBar);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "WndMain";
            this.Text = "WndMain";
            this.toolStrip_StatusBar.ResumeLayout(false);
            this.toolStrip_StatusBar.PerformLayout();
            this.tabPage_MainWindow_ShiftProcessing.ResumeLayout(false);
            this.tabPage_MainWindow_ImageUpload.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabctl_MainWindow.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip_StatusBar;
        private System.Windows.Forms.ToolStrip toolStrip_WindowMenu;
        private System.Windows.Forms.ImageList imageList_TabHeader;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.TabPage tabPage_MainWindow_Settings;
        private System.Windows.Forms.TabPage tabPage_MainWindow_Log;
        private System.Windows.Forms.TabPage tabPage_MainWindow_ShiftProcessing;
        private System.Windows.Forms.TabPage tabPage_MainWindow_ImageUpload;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Components.UploadButton uploadButton_OcrImage;
        private System.Windows.Forms.FlowLayoutPanel flowLayout_Images;
        private System.Windows.Forms.TabControl tabctl_MainWindow;
        private Components.ShiftOverview shiftOverview1;
    }
}