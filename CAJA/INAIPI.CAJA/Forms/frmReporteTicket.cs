using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace INAIPI.CAJA.Forms
{
    public partial class frmReporteTicket : Form
    {
        private readonly string _guid, _beneficiario, _servicio, _subtotal, _itbis, _total, _fecha;
        private readonly string _cedula, _cajero;

        public frmReporteTicket(string guid, string beneficiario, string servicio,
            string subtotal, string itbis, string total, string fecha,
            string cedula = null, string cajero = null)
        {
            InitializeComponent();
            _guid = guid;
            _beneficiario = beneficiario;
            _servicio = servicio;
            _subtotal = subtotal;
            _itbis = itbis;
            _total = total;
            _fecha = fecha;
            _cedula = string.IsNullOrWhiteSpace(cedula) ? "N/D" : cedula;
            _cajero = string.IsNullOrWhiteSpace(cajero) ? "N/D" : cajero;
        }

        private void frmReporteTicket_Load(object sender, EventArgs e)
        {
            try
            {
                reportViewer1.LocalReport.DataSources.Clear();

                // Los nombres de columna deben coincidir con los campos de DataSetCajaLocal en el RDLC.
                DataTable dt = new DataTable();
                dt.Columns.Add("TransaccionGuid");
                dt.Columns.Add("BeneficiarioId");   // trae "Nombre (Exp. N)"
                dt.Columns.Add("DescripcionServicio");
                dt.Columns.Add("Subtotal");
                dt.Columns.Add("Itbis");
                dt.Columns.Add("Importe");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Cedula");
                dt.Columns.Add("Cajero");

                dt.Rows.Add(_guid, _beneficiario, _servicio, _subtotal, _itbis,
                            _total, _fecha, _cedula, _cajero);

                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetCajaLocal", dt));
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el ticket: {ex.Message}", "Fallo de Impresión",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}