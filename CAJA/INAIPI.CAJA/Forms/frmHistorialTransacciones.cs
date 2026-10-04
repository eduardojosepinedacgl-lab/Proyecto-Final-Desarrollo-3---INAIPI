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
    }
}