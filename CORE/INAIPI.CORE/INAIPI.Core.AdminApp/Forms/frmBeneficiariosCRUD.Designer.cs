namespace INAIPI.Core.AdminApp.Forms
{
    partial class frmBeneficiariosCRUD
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvBeneficiarios;
        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtBeneficiarioId;
        private System.Windows.Forms.Label lblNombres;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.Label lblApellidos;
        private System.Windows.Forms.TextBox txtApellidos;
        private System.Windows.Forms.Label lblFechaNac;
        private System.Windows.Forms.DateTimePicker dtpFechaNacimiento;
        private System.Windows.Forms.Label lblCentro;
        private System.Windows.Forms.ComboBox cmbCentros;
        private System.Windows.Forms.Label lblTutorDoc;
        private System.Windows.Forms.TextBox txtDocumentoTutor;
        private System.Windows.Forms.Label lblTutorNombres;
        private System.Windows.Forms.TextBox txtNombresTutor;
        private System.Windows.Forms.Label lblTutorApellidos;
        private System.Windows.Forms.TextBox txtApellidosTutor;
        private System.Windows.Forms.Label lblTutorTel;
        private System.Windows.Forms.TextBox txtTelefonoTutor;
        private System.Windows.Forms.Label lblTutorDir;
        private System.Windows.Forms.TextBox txtDireccionTutor;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnCargar;

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
            this.dgvBeneficiarios = new System.Windows.Forms.DataGridView();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.lblId = new System.Windows.Forms.Label();
            this.txtBeneficiarioId = new System.Windows.Forms.TextBox();
            this.lblNombres = new System.Windows.Forms.Label();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.lblApellidos = new System.Windows.Forms.Label();
            this.txtApellidos = new System.Windows.Forms.TextBox();
            this.lblFechaNac = new System.Windows.Forms.Label();
            this.dtpFechaNacimiento = new System.Windows.Forms.DateTimePicker();
            this.lblCentro = new System.Windows.Forms.Label();
            this.cmbCentros = new System.Windows.Forms.ComboBox();
            this.lblTutorDoc = new System.Windows.Forms.Label();
            this.txtDocumentoTutor = new System.Windows.Forms.TextBox();
            this.lblTutorNombres = new System.Windows.Forms.Label();
            this.txtNombresTutor = new System.Windows.Forms.TextBox();
            this.lblTutorApellidos = new System.Windows.Forms.Label();
            this.txtApellidosTutor = new System.Windows.Forms.TextBox();
            this.lblTutorTel = new System.Windows.Forms.Label();
            this.txtTelefonoTutor = new System.Windows.Forms.TextBox();
            this.lblTutorDir = new System.Windows.Forms.Label();
            this.txtDireccionTutor = new System.Windows.Forms.TextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnCargar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBeneficiarios)).BeginInit();
            this.grpDatos.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvBeneficiarios
            // 
            this.dgvBeneficiarios.AllowUserToAddRows = false;
            this.dgvBeneficiarios.AllowUserToDeleteRows = false;
            this.dgvBeneficiarios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBeneficiarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBeneficiarios.Location = new System.Drawing.Point(12, 270);
            this.dgvBeneficiarios.MultiSelect = false;
            this.dgvBeneficiarios.Name = "dgvBeneficiarios";
            this.dgvBeneficiarios.ReadOnly = true;
            this.dgvBeneficiarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBeneficiarios.Size = new System.Drawing.Size(960, 329);
            this.dgvBeneficiarios.TabIndex = 0;
            this.dgvBeneficiarios.SelectionChanged += new System.EventHandler(this.dgvBeneficiarios_SelectionChanged);
            // 
            // grpDatos
            // 
            this.grpDatos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDatos.Controls.Add(this.lblVersion);
            this.grpDatos.Controls.Add(this.txtVersion);
            this.grpDatos.Controls.Add(this.lblTutorDir);
            this.grpDatos.Controls.Add(this.txtDireccionTutor);
            this.grpDatos.Controls.Add(this.lblTutorTel);
            this.grpDatos.Controls.Add(this.txtTelefonoTutor);
            this.grpDatos.Controls.Add(this.lblTutorApellidos);
            this.grpDatos.Controls.Add(this.txtApellidosTutor);
            this.grpDatos.Controls.Add(this.lblTutorNombres);
            this.grpDatos.Controls.Add(this.txtNombresTutor);
            this.grpDatos.Controls.Add(this.lblTutorDoc);
            this.grpDatos.Controls.Add(this.txtDocumentoTutor);
            this.grpDatos.Controls.Add(this.lblCentro);
            this.grpDatos.Controls.Add(this.cmbCentros);
            this.grpDatos.Controls.Add(this.lblFechaNac);
            this.grpDatos.Controls.Add(this.dtpFechaNacimiento);
            this.grpDatos.Controls.Add(this.lblApellidos);
            this.grpDatos.Controls.Add(this.txtApellidos);
            this.grpDatos.Controls.Add(this.lblNombres);
            this.grpDatos.Controls.Add(this.txtNombres);
            this.grpDatos.Controls.Add(this.lblId);
            this.grpDatos.Controls.Add(this.txtBeneficiarioId);
            this.grpDatos.Location = new System.Drawing.Point(12, 12);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(960, 210);
            this.grpDatos.TabIndex = 1;
            this.grpDatos.TabStop = false;
            this.grpDatos.Text = "Información del Beneficiario y Tutor";
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(15, 25);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(81, 13);
            this.lblId.TabIndex = 0;
            this.lblId.Text = "Beneficiario ID:";
            // 
            // txtBeneficiarioId
            // 
            this.txtBeneficiarioId.Location = new System.Drawing.Point(105, 22);
            this.txtBeneficiarioId.Name = "txtBeneficiarioId";
            this.txtBeneficiarioId.ReadOnly = true;
            this.txtBeneficiarioId.Size = new System.Drawing.Size(80, 20);
            this.txtBeneficiarioId.TabIndex = 1;
            // 
            // lblNombres
            // 
            this.lblNombres.AutoSize = true;
            this.lblNombres.Location = new System.Drawing.Point(15, 60);
            this.lblNombres.Name = "lblNombres";
            this.lblNombres.Size = new System.Drawing.Size(52, 13);
            this.lblNombres.TabIndex = 2;
            this.lblNombres.Text = "Nombres:";
            // 
            // txtNombres
            // 
            this.txtNombres.Location = new System.Drawing.Point(105, 57);
            this.txtNombres.MaxLength = 100;
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(200, 20);
            this.txtNombres.TabIndex = 3;
            // 
            // lblApellidos
            // 
            this.lblApellidos.AutoSize = true;
            this.lblApellidos.Location = new System.Drawing.Point(15, 95);
            this.lblApellidos.Name = "lblApellidos";
            this.lblApellidos.Size = new System.Drawing.Size(52, 13);
            this.lblApellidos.TabIndex = 4;
            this.lblApellidos.Text = "Apellidos:";
            // 
            // txtApellidos
            // 
            this.txtApellidos.Location = new System.Drawing.Point(105, 92);
            this.txtApellidos.MaxLength = 100;
            this.txtApellidos.Name = "txtApellidos";
            this.txtApellidos.Size = new System.Drawing.Size(200, 20);
            this.txtApellidos.TabIndex = 5;
            // 
            // lblFechaNac
            // 
            this.lblFechaNac.AutoSize = true;
            this.lblFechaNac.Location = new System.Drawing.Point(15, 130);
            this.lblFechaNac.Name = "lblFechaNac";
            this.lblFechaNac.Size = new System.Drawing.Size(84, 13);
            this.lblFechaNac.TabIndex = 6;
            this.lblFechaNac.Text = "Fecha Nacim.:";
            // 
            // dtpFechaNacimiento
            // 
            this.dtpFechaNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaNacimiento.Location = new System.Drawing.Point(105, 127);
            this.dtpFechaNacimiento.Name = "dtpFechaNacimiento";
            this.dtpFechaNacimiento.Size = new System.Drawing.Size(120, 20);
            this.dtpFechaNacimiento.TabIndex = 7;
            // 
            // lblCentro
            // 
            this.lblCentro.AutoSize = true;
            this.lblCentro.Location = new System.Drawing.Point(15, 165);
            this.lblCentro.Name = "lblCentro";
            this.lblCentro.Size = new System.Drawing.Size(43, 13);
            this.lblCentro.TabIndex = 8;
            this.lblCentro.Text = "Centro:";
            // 
            // cmbCentros
            // 
            this.cmbCentros.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCentros.FormattingEnabled = true;
            this.cmbCentros.Location = new System.Drawing.Point(105, 162);
            this.cmbCentros.Name = "cmbCentros";
            this.cmbCentros.Size = new System.Drawing.Size(200, 21);
            this.cmbCentros.TabIndex = 9;
            // 
            // lblTutorDoc
            // 
            this.lblTutorDoc.AutoSize = true;
            this.lblTutorDoc.Location = new System.Drawing.Point(340, 25);
            this.lblTutorDoc.Name = "lblTutorDoc";
            this.lblTutorDoc.Size = new System.Drawing.Size(73, 13);
            this.lblTutorDoc.TabIndex = 10;
            this.lblTutorDoc.Text = "Cédula Tutor:";
            // 
            // txtDocumentoTutor
            // 
            this.txtDocumentoTutor.Location = new System.Drawing.Point(430, 22);
            this.txtDocumentoTutor.MaxLength = 20;
            this.txtDocumentoTutor.Name = "txtDocumentoTutor";
            this.txtDocumentoTutor.Size = new System.Drawing.Size(150, 20);
            this.txtDocumentoTutor.TabIndex = 11;
            // 
            // lblTutorNombres
            // 
            this.lblTutorNombres.AutoSize = true;
            this.lblTutorNombres.Location = new System.Drawing.Point(340, 60);
            this.lblTutorNombres.Name = "lblTutorNombres";
            this.lblTutorNombres.Size = new System.Drawing.Size(81, 13);
            this.lblTutorNombres.TabIndex = 12;
            this.lblTutorNombres.Text = "Nombres Tutor:";
            // 
            // txtNombresTutor
            // 
            this.txtNombresTutor.Location = new System.Drawing.Point(430, 57);
            this.txtNombresTutor.MaxLength = 100;
            this.txtNombresTutor.Name = "txtNombresTutor";
            this.txtNombresTutor.Size = new System.Drawing.Size(200, 20);
            this.txtNombresTutor.TabIndex = 13;
            // 
            // lblTutorApellidos
            // 
            this.lblTutorApellidos.AutoSize = true;
            this.lblTutorApellidos.Location = new System.Drawing.Point(340, 95);
            this.lblTutorApellidos.Name = "lblTutorApellidos";
            this.lblTutorApellidos.Size = new System.Drawing.Size(81, 13);
            this.lblTutorApellidos.TabIndex = 14;
            this.lblTutorApellidos.Text = "Apellidos Tutor:";
            // 
            // txtApellidosTutor
            // 
            this.txtApellidosTutor.Location = new System.Drawing.Point(430, 92);
            this.txtApellidosTutor.MaxLength = 100;
            this.txtApellidosTutor.Name = "txtApellidosTutor";
            this.txtApellidosTutor.Size = new System.Drawing.Size(200, 20);
            this.txtApellidosTutor.TabIndex = 15;
            // 
            // lblTutorTel
            // 
            this.lblTutorTel.AutoSize = true;
            this.lblTutorTel.Location = new System.Drawing.Point(340, 130);
            this.lblTutorTel.Name = "lblTutorTel";
            this.lblTutorTel.Size = new System.Drawing.Size(81, 13);
            this.lblTutorTel.TabIndex = 16;
            this.lblTutorTel.Text = "Teléfono Tutor:";
            // 
            // txtTelefonoTutor
            // 
            this.txtTelefonoTutor.Location = new System.Drawing.Point(430, 127);
            this.txtTelefonoTutor.MaxLength = 20;
            this.txtTelefonoTutor.Name = "txtTelefonoTutor";
            this.txtTelefonoTutor.Size = new System.Drawing.Size(150, 20);
            this.txtTelefonoTutor.TabIndex = 17;
            // 
            // lblTutorDir
            // 
            this.lblTutorDir.AutoSize = true;
            this.lblTutorDir.Location = new System.Drawing.Point(340, 165);
            this.lblTutorDir.Name = "lblTutorDir";
            this.lblTutorDir.Size = new System.Drawing.Size(84, 13);
            this.lblTutorDir.TabIndex = 18;
            this.lblTutorDir.Text = "Dirección Tutor:";
            // 
            // txtDireccionTutor
            // 
            this.txtDireccionTutor.Location = new System.Drawing.Point(430, 162);
            this.txtDireccionTutor.MaxLength = 250;
            this.txtDireccionTutor.Name = "txtDireccionTutor";
            this.txtDireccionTutor.Size = new System.Drawing.Size(350, 20);
            this.txtDireccionTutor.TabIndex = 19;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(670, 25);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(83, 13);
            this.lblVersion.TabIndex = 20;
            this.lblVersion.Text = "Versión Control:";
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(760, 22);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(50, 20);
            this.txtVersion.TabIndex = 21;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(12, 230);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(110, 30);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "Nuevo Registro";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnActualizar
            // 
            this.btnActualizar.Location = new System.Drawing.Point(135, 230);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(110, 30);
            this.btnActualizar.TabIndex = 3;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(260, 230);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(90, 30);
            this.btnLimpiar.TabIndex = 4;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnCargar
            // 
            this.btnCargar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCargar.Location = new System.Drawing.Point(862, 230);
            this.btnCargar.Name = "btnCargar";
            this.btnCargar.Size = new System.Drawing.Size(110, 30);
            this.btnCargar.TabIndex = 5;
            this.btnCargar.Text = "Refrescar Lista";
            this.btnCargar.UseVisualStyleBackColor = true;
            this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // frmBeneficiariosCRUD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.btnCargar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.grpDatos);
            this.Controls.Add(this.dgvBeneficiarios);
            this.Name = "frmBeneficiariosCRUD";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mantenimiento de Beneficiarios y Tutores";
            this.Load += new System.EventHandler(this.frmBeneficiariosCRUD_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBeneficiarios)).EndInit();
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}