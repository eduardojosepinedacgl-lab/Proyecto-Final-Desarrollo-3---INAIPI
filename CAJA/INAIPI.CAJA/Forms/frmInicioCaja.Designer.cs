namespace INAIPI.CAJA.Forms
{
    partial class frmInicioCaja
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

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.txtMontoInicial = new System.Windows.Forms.TextBox();
            this.btnAbrirTurno = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblBienvenida = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblBienvenida
            // 
            this.lblBienvenida.AutoSize = true;
            this.lblBienvenida.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBienvenida.ForeColor = System.Drawing.Color.SeaGreen;
            this.lblBienvenida.Location = new System.Drawing.Point(34, 15);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.Size = new System.Drawing.Size(155, 20);
            this.lblBienvenida.TabIndex = 4;
            this.lblBienvenida.Text = "Bienvenido/a, Cajero";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(34, 55);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(262, 19);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Ingrese el Monto Inicial (Fondo de Caja):";
            // 
            // txtMontoInicial
            // 
            this.txtMontoInicial.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMontoInicial.Location = new System.Drawing.Point(38, 80);
            this.txtMontoInicial.Name = "txtMontoInicial";
            this.txtMontoInicial.Size = new System.Drawing.Size(258, 33);
            this.txtMontoInicial.TabIndex = 1;
            this.txtMontoInicial.Text = "0.00";
            this.txtMontoInicial.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnAbrirTurno
            // 
            this.btnAbrirTurno.BackColor = System.Drawing.Color.SeaGreen;
            this.btnAbrirTurno.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbrirTurno.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbrirTurno.ForeColor = System.Drawing.Color.White;
            this.btnAbrirTurno.Location = new System.Drawing.Point(176, 135);
            this.btnAbrirTurno.Name = "btnAbrirTurno";
            this.btnAbrirTurno.Size = new System.Drawing.Size(120, 35);
            this.btnAbrirTurno.TabIndex = 2;
            this.btnAbrirTurno.Text = "Abrir Turno";
            this.btnAbrirTurno.UseVisualStyleBackColor = false;
            this.btnAbrirTurno.Click += new System.EventHandler(this.btnAbrirTurno_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.IndianRed;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(38, 135);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(120, 35);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // frmInicioCaja
            // 
            this.AcceptButton = this.btnAbrirTurno;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(334, 195);
            this.Controls.Add(this.lblBienvenida);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAbrirTurno);
            this.Controls.Add(this.txtMontoInicial);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInicioCaja";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Apertura de Turno - INAIPI";
            this.Load += new System.EventHandler(this.frmInicioCaja_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TextBox txtMontoInicial;
        private System.Windows.Forms.Button btnAbrirTurno;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblBienvenida;
    }
}