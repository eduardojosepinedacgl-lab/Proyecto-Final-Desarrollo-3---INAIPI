using System;
using System.Collections.Generic;
using System.Windows.Forms;
using INAIPI.CAJA.Models;
using INAIPI.CAJA.Data;
using INAIPI.CAJA.Services;
using INAIPI.CAJA.Helpers;
using System.Data;

namespace INAIPI.CAJA.Forms
{
    public partial class frmCobroServicio : Form
    {
        private List<ServicioCatalogoDTO> _catalogoServicios = new List<ServicioCatalogoDTO>();

        public frmCobroServicio()
        {
            InitializeComponent();
        }

        private void frmCobroServicio_Load(object sender, EventArgs e)
        {
            txtMonto.ReadOnly = true;
            CargarCatalogoDesdeBaseLocal();
        }

        private void CargarCatalogoDesdeBaseLocal()
        {
            var repo = new LocalRepository();
            _catalogoServicios = repo.ObtenerCatalogoServicios();

            cmbComponente.DataSource = _catalogoServicios;
            cmbComponente.DisplayMember = "Nombre";
            cmbComponente.ValueMember = "ComponenteId";

            if (_catalogoServicios.Count > 0)
            {
                cmbComponente.SelectedIndex = 0;
                ActualizarDesglosePrecio();
            }
        }

        private void cmbComponente_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarDesglosePrecio();
        }

        private void btnBuscarTutor_Click(object sender, EventArgs e)
        {
            string cedula = txtCedulaTutor.Text.Trim();

            if (cedula.Length != 11)
            {
                MessageBox.Show("La cédula debe contener exactamente 11 dígitos numéricos sin guiones.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new LocalRepository();
            DataTable dtNiños = repo.BuscarBeneficiariosPorCedulaTutor(cedula);

            if (dtNiños != null && dtNiños.Rows.Count > 0)
            {
                cmbBeneficiarios.DataSource = dtNiños;
                cmbBeneficiarios.DisplayMember = "NombreNiño";
                cmbBeneficiarios.ValueMember = "BeneficiarioId";
                cmbBeneficiarios.SelectedIndex = 0;
            }
            else
            {
                cmbBeneficiarios.DataSource = null;
                MessageBox.Show("No se encontraron beneficiarios asociados a esta cédula en el padrón.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtCedulaTutor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Solo números
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnBuscarTutor_Click(sender, e);
            }
        }
        private void ActualizarDesglosePrecio()
        {
            if (cmbComponente.SelectedItem is ServicioCatalogoDTO servicioSeleccionado)
            {
                decimal subtotal = servicioSeleccionado.PrecioFijo;
                decimal itbis = ReglasNegocio.CalcularItbis(subtotal);
                decimal total = ReglasNegocio.CalcularTotal(subtotal);

                lblSubtotalValor.Text = $"RD$ {subtotal:N2}";
                lblItbisValor.Text = $"RD$ {itbis:N2}";
                lblTotalValor.Text = $"RD$ {total:N2}";
                txtMonto.Text = total.ToString("F2");
            }
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
                if (cmbBeneficiarios.SelectedValue == null)
                {
                    MessageBox.Show("Debe buscar una cédula y seleccionar un beneficiario antes de procesar el cobro.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int beneficiarioId = Convert.ToInt32(cmbBeneficiarios.SelectedValue);

                if (!(cmbComponente.SelectedItem is ServicioCatalogoDTO servicioSeleccionado))
                {
                    MessageBox.Show("Seleccione un componente de atención válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal subtotal = servicioSeleccionado.PrecioFijo;
                decimal itbis = ReglasNegocio.CalcularItbis(subtotal);
                decimal total = ReglasNegocio.CalcularTotal(subtotal);

                var transaccion = new TransaccionCajaDTO
                {
                    TransaccionGuid = Guid.NewGuid(),
                    BeneficiarioId = beneficiarioId,
                    CentroId = ReglasNegocio.ObtenerCentroIdActual(),
                    ComponenteId = servicioSeleccionado.ComponenteId,
                    UsuarioRegistroId = TurnoCaja.CajeroId,
                    DescripcionServicio = servicioSeleccionado.Nombre,
                    Subtotal = subtotal,
                    Itbis = itbis,
                    Monto = total,
                    FechaRegistro = DateTime.Now
                };

                var repo = new LocalRepository();
                repo.GuardarTransaccionContingencia(transaccion);

                var sndClient = new ConexionSNDClient();
                bool enviadoOnline = sndClient.EnviarTransaccion(transaccion);

                if (enviadoOnline)
                {
                    repo.MarcarComoSincronizado(transaccion.TransaccionGuid);
                    MessageBox.Show("Transacción procesada correctamente en línea.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sin red. Transacción guardada en contingencia (Outbox).", "Modo Offline", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Datos extra del comprobante: infante, cédula del tutor y cajero
                string beneficiarioTicket = $"{(cmbBeneficiarios.Text ?? "").Trim()} (Exp. {transaccion.BeneficiarioId})";

                string cedula = txtCedulaTutor.Text.Trim();
                string cedulaTicket = cedula.Length == 11
                    ? $"{cedula.Substring(0, 3)}-{cedula.Substring(3, 7)}-{cedula.Substring(10, 1)}"
                    : cedula;

                string nombreCajero = repo.ObtenerNombreCajero(TurnoCaja.CajeroId);
                string cajeroTicket = string.IsNullOrWhiteSpace(nombreCajero)
                    ? $"ID {TurnoCaja.CajeroId}"
                    : $"{nombreCajero} (ID {TurnoCaja.CajeroId})";

                frmReporteTicket frmTicket = new frmReporteTicket(
                    transaccion.TransaccionGuid.ToString(),
                    beneficiarioTicket,
                    transaccion.DescripcionServicio,
                    transaccion.Subtotal.ToString("F2"),
                    transaccion.Itbis.ToString("F2"),
                    transaccion.Monto.ToString("F2"),
                    transaccion.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss"),
                    cedulaTicket,
                    cajeroTicket
                );

                frmTicket.MdiParent = this.MdiParent;
                frmTicket.Show();

                LimpiarControles();
            }
            catch (Exception ex)
            {
                CajaLogger.Error("Fallo crítico al procesar el cobro en ventanilla.", ex);
                MessageBox.Show($"Ocurrió un error al procesar el cobro: {ex.Message}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnProcesarCobro.Enabled = true;
            }
        }

        private void LimpiarControles()
        {
            txtCedulaTutor.Clear();
            if (cmbComponente.Items.Count > 0)
            {
                cmbComponente.SelectedIndex = 0;
            }
            ActualizarDesglosePrecio();
            txtCedulaTutor.Focus();
        }
    }
}