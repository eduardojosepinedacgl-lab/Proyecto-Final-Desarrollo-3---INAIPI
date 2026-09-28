using INAIPI.Core.AdminApp.Helpers;
using log4net;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace INAIPI.Core.AdminApp.Forms
{
    public partial class frmReporteViewer : Form
    {
        private static readonly ILog Log = LoggerHelper.ObtenerLogger(typeof(frmReporteViewer));
        private readonly string _connectionString;

        public frmReporteViewer()
        {
            InitializeComponent();
            _connectionString = ConfigurationManager.ConnectionStrings["CnCoreDb"].ConnectionString; 
        }

        private void frmReporteViewer_Load(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Now.AddMonths(-6);
            dtpHasta.Value = DateTime.Now;
            CargarComboCentros();
            reportViewerCore.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local;
        }

        private void CargarComboCentros()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(_connectionString))
                using (SqlCommand comando = new SqlCommand("SELECT CentroId, (CodigoCentro + ' - ' + NombreCentro) AS Descripcion FROM dbo.tblCentros WHERE Estado = 1 ORDER BY NombreCentro", conexion))
                {
                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(lector);

                        DataRow filaTodos = dt.NewRow();
                        filaTodos["CentroId"] = 0;
                        filaTodos["Descripcion"] = "-- Todos los Centros --";
                        dt.Rows.InsertAt(filaTodos, 0);

                        cmbCentroFiltro.DataSource = dt;
                        cmbCentroFiltro.DisplayMember = "Descripcion";
                        cmbCentroFiltro.ValueMember = "CentroId";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al cargar centros en el filtro del reporte.", ex); 
            }
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            try
            {
                int centroSeleccionado = Convert.ToInt32(cmbCentroFiltro.SelectedValue);
                int? centroIdParam = centroSeleccionado == 0 ? (int?)null : centroSeleccionado;
                DateTime fechaInicio = dtpDesde.Value.Date;
                DateTime fechaFin = dtpHasta.Value.Date;

                DataTable dtDatos = new DataTable("dtBeneficiarios");

                using (SqlConnection conexion = new SqlConnection(_connectionString))
                using (SqlCommand comando = new SqlCommand("dbo.ppGetBeneficiariosPorCentro", conexion)) 
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = (object)centroIdParam ?? DBNull.Value }); 
                    comando.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date) { Value = fechaInicio }); 
                    comando.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date) { Value = fechaFin }); 

                    conexion.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(comando))
                    {
                        da.Fill(dtDatos); 
                    }
                }

                string rutaReporte = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Reports\rptBeneficiariosPorCentro.rdlc"); 
                if (!File.Exists(rutaReporte))
                {
                    MessageBox.Show("No se encontró la definición del reporte RDLC en la ruta esperada:\n" + rutaReporte, "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                reportViewerCore.LocalReport.DataSources.Clear();
                reportViewerCore.LocalReport.ReportPath = rutaReporte; 

                Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("dsBeneficiariosCentro", dtDatos); 
                reportViewerCore.LocalReport.DataSources.Add(rds); 

                reportViewerCore.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewerCore.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                reportViewerCore.ZoomPercent = 100;
                reportViewerCore.RefreshReport(); 

                Log.InfoFormat("Reporte RDLC generado con {0} registros.", dtDatos.Rows.Count); 
            }
            catch (Exception ex)
            {
                Log.Error("Error al generar y renderizar el reporte RDLC.", ex); 
                MessageBox.Show("Error durante la generación del reporte:\n" + ex.Message, "Error de Reporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
