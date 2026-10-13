using System;
using System.Collections.Generic;
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

        private void frmCobroServicio_Load(object sender, EventArgs e)
        {
            // Catálogo oficial dictado por la Dirección de Desarrollo Infantil (CORE)
            var componentes = new Dictionary<int, string>
            {
                { 1, "Estimulación Oportuna y Educación Inicial" },
                { 2, "Salud y Nutrición" },
                { 3, "Atención a Necesidades Educativas Especiales" },
                { 4, "Protección Contra la Violencia" },
                { 5, "Registro de Nacimiento e Identidad" },
                { 6, "Familia y Comunidad" }
            };

            cmbComponente.DataSource = new BindingSource(componentes, null);
            cmbComponente.DisplayMember = "Value"; // Lo que lee el usuario
            cmbComponente.ValueMember = "Key";     // El número que va a la base de datos
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnProcesarCobro_Click(object sender, EventArgs e)
        {
            btnProcesarCobro.Enabled = false;

            try
            {
                if (string.IsNullOrWhiteSpace(txtBeneficiario.Text) ||
                    string.IsNullOrWhiteSpace(txtMonto.Text))
                {
                    MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int componenteSeleccionadoId = ((KeyValuePair<int, string>)cmbComponente.SelectedItem).Key;
                string nombreComponente = ((KeyValuePair<int, string>)cmbComponente.SelectedItem).Value;

                var transaccion = new TransaccionCajaDTO
                {
                    TransaccionGuid = Guid.NewGuid(),
                    BeneficiarioId = int.Parse(txtBeneficiario.Text),
                    Monto = decimal.Parse(txtMonto.Text), 

                    CentroId = 1, // ID de sede quemado por defecto
                    ComponenteId = componenteSeleccionadoId,
                    UsuarioRegistroId = TurnoCaja.CajeroId, // Extraído de la memoria de login

                    DescripcionServicio = nombreComponente, // Usamos el nombre del combo para el recibo
                    FechaRegistro = DateTime.Now // Reemplaza a Fecha
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

                frmReporteTicket frmTicket = new frmReporteTicket(
                    transaccion.TransaccionGuid.ToString(),
                    transaccion.BeneficiarioId.ToString(),
                    transaccion.DescripcionServicio,
                    transaccion.Monto.ToString("F2"),
                    transaccion.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss")
                );

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
            txtBeneficiario.Clear();
            txtMonto.Text = "0.00"; //Monto por defecto
            txtBeneficiario.Focus();
        }
    }
}