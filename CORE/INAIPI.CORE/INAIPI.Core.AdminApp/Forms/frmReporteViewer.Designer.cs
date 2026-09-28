namespace INAIPI.Core.AdminApp.Forms
{
    partial class frmReporteViewer
    {
        private System.ComponentModel.IContainer components = null;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewerCore;
        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.Label lblCentroFiltro;
        private System.Windows.Forms.ComboBox cmbCentroFiltro;
        private System.Windows.Forms.Label lblFechaDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblFechaHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnGenerarReporte;

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
            this.reportViewerCore = new Microsoft.Reporting.WinForms.ReportViewer();
            this.panelFiltros = new System.Windows.Forms.Panel();
            this.btnGenerarReporte = new System.Windows.Forms.Button();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.cmbCentroFiltro = new System.Windows.Forms.ComboBox();
            this.lblCentroFiltro = new System.Windows.Forms.Label();
            this.panelFiltros.SuspendLayout();
            this.SuspendLayout();
            // 
            // reportViewerCore
            // 
            this.reportViewerCore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewerCore.Location = new System.Drawing.Point(0, 60);
            this.reportViewerCore.Name = "reportViewerCore";
            this.reportViewerCore.ServerReport.BearerToken = null;
            this.reportViewerCore.Size = new System.Drawing.Size(984, 551);
            this.reportViewerCore.TabIndex = 0;
            // 
            // panelFiltros
            // 
            this.panelFiltros.Controls.Add(this.btnGenerarReporte);
            this.panelFiltros.Controls.Add(this.dtpHasta);
            this.panelFiltros.Controls.Add(this.lblFechaHasta);
            this.panelFiltros.Controls.Add(this.dtpDesde);
            this.panelFiltros.Controls.Add(this.lblFechaDesde);
            this.panelFiltros.Controls.Add(this.cmbCentroFiltro);
            this.panelFiltros.Controls.Add(this.lblCentroFiltro);
            this.panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltros.Location = new System.Drawing.Point(0, 0);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Size = new System.Drawing.Size(984, 60);
            this.panelFiltros.TabIndex = 1;
            // 
            // btnGenerarReporte
            // 
            this.btnGenerarReporte.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerarReporte.Location = new System.Drawing.Point(680, 16);
            this.btnGenerarReporte.Name = "btnGenerarReporte";
            this.btnGenerarReporte.Size = new System.Drawing.Size(120, 28);
            this.btnGenerarReporte.TabIndex = 6;
            this.btnGenerarReporte.Text = "Generar Reporte";
            this.btnGenerarReporte.UseVisualStyleBackColor = true;
            this.btnGenerarReporte.Click += new System.EventHandler(this.btnGenerarReporte_Click);
            // 
            // dtpHasta
            // 
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(540, 20);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(110, 20);
            this.dtpHasta.TabIndex = 5;
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Location = new System.Drawing.Point(495, 24);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(38, 13);
            this.lblFechaHasta.TabIndex = 4;
            this.lblFechaHasta.Text = "Hasta:";
            // 
            // dtpDesde
            // 
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(365, 20);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(110, 20);
            this.dtpDesde.TabIndex = 3;
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.AutoSize = true;
            this.lblFechaDesde.Location = new System.Drawing.Point(318, 24);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(41, 13);
            this.lblFechaDesde.TabIndex = 2;
            this.lblFechaDesde.Text = "Desde:";
            // 
            // cmbCentroFiltro
            // 
            this.cmbCentroFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCentroFiltro.FormattingEnabled = true;
            this.cmbCentroFiltro.Location = new System.Drawing.Point(65, 20);
            this.cmbCentroFiltro.Name = "cmbCentroFiltro";
            this.cmbCentroFiltro.Size = new System.Drawing.Size(230, 21);
            this.cmbCentroFiltro.TabIndex = 1;
            // 
            // lblCentroFiltro
            // 
            this.lblCentroFiltro.AutoSize = true;
            this.lblCentroFiltro.Location = new System.Drawing.Point(15, 24);
            this.lblCentroFiltro.Name = "lblCentroFiltro";
            this.lblCentroFiltro.Size = new System.Drawing.Size(41, 13);
            this.lblCentroFiltro.TabIndex = 0;
            this.lblCentroFiltro.Text = "Centro:";
            // 
            // frmReporteViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.reportViewerCore);
            this.Controls.Add(this.panelFiltros);
            this.Name = "frmReporteViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Institucional de Beneficiarios";
            this.Load += new System.EventHandler(this.frmReporteViewer_Load);
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}