using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace LegacyWorkshopFixer {
  public static class Symlink {
    public static void Create(string fromFile, string linkLocation) {
      if (!File.Exists(fromFile)) {
        throw new FileNotFoundException("Symlink source file not found");
      }
      if (File.Exists(linkLocation)) {
        throw new IOException("Symlink target already exists");
      }

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        try {
          SymlinkWin.Create(fromFile, linkLocation);
          return;
        } catch (Win32Exception e) {
          throw new IOException(e.Message);
        }
      }

      throw new PlatformNotSupportedException();
    }

    public static bool IsSymlink(string path) {
      if (!File.Exists(path)) {
        return false;
      }

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        try {
          return SymlinkWin.IsSymlink(path);
        } catch (Win32Exception e) {
          throw new IOException(e.Message);
        }
      }

      throw new PlatformNotSupportedException();
    }

    public static string TargetOf(string link) {
      if (!File.Exists(link)) {
        throw new FileNotFoundException("File not found");
      }

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        try {
          return SymlinkWin.TargetOf(link);
        } catch (Win32Exception e) {
          throw new IOException(e.Message);
        }
      }

      throw new PlatformNotSupportedException();
    }
  }
}
