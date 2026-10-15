using INAIPI.Core.AdminApp.Helpers;
using INAIPI.Core.Services.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static INAIPI.Core.Services.Models.EntidadesDTO;

namespace INAIPI.Core.AdminApp.Forms
{
    public partial class frmBeneficiariosCRUD : Form
    {
        public const int EdadMaximaBeneficiarioAnios = 5;
        private static readonly ILog Log = LoggerHelper.ObtenerLogger(typeof(frmBeneficiariosCRUD));
        private readonly UsuarioAutenticadoDTO _usuario;
        private readonly string _connectionString;
        private int _tutorIdSeleccionado = 0;

        public frmBeneficiariosCRUD(UsuarioAutenticadoDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            _connectionString = ConfigurationManager.ConnectionStrings["CnCoreDb"].ConnectionString;
        }

        private void frmBeneficiariosCRUD_Load(object sender, EventArgs e)
        {
            ConfigurarRangoEdadPrimeraInfancia();
            CargarComboCentros();
            CargarGrillaBeneficiarios();
        }

        private void ConfigurarRangoEdadPrimeraInfancia()
        {
            DateTime hoy = DateTime.Today;
            DateTime fechaMinima = hoy.AddYears(-(EdadMaximaBeneficiarioAnios + 1)).AddDays(1);
            dtpFechaNacimiento.MinDate = new DateTime(1900, 1, 1);
            dtpFechaNacimiento.MaxDate = hoy;

            DateTime valorPorDefecto = hoy.AddYears(-3);
            if (valorPorDefecto >= fechaMinima && valorPorDefecto <= hoy)
            {
                dtpFechaNacimiento.Value = valorPorDefecto;
            }
        }

