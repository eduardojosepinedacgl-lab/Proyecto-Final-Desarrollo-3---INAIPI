using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using INAIPI.CAJA.Models;
using INAIPI.CAJA.Services;

namespace INAIPI.CAJA.Forms
{
    public partial class frmPrincipalCaja : Form
    {
        public frmPrincipalCaja()
        {
            InitializeComponent();
        }

        private void frmPrincipalCaja_Shown(object sender, EventArgs e)
        {
            if (TurnoCaja.TurnoAbierto)
            {
                this.Show();
                this.Text = $"INAIPI | Sistema de Gestión de Caja | Operador: {TurnoCaja.CajeroId} | Fondo: {TurnoCaja.MontoInicial:C2}";
                temporizadorSincronizacion.Start();
            }
            else
            {
                Application.Exit();
            }
        }

        private async void temporizadorSincronizacion_Tick(object sender, EventArgs e)
        {
            temporizadorSincronizacion.Stop();
            try
            {
                var sincronizador = new SincronizadorBackground();
                bool online = await Task.Run(() => sincronizador.ProcesarPendientes());
                ActualizarIndicadorVisual(online);
            }
            finally
            {
                temporizadorSincronizacion.Start();
            }
        }

        public void ActualizarIndicadorVisual(bool estaConectado)
        {
            if (estaConectado)
            {
                lblEstadoConectividad.Text = "ESTADO: ONLINE (Conectado al Módulo Central / SND)";
                lblEstadoConectividad.ForeColor = Color.MediumSeaGreen;
            }
            else
            {
                lblEstadoConectividad.Text = "ESTADO: OFFLINE (Modo Contingencia - Almacenamiento Local Outbox)";
                lblEstadoConectividad.ForeColor = Color.Tomato;
            }
        }

        private void btnGestionCobros_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new frmCobroServicio());
        }

        private void btnRegistroTransacciones_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new frmHistorialTransacciones());
        }

        private void btnAuditoriaCierre_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new frmCuadreCaja());
        }

        private void btnSalirSistema_Click(object sender, EventArgs e)
        {
            if (TurnoCaja.TurnoAbierto)
            {
                MessageBox.Show("Vamos a realizar el cuadre de caja antes de salir.", "Cierre Requerido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Aquí pones el código que simula el clic en tu pestaña "Auditoría y Cierre de Turno"
                // para abrirle el formulario de cuadre automáticamente.
                AbrirFormularioHijo(new frmCuadreCaja());
                return;
            }

            Application.Exit();
        }

        private void AbrirFormularioHijo(Form formHijo)
        {
            foreach (Form hijo in this.MdiChildren)
            {
                hijo.Close();
            }

            formHijo.MdiParent = this;
            formHijo.Dock = DockStyle.Fill;
            formHijo.Show();
        }

        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (TurnoCaja.TurnoAbierto)
                {
                    MessageBox.Show("Debe realizar el Cierre de Turno antes de salir del sistema.", "Cierre Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Esto aborta el cierre de la ventana
                }
            }
        }
    }
}