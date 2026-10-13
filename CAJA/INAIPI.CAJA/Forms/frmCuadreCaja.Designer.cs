namespace INAIPI.CAJA.Forms
{
    partial class frmCuadreCaja
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
            this.gbSistema = new System.Windows.Forms.GroupBox();
            this.lblTotalEsperado = new System.Windows.Forms.Label();
            this.lblTotalCobros = new System.Windows.Forms.Label();
            this.lblMontoInicial = new System.Windows.Forms.Label();
            this.gbFisico = new System.Windows.Forms.GroupBox();
            this.txtEfectivoFisico = new System.Windows.Forms.TextBox();
            this.lblInstruccion = new System.Windows.Forms.Label();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.lblDiferencia = new System.Windows.Forms.Label();
            this.btnCerrarTurno = new System.Windows.Forms.Button();
            this.gbSistema.SuspendLayout();
            this.gbFisico.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSistema
            // 
            this.gbSistema.Controls.Add(this.lblTotalEsperado);
            this.gbSistema.Controls.Add(this.lblTotalCobros);
            this.gbSistema.Controls.Add(this.lblMontoInicial);
            this.gbSistema.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSistema.Location = new System.Drawing.Point(23, 22);
            this.gbSistema.Name = "gbSistema";
            this.gbSistema.Size = new System.Drawing.Size(370, 130);
            this.gbSistema.TabIndex = 0;
            this.gbSistema.TabStop = false;
            this.gbSistema.Text = "Cálculo del Sistema";
            // 
            // lblTotalEsperado
            // 
            this.lblTotalEsperado.AutoSize = true;
            this.lblTotalEsperado.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalEsperado.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTotalEsperado.Location = new System.Drawing.Point(20, 89);
            this.lblTotalEsperado.Name = "lblTotalEsperado";
            this.lblTotalEsperado.Size = new System.Drawing.Size(206, 21);
            this.lblTotalEsperado.TabIndex = 2;
            this.lblTotalEsperado.Text = "Total Esperado en Caja: $0.00";
            // 
            // lblTotalCobros
            // 
            this.lblTotalCobros.AutoSize = true;
            this.lblTotalCobros.Location = new System.Drawing.Point(21, 60);
            this.lblTotalCobros.Name = "lblTotalCobros";
            this.lblTotalCobros.Size = new System.Drawing.Size(199, 17);
            this.lblTotalCobros.TabIndex = 1;
            this.lblTotalCobros.Text = "Total Cobrado (Transacciones): $0.00";
            // 
            // lblMontoInicial
            // 
            this.lblMontoInicial.AutoSize = true;
            this.lblMontoInicial.Location = new System.Drawing.Point(21, 33);
            this.lblMontoInicial.Name = "lblMontoInicial";
            this.lblMontoInicial.Size = new System.Drawing.Size(182, 17);
            this.lblMontoInicial.TabIndex = 0;
            this.lblMontoInicial.Text = "Monto Inicial (Apertura): $0.00";
            // 
            // gbFisico
            // 
            this.gbFisico.Controls.Add(this.txtEfectivoFisico);
            this.gbFisico.Controls.Add(this.lblInstruccion);
            this.gbFisico.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFisico.Location = new System.Drawing.Point(23, 169);
            this.gbFisico.Name = "gbFisico";
            this.gbFisico.Size = new System.Drawing.Size(370, 94);
            this.gbFisico.TabIndex = 1;
            this.gbFisico.TabStop = false;
            this.gbFisico.Text = "Conteo Físico";
            // 
            // txtEfectivoFisico
            // 
            this.txtEfectivoFisico.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEfectivoFisico.Location = new System.Drawing.Point(25, 47);
            this.txtEfectivoFisico.Name = "txtEfectivoFisico";
            this.txtEfectivoFisico.Size = new System.Drawing.Size(320, 33);
            this.txtEfectivoFisico.TabIndex = 1;
            this.txtEfectivoFisico.Text = "0.00";
            this.txtEfectivoFisico.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblInstruccion
            // 
            this.lblInstruccion.AutoSize = true;
            this.lblInstruccion.Location = new System.Drawing.Point(21, 25);
            this.lblInstruccion.Name = "lblInstruccion";
            this.lblInstruccion.Size = new System.Drawing.Size(220, 17);
            this.lblInstruccion.TabIndex = 0;
            this.lblInstruccion.Text = "Digite el efectivo total en la gaveta:";
            // 
            // btnCalcular
            // 
            this.btnCalcular.BackColor = System.Drawing.Color.SteelBlue;
            this.btnCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalcular.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcular.ForeColor = System.Drawing.Color.White;
            this.btnCalcular.Location = new System.Drawing.Point(23, 275);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(370, 35);
            this.btnCalcular.TabIndex = 2;
            this.btnCalcular.Text = "Calcular Diferencia";
            this.btnCalcular.UseVisualStyleBackColor = false;
            this.btnCalcular.Click += new System.EventHandler(this.btnCalcular_Click);
            // 
            // lblDiferencia
            // 
            this.lblDiferencia.AutoSize = true;
            this.lblDiferencia.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiferencia.Location = new System.Drawing.Point(23, 325);
            this.lblDiferencia.Name = "lblDiferencia";
            this.lblDiferencia.Size = new System.Drawing.Size(161, 25);
            this.lblDiferencia.TabIndex = 3;
            this.lblDiferencia.Text = "Diferencia: $0.00";
            // 
            // btnCerrarTurno
            // 
            this.btnCerrarTurno.BackColor = System.Drawing.Color.Firebrick;
            this.btnCerrarTurno.Enabled = false;
            this.btnCerrarTurno.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarTurno.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrarTurno.ForeColor = System.Drawing.Color.White;
            this.btnCerrarTurno.Location = new System.Drawing.Point(23, 365);
            this.btnCerrarTurno.Name = "btnCerrarTurno";
            this.btnCerrarTurno.Size = new System.Drawing.Size(370, 40);
            this.btnCerrarTurno.TabIndex = 4;
            this.btnCerrarTurno.Text = "Generar Reporte y Cerrar Turno";
            this.btnCerrarTurno.UseVisualStyleBackColor = false;
            this.btnCerrarTurno.Click += new System.EventHandler(this.btnCerrarTurno_Click);
            // 
            // frmCuadreCaja
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(419, 426);
            this.Controls.Add(this.btnCerrarTurno);
            this.Controls.Add(this.lblDiferencia);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.gbFisico);
            this.Controls.Add(this.gbSistema);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmCuadreCaja";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cuadre de Caja - Fin de Turno";
            this.Load += new System.EventHandler(this.frmCuadreCaja_Load);
            this.gbSistema.ResumeLayout(false);
            this.gbSistema.PerformLayout();
            this.gbFisico.ResumeLayout(false);
            this.gbFisico.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSistema;
        private System.Windows.Forms.Label lblTotalEsperado;
        private System.Windows.Forms.Label lblTotalCobros;
        private System.Windows.Forms.Label lblMontoInicial;
        private System.Windows.Forms.GroupBox gbFisico;
        private System.Windows.Forms.TextBox txtEfectivoFisico;
        private System.Windows.Forms.Label lblInstruccion;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.Label lblDiferencia;
        private System.Windows.Forms.Button btnCerrarTurno;
    }
}