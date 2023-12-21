namespace BananaHackV2.UI.Components
{
    partial class FlowImageContainer
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
            this.pb_ImageDisplay = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_ImageDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_ImageDisplay
            // 
            this.pb_ImageDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb_ImageDisplay.Location = new System.Drawing.Point(0, 0);
            this.pb_ImageDisplay.Name = "pb_ImageDisplay";
            this.pb_ImageDisplay.Size = new System.Drawing.Size(413, 211);
            this.pb_ImageDisplay.TabIndex = 0;
            this.pb_ImageDisplay.TabStop = false;
            // 
            // FlowImageContainer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.Controls.Add(this.pb_ImageDisplay);
            this.Name = "FlowImageContainer";
            this.Size = new System.Drawing.Size(413, 211);
            ((System.ComponentModel.ISupportInitialize)(this.pb_ImageDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_ImageDisplay;
    }
}
