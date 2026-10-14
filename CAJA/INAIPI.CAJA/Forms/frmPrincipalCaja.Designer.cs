namespace INAIPI.CAJA.Forms
{
    partial class frmPrincipalCaja
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlMenuLateral = new System.Windows.Forms.Panel();
            this.btnSalirSistema = new System.Windows.Forms.Button();
            this.btnAuditoriaCierre = new System.Windows.Forms.Button();
            this.btnRegistroTransacciones = new System.Windows.Forms.Button();
            this.btnGestionCobros = new System.Windows.Forms.Button();
            this.pnlLogo = new System.Windows.Forms.Panel();
            this.lblLogoTitulo = new System.Windows.Forms.Label();
            this.statusStripInferior = new System.Windows.Forms.StatusStrip();
            this.lblEstadoConectividad = new System.Windows.Forms.ToolStripStatusLabel();
            this.temporizadorSincronizacion = new System.Windows.Forms.Timer(this.components);
            this.pnlMenuLateral.SuspendLayout();
            this.pnlLogo.SuspendLayout();
            this.statusStripInferior.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMenuLateral
            // 
            this.pnlMenuLateral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(46)))), ((int)(((byte)(59)))));
            this.pnlMenuLateral.Controls.Add(this.btnSalirSistema);
            this.pnlMenuLateral.Controls.Add(this.btnAuditoriaCierre);
            this.pnlMenuLateral.Controls.Add(this.btnRegistroTransacciones);
            this.pnlMenuLateral.Controls.Add(this.btnGestionCobros);
            this.pnlMenuLateral.Controls.Add(this.pnlLogo);
            this.pnlMenuLateral.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenuLateral.Location = new System.Drawing.Point(0, 0);
            this.pnlMenuLateral.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlMenuLateral.Name = "pnlMenuLateral";
            this.pnlMenuLateral.Size = new System.Drawing.Size(375, 1048);
            this.pnlMenuLateral.TabIndex = 1;
            // 
            // btnSalirSistema
            // 
            this.btnSalirSistema.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSalirSistema.FlatAppearance.BorderSize = 0;
            this.btnSalirSistema.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.btnSalirSistema.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalirSistema.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalirSistema.ForeColor = System.Drawing.Color.White;
            this.btnSalirSistema.Location = new System.Drawing.Point(0, 956);
            this.btnSalirSistema.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSalirSistema.Name = "btnSalirSistema";
            this.btnSalirSistema.Size = new System.Drawing.Size(375, 92);
            this.btnSalirSistema.TabIndex = 4;
            this.btnSalirSistema.Text = "  Salir del Sistema";
            this.btnSalirSistema.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalirSistema.UseVisualStyleBackColor = true;
            this.btnSalirSistema.Click += new System.EventHandler(this.btnSalirSistema_Click);
            // 
            // btnAuditoriaCierre
            // 
            this.btnAuditoriaCierre.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAuditoriaCierre.FlatAppearance.BorderSize = 0;
            this.btnAuditoriaCierre.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.btnAuditoriaCierre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuditoriaCierre.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAuditoriaCierre.ForeColor = System.Drawing.Color.White;
            this.btnAuditoriaCierre.Location = new System.Drawing.Point(0, 338);
            this.btnAuditoriaCierre.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAuditoriaCierre.Name = "btnAuditoriaCierre";
            this.btnAuditoriaCierre.Size = new System.Drawing.Size(375, 92);
            this.btnAuditoriaCierre.TabIndex = 3;
            this.btnAuditoriaCierre.Text = "  Auditoría y Cierre de Turno";
            this.btnAuditoriaCierre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAuditoriaCierre.UseVisualStyleBackColor = true;
            this.btnAuditoriaCierre.Click += new System.EventHandler(this.btnAuditoriaCierre_Click);
            // 
            // btnRegistroTransacciones
            // 
            this.btnRegistroTransacciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRegistroTransacciones.FlatAppearance.BorderSize = 0;
            this.btnRegistroTransacciones.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.btnRegistroTransacciones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistroTransacciones.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistroTransacciones.ForeColor = System.Drawing.Color.White;
            this.btnRegistroTransacciones.Location = new System.Drawing.Point(0, 246);
            this.btnRegistroTransacciones.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRegistroTransacciones.Name = "btnRegistroTransacciones";
            this.btnRegistroTransacciones.Size = new System.Drawing.Size(375, 92);
            this.btnRegistroTransacciones.TabIndex = 2;
            this.btnRegistroTransacciones.Text = "  Registro de Transacciones";
            this.btnRegistroTransacciones.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRegistroTransacciones.UseVisualStyleBackColor = true;
            this.btnRegistroTransacciones.Click += new System.EventHandler(this.btnRegistroTransacciones_Click);
            // 
            // btnGestionCobros
            // 
            this.btnGestionCobros.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGestionCobros.FlatAppearance.BorderSize = 0;
            this.btnGestionCobros.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(40)))));
            this.btnGestionCobros.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGestionCobros.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGestionCobros.ForeColor = System.Drawing.Color.White;
            this.btnGestionCobros.Location = new System.Drawing.Point(0, 154);
            this.btnGestionCobros.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnGestionCobros.Name = "btnGestionCobros";
            this.btnGestionCobros.Size = new System.Drawing.Size(375, 92);
            this.btnGestionCobros.TabIndex = 1;
            this.btnGestionCobros.Text = "  Gestión de Cobros";
            this.btnGestionCobros.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGestionCobros.UseVisualStyleBackColor = true;
            this.btnGestionCobros.Click += new System.EventHandler(this.btnGestionCobros_Click);
            // 
            // pnlLogo
            // 
            this.pnlLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.pnlLogo.Controls.Add(this.lblLogoTitulo);
            this.pnlLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogo.Location = new System.Drawing.Point(0, 0);
            this.pnlLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(375, 154);
            this.pnlLogo.TabIndex = 0;
            // 
            // lblLogoTitulo
            // 
            this.lblLogoTitulo.AutoSize = true;
            this.lblLogoTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogoTitulo.ForeColor = System.Drawing.Color.White;
            this.lblLogoTitulo.Location = new System.Drawing.Point(34, 51);
            this.lblLogoTitulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogoTitulo.Name = "lblLogoTitulo";
            this.lblLogoTitulo.Size = new System.Drawing.Size(248, 48);
            this.lblLogoTitulo.TabIndex = 0;
            this.lblLogoTitulo.Text = "INAIPI | CAJA";
            // 
            // statusStripInferior
            // 
            this.statusStripInferior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.statusStripInferior.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStripInferior.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblEstadoConectividad});
            this.statusStripInferior.Location = new System.Drawing.Point(375, 1009);
            this.statusStripInferior.Name = "statusStripInferior";
            this.statusStripInferior.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStripInferior.Size = new System.Drawing.Size(1251, 39);
            this.statusStripInferior.TabIndex = 2;
            // 
            // lblEstadoConectividad
            // 
            this.lblEstadoConectividad.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoConectividad.ForeColor = System.Drawing.Color.DarkGray;
            this.lblEstadoConectividad.Name = "lblEstadoConectividad";
            this.lblEstadoConectividad.Size = new System.Drawing.Size(396, 32);
            this.lblEstadoConectividad.Text = "Iniciando servicio de telemetría...";
            // 
            // temporizadorSincronizacion
            // 
            this.temporizadorSincronizacion.Interval = 10000;
            this.temporizadorSincronizacion.Tick += new System.EventHandler(this.temporizadorSincronizacion_Tick);
            // 
            // frmPrincipalCaja
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1626, 1048);
            this.Controls.Add(this.statusStripInferior);
            this.Controls.Add(this.pnlMenuLateral);
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmPrincipalCaja";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "INAIPI - Módulo de Caja";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPrincipal_FormClosing);
            this.Shown += new System.EventHandler(this.frmPrincipalCaja_Shown);
            this.pnlMenuLateral.ResumeLayout(false);
            this.pnlLogo.ResumeLayout(false);
            this.pnlLogo.PerformLayout();
            this.statusStripInferior.ResumeLayout(false);
            this.statusStripInferior.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMenuLateral;
        private System.Windows.Forms.Panel pnlLogo;
        private System.Windows.Forms.Label lblLogoTitulo;
        private System.Windows.Forms.Button btnGestionCobros;
        private System.Windows.Forms.Button btnAuditoriaCierre;
        private System.Windows.Forms.Button btnRegistroTransacciones;
        private System.Windows.Forms.StatusStrip statusStripInferior;
        private System.Windows.Forms.ToolStripStatusLabel lblEstadoConectividad;
        private System.Windows.Forms.Button btnSalirSistema;
        private System.Windows.Forms.Timer temporizadorSincronizacion;
    }
}