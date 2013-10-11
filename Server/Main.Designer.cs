namespace Server
{
    partial class Main
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
            this.LogTabs = new System.Windows.Forms.TabControl();
            this.ConnectionTab = new System.Windows.Forms.TabPage();
            this.SystemLogTextBox = new System.Windows.Forms.TextBox();
            this.ExceptionTab = new System.Windows.Forms.TabPage();
            this.ExceptionLogTextBox = new System.Windows.Forms.TextBox();
            this.InfoPanel = new System.Windows.Forms.Panel();
            this.UserCountLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.InterfaceTimer = new System.Windows.Forms.Timer(this.components);
            this.LogTabs.SuspendLayout();
            this.ConnectionTab.SuspendLayout();
            this.ExceptionTab.SuspendLayout();
            this.InfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogTabs
            // 
            this.LogTabs.Controls.Add(this.ConnectionTab);
            this.LogTabs.Controls.Add(this.ExceptionTab);
            this.LogTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTabs.Location = new System.Drawing.Point(0, 78);
            this.LogTabs.Name = "LogTabs";
            this.LogTabs.SelectedIndex = 0;
            this.LogTabs.Size = new System.Drawing.Size(482, 359);
            this.LogTabs.TabIndex = 0;
            // 
            // ConnectionTab
            // 
            this.ConnectionTab.Controls.Add(this.SystemLogTextBox);
            this.ConnectionTab.Location = new System.Drawing.Point(4, 22);
            this.ConnectionTab.Name = "ConnectionTab";
            this.ConnectionTab.Padding = new System.Windows.Forms.Padding(3);
            this.ConnectionTab.Size = new System.Drawing.Size(474, 333);
            this.ConnectionTab.TabIndex = 0;
            this.ConnectionTab.Text = "System Logs";
            this.ConnectionTab.UseVisualStyleBackColor = true;
            // 
            // SystemLogTextBox
            // 
            this.SystemLogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SystemLogTextBox.Location = new System.Drawing.Point(3, 3);
            this.SystemLogTextBox.Multiline = true;
            this.SystemLogTextBox.Name = "SystemLogTextBox";
            this.SystemLogTextBox.ReadOnly = true;
            this.SystemLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SystemLogTextBox.Size = new System.Drawing.Size(468, 327);
            this.SystemLogTextBox.TabIndex = 1;
            // 
            // ExceptionTab
            // 
            this.ExceptionTab.Controls.Add(this.ExceptionLogTextBox);
            this.ExceptionTab.Location = new System.Drawing.Point(4, 22);
            this.ExceptionTab.Name = "ExceptionTab";
            this.ExceptionTab.Padding = new System.Windows.Forms.Padding(3);
            this.ExceptionTab.Size = new System.Drawing.Size(474, 333);
            this.ExceptionTab.TabIndex = 2;
            this.ExceptionTab.Text = "Exception Logs";
            this.ExceptionTab.UseVisualStyleBackColor = true;
            // 
            // ExceptionLogTextBox
            // 
            this.ExceptionLogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExceptionLogTextBox.Location = new System.Drawing.Point(3, 3);
            this.ExceptionLogTextBox.Multiline = true;
            this.ExceptionLogTextBox.Name = "ExceptionLogTextBox";
            this.ExceptionLogTextBox.ReadOnly = true;
            this.ExceptionLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ExceptionLogTextBox.Size = new System.Drawing.Size(468, 327);
            this.ExceptionLogTextBox.TabIndex = 1;
            // 
            // InfoPanel
            // 
            this.InfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InfoPanel.Controls.Add(this.UserCountLabel);
            this.InfoPanel.Controls.Add(this.label3);
            this.InfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.InfoPanel.Location = new System.Drawing.Point(0, 0);
            this.InfoPanel.Name = "InfoPanel";
            this.InfoPanel.Size = new System.Drawing.Size(482, 78);
            this.InfoPanel.TabIndex = 1;
            // 
            // UserCountLabel
            // 
            this.UserCountLabel.AutoSize = true;
            this.UserCountLabel.Location = new System.Drawing.Point(86, 8);
            this.UserCountLabel.Name = "UserCountLabel";
            this.UserCountLabel.Size = new System.Drawing.Size(13, 13);
            this.UserCountLabel.TabIndex = 13;
            this.UserCountLabel.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Connections:";
            // 
            // InterfaceTimer
            // 
            this.InterfaceTimer.Enabled = true;
            this.InterfaceTimer.Interval = 1;
            this.InterfaceTimer.Tick += new System.EventHandler(this.InterfaceTimer_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 437);
            this.Controls.Add(this.LogTabs);
            this.Controls.Add(this.InfoPanel);
            this.Name = "Main";
            this.Text = "Legend of Mir 2 Server";
            this.LogTabs.ResumeLayout(false);
            this.ConnectionTab.ResumeLayout(false);
            this.ConnectionTab.PerformLayout();
            this.ExceptionTab.ResumeLayout(false);
            this.ExceptionTab.PerformLayout();
            this.InfoPanel.ResumeLayout(false);
            this.InfoPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl LogTabs;
        private System.Windows.Forms.TabPage ConnectionTab;
        private System.Windows.Forms.TextBox SystemLogTextBox;
        private System.Windows.Forms.TabPage ExceptionTab;
        private System.Windows.Forms.TextBox ExceptionLogTextBox;
        private System.Windows.Forms.Panel InfoPanel;
        private System.Windows.Forms.Timer InterfaceTimer;
        private System.Windows.Forms.Label UserCountLabel;
        private System.Windows.Forms.Label label3;
    }
}

