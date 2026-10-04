using System;
using System.Windows.Forms;
using INAIPI.CAJA.Models;
using INAIPI.CAJA.Data;
using INAIPI.CAJA.Services;

namespace INAIPI.CAJA.Forms
{
    public partial class frmCobroServicio : Form
    {
        public frmCobroServicio()
        {
            InitializeComponent();
        }

        private void btnProcesarCobro_Click(object sender, EventArgs e)
        {
            btnProcesarCobro.Enabled = false;

            try
            {
                if (string.IsNullOrWhiteSpace(txtBeneficiarioId.Text) ||
                    string.IsNullOrWhiteSpace(txtDescripcionServicio.Text) ||
                    string.IsNullOrWhiteSpace(txtImporte.Text))
                {
                    MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var transaccion = new TransaccionCajaDTO
                {
                    BeneficiarioId = int.Parse(txtBeneficiarioId.Text),
                    DescripcionServicio = txtDescripcionServicio.Text,
                    Importe = decimal.Parse(txtImporte.Text)
                };

                var sndClient = new ConexionSNDClient();
                bool enviadoOnline = sndClient.EnviarTransaccion(transaccion);

                if (!enviadoOnline)
                {
                    var repo = new LocalRepository();
                    repo.GuardarTransaccionContingencia(transaccion);
                    MessageBox.Show("Sin red. Transacción guardada en contingencia (Outbox).", "Modo Offline", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Transacción procesada correctamente en línea.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // --- ABRIR EL COMPROBANTE AUTOMÁTICAMENTE ---
                frmReporteTicket frmTicket = new frmReporteTicket(
                    transaccion.TransaccionGuid.ToString(),
                    transaccion.BeneficiarioId.ToString(),
                    transaccion.DescripcionServicio,
                    transaccion.Importe.ToString("F2"),
                    transaccion.Fecha.ToString("dd/MM/yyyy HH:mm:ss")
                );

                // Lo mantenemos dentro de la ventana gris principal (MDI)
                frmTicket.MdiParent = this.MdiParent;
                frmTicket.Show();

                LimpiarControles();
            }
            catch (Exception ex)
            {
                INAIPI.CAJA.Helpers.CajaLogger.Error("Fallo crítico al procesar el cobro en ventanilla.", ex);
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnProcesarCobro.Enabled = true;
            }
        }

        private void LimpiarControles()
        {
            txtBeneficiarioId.Clear();
            txtDescripcionServicio.Clear();
            txtImporte.Clear();
            txtBeneficiarioId.Focus();
        }
    }
}