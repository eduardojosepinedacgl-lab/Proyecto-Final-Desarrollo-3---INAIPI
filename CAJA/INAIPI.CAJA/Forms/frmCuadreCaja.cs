using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using INAIPI.CAJA.Models;
using INAIPI.CAJA.Data;
using INAIPI.CAJA.Helpers;

namespace INAIPI.CAJA.Forms
{
    public partial class frmCuadreCaja : Form
    {
        private decimal totalCobrosSistema = 0m;
        private decimal totalEsperado = 0m;
        private DataTable dtCuadre;

        public frmCuadreCaja()
        {
            InitializeComponent();
        }

        private void frmCuadreCaja_Load(object sender, EventArgs e)
        {
            lblMontoInicial.Text = $"Monto Inicial (Apertura): {TurnoCaja.MontoInicial:C2}";

            var repo = new LocalRepository();
            dtCuadre = repo.ObtenerCuadreDeTurno(TurnoCaja.CajeroId, TurnoCaja.FechaApertura, DateTime.Now);

            totalCobrosSistema = CalcularTotalDesdeDataTable(dtCuadre);

            lblTotalCobros.Text = $"Total Cobrado (Transacciones): {totalCobrosSistema:C2}";
            totalEsperado = TurnoCaja.MontoInicial + totalCobrosSistema;
            lblTotalEsperado.Text = $"Total Esperado en Caja: {totalEsperado:C2}";
        }

