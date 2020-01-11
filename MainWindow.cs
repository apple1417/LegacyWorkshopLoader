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

    private string ManualDir = "";
    private string WorkshopDir = "";

    private bool HasValidDirs() {
      if (!File.Exists(TalosLocation.Text)) {
        return false;
      }

      DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(TalosLocation.Text));
      if (new DirectoryInfo(Path.GetDirectoryName(TalosLocation.Text)).Name == "x64") {
        dir = dir.Parent.Parent;
      } else {
        dir = dir.Parent;
      }

      string manual = Path.Combine(dir.FullName, "Content", "Talos");
      string workshop = Path.Combine(dir.Parent.Parent.FullName, "workshop", "content", "257510");

      if (!Directory.Exists(manual) || !Directory.Exists(workshop)) {
        return false;
      }

      ManualDir = manual;
      WorkshopDir = workshop;

      return true;
    }

    private static readonly IEnumerable<char> INVALID_FILE_CHARS = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());
    private static readonly string SYMLINK_PREFIX = "zz_";
    private string GetModManualLocation(string name) {
      string safeName = string.Concat(name.Where(c => !INVALID_FILE_CHARS.Contains(c)));
      return Path.Combine(ManualDir, SYMLINK_PREFIX + safeName + ".gro");
    }

    private void ReloadMods() {
      if (!HasValidDirs()) {
        return;
      }

      // I'd love to use LINQ for this but somehow it causes null reference exceptions if optimize code is enabled
      List<string> existingSymlinks = new List<string>();
      foreach (string file in Directory.EnumerateFiles(ManualDir)) {
        if (Path.GetFileName(file).StartsWith(SYMLINK_PREFIX) && Symlink.IsSymlink(file) && File.Exists(Symlink.TargetOf(file))) {
          existingSymlinks.Add(file);
        }
      }

      List<ModEntry> allMods = new List<ModEntry>();
      foreach (string modDir in Directory.EnumerateDirectories(WorkshopDir).Reverse()) {
        string gro = Directory.EnumerateFiles(modDir, "*.gro").FirstOrDefault();
        if (gro == null) {
          continue;
        }
        
        string modId = Path.GetFileNameWithoutExtension(gro);
        string icon = Path.Combine(modDir, modId + ".jpg");
        string infoFile = Path.Combine(modDir, modId + ".txt");

        Match nameMatch = Regex.Match(File.ReadAllText(infoFile), "^.+?#(.+?)#");
        string name = nameMatch.Success ? nameMatch.Groups[1].Value : "Unknown Mod " + modId;

        string symlink = GetModManualLocation(name);
        existingSymlinks.Remove(symlink);

        allMods.Add(new ModEntry(name, icon, gro, symlink));
      }

      // Using DockStyle.Top puts the most recently added item at the top, so sort in reverse
      allMods.Sort((a, b) => b.ModName.CompareTo(a.ModName));
      ModPanel.Controls.Clear();
      foreach (ModEntry mod in allMods) {
        mod.Dock = DockStyle.Top;
        ModPanel.Controls.Add(mod);
      }

      // Symlinks of mods that were active but you've unsubsribed from
      foreach (string symlink in existingSymlinks) {
        try {
          File.Delete(symlink);
        } catch (Exception e) when (e is IOException || e is UnauthorizedAccessException) { }
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

      if (!HasValidDirs()) {
        ShowInvalidInstallMessage();
        return;
      }

      ReloadMods();
    }

    private void RefreshButton_Click(object sender, EventArgs e) {
      // If you typed your own talos location check it's valid
      if (TalosLocation.Text != TalosPicker.FileName) {
        if (!HasValidDirs()) {
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
