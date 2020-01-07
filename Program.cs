using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LegacyWorkshopLoader {
  static class Program {
    [STAThread]
    static void Main() {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !SymlinkWindows.IsAdmin) {
        MessageBox.Show(
          "This program requires administrator permissions to properly function on Windows, so will now exit.",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
        );
        return;
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainWindow());
    }
  }
}
