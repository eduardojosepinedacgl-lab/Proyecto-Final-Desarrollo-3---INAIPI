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
using System.Security.Cryptography;
using INAIPI.Core.AdminApp.Helpers;
using log4net;

namespace INAIPI.Core.AdminApp.Forms
{
    public partial class frmUsuariosRoles : Form
    {
        private static readonly ILog Log = LoggerHelper.ObtenerLogger(typeof(frmUsuariosRoles));
        private readonly string _connectionString;
        private int _usuarioSeleccionadoId = 0;

        public frmUsuariosRoles()
        {
            InitializeComponent();
            _connectionString = ConfigurationManager.ConnectionStrings["CnCoreDb"].ConnectionString;
        }

        private void frmUsuariosRoles_Load(object sender, EventArgs e)
        {
            CargarRoles();
            CargarCentros();
            CargarUsuarios();
        }

        private void CargarRoles()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT RolId, NombreRol FROM dbo.tblRoles WHERE Estado = 1 ORDER BY NombreRol", con))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    cmbRol.DisplayMember = "NombreRol";
                    cmbRol.ValueMember = "RolId";
                    cmbRol.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al cargar roles.", ex);
            }
        }

        private void CargarCentros()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT CentroId, NombreCentro FROM dbo.tblCentros WHERE Estado = 1 ORDER BY NombreCentro", con))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    // Habilitar nulos explícitamente en la columna antes de insertar
                    dt.Columns["CentroId"].AllowDBNull = true;

                    DataRow rowVacia = dt.NewRow();
                    rowVacia["CentroId"] = DBNull.Value;
                    rowVacia["NombreCentro"] = "[Sede Central / Global]";
                    dt.Rows.InsertAt(rowVacia, 0);

                    cmbCentro.DataSource = null;
                    cmbCentro.DisplayMember = "NombreCentro";
                    cmbCentro.ValueMember = "CentroId";
                    cmbCentro.DataSource = dt;
                    cmbCentro.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al cargar centros.", ex);
                MessageBox.Show("Error cargando centros: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                string sql = @"
                    SELECT 
                        u.UsuarioId,
                        u.Username,
                        u.Nombres,
                        u.Apellidos,
                        u.Email,
                        r.RolId,
                        r.NombreRol,
                        c.CentroId,
                        ISNULL(c.NombreCentro, '[Sede Central]') AS Centro,
                        u.Estado,
                        u.FechaCreacion
                    FROM dbo.tblUsuarios u
                    INNER JOIN dbo.tblRoles r ON u.RolId = r.RolId
                    LEFT JOIN dbo.tblCentros c ON u.CentroId = c.CentroId
                    ORDER BY u.UsuarioId DESC;";

                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dgvUsuarios.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al listar usuarios.", ex);
                MessageBox.Show("Error al cargar lista de usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string nombres = txtNombres.Text.Trim();
            string apellidos = txtApellidos.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
            {
                MessageBox.Show("Los campos Usuario, Nombres y Apellidos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_usuarioSeleccionadoId == 0)
            {
                if (password.Length < 12)
                {
                    MessageBox.Show("La contraseña para un nuevo usuario debe contener mínimo 12 caracteres (NIST 800-63).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                InsertarUsuario(username, password, nombres, apellidos, email);
            }
            else
            {
                ActualizarUsuario(username, password, nombres, apellidos, email);
            }

            LimpiarCampos();
            CargarUsuarios();
        }

        private void InsertarUsuario(string username, string password, string nombres, string apellidos, string email)
        {
            try
            {
                string passHash = CalcularSha256(password);
                string sql = @"
                    INSERT INTO dbo.tblUsuarios (Username, PasswordHash, Nombres, Apellidos, Email, RolId, CentroId, Estado, FechaCreacion)
                    VALUES (@Username, @PasswordHash, @Nombres, @Apellidos, @Email, @RolId, @CentroId, 1, GETDATE());";

                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50) { Value = username });
                    cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarChar, 64) { Value = passHash });
                    cmd.Parameters.Add(new SqlParameter("@Nombres", SqlDbType.NVarChar, 100) { Value = nombres });
                    cmd.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = apellidos });
                    cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = string.IsNullOrEmpty(email) ? (object)DBNull.Value : email });
                    cmd.Parameters.Add(new SqlParameter("@RolId", SqlDbType.Int) { Value = cmbRol.SelectedValue });
                    cmd.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = cmbCentro.SelectedValue ?? DBNull.Value });

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                Log.Info($"Usuario '{username}' registrado exitosamente con RolId {cmbRol.SelectedValue}.");
                MessageBox.Show("Usuario registrado con éxito.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error("Error al registrar usuario.", ex);
                MessageBox.Show("No se pudo registrar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarUsuario(string username, string password, string nombres, string apellidos, string email)
        {
            try
            {
                string sql;
                bool cambiarPass = !string.IsNullOrEmpty(password);

                if (cambiarPass)
                {
                    if (password.Length < 12)
                    {
                        MessageBox.Show("La nueva contraseña debe tener mínimo 12 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    sql = @"
                        UPDATE dbo.tblUsuarios 
                        SET Username = @Username,
                            PasswordHash = @PasswordHash,
                            Nombres = @Nombres,
                            Apellidos = @Apellidos,
                            Email = @Email,
                            RolId = @RolId,
                            CentroId = @CentroId
                        WHERE UsuarioId = @UsuarioId;";
                }
                else
                {
                    sql = @"
                        UPDATE dbo.tblUsuarios 
                        SET Username = @Username,
                            Nombres = @Nombres,
                            Apellidos = @Apellidos,
                            Email = @Email,
                            RolId = @RolId,
                            CentroId = @CentroId
                        WHERE UsuarioId = @UsuarioId;";
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = _usuarioSeleccionadoId });
                    cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50) { Value = username });
                    cmd.Parameters.Add(new SqlParameter("@Nombres", SqlDbType.NVarChar, 100) { Value = nombres });
                    cmd.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = apellidos });
                    cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = string.IsNullOrEmpty(email) ? (object)DBNull.Value : email });
                    cmd.Parameters.Add(new SqlParameter("@RolId", SqlDbType.Int) { Value = cmbRol.SelectedValue });
                    cmd.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = cmbCentro.SelectedValue ?? DBNull.Value });

                    if (cambiarPass)
                    {
                        cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarChar, 64) { Value = CalcularSha256(password) });
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                Log.Info($"Usuario ID {_usuarioSeleccionadoId} ('{username}') actualizado.");
                MessageBox.Show("Usuario actualizado con éxito.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error("Error al actualizar usuario.", ex);
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionadoId == 0)
            {
                MessageBox.Show("Seleccione un usuario de la lista.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = "UPDATE dbo.tblUsuarios SET Estado = CASE WHEN Estado = 1 THEN 0 ELSE 1 END WHERE UsuarioId = @UsuarioId;";
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = _usuarioSeleccionadoId });
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                Log.Info($"Estado alternado para UsuarioId {_usuarioSeleccionadoId}.");
                LimpiarCampos();
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                Log.Error("Error al alternar estado de usuario.", ex);
                MessageBox.Show("Error al modificar estado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvUsuarios.SelectedRows[0];
                _usuarioSeleccionadoId = Convert.ToInt32(row.Cells["UsuarioId"].Value);
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                txtPassword.Text = string.Empty;
                txtNombres.Text = row.Cells["Nombres"].Value.ToString();
                txtApellidos.Text = row.Cells["Apellidos"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value != DBNull.Value ? row.Cells["Email"].Value.ToString() : string.Empty;

                if (cmbRol.Items.Count > 0)
                    cmbRol.SelectedValue = row.Cells["RolId"].Value;

                // Validación defensiva para evitar SelectedIndex = 0 en listas vacías
                if (cmbCentro.Items.Count > 0)
                {
                    if (row.Cells["CentroId"].Value != DBNull.Value)
                        cmbCentro.SelectedValue = row.Cells["CentroId"].Value;
                    else
                        cmbCentro.SelectedIndex = 0;
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            _usuarioSeleccionadoId = 0;
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtNombres.Text = string.Empty;
            txtApellidos.Text = string.Empty;
            txtEmail.Text = string.Empty;
            if (cmbRol.Items.Count > 0) cmbRol.SelectedIndex = 0;
            if (cmbCentro.Items.Count > 0) cmbCentro.SelectedIndex = 0;
            dgvUsuarios.ClearSelection();
        }

        private static string CalcularSha256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}