using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LegacyWorkshopLoader {
  public static class CacheManager {
    private static string CacheDir = "";
    private static string ManualDir = "";
    private static string WorkshopDir = "";

    public static bool SetDirIfValid(string talosDir) {
      string cache = Path.Combine(talosDir, "LWL Cache");
      string manual = Path.Combine(talosDir, "Content", "Talos");
      string workshop = Path.Combine(new DirectoryInfo(talosDir).Parent.Parent.FullName, "workshop", "content", "257510");

      if (!Directory.Exists(manual) || !Directory.Exists(workshop)) {
        return false;
      }

      if (!Directory.Exists(cache)) {
        Directory.CreateDirectory(cache);
      }

      CacheDir = cache;
      ManualDir = manual;
      WorkshopDir = workshop;
      return true;
    }

    public static List<ModEntry> GetMods() {
      List<ModEntry> allMods = GetCachedMods();

      foreach (ModEntry mod in GetWorkshopMods()) {
        // If the mod is cached, check if there's been an update
        if (allMods.Any(m => m.ModName == mod.ModName)) {
          ModEntry cached = allMods.First(m => m.ModName == mod.ModName);

          if (!FilesEqual(mod.ModGroPath, cached.ModGroPath)) {
            File.Delete(cached.ModGroPath);
            cached.ModGroPath = CacheFile(mod.ModGroPath);
          }

          if (!FilesEqual(mod.ModIconPath, cached.ModIconPath)) {
            File.Delete(cached.ModIconPath);
            cached.ModIconPath = CacheFile(mod.ModIconPath);
          }

          // If the mod is not cached, cache it
        } else {
          mod.ModGroPath = CacheFile(mod.ModGroPath);
          mod.ModIconPath = CacheFile(mod.ModIconPath);

          allMods.Add(mod);
        }
      }

      SaveModList(allMods);

      return allMods;
    }

    private static string CacheFile(string path) {
      string newPath = Path.Combine(CacheDir, Path.GetFileName(path));
      File.Copy(path, newPath);
      return newPath;
    }

    private const string SETTINGS_FILE_NAME = "Mods.xml";
    private const string SETTINGS_HEADER = "Mods";
    private const string MOD_HEADER = "Mod";
    private const string MOD_NAME = "Name";
    private const string MOD_ICON = "Icon";
    private const string MOD_GRO = "Gro";

    private static List<ModEntry> GetCachedMods() {
      List<ModEntry> allMods = new List<ModEntry>();
      string settings = Path.Combine(CacheDir, SETTINGS_FILE_NAME);
      if (!File.Exists(settings)) {
        return allMods;
      }

      foreach (XElement child in XElement.Load(settings).Elements()) {
        ModEntry mod = GetModFromXML(child);
        if (mod == null) {
          continue;
        }

        allMods.Add(mod);
      }

      return allMods;
    }

    private static ModEntry GetModFromXML(XElement xmlMod) {
      if (xmlMod.Name != MOD_HEADER) {
        return null;
      }

      ModEntry mod = new ModEntry();
      foreach (XElement child in xmlMod.Elements()) {
        switch (child.Name.ToString()) {
          case MOD_NAME: {
            mod.ModName = child.Value;
            mod.ModManualPath = GetModManualLocation(child.Value);
            break;
          }
          case MOD_ICON: {
            mod.ModIconPath = child.Value;
            break;
          }
          case MOD_GRO: {
            mod.ModGroPath = child.Value;
            break;
          }
        }
      }

      return mod;
    }

    private static void SaveModList(List<ModEntry> modList) {
      using (XmlWriter writer = XmlWriter.Create(Path.Combine(CacheDir, SETTINGS_FILE_NAME), new XmlWriterSettings() {
        Indent = true,
        NewLineOnAttributes = true
      })) {
        writer.WriteStartDocument();
        writer.WriteStartElement(SETTINGS_HEADER);

        foreach (ModEntry mod in modList) {
          writer.WriteStartElement(MOD_HEADER);
          writer.WriteElementString(MOD_NAME, mod.ModName);
          writer.WriteElementString(MOD_ICON, mod.ModIconPath);
          writer.WriteElementString(MOD_GRO, mod.ModGroPath);
          writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
      }
    }

    private static List<ModEntry> GetWorkshopMods() {
      List<ModEntry> allMods = new List<ModEntry>();
      foreach (string modDir in Directory.EnumerateDirectories(WorkshopDir)) {
        string gro = Directory.EnumerateFiles(modDir, "*.gro").FirstOrDefault();
        if (gro == null) {
          continue;
        }

        string modId = Path.GetFileNameWithoutExtension(gro);
        string icon = Path.Combine(modDir, modId + ".jpg");
        string infoFile = Path.Combine(modDir, modId + ".txt");

        Match nameMatch = Regex.Match(File.ReadAllText(infoFile), "^.+?#(.+?)#");
        string name = nameMatch.Success ? nameMatch.Groups[1].Value : "Unknown Mod " + modId;

        string manual = GetModManualLocation(name);

        allMods.Add(new ModEntry(name, icon, gro, manual));
      }
      return allMods;
    }

    private static readonly IEnumerable<char> INVALID_FILE_CHARS = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());
    private static readonly string MANUAL_MOD_PREFIX = "zz_";
    private static string GetModManualLocation(string name) {
      string safeName = string.Concat(name.Where(c => !INVALID_FILE_CHARS.Contains(c)));
      return Path.Combine(ManualDir, MANUAL_MOD_PREFIX + safeName + ".gro");
    }

    private static bool FilesEqual(string pathA, string pathB) {
      FileInfo fileA = new FileInfo(pathA);
      FileInfo fileB = new FileInfo(pathB);
      if (fileA.Length != fileB.Length) {
        return false;
      }

      using (FileStream streamA = fileA.OpenRead())
      using (FileStream streamB = fileB.OpenRead()) {
        if (streamA.ReadByte() != streamB.ReadByte()) {
          return false;
        }
      }

      return true;
    }
  }
}