        private decimal CalcularTotalDesdeDataTable(DataTable dt)
        {
            decimal suma = 0m;
            foreach (DataRow row in dt.Rows)
            {
                suma += Convert.ToDecimal(row["TotalRecaudado"]);
            }
            return suma;
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtEfectivoFisico.Text, out decimal fisico))
            {
                decimal diferencia = fisico - totalEsperado;
                lblDiferencia.Text = $"Diferencia: {diferencia:C2}";

                if (diferencia == 0)
                {
                    lblDiferencia.ForeColor = Color.SeaGreen;
                    lblDiferencia.Text = $"Diferencia: {diferencia:C2} (CUADRE PERFECTO)";
                }
                else if (diferencia > 0)
                {
                    lblDiferencia.ForeColor = Color.SteelBlue;
                    lblDiferencia.Text = $"Diferencia: {diferencia:C2} (SOBRANTE)";
                }
                else
                {
                    lblDiferencia.ForeColor = Color.Firebrick;
                    lblDiferencia.Text = $"Diferencia: {diferencia:C2} (FALTANTE)";
                }

                btnCerrarTurno.Enabled = true;
            }
            else
            {
                MessageBox.Show("Ingrese el efectivo físico correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCerrarTurno_Click(object sender, EventArgs e)
        {
            decimal.TryParse(txtEfectivoFisico.Text, out decimal fisico);
            decimal diferencia = fisico - totalEsperado;
            string estadoCuadre = diferencia == 0 ? "PERFECTO" : (diferencia > 0 ? "SOBRANTE" : "FALTANTE");
            DateTime fechaCierre = DateTime.Now;

            string reporteAuditoria =
                $"CIERRE DE TURNO | " +
                $"Cajero ID: {TurnoCaja.CajeroId} | " +
                $"Fondo Inicial: {TurnoCaja.MontoInicial:C2} | " +
                $"Cobrado Sistema: {totalCobrosSistema:C2} | " +
                $"Físico Declarado: {fisico:C2} | " +
                $"Diferencia: {diferencia:C2} ({estadoCuadre})";

            if (diferencia < 0)
            {
                CajaLogger.Error(reporteAuditoria, new Exception("Alerta de Descuadre en Caja"));
            }
            else
            {
                CajaLogger.Info(reporteAuditoria);
            }

            MessageBox.Show($"Cuadre registrado en la bitácora de auditoría.\n\nSe generará el reporte de cierre en PDF.",
                            "Cierre Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Generar el reporte antes de salir
            GenerarReporteCierrePDF(dtCuadre, TurnoCaja.FechaApertura, fechaCierre, fisico, diferencia, estadoCuadre);

            TurnoCaja.TurnoAbierto = false;
            Application.Exit();
        }

        private void GenerarReporteCierrePDF(DataTable dt, DateTime apertura, DateTime cierre, decimal fisicoDeclarado, decimal diferencia, string estadoCuadre)
        {
            string rutaReporte = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Cierre_Turno_{TurnoCaja.Username}_{cierre:yyyyMMdd_HHmm}.html");
            int totalTransacciones = 0;

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html lang='es'><head><meta charset='utf-8'><title>Cuadre de Turno</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("  body { font-family: Segoe UI, sans-serif; margin: 30px; color: #333; }");
            sb.AppendLine("  h2 { color: #006837; margin-bottom: 5px; }");
            sb.AppendLine("  .meta { font-size: 0.95em; color: #555; margin-bottom: 20px; line-height: 1.5; }");
            sb.AppendLine("  table { width: 100%; border-collapse: collapse; margin-top: 15px; margin-bottom: 20px; }");
            sb.AppendLine("  th { background-color: #006837; color: white; padding: 10px; text-align: left; }");
            sb.AppendLine("  td { padding: 8px 10px; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("  .num { text-align: right; }");
            sb.AppendLine("  .resumen { width: 50%; float: right; border: 1px solid #ddd; border-radius: 5px; padding: 15px; background-color: #f4f6f6; }");
            sb.AppendLine("  .resumen-row { display: flex; justify-content: space-between; margin-bottom: 8px; font-size: 1.1em; }");
            sb.AppendLine("  .total-final { font-size: 1.2em; font-weight: bold; color: #006837; border-top: 2px solid #006837; padding-top: 10px; margin-top: 5px; }");
            sb.AppendLine("  .diferencia { font-size: 1.1em; font-weight: bold; padding-top: 10px; color: #d9534f; }");
            sb.AppendLine("  .btn-pdf { background-color: #006837; color: white; border: none; padding: 9px 18px; cursor: pointer; font-weight: bold; border-radius: 4px; }");
            sb.AppendLine("  @media print { .btn-pdf { display: none; } body { margin: 10mm; } }");
            sb.AppendLine("</style></head><body>");

            sb.AppendLine("<button class='btn-pdf' onclick='window.print()'>Descargar PDF de Cuadre</button>");
            sb.AppendLine("<h2>INAIPI - Cuadre de Turno (Ventanilla)</h2>");
            sb.AppendLine("<div class='meta'>");
            sb.AppendLine($"  <strong>Cajero:</strong> {TurnoCaja.NombreCajero} (Username: {TurnoCaja.Username})<br>");
            sb.AppendLine($"  <strong>Apertura:</strong> {apertura:dd/MM/yyyy hh:mm tt}<br>");
            sb.AppendLine($"  <strong>Cierre:</strong> {cierre:dd/MM/yyyy hh:mm tt}<br>");
            sb.AppendLine($"  <strong>Terminal:</strong> {Environment.MachineName}");
            sb.AppendLine("</div>");

            sb.AppendLine("<table><thead><tr><th>Servicio</th><th class='num'>Cant.</th><th class='num'>Subtotal</th><th class='num'>ITBIS</th><th class='num'>Total Cobrado</th></tr></thead><tbody>");

            foreach (DataRow row in dt.Rows)
            {
                int cant = Convert.ToInt32(row["CantidadTransacciones"]);
                decimal sub = Convert.ToDecimal(row["TotalSubtotal"]);
                decimal itb = Convert.ToDecimal(row["TotalItbis"]);
                decimal tot = Convert.ToDecimal(row["TotalRecaudado"]);
                totalTransacciones += cant;

                sb.AppendLine($"<tr><td>{row["DescripcionServicio"]}</td><td class='num'>{cant}</td><td class='num'>RD$ {sub:N2}</td><td class='num'>RD$ {itb:N2}</td><td class='num'><strong>RD$ {tot:N2}</strong></td></tr>");
            }
            sb.AppendLine("</tbody></table>");

            sb.AppendLine("<div class='resumen'>");
            sb.AppendLine($"  <div class='resumen-row'><span>Transacciones Procesadas:</span><span>{totalTransacciones}</span></div>");
            sb.AppendLine($"  <div class='resumen-row'><span>Fondo Inicial (Gaveta):</span><span>RD$ {TurnoCaja.MontoInicial:N2}</span></div>");
            sb.AppendLine($"  <div class='resumen-row'><span>Ventas Registradas:</span><span>RD$ {totalCobrosSistema:N2}</span></div>");
            sb.AppendLine($"  <div class='resumen-row total-final'><span>Total Esperado:</span><span>RD$ {totalEsperado:N2}</span></div>");
            sb.AppendLine($"  <div class='resumen-row'><span>Físico Declarado:</span><span>RD$ {fisicoDeclarado:N2}</span></div>");

            string colorDif = diferencia == 0 ? "#006837" : (diferencia > 0 ? "#337ab7" : "#d9534f");
            sb.AppendLine($"  <div class='resumen-row diferencia' style='color: {colorDif};'><span>Diferencia ({estadoCuadre}):</span><span>RD$ {diferencia:N2}</span></div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div style='clear: both;'></div>");

            sb.AppendLine("<div style='display: flex; justify-content: space-around; text-align: center; margin-top: 80px;'>");
            sb.AppendLine("  <div style='border-top: 1px solid #333; width: 200px; padding-top: 5px;'>Firma Cajero</div>");
            sb.AppendLine("  <div style='border-top: 1px solid #333; width: 200px; padding-top: 5px;'>Firma Supervisor</div>");
            sb.AppendLine("</div>");

            sb.AppendLine("</body></html>");

            File.WriteAllText(rutaReporte, sb.ToString(), Encoding.UTF8);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(rutaReporte) { UseShellExecute = true });
        }
    }
}