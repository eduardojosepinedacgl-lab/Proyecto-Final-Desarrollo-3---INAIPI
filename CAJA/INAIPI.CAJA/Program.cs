using System;
using System.Windows.Forms;
using INAIPI.CAJA.Forms;

namespace INAIPI.CAJA
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Flujo: 1. Login -> 2. Apertura de Turno -> 3. Pantalla Principal
            frmLoginCaja frmLogin = new frmLoginCaja();
            if (frmLogin.ShowDialog() == DialogResult.OK)
            {
                frmInicioCaja frmInicio = new frmInicioCaja();
                if (frmInicio.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new frmPrincipalCaja());
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }
    }
}