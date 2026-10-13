using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SqlClient;
using System.Configuration;
using INAIPI.CAJA.Models;

namespace INAIPI.CAJA.Forms
{
    public partial class frmCuadreCaja : Form
    {
        private decimal totalCobrosSistema = 0m;
        private decimal totalEsperado = 0m;

        public frmCuadreCaja()
        {
            InitializeComponent();
        }

        private void frmCuadreCaja_Load(object sender, EventArgs e)
        {
            lblMontoInicial.Text = $"Monto Inicial (Apertura): {TurnoCaja.MontoInicial:C2}";
            totalCobrosSistema = CalcularTotalCobrosBD();
            lblTotalCobros.Text = $"Total Cobrado (Transacciones): {totalCobrosSistema:C2}";
            totalEsperado = TurnoCaja.MontoInicial + totalCobrosSistema;
            lblTotalEsperado.Text = $"Total Esperado en Caja: {totalEsperado:C2}";
        }

        private decimal CalcularTotalCobrosBD()
        {
            decimal suma = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["CnCajaLocal"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT SUM(Importe) FROM dbo.tblTransaccionesLocales WHERE Fecha >= @fechaApertura";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fechaApertura", TurnoCaja.FechaApertura);
                        object result = cmd.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            suma = Convert.ToDecimal(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al auditar base local: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            string reporteAuditoria =
                $"CIERRE DE TURNO | " +
                $"Cajero ID: {TurnoCaja.CajeroId} | " +
                $"Fondo Inicial: {TurnoCaja.MontoInicial:C2} | " +
                $"Cobrado Sistema: {totalCobrosSistema:C2} | " +
                $"Físico Declarado: {fisico:C2} | " +
                $"Diferencia: {diferencia:C2} ({estadoCuadre})";

            if (diferencia < 0)
            {
                INAIPI.CAJA.Helpers.CajaLogger.Error(reporteAuditoria, new Exception("Alerta de Descuadre en Caja"));
            }
            else
            {
                INAIPI.CAJA.Helpers.CajaLogger.Info(reporteAuditoria);
            }

            MessageBox.Show($"Cuadre registrado en la bitácora de auditoría.\n\nResumen:\n{reporteAuditoria}",
                            "Cierre Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Application.Exit();
        }
    }
}