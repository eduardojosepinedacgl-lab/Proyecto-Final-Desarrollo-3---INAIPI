using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using INAIPI.Core.AdminApp.Forms;
using INAIPI.Core.AdminApp.Helpers;

namespace INAIPI.Core.AdminApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            LoggerHelper.InicializarLogging();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmLogin login = new frmLogin();
            if (login.ShowDialog() == DialogResult.OK && login.UsuarioAutenticado != null)
            {
                Application.Run(new frmPrincipal(login.UsuarioAutenticado));
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
