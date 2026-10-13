using System;
using System.Windows.Forms;
using INAIPI.CAJA.Data;

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
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                var repo = new LocalRepository();
                dgvHistorial.DataSource = repo.ObtenerHistorialTransacciones();

                // Darle formato de moneda a la columna Importe visualmente
                if (dgvHistorial.Columns["Importe"] != null)
                {
                    dgvHistorial.Columns["Importe"].DefaultCellStyle.Format = "C2";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al consultar el historial local: {ex.Message}", "Fallo de Lectura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHistorial_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow filaSeleccionada = dgvHistorial.Rows[e.RowIndex];

                    string guid = filaSeleccionada.Cells["TransaccionGuid"].Value.ToString();
                    string beneficiario = filaSeleccionada.Cells["BeneficiarioId"].Value.ToString();
                    string servicio = filaSeleccionada.Cells["DescripcionServicio"].Value.ToString();

                    string monto = Convert.ToDecimal(filaSeleccionada.Cells["Importe"].Value).ToString("F2");
                    string fecha = Convert.ToDateTime(filaSeleccionada.Cells["Fecha"].Value).ToString("dd/MM/yyyy HH:mm:ss");

                    frmReporteTicket frmTicket = new frmReporteTicket(guid, beneficiario, servicio, monto, fecha);

                    frmTicket.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar visualizar el recibo: " + ex.Message, "Error de Lectura", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}