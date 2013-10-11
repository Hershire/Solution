namespace Server
{
    partial class ConfigForm
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
            this.ConfigTabs = new System.Windows.Forms.TabControl();
            this.SetupPage = new System.Windows.Forms.TabPage();
            this.ConfigTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConfigTabs
            // 
            this.ConfigTabs.Controls.Add(this.SetupPage);
            this.ConfigTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConfigTabs.Location = new System.Drawing.Point(0, 0);
            this.ConfigTabs.Name = "ConfigTabs";
            this.ConfigTabs.SelectedIndex = 0;
            this.ConfigTabs.Size = new System.Drawing.Size(548, 423);
            this.ConfigTabs.TabIndex = 0;
            // 
            // SetupPage
            // 
            this.SetupPage.Location = new System.Drawing.Point(4, 22);
            this.SetupPage.Name = "SetupPage";
            this.SetupPage.Padding = new System.Windows.Forms.Padding(3);
            this.SetupPage.Size = new System.Drawing.Size(540, 397);
            this.SetupPage.TabIndex = 0;
            this.SetupPage.Text = "Setup";
            this.SetupPage.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 423);
            this.Controls.Add(this.ConfigTabs);
            this.Name = "ConfigForm";
            this.Text = "Config Form";
            this.ConfigTabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl ConfigTabs;
        private System.Windows.Forms.TabPage SetupPage;

    }
}