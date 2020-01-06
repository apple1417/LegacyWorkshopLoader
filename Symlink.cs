using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LegacyWorkshopFixer {
  public static class Symlink {
    public static void Create(string fromFile, string linkLocation) {
      // TODO
      File.Copy(fromFile, linkLocation);
    }

    public static bool IsSymlink(string path) {
      // TODO
      return false;
    }
  }
}
