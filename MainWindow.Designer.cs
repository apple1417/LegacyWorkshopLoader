namespace LegacyWorkshopFixer {
  partial class MainWindow {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.TalosLocation = new System.Windows.Forms.TextBox();
      this.TalosPicker = new System.Windows.Forms.OpenFileDialog();
      this.RefreshButton = new System.Windows.Forms.Button();
      this.OpenTalosButton = new System.Windows.Forms.Button();
      this.ModPanel = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // TalosLocation
      // 
      this.TalosLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TalosLocation.Location = new System.Drawing.Point(12, 12);
      this.TalosLocation.Name = "TalosLocation";
      this.TalosLocation.Size = new System.Drawing.Size(300, 20);
      this.TalosLocation.TabIndex = 0;
      this.TalosLocation.TabStop = false;
      this.TalosLocation.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TalosLocation_KeyPress);
      // 
      // TalosPicker
      // 
      this.TalosPicker.Filter = "Talos Executable|Talos*";
      // 
      // RefreshButton
      // 
      this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.RefreshButton.Image = global::LegacyWorkshopFixer.Properties.Resources.Refresh;
      this.RefreshButton.Location = new System.Drawing.Point(348, 10);
      this.RefreshButton.Name = "RefreshButton";
      this.RefreshButton.Size = new System.Drawing.Size(24, 24);
      this.RefreshButton.TabIndex = 2;
      this.RefreshButton.UseVisualStyleBackColor = true;
      this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
      // 
      // OpenTalosButton
      // 
      this.OpenTalosButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.OpenTalosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.OpenTalosButton.Image = global::LegacyWorkshopFixer.Properties.Resources.OpenFile;
      this.OpenTalosButton.Location = new System.Drawing.Point(318, 10);
      this.OpenTalosButton.Name = "OpenTalosButton";
      this.OpenTalosButton.Size = new System.Drawing.Size(24, 24);
      this.OpenTalosButton.TabIndex = 1;
      this.OpenTalosButton.UseVisualStyleBackColor = true;
      this.OpenTalosButton.Click += new System.EventHandler(this.OpenFolderButton_Click);
      // 
      // ModPanel
      // 
      this.ModPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ModPanel.AutoScroll = true;
      this.ModPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.ModPanel.Location = new System.Drawing.Point(12, 38);
      this.ModPanel.Name = "ModPanel";
      this.ModPanel.Size = new System.Drawing.Size(360, 411);
      this.ModPanel.TabIndex = 3;
      // 
      // MainWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(382, 461);
      this.Controls.Add(this.ModPanel);
      this.Controls.Add(this.RefreshButton);
      this.Controls.Add(this.OpenTalosButton);
      this.Controls.Add(this.TalosLocation);
      this.MinimumSize = new System.Drawing.Size(398, 300);
      this.Name = "MainWindow";
      this.Text = "Legacy Workshop Fixer";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

        #endregion

        private System.Windows.Forms.TextBox TalosLocation;
        private System.Windows.Forms.Button OpenTalosButton;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.OpenFileDialog TalosPicker;
        private System.Windows.Forms.Panel ModPanel;
    }
}

