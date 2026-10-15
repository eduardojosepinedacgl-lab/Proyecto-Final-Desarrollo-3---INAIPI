using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using INAIPI.CAJA.Data;
using INAIPI.CAJA.Helpers;
using INAIPI.CAJA.Models;

namespace INAIPI.CAJA.Forms
{
    public partial class frmHistorialTransacciones : Form
    {
        public frmHistorialTransacciones()
        {
            InitializeComponent();
        }

        private void frmHistorialTransacciones_Load(object sender, EventArgs e)
        {
            // Rango predeterminado: desde el día 1 del mes actual hasta la fecha actual
            DateTime hoy = DateTime.Today;
            dtpFechaDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpFechaHasta.Value = hoy;

            CargarHistorial();
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                var repo = new LocalRepository();
                DataTable dt = repo.ObtenerHistorialTransacciones();
                dgvHistorial.DataSource = dt;

                // Formato y cabeceras amigables
                if (dgvHistorial.Columns["TransaccionGuid"] != null)
                {
                    dgvHistorial.Columns["TransaccionGuid"].HeaderText = "Referencia GUID";
                    dgvHistorial.Columns["TransaccionGuid"].FillWeight = 120;
                }
                if (dgvHistorial.Columns["BeneficiarioId"] != null)
                {
                    dgvHistorial.Columns["BeneficiarioId"].HeaderText = "Expediente";
                    dgvHistorial.Columns["BeneficiarioId"].FillWeight = 60;
                }
                if (dgvHistorial.Columns["ComponenteId"] != null)
                {
                    dgvHistorial.Columns["ComponenteId"].Visible = false; // Se oculta el ID numérico
                }
                if (dgvHistorial.Columns["CentroId"] != null)
                {
                    dgvHistorial.Columns["CentroId"].HeaderText = "Centro";
                    dgvHistorial.Columns["CentroId"].FillWeight = 50;
                }
                if (dgvHistorial.Columns["UsuarioRegistroId"] != null)
                {
                    dgvHistorial.Columns["UsuarioRegistroId"].Visible = false; // Se oculta porque mostramos NombreCajero
                }
                if (dgvHistorial.Columns["NombreCajero"] != null)
                {
                    dgvHistorial.Columns["NombreCajero"].HeaderText = "Cajero";
                    dgvHistorial.Columns["NombreCajero"].FillWeight = 90;
                }
                if (dgvHistorial.Columns["DescripcionServicio"] != null)
                {
                    dgvHistorial.Columns["DescripcionServicio"].HeaderText = "Servicio Prestado";
                    dgvHistorial.Columns["DescripcionServicio"].FillWeight = 140;
                }
                if (dgvHistorial.Columns["Subtotal"] != null)
                {
                    dgvHistorial.Columns["Subtotal"].HeaderText = "Subtotal";
                    dgvHistorial.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
                    dgvHistorial.Columns["Subtotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvHistorial.Columns["Subtotal"].FillWeight = 70;
                }
                if (dgvHistorial.Columns["Itbis"] != null)
                {
                    dgvHistorial.Columns["Itbis"].HeaderText = "ITBIS (18%)";
                    dgvHistorial.Columns["Itbis"].DefaultCellStyle.Format = "C2";
                    dgvHistorial.Columns["Itbis"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvHistorial.Columns["Itbis"].FillWeight = 70;
                }
                if (dgvHistorial.Columns["Importe"] != null)
                {
                    dgvHistorial.Columns["Importe"].HeaderText = "Total";
                    dgvHistorial.Columns["Importe"].DefaultCellStyle.Format = "C2";
                    dgvHistorial.Columns["Importe"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvHistorial.Columns["Importe"].FillWeight = 75;
                }
                if (dgvHistorial.Columns["Fecha"] != null)
                {
                    dgvHistorial.Columns["Fecha"].HeaderText = "Fecha / Hora";
                    dgvHistorial.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dgvHistorial.Columns["Fecha"].FillWeight = 95;
                }
                if (dgvHistorial.Columns["Sincronizado"] != null)
                {
                    dgvHistorial.Columns["Sincronizado"].HeaderText = "Sinc.";
                    dgvHistorial.Columns["Sincronizado"].FillWeight = 45;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar el historial local: {ex.Message}", "Fallo de Lectura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHistorial_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow fila = dgvHistorial.Rows[e.RowIndex];

                string guid = fila.Cells["TransaccionGuid"].Value.ToString();
                int beneficiarioId = Convert.ToInt32(fila.Cells["BeneficiarioId"].Value);
                string servicio = fila.Cells["DescripcionServicio"].Value.ToString();

                decimal importeTotal = Convert.ToDecimal(fila.Cells["Importe"].Value);
                decimal subtotal = fila.DataGridView.Columns.Contains("Subtotal") && fila.Cells["Subtotal"].Value != DBNull.Value
                    ? Convert.ToDecimal(fila.Cells["Subtotal"].Value)
                    : Math.Round(importeTotal / (1m + ReglasNegocio.TasaItbis), 2, MidpointRounding.AwayFromZero);

                decimal itbis = fila.DataGridView.Columns.Contains("Itbis") && fila.Cells["Itbis"].Value != DBNull.Value
                    ? Convert.ToDecimal(fila.Cells["Itbis"].Value)
                    : ReglasNegocio.CalcularItbis(subtotal);

                string fecha = Convert.ToDateTime(fila.Cells["Fecha"].Value).ToString("dd/MM/yyyy HH:mm:ss");

                // Nombre del infante y cédula del tutor, desde el padrón local
                string beneficiarioTicket = $"Exp. {beneficiarioId}";
                string cedulaTicket = null;
                string nombreNino, cedulaTutor;
                if (new LocalRepository().ObtenerDatosBeneficiarioLocal(beneficiarioId, out nombreNino, out cedulaTutor))
                {
                    beneficiarioTicket = $"{nombreNino} (Exp. {beneficiarioId})";
                    cedulaTicket = FormatearCedula(cedulaTutor);
                }

                // Cajero que realizó ESA transacción (no el de la sesión actual)
                string nombreCajero = Convert.ToString(fila.Cells["NombreCajero"].Value);
                string idCajero = Convert.ToString(fila.Cells["UsuarioRegistroId"].Value);
                string cajeroTicket = string.IsNullOrWhiteSpace(nombreCajero)
                    ? $"ID {idCajero}"
                    : $"{nombreCajero} (ID {idCajero})";

                frmReporteTicket frmTicket = new frmReporteTicket(
                    guid,
                    beneficiarioTicket,
                    servicio,
                    subtotal.ToString("F2"),
                    itbis.ToString("F2"),
                    importeTotal.ToString("F2"),
                    fecha,
                    cedulaTicket,
                    cajeroTicket
                );

                frmTicket.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar visualizar el recibo: " + ex.Message, "Error de Lectura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string FormatearCedula(string cedula)
        {
            string c = (cedula ?? "").Replace("-", "").Trim();
            return c.Length == 11 ? $"{c.Substring(0, 3)}-{c.Substring(3, 7)}-{c.Substring(10, 1)}" : c;
        }

        private void btnReporteConsolidado_Click(object sender, EventArgs e)
        {
            DateTime desde = dtpFechaDesde.Value.Date;
            DateTime hasta = dtpFechaHasta.Value.Date;

            if (desde > hasta)
            {
                MessageBox.Show("La fecha inicial ('Desde') no puede ser mayor que la fecha final ('Hasta').", "Rango Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new LocalRepository();
            DataTable dt = repo.ObtenerConsolidacionPorRango(desde, hasta);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show($"No se registraron transacciones entre el {desde:dd/MM/yyyy} y el {hasta:dd/MM/yyyy}.", "Sin Datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string archivoHtml = GenerarHtmlConsolidado(dt, desde, hasta);
                Process.Start(new ProcessStartInfo(archivoHtml) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte de consolidación: {ex.Message}", "Error de Reporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerarHtmlConsolidado(DataTable dt, DateTime desde, DateTime hasta)
        {
            string nombreArchivo = desde == hasta
                ? $"Reporte_Consolidado_{desde:yyyyMMdd}.html"
                : $"Reporte_Consolidado_{desde:yyyyMMdd}_al_{hasta:yyyyMMdd}.html";

            string rutaReporte = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nombreArchivo);

            decimal granSubtotal = 0m;
            decimal granItbis = 0m;
            decimal granTotal = 0m;
            int totalTransacciones = 0;
            int totalSincronizadas = 0;
            int totalPendientes = 0;

            string textoRango = desde == hasta
                ? $"{desde:dd/MM/yyyy}"
                : $"Del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}";

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='es'>");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset='utf-8'>");
            sb.AppendLine("  <title>INAIPI - Consolidado de Caja</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body { font-family: Segoe UI, Arial, sans-serif; margin: 30px; color: #333; }");
            sb.AppendLine("    h2 { color: #2E5B53; margin-bottom: 5px; }");
            sb.AppendLine("    .meta { font-size: 0.9em; color: #666; margin-bottom: 20px; }");
            sb.AppendLine("    table { width: 100%; border-collapse: collapse; margin-top: 15px; }");
            sb.AppendLine("    th { background-color: #2E5B53; color: white; padding: 10px; text-align: left; }");
            sb.AppendLine("    td { padding: 8px 10px; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("    .num { text-align: right; }");
            sb.AppendLine("    .totales { font-weight: bold; background-color: #f2f7f4; border-top: 2px solid #2E5B53; }");
            sb.AppendLine("    .footer { margin-top: 30px; font-size: 0.8em; color: #888; text-align: right; }");
            sb.AppendLine("    .acciones { margin-bottom: 20px; }");
            sb.AppendLine("    .btn-pdf { background-color: #2E5B53; color: white; border: none; padding: 9px 18px; font-size: 13px; font-weight: bold; border-radius: 4px; cursor: pointer; }");
            sb.AppendLine("    .btn-pdf:hover { background-color: #244842; }");
            sb.AppendLine("    @media print {");
            sb.AppendLine("      .acciones { display: none; }");
            sb.AppendLine("      body { margin: 10mm; }");
            sb.AppendLine("    }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <div class='acciones'>");
            sb.AppendLine("    <button class='btn-pdf' onclick='window.print()'>Descargar como PDF / Imprimir</button>");
            sb.AppendLine("  </div>");
            sb.AppendLine("  <h2>INAIPI - Instituto Nacional de Atención Integral a la Primera Infancia</h2>");
            sb.AppendLine("  <h3>Reporte de Consolidación de Cobros en Ventanilla</h3>");
            sb.AppendLine($"  <div class='meta'><strong>Período:</strong> {textoRango} | <strong>Cajero:</strong> {TurnoCaja.NombreCajero} | <strong>Terminal:</strong> {Environment.MachineName}</div>");
            sb.AppendLine("  <table>");
            sb.AppendLine("    <thead>");
            sb.AppendLine("      <tr>");
            sb.AppendLine("        <th>Servicio / Componente</th>");
            sb.AppendLine("        <th class='num'>Cant.</th>");
            sb.AppendLine("        <th class='num'>Subtotal</th>");
            sb.AppendLine("        <th class='num'>ITBIS (18%)</th>");
            sb.AppendLine("        <th class='num'>Total Cobrado</th>");
            sb.AppendLine("        <th class='num'>Sincronizadas</th>");
            sb.AppendLine("        <th class='num'>Pendientes (Outbox)</th>");
            sb.AppendLine("      </tr>");
            sb.AppendLine("    </thead>");
            sb.AppendLine("    <tbody>");

            foreach (DataRow row in dt.Rows)
            {
                int cant = Convert.ToInt32(row["CantidadTransacciones"]);
                decimal sub = Convert.ToDecimal(row["TotalSubtotal"]);
                decimal itb = Convert.ToDecimal(row["TotalItbis"]);
                decimal tot = Convert.ToDecimal(row["TotalRecaudado"]);
                int sinc = Convert.ToInt32(row["CantidadSincronizadas"]);
                int pend = Convert.ToInt32(row["CantidadPendientes"]);

                granSubtotal += sub;
                granItbis += itb;
                granTotal += tot;
                totalTransacciones += cant;
                totalSincronizadas += sinc;
                totalPendientes += pend;

                sb.AppendLine("      <tr>");
                sb.AppendLine($"        <td>{row["DescripcionServicio"]}</td>");
                sb.AppendLine($"        <td class='num'>{cant}</td>");
                sb.AppendLine($"        <td class='num'>RD$ {sub:N2}</td>");
                sb.AppendLine($"        <td class='num'>RD$ {itb:N2}</td>");
                sb.AppendLine($"        <td class='num'><strong>RD$ {tot:N2}</strong></td>");
                sb.AppendLine($"        <td class='num'>{sinc}</td>");
                sb.AppendLine($"        <td class='num'>{pend}</td>");
                sb.AppendLine("      </tr>");
            }

            sb.AppendLine("      <tr class='totales'>");
            sb.AppendLine("        <td>TOTAL GENERAL</td>");
            sb.AppendLine($"        <td class='num'>{totalTransacciones}</td>");
            sb.AppendLine($"        <td class='num'>RD$ {granSubtotal:N2}</td>");
            sb.AppendLine($"        <td class='num'>RD$ {granItbis:N2}</td>");
            sb.AppendLine($"        <td class='num'>RD$ {granTotal:N2}</td>");
            sb.AppendLine($"        <td class='num'>{totalSincronizadas}</td>");
            sb.AppendLine($"        <td class='num'>{totalPendientes}</td>");
            sb.AppendLine("      </tr>");
            sb.AppendLine("    </tbody>");
            sb.AppendLine("  </table>");
            sb.AppendLine($"  <div class='footer'>Generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Módulo de Caja INAIPI</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText(rutaReporte, sb.ToString(), Encoding.UTF8);
            return rutaReporte;
        }
    }
}