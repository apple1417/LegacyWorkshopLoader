using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LegacyWorkshopLoader {
  public partial class MainWindow : Form {
    public MainWindow() {
      InitializeComponent();

      if (TryDetectGameDir()) {
        ReloadMods();
      }
    }

    private bool TryDetectGameDir() {
      string steamapps;
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        steamapps = @"C:\Program Files (x86)\Steam\steamapps";
      } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
        steamapps = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @".steam/steam/steamapps");
      } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
        steamapps = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"Library/Application Support/Steam/steamapps");
      } else {
        return false;
      }

      string libraryFolders = Path.Combine(steamapps, "libraryfolders.vdf");
      if (!File.Exists(libraryFolders)) {
        return false;
      }

      List<string> steamLibraries = new List<string>() {
        steamapps
      };
      foreach (Match m in Regex.Matches(File.ReadAllText(libraryFolders), "\\t+\"\\d+\"\\t+\"(.+?)\"")) {
        string dir = m.Groups[1].Value.Replace(@"\\", @"\");
        if (!Directory.Exists(dir)) {
          continue;
        }

        steamLibraries.Add(Path.Combine(dir, "steamapps"));
      }

      foreach (string dir in steamLibraries) {
        if (!File.Exists(Path.Combine(dir, "appmanifest_257510.acf"))) {
          continue;
        }

        string bin = Path.Combine(dir, "common", "The Talos Principle", "Bin");
        if (!Directory.Exists(bin)) {
          return false;
        }

        string exe = Directory.EnumerateFiles(bin, "Talos*", SearchOption.AllDirectories).FirstOrDefault();
        if (exe == null) {
          return false;
        }

        TalosLocation.Text = Path.Combine(bin, exe);
        return true;
      }

      return false;
    }

    private bool IsValidDir() {
      if (!File.Exists(TalosLocation.Text)) {
        return false;
      }

      DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(TalosLocation.Text));
      if (dir.Name == "x64") {
        return CacheManager.SetDirIfValid(dir.Parent.Parent.FullName);
      } else {
        return CacheManager.SetDirIfValid(dir.Parent.FullName);
      }
    }

    private void ReloadMods() {
      if (!IsValidDir()) {
        return;
      }

      // Using DockStyle.Top puts the most recently added item at the top, so sort in reverse
      List<ModEntry> allMods = CacheManager.GetMods();
      allMods.Sort((a, b) => b.ModName.CompareTo(a.ModName));

      ModPanel.Controls.Clear();
      foreach (ModEntry mod in allMods) {
        mod.Dock = DockStyle.Top;
        ModPanel.Controls.Add(mod);
      }
    }

    private void ShowInvalidInstallMessage() => MessageBox.Show(
      "The selected path was not detected as a valid Talos install.",
      "Invalid Talos Path",
      MessageBoxButtons.OK,
      MessageBoxIcon.Warning
    );

    private void OpenFolderButton_Click(object sender, EventArgs e) {
      if (File.Exists(TalosLocation.Text)) {
        TalosPicker.InitialDirectory = Path.GetDirectoryName(TalosLocation.Text);
      }

      DialogResult result = TalosPicker.ShowDialog();
      if (result != DialogResult.OK) {
        return;
      }

      TalosLocation.Text = TalosPicker.FileName;

      if (!IsValidDir()) {
        ShowInvalidInstallMessage();
        return;
      }

      ReloadMods();
    }

    private void RefreshButton_Click(object sender, EventArgs e) {
      // If you typed your own talos location check it's valid
      if (TalosLocation.Text != TalosPicker.FileName) {
        if (!IsValidDir()) {
          ShowInvalidInstallMessage();
          return;
        }
      }

      ReloadMods();
    }

    // If you press enter in the text box, try reload the mods
    private void TalosLocation_KeyPress(object sender, KeyPressEventArgs e) {
      if (e.KeyChar == (char) Keys.Return) {
        e.Handled = true;
        RefreshButton_Click(sender, e);
      }
    }
  }
}
