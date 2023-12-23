namespace BananaHackV2.UI.Components
{
    partial class ShiftOverview
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbtn_Remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tstb_Month = new System.Windows.Forms.ToolStripTextBox();
            this.panel_SelectionContainer = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_EntryContainer = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.monthPanel1 = new BananaHackV2.UI.Components.ShiftMonthControl();
            this.toolStrip1.SuspendLayout();
            this.panel_SelectionContainer.SuspendLayout();
            this.panel_EntryContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtn_Remove,
            this.toolStripSeparator1,
            this.tstb_Month});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1080, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbtn_Remove
            // 
            this.tsbtn_Remove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtn_Remove.Image = global::BananaHackV2.Properties.Resources.StatusOffline_16x;
            this.tsbtn_Remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtn_Remove.Name = "tsbtn_Remove";
            this.tsbtn_Remove.Size = new System.Drawing.Size(23, 22);
            this.tsbtn_Remove.Text = "Remove";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tstb_Month
            // 
            this.tstb_Month.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tstb_Month.Name = "tstb_Month";
            this.tstb_Month.Size = new System.Drawing.Size(100, 25);
            // 
            // panel_SelectionContainer
            // 
            this.panel_SelectionContainer.Controls.Add(this.comboBox1);
            this.panel_SelectionContainer.Controls.Add(this.label1);
            this.panel_SelectionContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_SelectionContainer.Location = new System.Drawing.Point(0, 25);
            this.panel_SelectionContainer.Name = "panel_SelectionContainer";
            this.panel_SelectionContainer.Size = new System.Drawing.Size(301, 234);
            this.panel_SelectionContainer.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(0, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(301, 21);
            this.comboBox1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // panel_EntryContainer
            // 
            this.panel_EntryContainer.Controls.Add(this.button2);
            this.panel_EntryContainer.Controls.Add(this.button1);
            this.panel_EntryContainer.Controls.Add(this.monthPanel1);
            this.panel_EntryContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_EntryContainer.Location = new System.Drawing.Point(301, 25);
            this.panel_EntryContainer.Name = "panel_EntryContainer";
            this.panel_EntryContainer.Size = new System.Drawing.Size(779, 234);
            this.panel_EntryContainer.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(410, 63);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(410, 92);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // monthPanel1
            // 
            this.monthPanel1.AllowOutOfMonth = false;
            this.monthPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.monthPanel1.BorderStyle = BananaHackV2.UI.Components.ShiftMonthControl.DayBorder.Horizontal;
            this.monthPanel1.BorderThickness = 1;
            this.monthPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monthPanel1.HighlightBackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.monthPanel1.HighlightFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.monthPanel1.HighlightForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.monthPanel1.Location = new System.Drawing.Point(0, 0);
            this.monthPanel1.MaximumSize = new System.Drawing.Size(301, 234);
            this.monthPanel1.Month = 12;
            this.monthPanel1.Name = "monthPanel1";
            this.monthPanel1.OutOfMonthBackColor = System.Drawing.SystemColors.Control;
            this.monthPanel1.OutOfMonthFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthPanel1.OutOfMonthForeColor = System.Drawing.SystemColors.GrayText;
            this.monthPanel1.SelectedDayBackColor = System.Drawing.SystemColors.Highlight;
            this.monthPanel1.SelectedDayForeColor = System.Drawing.SystemColors.HighlightText;
            this.monthPanel1.Size = new System.Drawing.Size(301, 234);
            this.monthPanel1.TabIndex = 0;
            this.monthPanel1.Text = "monthPanel1";
            this.monthPanel1.Year = 2023;
            // 
            // ShiftOverview
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.panel_EntryContainer);
            this.Controls.Add(this.panel_SelectionContainer);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ShiftOverview";
            this.Size = new System.Drawing.Size(1080, 259);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel_SelectionContainer.ResumeLayout(false);
            this.panel_SelectionContainer.PerformLayout();
            this.panel_EntryContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbtn_Remove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox tstb_Month;
        private System.Windows.Forms.Panel panel_SelectionContainer;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel_EntryContainer;
        private ShiftMonthControl monthPanel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}
