namespace INAIPI.CAJA.Forms
{
    partial class frmPrincipalCaja
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.StatusStrip barraEstadoCaja;
        private System.Windows.Forms.ToolStripStatusLabel lblEstadoConectividad;
        private System.Windows.Forms.Timer temporizadorSincronizacion;
        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem menuTransacciones;
        private System.Windows.Forms.ToolStripMenuItem subMenuCobro;

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
            this.components = new System.ComponentModel.Container();
            this.barraEstadoCaja = new System.Windows.Forms.StatusStrip();
            this.lblEstadoConectividad = new System.Windows.Forms.ToolStripStatusLabel();
            this.temporizadorSincronizacion = new System.Windows.Forms.Timer(this.components);
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.menuTransacciones = new System.Windows.Forms.ToolStripMenuItem();
            this.subMenuCobro = new System.Windows.Forms.ToolStripMenuItem();
            this.historialDeTransaccionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barraEstadoCaja.SuspendLayout();
            this.menuPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // barraEstadoCaja
            // 
            this.barraEstadoCaja.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.barraEstadoCaja.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblEstadoConectividad});
            this.barraEstadoCaja.Location = new System.Drawing.Point(0, 657);
            this.barraEstadoCaja.Name = "barraEstadoCaja";
            this.barraEstadoCaja.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.barraEstadoCaja.Size = new System.Drawing.Size(1200, 35);
            this.barraEstadoCaja.TabIndex = 1;
            this.barraEstadoCaja.Text = "statusStrip1";
            // 
            // lblEstadoConectividad
            // 
            this.lblEstadoConectividad.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblEstadoConectividad.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblEstadoConectividad.Name = "lblEstadoConectividad";
            this.lblEstadoConectividad.Size = new System.Drawing.Size(479, 28);
            this.lblEstadoConectividad.Text = "Estado: INICIANDO... Verificando enlace con SND.";
            // 
            // temporizadorSincronizacion
            // 
            this.temporizadorSincronizacion.Enabled = true;
            this.temporizadorSincronizacion.Interval = 10000;
            this.temporizadorSincronizacion.Tick += new System.EventHandler(this.temporizadorSincronizacion_Tick);
            // 
            // menuPrincipal
            // 
            this.menuPrincipal.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuPrincipal.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTransacciones});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Size = new System.Drawing.Size(1200, 35);
            this.menuPrincipal.TabIndex = 2;
            this.menuPrincipal.Text = "menuStrip1";
            // 
            // menuTransacciones
            // 
            this.menuTransacciones.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subMenuCobro,
            this.historialDeTransaccionesToolStripMenuItem});
            this.menuTransacciones.Name = "menuTransacciones";
            this.menuTransacciones.Size = new System.Drawing.Size(135, 29);
            this.menuTransacciones.Text = "Transacciones";
            // 
            // subMenuCobro
            // 
            this.subMenuCobro.Name = "subMenuCobro";
            this.subMenuCobro.Size = new System.Drawing.Size(270, 34);
            this.subMenuCobro.Text = "Cobro de Servicio";
            this.subMenuCobro.Click += new System.EventHandler(this.subMenuCobro_Click);
            // 
            // historialDeTransaccionesToolStripMenuItem
            // 
            this.historialDeTransaccionesToolStripMenuItem.Name = "historialDeTransaccionesToolStripMenuItem";
            this.historialDeTransaccionesToolStripMenuItem.Size = new System.Drawing.Size(316, 34);
            this.historialDeTransaccionesToolStripMenuItem.Text = "Historial de Transacciones";
            this.historialDeTransaccionesToolStripMenuItem.Click += new System.EventHandler(this.historialDeTransaccionesToolStripMenuItem_Click);
            // 
            // frmPrincipalCaja
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 692);
            this.Controls.Add(this.barraEstadoCaja);
            this.Controls.Add(this.menuPrincipal);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuPrincipal;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmPrincipalCaja";
            this.Text = "INAIPI - Módulo de Caja (Soporte Offline)";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.barraEstadoCaja.ResumeLayout(false);
            this.barraEstadoCaja.PerformLayout();
            this.menuPrincipal.ResumeLayout(false);
            this.menuPrincipal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ToolStripMenuItem historialDeTransaccionesToolStripMenuItem;
    }
}