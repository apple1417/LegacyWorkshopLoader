using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LegacyWorkshopLoader {
  public static class SymlinkLinux {
#pragma warning disable IDE1006 // Naming Styles
    [DllImport("MonoPosixHelper", SetLastError = true, EntryPoint = "Mono_Posix_Syscall_lstat")]
    private static extern int lstat(string pathname, ref StatStruct statbuf);

    [DllImport("libc", SetLastError = true)]
    private static extern int readlink(string pathname, StringBuilder buf, IntPtr bufsiz);

    [DllImport("libc", SetLastError = true)]
    public static extern IntPtr strerror(int errnum);

    [DllImport("libc", SetLastError = true)]
    private static extern int symlink(string target, string linkpath);
#pragma warning restore IDE1006 // Naming Styles

    private struct StatStruct {
      public UInt64 st_dev;
      public UInt64 st_ino;
      public UInt32 st_mode;
      private UInt32 _padding_;
      public UInt64 st_nlink;
      public UInt32 st_uid;
      public UInt32 st_gid;
      public UInt64 st_rdev;
      public Int64 st_size;
      public Int64 st_blksize;
      public Int64 st_blocks;
      public Timespec st_atime;
      public Timespec st_mtime;
      public Timespec st_ctime;
    }

    private struct Timespec {
      public UInt64 tv_sec;
      public UInt64 tv_nsec;
    }

    private const int PATH_MAX = 4096;
    private const int S_IFLNK = 0xA000;

    private static Exception LastError() {
      int errno = Marshal.GetLastWin32Error();
      string msg = "libc error: " + Marshal.PtrToStringAnsi(SymlinkLinux.strerror(errno));

      switch (errno) {
        case   1: // EPERM
        case  13: /* EACCES */ return new UnauthorizedAccessException(msg);

        case   2: /* ENOENT */ return new FileNotFoundException(msg);

        case   5: // EIO
        case  17: // EEXIST
        case  28: // ENOSPC
        case  30: // EROFS
        case  40: // ELOOP
        case 122: /* EDQUOT */ return new IOException(msg);

        case  12: /* ENOMEM */ return new OutOfMemoryException(msg);
        
        case  14: // EINVAL
        case  22: /* EFAULT */ return new ArgumentException(msg);

        case  20: /* ENOTDIR */ return new DirectoryNotFoundException(msg);

        case  36: /* ENAMETOOLONG */ return new PathTooLongException(msg);

        default: return new Win32Exception(errno, msg);
      }
    }

    public static void Create(string fromFile, string linkLocation) {
      int result = symlink(fromFile, linkLocation);
      if (result != 0) {
        throw LastError();
      }
    }

    public static bool IsSymlink(string path) {
      StatStruct fileStat = new StatStruct();
      int result = lstat(path, ref fileStat);
      if (result != 0) {
        throw LastError();
      }

      return (fileStat.st_mode & S_IFLNK) != 0;
    }

    public static string TargetOf(string link) {
      StringBuilder target = new StringBuilder(PATH_MAX);
      int len = readlink(link, target, new IntPtr(target.Capacity));
      if (len == -1) {
        throw LastError();
      }

      return Path.GetFullPath(target.ToString());
    }
  }
}
