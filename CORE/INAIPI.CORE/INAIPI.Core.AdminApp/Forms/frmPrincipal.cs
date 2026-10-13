using INAIPI.Core.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static INAIPI.Core.Services.Models.EntidadesDTO;
using INAIPI.Core.AdminApp.Helpers;
using log4net;

namespace INAIPI.Core.AdminApp.Forms
{
    public partial class frmPrincipal : Form
    {
        private static readonly ILog Log = LoggerHelper.ObtenerLogger(typeof(frmPrincipal));
        private readonly UsuarioAutenticadoDTO _usuarioSesion;

        public frmPrincipal(UsuarioAutenticadoDTO usuario)
        {
            InitializeComponent();
            _usuarioSesion = usuario;
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            if (_usuarioSesion == null)
            {
                MessageBox.Show("Sesión no válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Despliegue de metadatos en la barra de estado
            lblStatusUsuario.Text = $"Usuario: {_usuarioSesion.Username} | Rol: {_usuarioSesion.NombreRol} | Centro: {_usuarioSesion.NombreCentro ?? "Sede Central"}";

            // Aplicación de Políticas RBAC
            AplicarPoliticasAcceso();
        }

        private void AplicarPoliticasAcceso()
        {
            string rol = _usuarioSesion.NombreRol?.Trim();

            switch (rol)
            {
                case "Cajero":
                    // Rechazo total y redirección funcional hacia el canal de caja
                    MessageBox.Show(
                        "Tu rol no tiene pantallas en esta aplicación. El trabajo de ventanilla se hace en la aplicación de Caja.",
                        "Acceso Denegado por Política",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    Log.Warn($"Usuario '{_usuarioSesion.Username}' con rol 'Cajero' intentó ingresar a AdminApp.");
                    this.Close();
                    Application.Exit();
                    break;

                case "Auditor":
                    // Restricción a modo de sólo lectura e inspección
                    menuBeneficiarios.Visible = false;
                    menuUsuarios.Visible = false;
                    menuReportes.Visible = true;
                    menuAuditoria.Visible = true;
                    Log.Info($"Sesión de Auditor iniciada: {_usuarioSesion.Username}. Menús de edición bloqueados.");
                    break;

                case "Coordinador":
                    // Acceso a operaciones con beneficiarios y reportes, sin gestión de usuarios ni auditoría interna
                    menuBeneficiarios.Visible = true;
                    menuReportes.Visible = true;
                    menuUsuarios.Visible = false;
                    menuAuditoria.Visible = false;
                    Log.Info($"Sesión de Coordinador iniciada: {_usuarioSesion.Username}.");
                    break;

                case "Administrador":
                    // Acceso total
                    menuBeneficiarios.Visible = true;
                    menuReportes.Visible = true;
                    menuUsuarios.Visible = true;
                    menuAuditoria.Visible = true;
                    Log.Info($"Sesión Administrativa completa iniciada: {_usuarioSesion.Username}.");
                    break;

                default:
                    MessageBox.Show("Perfil de usuario sin permisos configurados.", "Acceso Restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    menuBeneficiarios.Visible = false;
                    menuReportes.Visible = false;
                    menuUsuarios.Visible = false;
                    menuAuditoria.Visible = false;
                    break;
            }
        }

        private void menuBeneficiarios_Click(object sender, EventArgs e)
        {
            foreach (Form form in this.MdiChildren)
            {
                if (form is frmBeneficiariosCRUD)
                {
                    form.Activate();
                    return;
                }
            }

            frmBeneficiariosCRUD frm = new frmBeneficiariosCRUD(_usuarioSesion);
            frm.MdiParent = this;
            frm.Show();
        }

        private void menuReportes_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo<frmReporteViewer>();
        }

        private void menuUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo<frmUsuariosRoles>();
        }

        private void menuAuditoria_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo<frmBitacoraAuditoria>();
        }

        private void menuSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AbrirFormularioHijo<T>() where T : Form, new()
        {
            foreach (Form form in this.MdiChildren)
            {
                if (form is T)
                {
                    form.Activate();
                    return;
                }
            }

            T nuevoForm = new T();
            nuevoForm.MdiParent = this;
            nuevoForm.Show();
        }
    }
}
