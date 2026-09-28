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

namespace INAIPI.Core.AdminApp.Forms
{
    public partial class frmPrincipal : Form
    {
        private readonly UsuarioAutenticadoDTO _usuario;

        public frmPrincipal(UsuarioAutenticadoDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            ConfigurarSesion();
        }

        private void ConfigurarSesion()
        {
            lblStatusUsuario.Text = string.Format("Usuario: {0} ({1})", _usuario.Username, _usuario.NombreRol);
            lblStatusCentro.Text = string.Format("Centro: {0}", _usuario.NombreCentro ?? "Administración General");
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

            frmBeneficiariosCRUD frm = new frmBeneficiariosCRUD(_usuario);
            frm.MdiParent = this; 
            frm.Show(); 
        }

        private void menuReporteBeneficiarios_Click(object sender, EventArgs e)
        {
            foreach (Form form in this.MdiChildren)
            {
                if (form is frmReporteViewer)
                {
                    form.Activate();
                    return;
                }
            }

            frmReporteViewer frm = new frmReporteViewer();
            frm.MdiParent = this; 
            frm.Show(); 
        }

        private void menuSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
