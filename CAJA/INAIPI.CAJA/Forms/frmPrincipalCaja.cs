using System;
using System.Drawing;
using System.Windows.Forms;

namespace INAIPI.CAJA.Forms
{
    public partial class frmPrincipalCaja : Form
    {
        public frmPrincipalCaja()
        {
            InitializeComponent();
        }

        private async void temporizadorSincronizacion_Tick(object sender, EventArgs e)
        {
            // 1. Detenemos el reloj temporalmente para que no se solapen ejecuciones si hay lentitud
            temporizadorSincronizacion.Stop();

            try
            {
                var sincronizador = new Services.SincronizadorBackground();

                // 2. Lo mandamos a ejecutar en un hilo secundario para que la interfaz gráfica no se congele (Task.Run)
                bool online = await System.Threading.Tasks.Task.Run(() => sincronizador.ProcesarPendientes());

                // 3. Actualizamos la barra de estado inferior
                ActualizarIndicadorVisual(online);
            }
            finally
            {
                // 4. Volvemos a encender el reloj para el próximo ciclo de 10 segundos
                temporizadorSincronizacion.Start();
            }
        }

        public void ActualizarIndicadorVisual(bool estaConectado)
        {
            if (estaConectado)
            {
                lblEstadoConectividad.Text = "Estado: ONLINE (Conectado al Módulo Central / SND)";
                lblEstadoConectividad.ForeColor = Color.Green;
            }
            else
            {
                lblEstadoConectividad.Text = "Estado: OFFLINE (Modo Contingencia - Almacenamiento Local Outbox)";
                lblEstadoConectividad.ForeColor = Color.Red;
            }
        }

        private void subMenuCobro_Click(object sender, EventArgs e)
        {
            // Instanciar el formulario de cobro
            frmCobroServicio frmCobro = new frmCobroServicio();

            // Asignar este formulario principal como su contenedor Padre (MDI)
            frmCobro.MdiParent = this;

            // Mostrar el formulario hijo según requerimiento del SRS
            frmCobro.Show();
        }

        private void historialDeTransaccionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmHistorialTransacciones frmHistorial = new frmHistorialTransacciones();
            frmHistorial.MdiParent = this;
            frmHistorial.Show();
        }
    }
}