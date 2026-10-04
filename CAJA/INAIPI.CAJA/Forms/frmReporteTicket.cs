using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace INAIPI.CAJA.Forms
{
    public partial class frmReporteTicket : Form
    {
        private string _guid, _beneficiario, _servicio, _importe, _fecha;

        // Constructor que recibe los datos exactos a imprimir
        public frmReporteTicket(string guid, string beneficiario, string servicio, string importe, string fecha)
        {
            InitializeComponent();
            _guid = guid;
            _beneficiario = beneficiario;
            _servicio = servicio;
            _importe = importe;
            _fecha = fecha;
        }

        private void frmReporteTicket_Load(object sender, EventArgs e)
        {
            try
            {
                // Limpiar orígenes previos
                reportViewer1.LocalReport.DataSources.Clear();

                // Crear un DataTable temporal que coincida con tu dsCajaLocal
                DataTable dt = new DataTable();
                dt.Columns.Add("TransaccionGuid");
                dt.Columns.Add("BeneficiarioId");
                dt.Columns.Add("DescripcionServicio");
                dt.Columns.Add("Importe");
                dt.Columns.Add("Fecha");

                // Inyectar la fila con la información recibida
                dt.Rows.Add(_guid, _beneficiario, _servicio, _importe, _fecha);

                // Vincular los datos al reporte (El nombre "DataSetCajaLocal" debe coincidir con el nombre que le diste al agregarlo en el RDLC)
                ReportDataSource rds = new ReportDataSource("DataSetCajaLocal", dt);
                reportViewer1.LocalReport.DataSources.Add(rds);

                // Refrescar el visor para mostrar el ticket
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el ticket: {ex.Message}", "Fallo de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}