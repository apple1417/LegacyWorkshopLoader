﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace LegacyWorkshopLoader {
  public partial class ModEntry : UserControl {
    public string ModGroPath;
    public string ModManualPath;

    public ModEntry() : this("Unknown Mod", null, null, null) { }
    public ModEntry(string ModName, string ModIconPath, string ModGroPath, string ModManualPath) {
      InitializeComponent();

      this.ModName = ModName;
      this.ModIconPath = ModIconPath;
      this.ModGroPath = ModGroPath;
      this.ModManualPath = ModManualPath;

      _modIcon.BackColor = Color.FromArgb(0, 255, 255, 255);
    }

    public string ModName {
      get {
        return _modName.Text;
      }
      set {
        _modName.Text = value;
      }
    }

    private static readonly Image EnabledMissing = Image.FromHbitmap(Properties.Resources.MissingIcon.GetHbitmap());
    private static readonly Image DisabledMissing = ConvertToDisabledIcon(EnabledMissing);

    private Image EnabledIcon;
    private Image DisabledIcon;

    private string _modIconPath;
    public string ModIconPath {
      get {
        return _modIconPath;
      }
      set {
        if (File.Exists(value)) {
          // Doing it like this releases the lock on the file
          using (Image imgTmp = Image.FromFile(value)) {
            EnabledIcon = new Bitmap(imgTmp);
          }
          DisabledIcon = ConvertToDisabledIcon(EnabledIcon);

          _modIconPath = value;
        } else {
          EnabledIcon = EnabledMissing;
          DisabledIcon = DisabledMissing;

          _modIconPath = null;
        }

        _modIcon.Image = IsModEnabled ? EnabledIcon : DisabledIcon;
      }
    }

    private static Image ConvertToDisabledIcon(Image img) {
      Image newImg = new Bitmap(img.Width, img.Height);
      using (Graphics g = Graphics.FromImage(newImg)) {
        // Convert to grayscale and 75% opacity
        ColorMatrix colorMatrix = new ColorMatrix(new float[][] {
          new float[] { .30f, .30f, .30f,    0, 0 },
          new float[] { .59f, .59f, .59f,    0, 0 },
          new float[] { .11f, .11f, .11f,    0, 0 },
          new float[] {    0,    0,    0, .75f, 0 },
          new float[] {    0,    0,    0,    0, 1 }
        });

        using (ImageAttributes attributes = new ImageAttributes()) {
          attributes.SetColorMatrix(colorMatrix);
          g.DrawImage(
            img,
            new Rectangle(0, 0, img.Width, img.Height),
            0,
            0,
            img.Width,
            img.Height,
            GraphicsUnit.Pixel,
            attributes
          );
        }
      }
      return newImg;
    }

    private void ModEntry_SizeChanged(object sender, EventArgs e) {
      _modIcon.Height = Height - _modIcon.Margin.Vertical;
      _modIcon.Width = _modIcon.Height;
    }

    private static readonly Image ButtonEnabled = Image.FromHbitmap(Properties.Resources.ButtonEnabled.GetHbitmap());
    private static readonly Image ButtonDisabled = Image.FromHbitmap(Properties.Resources.ButtonDisabled.GetHbitmap());

    public bool IsModEnabled {
      get {
        bool isEnabled = File.Exists(ModManualPath);
        _modButton.Image = isEnabled ? ButtonEnabled : ButtonDisabled;

        return isEnabled;
      }
    }
    public event EventHandler OnModEnable;
    public event EventHandler OnModDisable;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles")]
    private void _modButton_Click(object sender, EventArgs e) {
      try {
        if (IsModEnabled) {
          File.Delete(ModManualPath);
        } else {
          Symlink.Create(ModGroPath, ModManualPath);
        }

      } catch (Exception ex) when (
           ex is ArgumentException
        || ex is IOException
        || ex is NotSupportedException
        || ex is UnauthorizedAccessException
      ) {
        MessageBox.Show(
          "An error occured while changing mod state: " + ex.Message,
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
        );
        return;
      }

      if (IsModEnabled) {
        _modIcon.Image = EnabledIcon;
        OnModEnable?.Invoke(this, EventArgs.Empty);
      } else {
        _modIcon.Image = DisabledIcon;
        OnModDisable?.Invoke(this, EventArgs.Empty);
      }
    }

    public override string ToString() => ModName;
  }
}
