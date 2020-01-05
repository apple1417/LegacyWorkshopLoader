using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegacyWorkshopFixer {
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
        steamapps = @"~/.steam/steam/SteamApps";
      } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
        steamapps = @"~/Library/Application Support/Steam/steamapps";
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

        string bin = Path.Combine(dir, "common", "The Talos Principle", "bin");
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

      DirectoryInfo talosBase = new DirectoryInfo(Path.GetDirectoryName(TalosLocation.Text));
      if (new DirectoryInfo(Path.GetDirectoryName(TalosLocation.Text)).Name == "x64") {
        talosBase = talosBase.Parent.Parent;
      } else {
        talosBase = talosBase.Parent;
      }

      string manual = Path.Combine(talosBase.FullName, "Content", "Talos");
      string workshop = Path.Combine(talosBase.Parent.Parent.FullName, "workshop", "content", "257510");

      if (!Directory.Exists(manual) || !Directory.Exists(workshop)) {
        return false;
      }

      ManualDir = manual;
      WorkshopDir = workshop;

      return true;
    }

    private void ReloadMods() {
      if (!HasValidDirs()) {
        return;
      }

      // TODO: proper table
      int y = 35;
      foreach (string modDir in Directory.EnumerateDirectories(WorkshopDir)) {
        string gro = Directory.EnumerateFiles(modDir, "*.gro").FirstOrDefault();
        if (gro == null) {
          continue;
        }
        
        string modId = Path.GetFileNameWithoutExtension(gro);
        string icon = Path.Combine(modDir, modId + ".jpg");
        string infoFile = Path.Combine(modDir, modId + ".txt");

        string name;
        string[] splitInfo = File.ReadAllText(infoFile).Split('#');
        if (splitInfo.Length < 2) {
          name = "Unknown Mod";
        } else {
          name = splitInfo[1];
        }

        ModEntry mod = new ModEntry(name, gro, icon, false) {
          Location = new Point(15, y),
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        y += 60;
        Controls.Add(mod);
      }
    }

    private void OpenFolderButton_Click(object sender, EventArgs e) {
      if (File.Exists(TalosLocation.Text)) {
        TalosPicker.InitialDirectory = Path.GetDirectoryName(TalosLocation.Text);
      }
      TalosPicker.ShowDialog();
      TalosLocation.Text = TalosPicker.FileName;

      ReloadMods();
    }
  }
}
