using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegacyWorkshopFixer {
  static class Program {
    [STAThread]
    static void Main() {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !SymlinkWin.IsAdmin) {
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
