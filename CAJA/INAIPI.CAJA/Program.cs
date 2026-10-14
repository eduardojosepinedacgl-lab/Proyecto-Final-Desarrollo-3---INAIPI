using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using INAIPI.CAJA.Forms;

namespace INAIPI.CAJA
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmInicioCaja frmInicioCaja = new frmInicioCaja();
            if (frmInicioCaja.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new frmPrincipalCaja());
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
