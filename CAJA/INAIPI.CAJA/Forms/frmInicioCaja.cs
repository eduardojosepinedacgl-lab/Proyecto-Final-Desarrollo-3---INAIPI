using System;
using System.Windows.Forms;
using INAIPI.CAJA.Helpers;
using INAIPI.CAJA.Models;

namespace INAIPI.CAJA.Forms
{
    public partial class frmInicioCaja : Form
    {
        public frmInicioCaja()
        {
            InitializeComponent();
        }

        private void frmInicioCaja_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = $"Bienvenido/a, {TurnoCaja.NombreCajero}";
        }

        private void btnAbrirTurno_Click(object sender, EventArgs e)
        {
            bool esMontoValido = decimal.TryParse(txtMontoInicial.Text, out decimal monto) && monto >= 0;

            if (esMontoValido)
            {
                TurnoCaja.MontoInicial = monto;
                TurnoCaja.TurnoAbierto = true;
                TurnoCaja.FechaApertura = DateTime.Now;

                CajaLogger.Info($"INICIO DE TURNO | Trabajador: {TurnoCaja.Username} (Cód: {TurnoCaja.CajeroId}) | Fondo Inicial: {TurnoCaja.MontoInicial:C2} | Terminal: {Environment.MachineName}");

                MessageBox.Show($"Turno abierto exitosamente.\n\nOperador: {TurnoCaja.NombreCajero}\nFondo en Gaveta: {monto:C2}", "Apertura de Caja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("El monto inicial debe ser un número válido mayor o igual a cero.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}