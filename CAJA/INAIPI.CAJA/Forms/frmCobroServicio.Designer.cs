namespace INAIPI.CAJA.Forms
{
    partial class frmCobroServicio
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblBeneficiario;
        private System.Windows.Forms.TextBox txtBeneficiarioId;
        private System.Windows.Forms.Label lblServicio;
        private System.Windows.Forms.TextBox txtDescripcionServicio;
        private System.Windows.Forms.Label lblImporte;
        private System.Windows.Forms.TextBox txtImporte;
        private System.Windows.Forms.Button btnProcesarCobro;

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
            this.lblBeneficiario = new System.Windows.Forms.Label();
            this.txtBeneficiarioId = new System.Windows.Forms.TextBox();
            this.lblServicio = new System.Windows.Forms.Label();
            this.txtDescripcionServicio = new System.Windows.Forms.TextBox();
            this.lblImporte = new System.Windows.Forms.Label();
            this.txtImporte = new System.Windows.Forms.TextBox();
            this.btnProcesarCobro = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblBeneficiario
            this.lblBeneficiario.AutoSize = true;
            this.lblBeneficiario.Location = new System.Drawing.Point(30, 30);
            this.lblBeneficiario.Name = "lblBeneficiario";
            this.lblBeneficiario.Size = new System.Drawing.Size(95, 13);
            this.lblBeneficiario.Text = "ID del Beneficiario:";

            // txtBeneficiarioId
            this.txtBeneficiarioId.Location = new System.Drawing.Point(33, 46);
            this.txtBeneficiarioId.Name = "txtBeneficiarioId";
            this.txtBeneficiarioId.Size = new System.Drawing.Size(120, 20);

            // lblServicio
            this.lblServicio.AutoSize = true;
            this.lblServicio.Location = new System.Drawing.Point(30, 80);
            this.lblServicio.Name = "lblServicio";
            this.lblServicio.Size = new System.Drawing.Size(122, 13);
            this.lblServicio.Text = "Descripción del Servicio:";

            // txtDescripcionServicio
            this.txtDescripcionServicio.Location = new System.Drawing.Point(33, 96);
            this.txtDescripcionServicio.Name = "txtDescripcionServicio";
            this.txtDescripcionServicio.Size = new System.Drawing.Size(250, 20);

            // lblImporte
            this.lblImporte.AutoSize = true;
            this.lblImporte.Location = new System.Drawing.Point(30, 130);
            this.lblImporte.Name = "lblImporte";
            this.lblImporte.Size = new System.Drawing.Size(45, 13);
            this.lblImporte.Text = "Importe:";

            // txtImporte
            this.txtImporte.Location = new System.Drawing.Point(33, 146);
            this.txtImporte.Name = "txtImporte";
            this.txtImporte.Size = new System.Drawing.Size(120, 20);

            // btnProcesarCobro
            this.btnProcesarCobro.Location = new System.Drawing.Point(33, 190);
            this.btnProcesarCobro.Name = "btnProcesarCobro";
            this.btnProcesarCobro.Size = new System.Drawing.Size(250, 40);
            this.btnProcesarCobro.Text = "Procesar Cobro";
            this.btnProcesarCobro.UseVisualStyleBackColor = true;
            this.btnProcesarCobro.Click += new System.EventHandler(this.btnProcesarCobro_Click);

            // frmCobroServicio
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 260);
            this.Controls.Add(this.btnProcesarCobro);
            this.Controls.Add(this.txtImporte);
            this.Controls.Add(this.lblImporte);
            this.Controls.Add(this.txtDescripcionServicio);
            this.Controls.Add(this.lblServicio);
            this.Controls.Add(this.txtBeneficiarioId);
            this.Controls.Add(this.lblBeneficiario);
            this.Name = "frmCobroServicio";
            this.Text = "Cobro de Servicio";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}