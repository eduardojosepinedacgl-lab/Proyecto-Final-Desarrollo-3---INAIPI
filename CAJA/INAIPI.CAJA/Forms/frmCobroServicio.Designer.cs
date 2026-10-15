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
            this.gbDatosTransaccion = new System.Windows.Forms.GroupBox();
            this.lblCedulaTutor = new System.Windows.Forms.Label();
            this.txtCedulaTutor = new System.Windows.Forms.TextBox();
            this.btnBuscarTutor = new System.Windows.Forms.Button();
            this.lblSeleccioneNiño = new System.Windows.Forms.Label();
            this.cmbBeneficiarios = new System.Windows.Forms.ComboBox();
            this.lblComponente = new System.Windows.Forms.Label();
            this.cmbComponente = new System.Windows.Forms.ComboBox();
            this.lblTituloSubtotal = new System.Windows.Forms.Label();
            this.lblSubtotalValor = new System.Windows.Forms.Label();
            this.lblTituloItbis = new System.Windows.Forms.Label();
            this.lblItbisValor = new System.Windows.Forms.Label();
            this.lblTituloTotal = new System.Windows.Forms.Label();
            this.lblTotalValor = new System.Windows.Forms.Label();
            this.lblMonto = new System.Windows.Forms.Label();
            this.txtMonto = new System.Windows.Forms.TextBox();
            this.btnProcesarCobro = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.gbDatosTransaccion.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblTitulo.Location = new System.Drawing.Point(34, 28);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(287, 45);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Cobro de Servicio";
            // 
            // gbDatosTransaccion
            // 
            this.gbDatosTransaccion.Controls.Add(this.lblCedulaTutor);
            this.gbDatosTransaccion.Controls.Add(this.txtCedulaTutor);
            this.gbDatosTransaccion.Controls.Add(this.btnBuscarTutor);
            this.gbDatosTransaccion.Controls.Add(this.lblSeleccioneNiño);
            this.gbDatosTransaccion.Controls.Add(this.cmbBeneficiarios);
            this.gbDatosTransaccion.Controls.Add(this.lblComponente);
            this.gbDatosTransaccion.Controls.Add(this.cmbComponente);
            this.gbDatosTransaccion.Controls.Add(this.lblTituloSubtotal);
            this.gbDatosTransaccion.Controls.Add(this.lblSubtotalValor);
            this.gbDatosTransaccion.Controls.Add(this.lblTituloItbis);
            this.gbDatosTransaccion.Controls.Add(this.lblItbisValor);
            this.gbDatosTransaccion.Controls.Add(this.lblTituloTotal);
            this.gbDatosTransaccion.Controls.Add(this.lblTotalValor);
            this.gbDatosTransaccion.Controls.Add(this.lblMonto);
            this.gbDatosTransaccion.Controls.Add(this.txtMonto);
            this.gbDatosTransaccion.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbDatosTransaccion.Location = new System.Drawing.Point(42, 89);
            this.gbDatosTransaccion.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbDatosTransaccion.Name = "gbDatosTransaccion";
            this.gbDatosTransaccion.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbDatosTransaccion.Size = new System.Drawing.Size(651, 569);
            this.gbDatosTransaccion.TabIndex = 1;
            this.gbDatosTransaccion.TabStop = false;
            this.gbDatosTransaccion.Text = "Datos de la Transacción";
            // 
            // lblCedulaTutor
            // 
            this.lblCedulaTutor.AutoSize = true;
            this.lblCedulaTutor.Location = new System.Drawing.Point(34, 38);
            this.lblCedulaTutor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCedulaTutor.Name = "lblCedulaTutor";
            this.lblCedulaTutor.Size = new System.Drawing.Size(278, 28);
            this.lblCedulaTutor.TabIndex = 0;
            this.lblCedulaTutor.Text = "Cédula del Tutor (Sin guiones):";
            // 
            // txtCedulaTutor
            // 
            this.txtCedulaTutor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCedulaTutor.Location = new System.Drawing.Point(40, 69);
            this.txtCedulaTutor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCedulaTutor.MaxLength = 11;
            this.txtCedulaTutor.Name = "txtCedulaTutor";
            this.txtCedulaTutor.Size = new System.Drawing.Size(373, 39);
            this.txtCedulaTutor.TabIndex = 1;
            this.txtCedulaTutor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCedulaTutor_KeyPress);
            // 
            // btnBuscarTutor
            // 
            this.btnBuscarTutor.BackColor = System.Drawing.Color.SteelBlue;
            this.btnBuscarTutor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarTutor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarTutor.ForeColor = System.Drawing.Color.White;
            this.btnBuscarTutor.Location = new System.Drawing.Point(428, 68);
            this.btnBuscarTutor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnBuscarTutor.Name = "btnBuscarTutor";
            this.btnBuscarTutor.Size = new System.Drawing.Size(186, 48);
            this.btnBuscarTutor.TabIndex = 2;
            this.btnBuscarTutor.Text = "🔍 Buscar";
            this.btnBuscarTutor.UseVisualStyleBackColor = false;
            this.btnBuscarTutor.Click += new System.EventHandler(this.btnBuscarTutor_Click);
            // 
            // lblSeleccioneNiño
            // 
            this.lblSeleccioneNiño.AutoSize = true;
            this.lblSeleccioneNiño.Location = new System.Drawing.Point(34, 131);
            this.lblSeleccioneNiño.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSeleccioneNiño.Name = "lblSeleccioneNiño";
            this.lblSeleccioneNiño.Size = new System.Drawing.Size(292, 28);
            this.lblSeleccioneNiño.TabIndex = 3;
            this.lblSeleccioneNiño.Text = "Seleccione Beneficiario (Niño/a):";
            // 
            // cmbBeneficiarios
            // 
            this.cmbBeneficiarios.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBeneficiarios.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBeneficiarios.FormattingEnabled = true;
            this.cmbBeneficiarios.Location = new System.Drawing.Point(40, 162);
            this.cmbBeneficiarios.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbBeneficiarios.Name = "cmbBeneficiarios";
            this.cmbBeneficiarios.Size = new System.Drawing.Size(571, 39);
            this.cmbBeneficiarios.TabIndex = 4;
            // 
            // lblComponente
            // 
            this.lblComponente.AutoSize = true;
            this.lblComponente.Location = new System.Drawing.Point(34, 223);
            this.lblComponente.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblComponente.Name = "lblComponente";
            this.lblComponente.Size = new System.Drawing.Size(240, 28);
            this.lblComponente.TabIndex = 5;
            this.lblComponente.Text = "Componente de Atención:";
            // 
            // cmbComponente
            // 
            this.cmbComponente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComponente.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbComponente.FormattingEnabled = true;
            this.cmbComponente.Location = new System.Drawing.Point(40, 254);
            this.cmbComponente.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbComponente.Name = "cmbComponente";
            this.cmbComponente.Size = new System.Drawing.Size(571, 39);
            this.cmbComponente.TabIndex = 6;
            this.cmbComponente.SelectedIndexChanged += new System.EventHandler(this.cmbComponente_SelectedIndexChanged);
            // 
            // lblTituloSubtotal
            // 
            this.lblTituloSubtotal.AutoSize = true;
            this.lblTituloSubtotal.ForeColor = System.Drawing.Color.DimGray;
            this.lblTituloSubtotal.Location = new System.Drawing.Point(36, 320);
            this.lblTituloSubtotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTituloSubtotal.Name = "lblTituloSubtotal";
            this.lblTituloSubtotal.Size = new System.Drawing.Size(231, 28);
            this.lblTituloSubtotal.TabIndex = 7;
            this.lblTituloSubtotal.Text = "Subtotal (Tarifa Fija RD$):";
            // 
            // lblSubtotalValor
            // 
            this.lblSubtotalValor.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtotalValor.Location = new System.Drawing.Point(344, 318);
            this.lblSubtotalValor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSubtotalValor.Name = "lblSubtotalValor";
            this.lblSubtotalValor.Size = new System.Drawing.Size(270, 31);
            this.lblSubtotalValor.TabIndex = 8;
            this.lblSubtotalValor.Text = "RD$ 0.00";
            this.lblSubtotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTituloItbis
            // 
            this.lblTituloItbis.AutoSize = true;
            this.lblTituloItbis.ForeColor = System.Drawing.Color.DimGray;
            this.lblTituloItbis.Location = new System.Drawing.Point(36, 363);
            this.lblTituloItbis.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTituloItbis.Name = "lblTituloItbis";
            this.lblTituloItbis.Size = new System.Drawing.Size(150, 28);
            this.lblTituloItbis.TabIndex = 9;
            this.lblTituloItbis.Text = "ITBIS Fijo (18%):";
            // 
            // lblItbisValor
            // 
            this.lblItbisValor.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItbisValor.Location = new System.Drawing.Point(344, 362);
            this.lblItbisValor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblItbisValor.Name = "lblItbisValor";
            this.lblItbisValor.Size = new System.Drawing.Size(270, 31);
            this.lblItbisValor.TabIndex = 10;
            this.lblItbisValor.Text = "RD$ 0.00";
            this.lblItbisValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTituloTotal
            // 
            this.lblTituloTotal.AutoSize = true;
            this.lblTituloTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloTotal.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblTituloTotal.Location = new System.Drawing.Point(36, 408);
            this.lblTituloTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTituloTotal.Name = "lblTituloTotal";
            this.lblTituloTotal.Size = new System.Drawing.Size(162, 28);
            this.lblTituloTotal.TabIndex = 11;
            this.lblTituloTotal.Text = "Total Calculado:";
            // 
            // lblTotalValor
            // 
            this.lblTotalValor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalValor.ForeColor = System.Drawing.Color.SeaGreen;
            this.lblTotalValor.Location = new System.Drawing.Point(344, 406);
            this.lblTotalValor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalValor.Name = "lblTotalValor";
            this.lblTotalValor.Size = new System.Drawing.Size(270, 32);
            this.lblTotalValor.TabIndex = 12;
            this.lblTotalValor.Text = "RD$ 0.00";
            this.lblTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMonto
            // 
            this.lblMonto.AutoSize = true;
            this.lblMonto.Location = new System.Drawing.Point(36, 457);
            this.lblMonto.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(257, 28);
            this.lblMonto.TabIndex = 13;
            this.lblMonto.Text = "Monto Total a Cobrar (RD$):";
            // 
            // txtMonto
            // 
            this.txtMonto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.txtMonto.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMonto.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.txtMonto.Location = new System.Drawing.Point(40, 489);
            this.txtMonto.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMonto.Name = "txtMonto";
            this.txtMonto.ReadOnly = true;
            this.txtMonto.Size = new System.Drawing.Size(571, 45);
            this.txtMonto.TabIndex = 14;
            this.txtMonto.TabStop = false;
            this.txtMonto.Text = "0.00";
            this.txtMonto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnProcesarCobro
            // 
            this.btnProcesarCobro.BackColor = System.Drawing.Color.SeaGreen;
            this.btnProcesarCobro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcesarCobro.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcesarCobro.ForeColor = System.Drawing.Color.White;
            this.btnProcesarCobro.Location = new System.Drawing.Point(400, 685);
            this.btnProcesarCobro.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnProcesarCobro.Name = "btnProcesarCobro";
            this.btnProcesarCobro.Size = new System.Drawing.Size(292, 69);
            this.btnProcesarCobro.TabIndex = 2;
            this.btnProcesarCobro.Text = "Procesar Cobro";
            this.btnProcesarCobro.UseVisualStyleBackColor = false;
            this.btnProcesarCobro.Click += new System.EventHandler(this.btnProcesarCobro_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.Gray;
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(42, 685);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(225, 69);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // frmCobroServicio
            // 
            this.AcceptButton = this.btnProcesarCobro;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(736, 785);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnProcesarCobro);
            this.Controls.Add(this.gbDatosTransaccion);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
        private System.Windows.Forms.Label lblCedulaTutor;
        private System.Windows.Forms.TextBox txtCedulaTutor;
        private System.Windows.Forms.Button btnBuscarTutor;
        private System.Windows.Forms.Label lblSeleccioneNiño;
        private System.Windows.Forms.ComboBox cmbBeneficiarios;
        private System.Windows.Forms.Label lblComponente;
        private System.Windows.Forms.ComboBox cmbComponente;
        private System.Windows.Forms.Label lblTituloSubtotal;
        private System.Windows.Forms.Label lblSubtotalValor;
        private System.Windows.Forms.Label lblTituloItbis;
        private System.Windows.Forms.Label lblItbisValor;
        private System.Windows.Forms.Label lblTituloTotal;
        private System.Windows.Forms.Label lblTotalValor;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.TextBox txtMonto;
        private System.Windows.Forms.Button btnProcesarCobro;
        private System.Windows.Forms.Button btnCancelar;
    }
}