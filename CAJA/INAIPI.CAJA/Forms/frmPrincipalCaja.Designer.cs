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
            this.pnlSeparador2 = new System.Windows.Forms.Panel();
            this.btnAuditoriaCierre = new System.Windows.Forms.Button();
            this.pnlSeparador1 = new System.Windows.Forms.Panel();
            this.btnRegistroTransacciones = new System.Windows.Forms.Button();
            this.pnlSeparador0 = new System.Windows.Forms.Panel();
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
            this.pnlMenuLateral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(104)))), ((int)(((byte)(55)))));
            this.pnlMenuLateral.Controls.Add(this.btnAuditoriaCierre);
            this.pnlMenuLateral.Controls.Add(this.pnlSeparador2);
            this.pnlMenuLateral.Controls.Add(this.btnRegistroTransacciones);
            this.pnlMenuLateral.Controls.Add(this.pnlSeparador1);
            this.pnlMenuLateral.Controls.Add(this.btnGestionCobros);
            this.pnlMenuLateral.Controls.Add(this.pnlSeparador0);
            this.pnlMenuLateral.Controls.Add(this.pnlLogo);
            this.pnlMenuLateral.Controls.Add(this.btnSalirSistema);
            this.pnlMenuLateral.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenuLateral.Location = new System.Drawing.Point(0, 0);
            this.pnlMenuLateral.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlMenuLateral.Name = "pnlMenuLateral";
            this.pnlMenuLateral.Padding = new System.Windows.Forms.Padding(15, 0, 15, 15);
            this.pnlMenuLateral.Size = new System.Drawing.Size(375, 1048);
            this.pnlMenuLateral.TabIndex = 1;
            // 
            // btnSalirSistema
            // 
            this.btnSalirSistema.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnSalirSistema.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalirSistema.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSalirSistema.FlatAppearance.BorderSize = 0;
            this.btnSalirSistema.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalirSistema.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalirSistema.ForeColor = System.Drawing.Color.White;
            this.btnSalirSistema.Location = new System.Drawing.Point(15, 973);
            this.btnSalirSistema.Name = "btnSalirSistema";
            this.btnSalirSistema.Size = new System.Drawing.Size(345, 60);
            this.btnSalirSistema.TabIndex = 4;
            this.btnSalirSistema.Text = "🚪 Salir del Sistema";
            this.btnSalirSistema.UseVisualStyleBackColor = false;
            this.btnSalirSistema.Click += new System.EventHandler(this.btnSalirSistema_Click);
            // 
            // pnlSeparador2
            // 
            this.pnlSeparador2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSeparador2.Location = new System.Drawing.Point(15, 314);
            this.pnlSeparador2.Name = "pnlSeparador2";
            this.pnlSeparador2.Size = new System.Drawing.Size(345, 15);
            this.pnlSeparador2.TabIndex = 6;
            this.pnlSeparador2.BackColor = System.Drawing.Color.Transparent;
            // 
            // btnAuditoriaCierre
            // 
            this.btnAuditoriaCierre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(45)))));
            this.btnAuditoriaCierre.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAuditoriaCierre.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAuditoriaCierre.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(130)))), ((int)(((byte)(70)))));
            this.btnAuditoriaCierre.FlatAppearance.BorderSize = 0;
            this.btnAuditoriaCierre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuditoriaCierre.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAuditoriaCierre.ForeColor = System.Drawing.Color.White;
            this.btnAuditoriaCierre.Location = new System.Drawing.Point(15, 254);
            this.btnAuditoriaCierre.Name = "btnAuditoriaCierre";
            this.btnAuditoriaCierre.Size = new System.Drawing.Size(345, 60);
            this.btnAuditoriaCierre.TabIndex = 3;
            this.btnAuditoriaCierre.Text = "🔐 Auditoría y Cierre de Turno";
            this.btnAuditoriaCierre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAuditoriaCierre.UseVisualStyleBackColor = false;
            this.btnAuditoriaCierre.Click += new System.EventHandler(this.btnAuditoriaCierre_Click);
            // 
            // pnlSeparador1
            // 
            this.pnlSeparador1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSeparador1.Location = new System.Drawing.Point(15, 239);
            this.pnlSeparador1.Name = "pnlSeparador1";
            this.pnlSeparador1.Size = new System.Drawing.Size(345, 15);
            this.pnlSeparador1.TabIndex = 5;
            this.pnlSeparador1.BackColor = System.Drawing.Color.Transparent;
            // 
            // btnRegistroTransacciones
            // 
            this.btnRegistroTransacciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(45)))));
            this.btnRegistroTransacciones.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegistroTransacciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRegistroTransacciones.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(130)))), ((int)(((byte)(70)))));
            this.btnRegistroTransacciones.FlatAppearance.BorderSize = 0;
            this.btnRegistroTransacciones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistroTransacciones.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistroTransacciones.ForeColor = System.Drawing.Color.White;
            this.btnRegistroTransacciones.Location = new System.Drawing.Point(15, 179);
            this.btnRegistroTransacciones.Name = "btnRegistroTransacciones";
            this.btnRegistroTransacciones.Size = new System.Drawing.Size(345, 60);
            this.btnRegistroTransacciones.TabIndex = 2;
            this.btnRegistroTransacciones.Text = "📄 Registro de Transacciones";
            this.btnRegistroTransacciones.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRegistroTransacciones.UseVisualStyleBackColor = false;
            this.btnRegistroTransacciones.Click += new System.EventHandler(this.btnRegistroTransacciones_Click);
            // 
            // pnlSeparador0
            // 
            this.pnlSeparador0.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSeparador0.Location = new System.Drawing.Point(15, 164);
            this.pnlSeparador0.Name = "pnlSeparador0";
            this.pnlSeparador0.Size = new System.Drawing.Size(345, 15);
            this.pnlSeparador0.TabIndex = 7;
            this.pnlSeparador0.BackColor = System.Drawing.Color.Transparent;
            // 
            // btnGestionCobros
            // 
            this.btnGestionCobros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(85)))), ((int)(((byte)(45)))));
            this.btnGestionCobros.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGestionCobros.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGestionCobros.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(130)))), ((int)(((byte)(70)))));
            this.btnGestionCobros.FlatAppearance.BorderSize = 0;
            this.btnGestionCobros.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGestionCobros.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGestionCobros.ForeColor = System.Drawing.Color.White;
            this.btnGestionCobros.Location = new System.Drawing.Point(15, 104);
            this.btnGestionCobros.Name = "btnGestionCobros";
            this.btnGestionCobros.Size = new System.Drawing.Size(345, 60);
            this.btnGestionCobros.TabIndex = 1;
            this.btnGestionCobros.Text = "💰 Gestión de Cobros";
            this.btnGestionCobros.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGestionCobros.UseVisualStyleBackColor = false;
            this.btnGestionCobros.Click += new System.EventHandler(this.btnGestionCobros_Click);
            // 
            // pnlLogo
            // 
            this.pnlLogo.BackColor = System.Drawing.Color.Transparent;
            this.pnlLogo.Controls.Add(this.lblLogoTitulo);
            this.pnlLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogo.Location = new System.Drawing.Point(15, 0);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(345, 104);
            this.pnlLogo.TabIndex = 0;
            // 
            // lblLogoTitulo
            // 
            this.lblLogoTitulo.AutoSize = true;
            this.lblLogoTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogoTitulo.ForeColor = System.Drawing.Color.White;
            this.lblLogoTitulo.Location = new System.Drawing.Point(75, 34);
            this.lblLogoTitulo.Name = "lblLogoTitulo";
            this.lblLogoTitulo.Size = new System.Drawing.Size(168, 32);
            this.lblLogoTitulo.TabIndex = 0;
            this.lblLogoTitulo.Text = "INAIPI | CAJA";
            // 
            // statusStripInferior
            // 
            this.statusStripInferior.BackColor = System.Drawing.Color.White;
            this.statusStripInferior.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStripInferior.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblEstadoConectividad});
            this.statusStripInferior.Location = new System.Drawing.Point(375, 1019);
            this.statusStripInferior.Name = "statusStripInferior";
            this.statusStripInferior.Size = new System.Drawing.Size(1251, 29);
            this.statusStripInferior.TabIndex = 2;
            // 
            // lblEstadoConectividad
            // 
            this.lblEstadoConectividad.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoConectividad.ForeColor = System.Drawing.Color.DarkGray;
            this.lblEstadoConectividad.Name = "lblEstadoConectividad";
            this.lblEstadoConectividad.Size = new System.Drawing.Size(271, 24);
            this.lblEstadoConectividad.Text = "Iniciando servicio de telemetría...";
            // 
            // temporizadorSincronizacion
            // 
            this.temporizadorSincronizacion.Interval = 10000;
            this.temporizadorSincronizacion.Tick += new System.EventHandler(this.temporizadorSincronizacion_Tick);
            // 
            // frmPrincipalCaja
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(1626, 1048);
            this.Controls.Add(this.statusStripInferior);
            this.Controls.Add(this.pnlMenuLateral);
            this.IsMdiContainer = true;
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
        private System.Windows.Forms.Panel pnlSeparador1;
        private System.Windows.Forms.Button btnAuditoriaCierre;
        private System.Windows.Forms.Button btnRegistroTransacciones;
        private System.Windows.Forms.Panel pnlSeparador2;
        private System.Windows.Forms.Panel pnlSeparador0;
        private System.Windows.Forms.StatusStrip statusStripInferior;
        private System.Windows.Forms.ToolStripStatusLabel lblEstadoConectividad;
        private System.Windows.Forms.Button btnSalirSistema;
        private System.Windows.Forms.Timer temporizadorSincronizacion;
    }
}