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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static INAIPI.Core.Services.Models.EntidadesDTO;

namespace INAIPI.Core.AdminApp.Forms
{
    public partial class frmLogin : Form
    {
        private static readonly ILog Log = LoggerHelper.ObtenerLogger(typeof(frmLogin));
        public UsuarioAutenticadoDTO UsuarioAutenticado { get; private set; }

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Debe ingresar usuario y contraseña.";
                return;
            }

            // Validación conforme a política de contraseñas NIST 800-63 (mínimo 12 caracteres)[cite: 8]
            if (password.Length < 12)
            {
                lblError.Text = "La contraseña debe poseer un mínimo estricto de 12 caracteres.";
                return;
            }

            try
            {
                string hash = CalcularSha256(password);
                string connectionString = ConfigurationManager.ConnectionStrings["CnCoreDb"].ConnectionString; 

                using (SqlConnection conexion = new SqlConnection(connectionString))
                using (SqlCommand comando = new SqlCommand("dbo.ppValidarUsuario", conexion)) 
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username }); 
                    comando.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 256) { Value = hash }); 

                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (lector.Read())
                        {
                            UsuarioAutenticado = new UsuarioAutenticadoDTO
                            {
                                UsuarioId = lector.GetInt32(lector.GetOrdinal("UsuarioId")),
                                Username = lector.GetString(lector.GetOrdinal("Username")),
                                Nombres = lector.GetString(lector.GetOrdinal("Nombres")),
                                Apellidos = lector.GetString(lector.GetOrdinal("Apellidos")),
                                Email = lector.GetString(lector.GetOrdinal("Email")),
                                RolId = lector.GetInt32(lector.GetOrdinal("RolId")),
                                NombreRol = lector.GetString(lector.GetOrdinal("NombreRol")),
                                CentroId = lector.IsDBNull(lector.GetOrdinal("CentroId")) ? (int?)null : lector.GetInt32(lector.GetOrdinal("CentroId")),
                                NombreCentro = lector.IsDBNull(lector.GetOrdinal("NombreCentro")) ? null : lector.GetString(lector.GetOrdinal("NombreCentro"))
                            };

                            Log.InfoFormat("Acceso autorizado para el usuario '{0}' con rol '{1}'.", username, UsuarioAutenticado.NombreRol); 
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            Log.WarnFormat("Fallo de autenticación para el usuario '{0}'.", username); 
                            lblError.Text = "Credenciales incorrectas o usuario inactivo.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Excepción durante la validación de credenciales en SQL Server.", ex); 
                lblError.Text = "Error al intentar conectar con el repositorio de datos central.";
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private static string CalcularSha256(string textoPlano)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(textoPlano));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