        private void CargarComboCentros()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(_connectionString))
                using (SqlCommand comando = new SqlCommand("SELECT CentroId, (CodigoCentro + ' - ' + NombreCentro) AS DescripcionCentro FROM dbo.tblCentros WHERE Estado = 1 ORDER BY NombreCentro", conexion))
                {
                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(lector);
                        cmbCentros.DisplayMember = "DescripcionCentro";
                        cmbCentros.ValueMember = "CentroId";
                        cmbCentros.DataSource = dt;
                    }
                }

                if (_usuario.CentroId.HasValue)
                {
                    cmbCentros.SelectedValue = _usuario.CentroId.Value;
                    cmbCentros.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al cargar centros en el ComboBox.", ex);
                MessageBox.Show("No se pudieron cargar los centros disponibles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGrillaBeneficiarios()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(_connectionString))
                using (SqlCommand comando = new SqlCommand("dbo.ppGetBeneficiariosPorCentro", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = (object)_usuario.CentroId ?? DBNull.Value });
                    comando.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date) { Value = DBNull.Value });
                    comando.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date) { Value = DBNull.Value });

                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(lector);
                        dgvBeneficiarios.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al poblar la grilla de beneficiarios.", ex);
                MessageBox.Show("Error al consultar beneficiarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            int centroId = Convert.ToInt32(cmbCentros.SelectedValue);
            string documentoLimpio = txtDocumentoTutor.Text.Trim().Replace("-", "");

            try
            {
                using (SqlConnection conexion = new SqlConnection(_connectionString))
                using (SqlCommand comando = new SqlCommand("dbo.ppInsertarBeneficiario", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.Add(new SqlParameter("@Nombres", SqlDbType.NVarChar, 100) { Value = txtNombres.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = txtApellidos.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dtpFechaNacimiento.Value.Date });
                    comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = centroId });
                    comando.Parameters.Add(new SqlParameter("@DocumentoTutor", SqlDbType.NVarChar, 20) { Value = documentoLimpio });
                    comando.Parameters.Add(new SqlParameter("@NombresTutor", SqlDbType.NVarChar, 100) { Value = txtNombresTutor.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@ApellidosTutor", SqlDbType.NVarChar, 100) { Value = txtApellidosTutor.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@TelefonoTutor", SqlDbType.NVarChar, 20) { Value = txtTelefonoTutor.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@DireccionTutor", SqlDbType.NVarChar, 250) { Value = txtDireccionTutor.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@FotografiaUrl", SqlDbType.NVarChar, 500) { Value = DBNull.Value });

                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (lector.Read())
                        {
                            int codigo = lector.GetInt32(lector.GetOrdinal("Codigo"));
                            string mensaje = lector.GetString(lector.GetOrdinal("Mensaje"));
                            string detalle = lector.IsDBNull(lector.GetOrdinal("DetalleTecnico"))
                                ? string.Empty
                                : lector.GetString(lector.GetOrdinal("DetalleTecnico"));

                            if (codigo == 200)
                            {
                                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LimpiarFormulario();
                                CargarGrillaBeneficiarios();
                            }
                            else
                            {
                                Log.WarnFormat("Rechazo al insertar beneficiario (Código {0}): {1} | Detalle: {2}", codigo, mensaje, detalle);
                                MessageBox.Show(mensaje, "Validación del Servidor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Excepción al ejecutar ppInsertarBeneficiario.", ex);
                MessageBox.Show("No se pudo guardar el beneficiario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBeneficiarioId.Text))
            {
                MessageBox.Show("Seleccione un registro de la lista para actualizar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidarCampos()) return;

            int beneficiarioId = Convert.ToInt32(txtBeneficiarioId.Text);
            int versionOriginal = string.IsNullOrWhiteSpace(txtVersion.Text) ? 1 : Convert.ToInt32(txtVersion.Text);
            int centroId = Convert.ToInt32(cmbCentros.SelectedValue);
            string documentoLimpio = txtDocumentoTutor.Text.Trim().Replace("-", "");

            try
            {
                using (SqlConnection conexion = new SqlConnection(_connectionString))
                using (SqlCommand comando = new SqlCommand("dbo.ppActualizarBeneficiario", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.Add(new SqlParameter("@BeneficiarioId", SqlDbType.Int) { Value = beneficiarioId });
                    comando.Parameters.Add(new SqlParameter("@Nombres", SqlDbType.NVarChar, 100) { Value = txtNombres.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = txtApellidos.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dtpFechaNacimiento.Value.Date });
                    comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = centroId });
                    comando.Parameters.Add(new SqlParameter("@TutorId", SqlDbType.Int) { Value = _tutorIdSeleccionado });
                    comando.Parameters.Add(new SqlParameter("@FotografiaUrl", SqlDbType.NVarChar, 500) { Value = DBNull.Value });
                    comando.Parameters.Add(new SqlParameter("@VersionOriginal", SqlDbType.Int) { Value = versionOriginal });
                    comando.Parameters.Add(new SqlParameter("@DocumentoTutor", SqlDbType.NVarChar, 20) { Value = documentoLimpio });
                    comando.Parameters.Add(new SqlParameter("@NombresTutor", SqlDbType.NVarChar, 100) { Value = txtNombresTutor.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@ApellidosTutor", SqlDbType.NVarChar, 100) { Value = txtApellidosTutor.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@TelefonoTutor", SqlDbType.NVarChar, 20) { Value = txtTelefonoTutor.Text.Trim() });
                    comando.Parameters.Add(new SqlParameter("@DireccionTutor", SqlDbType.NVarChar, 250) { Value = txtDireccionTutor.Text.Trim() });

                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (lector.Read())
                        {
                            int codigo = lector.GetInt32(lector.GetOrdinal("Codigo"));
                            string mensaje = lector.GetString(lector.GetOrdinal("Mensaje"));

                            if (codigo == 200)
                            {
                                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LimpiarFormulario();
                                CargarGrillaBeneficiarios();
                            }
                            else if (codigo == 409)
                            {
                                Log.WarnFormat("Conflicto de concurrencia detectado para Beneficiario ID {0}.", beneficiarioId);
                                MessageBox.Show(mensaje + "\nLos datos en pantalla serán refrescados.", "Conflicto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                CargarGrillaBeneficiarios();
                            }
                            else
                            {
                                MessageBox.Show(mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Excepción al ejecutar ppActualizarBeneficiario.", ex);
                MessageBox.Show("Error al actualizar el beneficiario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvBeneficiarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBeneficiarios.SelectedRows.Count > 0)
            {
                DataGridViewRow fila = dgvBeneficiarios.SelectedRows[0];
                if (fila.Cells["BeneficiarioId"].Value == null || fila.Cells["BeneficiarioId"].Value == DBNull.Value)
                    return;

                txtBeneficiarioId.Text = fila.Cells["BeneficiarioId"].Value.ToString();
                txtNombres.Text = Convert.ToString(fila.Cells["NombreBeneficiario"].Value);
                txtApellidos.Text = Convert.ToString(fila.Cells["ApellidoBeneficiario"].Value);
                txtVersion.Text = Convert.ToString(fila.Cells["VersionRegistro"].Value);
                cmbCentros.SelectedValue = fila.Cells["CentroId"].Value;

                if (fila.DataGridView.Columns.Contains("FechaNacimientoStr") &&
                    DateTime.TryParseExact(Convert.ToString(fila.Cells["FechaNacimientoStr"].Value), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaNac))
                {
                    if (fechaNac >= dtpFechaNacimiento.MinDate && fechaNac <= dtpFechaNacimiento.MaxDate)
                        dtpFechaNacimiento.Value = fechaNac;
                }

                _tutorIdSeleccionado = Convert.ToInt32(fila.Cells["TutorId"].Value);
                txtDocumentoTutor.Text = Convert.ToString(fila.Cells["DocumentoTutor"].Value);
                txtTelefonoTutor.Text = Convert.ToString(fila.Cells["TelefonoTutor"].Value);
                txtDireccionTutor.Text = Convert.ToString(fila.Cells["DireccionTutor"].Value);

                if (fila.DataGridView.Columns.Contains("NombresTutor") && fila.DataGridView.Columns.Contains("ApellidosTutor"))
                {
                    txtNombresTutor.Text = Convert.ToString(fila.Cells["NombresTutor"].Value);
                    txtApellidosTutor.Text = Convert.ToString(fila.Cells["ApellidosTutor"].Value);
                }
                else
                {
                    string nombreCompleto = Convert.ToString(fila.Cells["NombreCompletoTutor"].Value);
                    string[] partes = nombreCompleto.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    txtNombresTutor.Text = partes.Length > 0 ? partes[0] : string.Empty;
                    txtApellidosTutor.Text = partes.Length > 1 ? partes[1] : string.Empty;
                }
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombres.Text) || string.IsNullOrWhiteSpace(txtApellidos.Text))
            {
                MessageBox.Show("Los nombres y apellidos del beneficiario son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DateTime hoy = DateTime.Today;
            DateTime fechaMinima = hoy.AddYears(-(EdadMaximaBeneficiarioAnios + 1)).AddDays(1);
            DateTime fechaNac = dtpFechaNacimiento.Value.Date;
            if (fechaNac > hoy || fechaNac < fechaMinima)
            {
                MessageBox.Show($"La fecha de nacimiento es inválida. El beneficiario debe tener entre 0 y {EdadMaximaBeneficiarioAnios} años cumplidos.", "Validación de Edad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbCentros.SelectedValue == null || !int.TryParse(cmbCentros.SelectedValue.ToString(), out int centroId) || centroId <= 0)
            {
                MessageBox.Show("Debe seleccionar un centro de atención válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string docLimpio = txtDocumentoTutor.Text.Trim().Replace("-", "");
            if (string.IsNullOrWhiteSpace(docLimpio) || docLimpio.Length != 11 || !docLimpio.All(char.IsDigit))
            {
                MessageBox.Show("La cédula del tutor debe contener exactamente 11 dígitos numéricos (sin guiones).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombresTutor.Text) ||
                string.IsNullOrWhiteSpace(txtApellidosTutor.Text) ||
                string.IsNullOrWhiteSpace(txtTelefonoTutor.Text) ||
                string.IsNullOrWhiteSpace(txtDireccionTutor.Text))
            {
                MessageBox.Show("Todos los datos del tutor legal (nombres, apellidos, teléfono y dirección) son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarFormulario()
        {
            txtBeneficiarioId.Text = string.Empty;
            txtNombres.Text = string.Empty;
            txtApellidos.Text = string.Empty;
            txtVersion.Text = string.Empty;
            txtDocumentoTutor.Text = string.Empty;
            txtNombresTutor.Text = string.Empty;
            txtApellidosTutor.Text = string.Empty;
            txtTelefonoTutor.Text = string.Empty;
            txtDireccionTutor.Text = string.Empty;
            _tutorIdSeleccionado = 0;
            dtpFechaNacimiento.Value = DateTime.Today.AddYears(-3);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            CargarGrillaBeneficiarios();
        }
    }
}