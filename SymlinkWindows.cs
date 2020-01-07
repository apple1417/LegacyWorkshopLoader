using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace LegacyWorkshopLoader {
  public static class SymlinkWindows {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateFileW(
      string lpFileName,
      Int32 dwDesiredAccess,
      Int32 dwShareMode,
      IntPtr lpSecurityAttributes,
      Int32 dwCreationDisposition,
      Int32 dwFlagsAndAttributes,
      IntPtr hTemplateFile
    );

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CreateSymbolicLinkW(
      string lpSymlinkFileName,
      string lpTargetFileName,
      Int32 dwFlags
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FindClose(IntPtr hFindFile);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr FindFirstFileW(string lpFileName, ref LPWIN32_FIND_DATAW lpFindFileData);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern Int32 GetFinalPathNameByHandleW(IntPtr hFile, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszFilePath, Int32 cchFilePath, Int32 dwFlags);

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

    private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    private const Int32 MAX_PATH = 260;
    private const Int32 MAX_PATH_WIDE = 32767;

    private const Int32 FILE_ATTRIBUTE_REPARSE_POINT = 0x400;
    private const Int32 IO_REPARSE_TAG_SYMLINK = unchecked((Int32) 0xA000000C);

    private const Int32 OPEN_EXISTING = 3;

    private const Int32 ERROR_ACCESS_DENIED = 5;

    private static Exception LastError() {
      int errno = Marshal.GetLastWin32Error();
      string msg = "Windows error: " + new Win32Exception(errno).Message;

      switch (errno) {
        case  2: /* ERROR_FILE_NOT_FOUND */ return new FileNotFoundException(msg);

        case  3: // ERROR_PATH_NOT_FOUND */
        case  6: /* ERROR_INVALID_HANDLE */ return new IOException(msg);

        case  5: /* ERROR_ACCESS_DENIED */ return new UnauthorizedAccessException(msg);

        case  8: /* ERROR_NOT_ENOUGH_MEMORY */ return new OutOfMemoryException(msg);

        case 87: /* ERROR_INVALID_PARAMETER */
          return new ArgumentException(msg);

        default: return new Win32Exception(errno, msg);
      }
    }

    public static bool IsAdmin => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    public static void Create(string fromFile, string linkLocation) {
      if (!IsAdmin) {
        throw new Win32Exception(ERROR_ACCESS_DENIED);
      }

      bool success = CreateSymbolicLinkW(linkLocation, fromFile, 0);
      if (!success) {
        throw LastError();
      }
    }

    public static bool IsSymlink(string path) {
      LPWIN32_FIND_DATAW fileData = new LPWIN32_FIND_DATAW();
      IntPtr handle = FindFirstFileW(path, ref fileData);

      if (handle == INVALID_HANDLE_VALUE) {
        throw LastError();
      }

      bool isSymlink;
      if ((fileData.dwFileAttributes & FILE_ATTRIBUTE_REPARSE_POINT) == 0) {
        isSymlink = false;
      } else {
        isSymlink = fileData.dwReserved0 == IO_REPARSE_TAG_SYMLINK;
      }

      bool success = FindClose(handle);
      if (!success) {
        throw LastError();
      }

      return isSymlink;
    }

    public static string TargetOf(string link) {
      IntPtr handle = CreateFileW(link, 0, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
      if (handle == INVALID_HANDLE_VALUE) {
        throw LastError();
      }

      StringBuilder target = new StringBuilder(MAX_PATH_WIDE);
      Int32 len = GetFinalPathNameByHandleW(handle, target, target.Capacity, 0);

      // If we got an invalid length then temporarily store the error so that we can close the handle first
      Exception ex = null;
      if (len == 0) {
        ex = LastError();
      }

      bool success = CloseHandle(handle);
      if (len == 0) {
        throw ex;
      }

      if (!success) {
        throw LastError();
      }

      // Remove the "\\?\" at the start
      target.Remove(0, 4);
      return target.ToString();
    }
  }
}
