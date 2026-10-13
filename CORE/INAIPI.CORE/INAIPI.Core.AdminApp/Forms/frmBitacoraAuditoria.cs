using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using INAIPI.Core.AdminApp.Helpers;
using log4net;

namespace INAIPI.Core.AdminApp.Forms
{
    public partial class frmBitacoraAuditoria : Form
    {
        private static readonly ILog Log = LoggerHelper.ObtenerLogger(typeof(frmBitacoraAuditoria));
        private readonly string _connectionString;

        public frmBitacoraAuditoria()
        {
            InitializeComponent();
            _connectionString = ConfigurationManager.ConnectionStrings["CnCoreDb"].ConnectionString;
        }

        private void frmBitacoraAuditoria_Load(object sender, EventArgs e)
        {
            cmbNivel.SelectedIndex = 0;
            CargarBitacora();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        private void CargarBitacora()
        {
            try
            {
                string query = @"
                    SELECT TOP 200 
                        Id, 
                        Date AS [Fecha/Hora], 
                        Thread AS [Hilo], 
                        Level AS [Nivel], 
                        Logger AS [Origen], 
                        Message AS [Mensaje], 
                        ISNULL(Exception, '') AS [Excepción]
                    FROM dbo.Log4NetLog
                    WHERE CAST(Date AS DATE) <= @FechaFiltro
                      AND (@Nivel = 'TODOS' OR Level = @Nivel)
                    ORDER BY Date DESC;";

                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@FechaFiltro", SqlDbType.Date) { Value = dtpFecha.Value.Date });
                    cmd.Parameters.Add(new SqlParameter("@Nivel", SqlDbType.VarChar, 50) { Value = cmbNivel.SelectedItem.ToString() });

                    con.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dgvLogs.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al consultar bitácora de auditoría.", ex);
                MessageBox.Show("No se pudo obtener la bitácora: " + ex.Message, "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}