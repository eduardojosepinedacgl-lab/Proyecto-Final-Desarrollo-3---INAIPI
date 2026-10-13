namespace INAIPI.CAJA.Forms
{
    partial class frmCobroServicio
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblBeneficiario = new System.Windows.Forms.Label();
            this.txtBeneficiario = new System.Windows.Forms.TextBox();
            this.lblComponente = new System.Windows.Forms.Label();
            this.cmbComponente = new System.Windows.Forms.ComboBox();
            this.lblMonto = new System.Windows.Forms.Label();
            this.txtMonto = new System.Windows.Forms.TextBox();
            this.btnProcesarCobro = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.gbDatosTransaccion = new System.Windows.Forms.GroupBox();
            this.gbDatosTransaccion.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblTitulo.Location = new System.Drawing.Point(23, 22);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(262, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Cobro de Servicio";
            // 
            // gbDatosTransaccion
            // 
            this.gbDatosTransaccion.Controls.Add(this.txtMonto);
            this.gbDatosTransaccion.Controls.Add(this.lblMonto);
            this.gbDatosTransaccion.Controls.Add(this.cmbComponente);
            this.gbDatosTransaccion.Controls.Add(this.lblComponente);
            this.gbDatosTransaccion.Controls.Add(this.txtBeneficiario);
            this.gbDatosTransaccion.Controls.Add(this.lblBeneficiario);
            this.gbDatosTransaccion.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDatosTransaccion.Location = new System.Drawing.Point(28, 68);
            this.gbDatosTransaccion.Name = "gbDatosTransaccion";
            this.gbDatosTransaccion.Size = new System.Drawing.Size(434, 237);
            this.gbDatosTransaccion.TabIndex = 1;
            this.gbDatosTransaccion.TabStop = false;
            this.gbDatosTransaccion.Text = "Datos de la Transacción";
            // 
            // lblBeneficiario
            // 
            this.lblBeneficiario.AutoSize = true;
            this.lblBeneficiario.Location = new System.Drawing.Point(23, 37);
            this.lblBeneficiario.Name = "lblBeneficiario";
            this.lblBeneficiario.Size = new System.Drawing.Size(184, 17);
            this.lblBeneficiario.TabIndex = 0;
            this.lblBeneficiario.Text = "ID del Beneficiario (Expediente):";
            // 
            // txtBeneficiario
            // 
            this.txtBeneficiario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBeneficiario.Location = new System.Drawing.Point(27, 57);
            this.txtBeneficiario.Name = "txtBeneficiario";
            this.txtBeneficiario.Size = new System.Drawing.Size(382, 29);
            this.txtBeneficiario.TabIndex = 1;
            // 
            // lblComponente
            // 
            this.lblComponente.AutoSize = true;
            this.lblComponente.Location = new System.Drawing.Point(23, 102);
            this.lblComponente.Name = "lblComponente";
            this.lblComponente.Size = new System.Drawing.Size(155, 17);
            this.lblComponente.TabIndex = 2;
            this.lblComponente.Text = "Componente de Atención:";
            // 
            // cmbComponente
            // 
            this.cmbComponente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComponente.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbComponente.FormattingEnabled = true;
            this.cmbComponente.Location = new System.Drawing.Point(27, 122);
            this.cmbComponente.Name = "cmbComponente";
            this.cmbComponente.Size = new System.Drawing.Size(382, 29);
            this.cmbComponente.TabIndex = 3;
            // 
            // lblMonto
            // 
            this.lblMonto.AutoSize = true;
            this.lblMonto.Location = new System.Drawing.Point(23, 166);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(121, 17);
            this.lblMonto.TabIndex = 4;
            this.lblMonto.Text = "Monto a Cobrar ($):";
            // 
            // txtMonto
            // 
            this.txtMonto.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMonto.Location = new System.Drawing.Point(27, 186);
            this.txtMonto.Name = "txtMonto";
            this.txtMonto.Size = new System.Drawing.Size(180, 33);
            this.txtMonto.TabIndex = 5;
            this.txtMonto.Text = "0.00";
            this.txtMonto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnProcesarCobro
            // 
            this.btnProcesarCobro.BackColor = System.Drawing.Color.SeaGreen;
            this.btnProcesarCobro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcesarCobro.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcesarCobro.ForeColor = System.Drawing.Color.White;
            this.btnProcesarCobro.Location = new System.Drawing.Point(267, 323);
            this.btnProcesarCobro.Name = "btnProcesarCobro";
            this.btnProcesarCobro.Size = new System.Drawing.Size(195, 45);
            this.btnProcesarCobro.TabIndex = 2;
            this.btnProcesarCobro.Text = "Procesar Cobro";
            this.btnProcesarCobro.UseVisualStyleBackColor = false;
            this.btnProcesarCobro.Click += new System.EventHandler(this.btnProcesarCobro_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.Gray;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(28, 323);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(150, 45);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // frmCobroServicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(491, 395);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnProcesarCobro);
            this.Controls.Add(this.gbDatosTransaccion);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmCobroServicio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Operación de Caja - INAIPI";
            this.Load += new System.EventHandler(this.frmCobroServicio_Load);
            this.gbDatosTransaccion.ResumeLayout(false);
            this.gbDatosTransaccion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox gbDatosTransaccion;
        private System.Windows.Forms.TextBox txtBeneficiario;
        private System.Windows.Forms.Label lblBeneficiario;
        private System.Windows.Forms.Label lblComponente;
        private System.Windows.Forms.ComboBox cmbComponente;
        private System.Windows.Forms.TextBox txtMonto;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.Button btnProcesarCobro;
        private System.Windows.Forms.Button btnCancelar;
    }
}