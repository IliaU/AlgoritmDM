namespace AlgoritmDM.Com.Scenariy.NakopDM
{
    partial class FConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.dgDiscReason = new System.Windows.Forms.DataGridView();
            this.DiscReasonId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DiscReasonName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Check = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.PnlFill = new System.Windows.Forms.Panel();
            this.dgPorogPoint = new System.Windows.Forms.DataGridView();
            this.Porog = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Procent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PnlFillTop = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.PnlFillBottom = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.PnlBottom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.PnlTop = new System.Windows.Forms.Panel();
            this.lblScenariyName = new System.Windows.Forms.Label();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDiscReason)).BeginInit();
            this.PnlFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPorogPoint)).BeginInit();
            this.PnlFillTop.SuspendLayout();
            this.PnlBottom.SuspendLayout();
            this.PnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.dgDiscReason);
            this.pnlLeft.Controls.Add(this.label1);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 27);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(212, 303);
            this.pnlLeft.TabIndex = 0;
            // 
            // dgDiscReason
            // 
            this.dgDiscReason.AllowUserToAddRows = false;
            this.dgDiscReason.AllowUserToDeleteRows = false;
            this.dgDiscReason.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDiscReason.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DiscReasonId,
            this.DiscReasonName,
            this.Check});
            this.dgDiscReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgDiscReason.Location = new System.Drawing.Point(0, 41);
            this.dgDiscReason.Name = "dgDiscReason";
            this.dgDiscReason.ReadOnly = true;
            this.dgDiscReason.Size = new System.Drawing.Size(212, 262);
            this.dgDiscReason.TabIndex = 1;
            this.dgDiscReason.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgDiscReason_CellClick);
            // 
            // DiscReasonId
            // 
            this.DiscReasonId.DataPropertyName = "DiscReasonId";
            this.DiscReasonId.HeaderText = "DiscReasonId";
            this.DiscReasonId.Name = "DiscReasonId";
            this.DiscReasonId.ReadOnly = true;
            this.DiscReasonId.Visible = false;
            // 
            // DiscReasonName
            // 
            this.DiscReasonName.DataPropertyName = "DiscReasonName";
            this.DiscReasonName.HeaderText = "Тип чека";
            this.DiscReasonName.Name = "DiscReasonName";
            this.DiscReasonName.ReadOnly = true;
            // 
            // Check
            // 
            this.Check.DataPropertyName = "Check";
            this.Check.HeaderText = "Флаг";
            this.Check.Name = "Check";
            this.Check.ReadOnly = true;
            this.Check.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Check.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Check.Width = 50;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.DarkBlue;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 41);
            this.label1.TabIndex = 0;
            this.label1.Text = "Типы чеков не учитываемые при накоплении.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // PnlFill
            // 
            this.PnlFill.Controls.Add(this.dgPorogPoint);
            this.PnlFill.Controls.Add(this.PnlFillTop);
            this.PnlFill.Controls.Add(this.PnlFillBottom);
            this.PnlFill.Controls.Add(this.splitter1);
            this.PnlFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlFill.Location = new System.Drawing.Point(212, 27);
            this.PnlFill.Name = "PnlFill";
            this.PnlFill.Size = new System.Drawing.Size(367, 276);
            this.PnlFill.TabIndex = 1;
            // 
            // dgPorogPoint
            // 
            this.dgPorogPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPorogPoint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Porog,
            this.Procent});
            this.dgPorogPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgPorogPoint.Location = new System.Drawing.Point(3, 42);
            this.dgPorogPoint.Name = "dgPorogPoint";
            this.dgPorogPoint.Size = new System.Drawing.Size(364, 200);
            this.dgPorogPoint.TabIndex = 3;
            this.dgPorogPoint.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgPorogPoint_CellValueChanged);
            this.dgPorogPoint.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgPorogPoint_UserAddedRow);
            this.dgPorogPoint.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgPorogPoint_UserDeletingRow);
            // 
            // Porog
            // 
            this.Porog.DataPropertyName = "Porog";
            this.Porog.HeaderText = "Порог срабатывания";
            this.Porog.Name = "Porog";
            this.Porog.Width = 150;
            // 
            // Procent
            // 
            this.Procent.DataPropertyName = "Procent";
            this.Procent.HeaderText = "Процент скидки";
            this.Procent.Name = "Procent";
            this.Procent.Width = 150;
            // 
            // PnlFillTop
            // 
            this.PnlFillTop.Controls.Add(this.label2);
            this.PnlFillTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.PnlFillTop.Location = new System.Drawing.Point(3, 0);
            this.PnlFillTop.Name = "PnlFillTop";
            this.PnlFillTop.Size = new System.Drawing.Size(364, 42);
            this.PnlFillTop.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.DarkBlue;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(364, 42);
            this.label2.TabIndex = 1;
            this.label2.Text = "Настройка порогов";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlFillBottom
            // 
            this.PnlFillBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlFillBottom.Location = new System.Drawing.Point(3, 242);
            this.PnlFillBottom.Name = "PnlFillBottom";
            this.PnlFillBottom.Size = new System.Drawing.Size(364, 34);
            this.PnlFillBottom.TabIndex = 1;
            this.PnlFillBottom.Visible = false;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 276);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // PnlBottom
            // 
            this.PnlBottom.Controls.Add(this.btnSave);
            this.PnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlBottom.Location = new System.Drawing.Point(212, 303);
            this.PnlBottom.Name = "PnlBottom";
            this.PnlBottom.Size = new System.Drawing.Size(367, 27);
            this.PnlBottom.TabIndex = 2;
            this.PnlBottom.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSave.ForeColor = System.Drawing.Color.DarkBlue;
            this.btnSave.Location = new System.Drawing.Point(218, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Сохранить изменения";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // PnlTop
            // 
            this.PnlTop.Controls.Add(this.lblScenariyName);
            this.PnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.PnlTop.Location = new System.Drawing.Point(0, 0);
            this.PnlTop.Name = "PnlTop";
            this.PnlTop.Size = new System.Drawing.Size(579, 27);
            this.PnlTop.TabIndex = 3;
            // 
            // lblScenariyName
            // 
            this.lblScenariyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblScenariyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblScenariyName.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblScenariyName.Location = new System.Drawing.Point(0, 0);
            this.lblScenariyName.Name = "lblScenariyName";
            this.lblScenariyName.Size = new System.Drawing.Size(579, 27);
            this.lblScenariyName.TabIndex = 1;
            this.lblScenariyName.Text = "Имя сценария";
            this.lblScenariyName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.SystemColors.Control;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(212, 300);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(367, 3);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // FConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 330);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.PnlFill);
            this.Controls.Add(this.PnlBottom);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.PnlTop);
            this.Name = "FConfig";
            this.Text = "Настройка сценария";
            this.pnlLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDiscReason)).EndInit();
            this.PnlFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPorogPoint)).EndInit();
            this.PnlFillTop.ResumeLayout(false);
            this.PnlBottom.ResumeLayout(false);
            this.PnlTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.DataGridView dgDiscReason;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PnlFill;
        private System.Windows.Forms.DataGridViewTextBoxColumn DiscReasonId;
        private System.Windows.Forms.DataGridViewTextBoxColumn DiscReasonName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Check;
        private System.Windows.Forms.Panel PnlBottom;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel PnlTop;
        private System.Windows.Forms.Label lblScenariyName;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridView dgPorogPoint;
        private System.Windows.Forms.Panel PnlFillTop;
        private System.Windows.Forms.Panel PnlFillBottom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Porog;
        private System.Windows.Forms.DataGridViewTextBoxColumn Procent;
        private System.Windows.Forms.Splitter splitter2;
    }
}