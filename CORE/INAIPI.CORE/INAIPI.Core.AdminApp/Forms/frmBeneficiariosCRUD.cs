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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static INAIPI.Core.Services.Models.EntidadesDTO;

namespace INAIPI.Core.AdminApp.Forms
{
    public partial class frmBeneficiariosCRUD : Form
    {
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
            CargarComboCentros();
            CargarGrillaBeneficiarios();
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
                        cmbCentros.DataSource = dt;
                        cmbCentros.DisplayMember = "DescripcionCentro";
                        cmbCentros.ValueMember = "CentroId";
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
                MessageBox.Show("Error al consultar beneficiarios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                using (SqlConnection conexion = new SqlConnection(_connectionString))
                using (SqlCommand comando = new SqlCommand("dbo.ppInsertarBeneficiario", conexion)) 
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.Add(new SqlParameter("@Nombres", SqlDbType.NVarChar, 100) { Value = txtNombres.Text.Trim() }); 
                    comando.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = txtApellidos.Text.Trim() }); 
                    comando.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dtpFechaNacimiento.Value.Date }); 
                    comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = Convert.ToInt32(cmbCentros.SelectedValue) }); 
                    comando.Parameters.Add(new SqlParameter("@DocumentoTutor", SqlDbType.NVarChar, 20) { Value = txtDocumentoTutor.Text.Trim() }); 
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

                            if (codigo == 200)
                            {
                                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LimpiarFormulario();
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
                Log.Error("Excepción al ejecutar ppInsertarBeneficiario.", ex); 
                MessageBox.Show("Ocurrió un error al persistir el registro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            int versionOriginal = Convert.ToInt32(txtVersion.Text);

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
                    comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = Convert.ToInt32(cmbCentros.SelectedValue) }); 
                    comando.Parameters.Add(new SqlParameter("@TutorId", SqlDbType.Int) { Value = _tutorIdSeleccionado }); 
                    comando.Parameters.Add(new SqlParameter("@FotografiaUrl", SqlDbType.NVarChar, 500) { Value = DBNull.Value }); 
                    comando.Parameters.Add(new SqlParameter("@VersionOriginal", SqlDbType.Int) { Value = versionOriginal });

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
                            else if (codigo == 409) // Conflicto de Concurrencia Optimista[cite: 8]
                            {
                                Log.WarnFormat("Conflicto de concurrencia detectado para Beneficiario ID {0}.", beneficiarioId);
                                MessageBox.Show(mensaje + "\nLos datos en pantalla serán refrescados.", "Conflicto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                CargarGrillaBeneficiarios();
                            }
                            else
                            {
                                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Excepción al ejecutar ppActualizarBeneficiario.", ex); 
                MessageBox.Show("Fallo de concurrencia o conectividad al actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvBeneficiarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBeneficiarios.SelectedRows.Count > 0)
            {
                DataGridViewRow fila = dgvBeneficiarios.SelectedRows[0];
                txtBeneficiarioId.Text = fila.Cells["BeneficiarioId"].Value.ToString();
                txtNombres.Text = fila.Cells["NombreBeneficiario"].Value.ToString();
                txtApellidos.Text = fila.Cells["ApellidoBeneficiario"].Value.ToString();
                txtVersion.Text = fila.Cells["VersionRegistro"].Value.ToString();
                cmbCentros.SelectedValue = fila.Cells["CentroId"].Value;

                _tutorIdSeleccionado = Convert.ToInt32(fila.Cells["TutorId"].Value);
                txtDocumentoTutor.Text = fila.Cells["DocumentoTutor"].Value.ToString();
                txtTelefonoTutor.Text = fila.Cells["TelefonoTutor"].Value.ToString();
                txtDireccionTutor.Text = fila.Cells["DireccionTutor"].Value.ToString();

                string nombreCompleto = fila.Cells["NombreCompletoTutor"].Value.ToString();
                string[] partes = nombreCompleto.Split(' ');
                txtNombresTutor.Text = partes.Length > 0 ? partes[0] : string.Empty;
                txtApellidosTutor.Text = partes.Length > 1 ? partes[1] : string.Empty;
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombres.Text) || string.IsNullOrWhiteSpace(txtApellidos.Text))
            {
                MessageBox.Show("Nombres y apellidos del beneficiario son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDocumentoTutor.Text) || string.IsNullOrWhiteSpace(txtNombresTutor.Text))
            {
                MessageBox.Show("La información del tutor legal es incompleta.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            dtpFechaNacimiento.Value = DateTime.Now.AddYears(-3);
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
