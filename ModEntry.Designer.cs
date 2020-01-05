namespace LegacyWorkshopFixer {
  partial class ModEntry {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModEntry));
      this._modIcon = new System.Windows.Forms.PictureBox();
      this._modButton = new System.Windows.Forms.Button();
      this._modName = new System.Windows.Forms.Label();
      this.LayoutTable = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this._modIcon)).BeginInit();
      this.LayoutTable.SuspendLayout();
      this.SuspendLayout();
      // 
      // _modIcon
      // 
      this._modIcon.Location = new System.Drawing.Point(3, 3);
      this._modIcon.Name = "_modIcon";
      this._modIcon.Size = new System.Drawing.Size(50, 50);
      this._modIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this._modIcon.TabIndex = 0;
      this._modIcon.TabStop = false;
      // 
      // _modButton
      // 
      this._modButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this._modButton.Image = ((System.Drawing.Image)(resources.GetObject("_modButton.Image")));
      this._modButton.Location = new System.Drawing.Point(273, 16);
      this._modButton.Name = "_modButton";
      this._modButton.Size = new System.Drawing.Size(24, 24);
      this._modButton.TabIndex = 2;
      this._modButton.UseVisualStyleBackColor = true;
      this._modButton.Click += new System.EventHandler(this._modButton_Click);
      // 
      // _modName
      // 
      this._modName.AutoSize = true;
      this._modName.Dock = System.Windows.Forms.DockStyle.Fill;
      this._modName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._modName.Location = new System.Drawing.Point(59, 0);
      this._modName.Name = "_modName";
      this._modName.Size = new System.Drawing.Size(208, 56);
      this._modName.TabIndex = 1;
      this._modName.Text = "Unknown Mod";
      this._modName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // LayoutTable
      // 
      this.LayoutTable.ColumnCount = 3;
      this.LayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.LayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.LayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.LayoutTable.Controls.Add(this._modIcon, 0, 0);
      this.LayoutTable.Controls.Add(this._modName, 1, 0);
      this.LayoutTable.Controls.Add(this._modButton, 2, 0);
      this.LayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
      this.LayoutTable.Location = new System.Drawing.Point(0, 0);
      this.LayoutTable.Margin = new System.Windows.Forms.Padding(0);
      this.LayoutTable.Name = "LayoutTable";
      this.LayoutTable.RowCount = 1;
      this.LayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.LayoutTable.Size = new System.Drawing.Size(300, 56);
      this.LayoutTable.TabIndex = 3;
      // 
      // ModEntry
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.LayoutTable);
      this.Name = "ModEntry";
      this.Size = new System.Drawing.Size(300, 56);
      this.SizeChanged += new System.EventHandler(this.ModEntry_SizeChanged);
      ((System.ComponentModel.ISupportInitialize)(this._modIcon)).EndInit();
      this.LayoutTable.ResumeLayout(false);
      this.LayoutTable.PerformLayout();
      this.ResumeLayout(false);

    }

        #endregion

        private System.Windows.Forms.PictureBox _modIcon;
        private System.Windows.Forms.Button _modButton;
        private System.Windows.Forms.Label _modName;
        private System.Windows.Forms.TableLayoutPanel LayoutTable;
  }
}
