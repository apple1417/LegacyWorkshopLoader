using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace LegacyWorkshopLoader {
  public static class Symlink {
    public static void Create(string fromFile, string linkLocation) {
      if (!File.Exists(fromFile)) {
        throw new FileNotFoundException("Symlink source file not found");
      }
      if (File.Exists(linkLocation)) {
        throw new IOException("Symlink target already exists");
      }

      try {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
          SymlinkWindows.Create(fromFile, linkLocation);
          return;
        } else /*if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))*/ {
          SymlinkLinux.Create(fromFile, linkLocation);
          return;
        }
      } catch (Win32Exception e) {
        throw new IOException(e.Message);
      }

      //throw new PlatformNotSupportedException();
    }

    public static bool IsSymlink(string path) {
      if (!File.Exists(path)) {
        return false;
      }

      try {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
          return SymlinkWindows.IsSymlink(path);
        } else /*if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))*/ {
          return SymlinkLinux.IsSymlink(path);
        }
      } catch (Win32Exception e) {
        throw new IOException(e.Message);
      }

      //throw new PlatformNotSupportedException();
    }

    public static string TargetOf(string link) {
      if (!File.Exists(link)) {
        throw new FileNotFoundException("File not found");
      }

      try {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
          return SymlinkWindows.TargetOf(link);
        } else /*if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))*/ {
          return SymlinkLinux.TargetOf(link);
        }
      } catch (Win32Exception e) {
        throw new IOException(e.Message);
      }

      //throw new PlatformNotSupportedException();
    }
  }
}
