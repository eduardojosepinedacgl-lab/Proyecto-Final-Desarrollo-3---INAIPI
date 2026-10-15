using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using INAIPI.CAJA.Data;
using INAIPI.CAJA.Helpers;
using INAIPI.CAJA.Models;
using INAIPI.CAJA.Services;

namespace INAIPI.CAJA.Forms
{
    public partial class frmLoginCaja : Form
    {
        private readonly LocalRepository _repo = new LocalRepository();

        public frmLoginCaja()
        {
            InitializeComponent();
        }

        private async void frmLoginCaja_Load(object sender, EventArgs e)
        {
            try
            {
                lblSincronizando.Text = "Sincronizando catálogos con el CORE...";
                lblSincronizando.ForeColor = System.Drawing.Color.DarkGray;

                var sndClient = new ConexionSNDClient();

                // 1. Descargar Cajeros desde la API del CORE
                bool cajerosSync = await sndClient.SincronizarCajerosAsync();

                // 2. Descargar Padrón de Niños desde la API del CORE
                bool padronSync = await sndClient.SincronizarPadronBeneficiariosAsync();

                if (cajerosSync && padronSync)
                {
                    lblSincronizando.Text = "✔ Catálogos y Padrón actualizados desde el CORE.";
                    lblSincronizando.ForeColor = System.Drawing.Color.SeaGreen;
                }
                else if (cajerosSync || padronSync)
                {
                    lblSincronizando.Text = "⚠ Sincronización parcial. Operando con datos locales.";
                    lblSincronizando.ForeColor = System.Drawing.Color.Goldenrod;
                }
                else
                {
                    lblSincronizando.Text = "⚠ Sin conexión. Modo Offline activado.";
                    lblSincronizando.ForeColor = System.Drawing.Color.Tomato;
                }
            }
            catch
            {
                lblSincronizando.Text = "⚠ Sin conexión. Validando contra caché local.";
                lblSincronizando.ForeColor = System.Drawing.Color.Tomato;
            }
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Ingrese su usuario y contraseña.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow cajeroRow = _repo.ObtenerCajeroPorUsername(user);

            if (cajeroRow == null)
            {
                CajaLogger.Error($"Intento de login con usuario inexistente localmente: {user}");
                MessageBox.Show("Usuario no encontrado en la terminal o inactivo.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool activo = Convert.ToBoolean(cajeroRow["Activo"]);
            int intentos = Convert.ToInt32(cajeroRow["IntentosFallidos"]);
            DateTime? bloqueadoHasta = cajeroRow["BloqueadoHasta"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(cajeroRow["BloqueadoHasta"]) : null;

            if (!activo)
            {
                MessageBox.Show("Este usuario de caja ha sido inhabilitado desde el CORE.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (bloqueadoHasta.HasValue && bloqueadoHasta.Value > DateTime.Now)
            {
                MessageBox.Show($"Usuario bloqueado por múltiples intentos fallidos.\nIntente de nuevo a las {bloqueadoHasta.Value:HH:mm}.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string hashAlmacenado = cajeroRow["PasswordHash"].ToString();
            string hashIngresado = CalcularSHA256Hash(pass);

            if (hashAlmacenado.Equals(hashIngresado, StringComparison.OrdinalIgnoreCase))
            {
                // Login Exitoso
                _repo.LimpiarIntentosLogin(user);

                TurnoCaja.CajeroId = Convert.ToInt32(cajeroRow["EmpleadoId"]);
                TurnoCaja.Username = user;
                TurnoCaja.NombreCajero = cajeroRow["NombreCompleto"].ToString();

                CajaLogger.Info($"Login exitoso en CAJA offline. Trabajador: {TurnoCaja.Username} (ID: {TurnoCaja.CajeroId})");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // Intento Fallido
                intentos++;
                DateTime? bloqueoNuevo = null;
                if (intentos >= 5)
                {
                    bloqueoNuevo = DateTime.Now.AddMinutes(15);
                    CajaLogger.Error($"Usuario {user} bloqueado temporalmente por {intentos} intentos fallidos en Caja.");
                }

                _repo.RegistrarIntentoFallidoLogin(user, intentos, bloqueoNuevo);

                string msg = intentos >= 5
                    ? "Demasiados intentos fallidos. Su cuenta local ha sido bloqueada por 15 minutos."
                    : $"Contraseña incorrecta. Intento {intentos} de 5.";

                MessageBox.Show(msg, "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string CalcularSHA256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}