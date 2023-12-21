namespace BananaHackV2.UI.Components
{
    partial class UploadButton
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
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btn_SelectFIle = new System.Windows.Forms.Button();
            this.lbl_CurrentOption = new System.Windows.Forms.Label();
            this.btn_Screenshot = new System.Windows.Forms.Button();
            this.tableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 3;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.55555F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.22222F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.22223F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayout.Controls.Add(this.btn_SelectFIle, 2, 0);
            this.tableLayout.Controls.Add(this.lbl_CurrentOption, 0, 0);
            this.tableLayout.Controls.Add(this.btn_Screenshot, 1, 0);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 1;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.Size = new System.Drawing.Size(282, 36);
            this.tableLayout.TabIndex = 0;
            this.tableLayout.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayout_Paint);
            // 
            // btn_SelectFIle
            // 
            this.btn_SelectFIle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SelectFIle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_SelectFIle.FlatAppearance.BorderSize = 0;
            this.btn_SelectFIle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_SelectFIle.Image = global::BananaHackV2.Properties.Resources.FindinFiles_16x;
            this.btn_SelectFIle.Location = new System.Drawing.Point(221, 3);
            this.btn_SelectFIle.Name = "btn_SelectFIle";
            this.btn_SelectFIle.Size = new System.Drawing.Size(58, 30);
            this.btn_SelectFIle.TabIndex = 9;
            this.btn_SelectFIle.Tag = "0";
            this.btn_SelectFIle.UseVisualStyleBackColor = true;
            // 
            // lbl_CurrentOption
            // 
            this.lbl_CurrentOption.AutoSize = true;
            this.lbl_CurrentOption.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_CurrentOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_CurrentOption.Location = new System.Drawing.Point(3, 0);
            this.lbl_CurrentOption.Name = "lbl_CurrentOption";
            this.lbl_CurrentOption.Size = new System.Drawing.Size(150, 36);
            this.lbl_CurrentOption.TabIndex = 0;
            this.lbl_CurrentOption.Text = "Select file";
            this.lbl_CurrentOption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_Screenshot
            // 
            this.btn_Screenshot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Screenshot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Screenshot.FlatAppearance.BorderSize = 0;
            this.btn_Screenshot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Screenshot.Image = global::BananaHackV2.Properties.Resources.Camera_16x;
            this.btn_Screenshot.Location = new System.Drawing.Point(159, 3);
            this.btn_Screenshot.Name = "btn_Screenshot";
            this.btn_Screenshot.Size = new System.Drawing.Size(56, 30);
            this.btn_Screenshot.TabIndex = 6;
            this.btn_Screenshot.Tag = "1";
            this.btn_Screenshot.UseVisualStyleBackColor = true;
            // 
            // UploadButton
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UploadButton";
            this.Size = new System.Drawing.Size(282, 36);
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Label lbl_CurrentOption;
        private System.Windows.Forms.Button btn_Screenshot;
        private System.Windows.Forms.Button btn_SelectFIle;
    }
}
