namespace INAIPI.Core.AdminApp.Forms
{
    partial class frmPrincipal
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuGestion;
        private System.Windows.Forms.ToolStripMenuItem menuBeneficiarios;
        private System.Windows.Forms.ToolStripMenuItem menuReportes;
        private System.Windows.Forms.ToolStripMenuItem menuUsuarios;
        private System.Windows.Forms.ToolStripMenuItem menuAuditoria;
        private System.Windows.Forms.ToolStripMenuItem menuSalir;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusUsuario;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuGestion = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBeneficiarios = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReportes = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUsuarios = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAuditoria = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSalir = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatusUsuario = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGestion,
            this.menuSalir});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1184, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // menuGestion
            // 
            this.menuGestion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBeneficiarios,
            this.menuReportes,
            this.menuUsuarios,
            this.menuAuditoria});
            this.menuGestion.Name = "menuGestion";
            this.menuGestion.Size = new System.Drawing.Size(59, 20);
            this.menuGestion.Text = "Gestión";
            // 
            // menuBeneficiarios
            // 
            this.menuBeneficiarios.Name = "menuBeneficiarios";
            this.menuBeneficiarios.Size = new System.Drawing.Size(185, 22);
            this.menuBeneficiarios.Text = "Beneficiarios";
            this.menuBeneficiarios.Click += new System.EventHandler(this.menuBeneficiarios_Click);
            // 
            // menuReportes
            // 
            this.menuReportes.Name = "menuReportes";
            this.menuReportes.Size = new System.Drawing.Size(185, 22);
            this.menuReportes.Text = "Reportes";
            this.menuReportes.Click += new System.EventHandler(this.menuReportes_Click);
            // 
            // menuUsuarios
            // 
            this.menuUsuarios.Name = "menuUsuarios";
            this.menuUsuarios.Size = new System.Drawing.Size(185, 22);
            this.menuUsuarios.Text = "Usuarios y Roles";
            this.menuUsuarios.Click += new System.EventHandler(this.menuUsuarios_Click);
            // 
            // menuAuditoria
            // 
            this.menuAuditoria.Name = "menuAuditoria";
            this.menuAuditoria.Size = new System.Drawing.Size(185, 22);
            this.menuAuditoria.Text = "Bitácora de Auditoría";
            this.menuAuditoria.Click += new System.EventHandler(this.menuAuditoria_Click);
            // 
            // menuSalir
            // 
            this.menuSalir.Name = "menuSalir";
            this.menuSalir.Size = new System.Drawing.Size(41, 20);
            this.menuSalir.Text = "Salir";
            this.menuSalir.Click += new System.EventHandler(this.menuSalir_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusUsuario});
            this.statusStrip.Location = new System.Drawing.Point(0, 689);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1184, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            // 
            // lblStatusUsuario
            // 
            this.lblStatusUsuario.Name = "lblStatusUsuario";
            this.lblStatusUsuario.Size = new System.Drawing.Size(89, 17);
            this.lblStatusUsuario.Text = "Usuario: [Estado]";
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 711);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "INAIPI - Sistema de Gestión Central (CORE)";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmPrincipal_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}