using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace LegacyWorkshopFixer {
  public partial class ModEntry : UserControl {
    public string ModGro;

    public ModEntry(string ModName, string ModGro, string ModIcon, bool IsModEnabled) {
      InitializeComponent();

      this.ModName = ModName;
      this.ModGro = ModGro;

      this.IsModEnabled = IsModEnabled;
      this.ModIcon = ModIcon;

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

    private static readonly Image EnabledMissing = Image.FromHbitmap(Properties.Resources.Question.GetHbitmap());
    private static readonly Image DisabledMissing = ConvertToDisabledIcon(EnabledMissing);

    private Image EnabledIcon;
    private Image DisabledIcon;
    public string ModIcon {
      set {
        if (File.Exists(value)) {
          EnabledIcon = Image.FromFile(value);
          DisabledIcon = ConvertToDisabledIcon(EnabledIcon);
        } else {
          EnabledIcon = EnabledMissing;
          DisabledIcon = DisabledMissing;
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

    private static readonly Image Checkmark = Image.FromHbitmap(Properties.Resources.Checkmark.GetHbitmap());
    private static readonly Image Cross = Image.FromHbitmap(Properties.Resources.Cross.GetHbitmap());

    private bool _isModEnabled;
    public bool IsModEnabled {
      get {
        return _isModEnabled;
      }
      private set {
        _isModEnabled = value;

        _modButton.Image = _isModEnabled ? Cross : Checkmark;
      }
    }
    public event EventHandler OnModEnable;
    public event EventHandler OnModDisable;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles")]
    private void _modButton_Click(object sender, EventArgs e) {
      IsModEnabled = !IsModEnabled;

      if (IsModEnabled) {
        OnModEnable?.Invoke(this, EventArgs.Empty);

        _modIcon.Image = EnabledIcon;
      } else {
        OnModDisable?.Invoke(this, EventArgs.Empty);

        _modIcon.Image = DisabledIcon;
      }
    }
  }
}
