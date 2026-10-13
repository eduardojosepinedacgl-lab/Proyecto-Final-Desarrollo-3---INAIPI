using System;
using System.Windows.Forms;
using INAIPI.CAJA.Models;

namespace INAIPI.CAJA.Forms
{
    public partial class frmInicioCaja : Form
    {
        public frmInicioCaja()
        {
            InitializeComponent();
        }

        private void btnAbrirTurno_Click(object sender, EventArgs e)
        {
            // Validamos que el cajero ID sea un número y el monto sea válido
            bool esMontoValido = decimal.TryParse(txtMontoInicial.Text, out decimal monto) && monto >= 0;
            bool esCajeroValido = int.TryParse(txtCajeroId.Text, out int cajeroId) && cajeroId > 0;

            if (esMontoValido && esCajeroValido)
            {
                // Guardamos en memoria
                TurnoCaja.CajeroId = cajeroId;
                TurnoCaja.MontoInicial = monto;
                TurnoCaja.TurnoAbierto = true;
                TurnoCaja.FechaApertura = DateTime.Now;

                MessageBox.Show($"Turno abierto exitosamente.\nCajero ID: {cajeroId}\nFondo de Caja: {monto:C2}", "Apertura de Caja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close(); // Libera el sistema
            }
            else
            {
                MessageBox.Show("Revise los datos. El ID del cajero debe ser un número entero y el monto debe ser mayor o igual a cero.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Cierra todo si se niegan a abrir turno
        }
    }
}