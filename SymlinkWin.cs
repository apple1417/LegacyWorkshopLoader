using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace LegacyWorkshopFixer {
  public static class SymlinkWin {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFileW(
      string lpFileName,
      Int32 dwDesiredAccess,
      Int32 dwShareMode,
      IntPtr lpSecurityAttributes,
      Int32 dwCreationDisposition,
      Int32 dwFlagsAndAttributes,
      SafeFileHandle hTemplateFile
    );

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CreateSymbolicLinkW(
      string lpSymlinkFileName,
      string lpTargetFileName,
      Int32 dwFlags
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(SafeHandle hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FindClose(SafeFileHandle hFindFile);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle FindFirstFileW(string lpFileName, ref LPWIN32_FIND_DATAW lpFindFileData);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern Int32 GetFinalPathNameByHandleW(SafeFileHandle hFile, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszFilePath, Int32 cchFilePath, Int32 dwFlags);

    [StructLayout(LayoutKind.Sequential)]
    private struct LPWIN32_FIND_DATAW {
      public Int32 dwFileAttributes;
      [Obsolete]
      public FILETIME ftCreationTime;
      [Obsolete]
      public FILETIME ftLastAccessTime;
      [Obsolete]
      public FILETIME ftLastWriteTime;
      public Int32 nFileSizeHigh;
      public Int32 nFileSizeLow;
      public Int32 dwReserved0;
      public Int32 dwReserved1;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_PATH)]
      public Char[] cFileName;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
      public Char[] cAlternateFileName;
      public Int32 dwFileType;
      public Int32 dwCreatorType;
      public Int16 wFinderFlags;
    }

    private static readonly SafeFileHandle NULL_PTR = new SafeFileHandle(IntPtr.Zero, true);

    private static readonly SafeFileHandle INVALID_HANDLE_VALUE = new SafeFileHandle(new IntPtr(-1), true);
    private const Int32 MAX_PATH = 260;
    private const Int32 MAX_PATH_WIDE = 32767;

    private const Int32 FILE_ATTRIBUTE_REPARSE_POINT = 0x400;
    private const Int32 IO_REPARSE_TAG_SYMLINK = unchecked((Int32) 0xA000000C);

    private const Int32 OPEN_EXISTING = 3;

    private const Int32 ERROR_ACCESS_DENIED = 5;

    public static bool IsAdmin => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    public static void Create(string fromFile, string linkLocation) {
      if (!IsAdmin) {
        throw new Win32Exception(ERROR_ACCESS_DENIED);
      }

      bool success = CreateSymbolicLinkW(linkLocation, fromFile, 0);
      if (!success) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }

    public static bool IsSymlink(string path) {
      LPWIN32_FIND_DATAW fileData = new LPWIN32_FIND_DATAW();
      SafeFileHandle handle = FindFirstFileW(path, ref fileData);

      if (handle == INVALID_HANDLE_VALUE) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }

      bool isSymlink;
      if ((fileData.dwFileAttributes & FILE_ATTRIBUTE_REPARSE_POINT) == 0) {
        isSymlink = false;
      } else {
        isSymlink = fileData.dwReserved0 == IO_REPARSE_TAG_SYMLINK;
      }

      bool success = FindClose(handle);
      if (!success) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }

      return isSymlink;
    }

    public static string TargetOf(string link) {
      SafeFileHandle handle = CreateFileW(link, 0, 0, IntPtr.Zero, OPEN_EXISTING, 0, NULL_PTR);
      if (handle == INVALID_HANDLE_VALUE) {
        throw  new Win32Exception(Marshal.GetLastWin32Error());
      }

      StringBuilder target = new StringBuilder(MAX_PATH_WIDE);
      Int32 len = GetFinalPathNameByHandleW(handle, target, target.Capacity, 0);
      if (len == 0) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }

      bool success = CloseHandle(handle);
      if (!success) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }

      // Remove the "\\?\" at the start
      target.Remove(0, 4);
      return target.ToString();
    }
  }
}
