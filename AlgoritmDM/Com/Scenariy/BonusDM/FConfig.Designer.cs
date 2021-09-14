namespace AlgoritmDM.Com.Scenariy.BonusDM
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
            this.PnlTop = new System.Windows.Forms.Panel();
            this.lblScenariyName = new System.Windows.Forms.Label();
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
            this.PnlFillBottom = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.PnlFillTop = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_SC_Perc = new System.Windows.Forms.ComboBox();
            this.chkBox_StartSCProgram = new System.Windows.Forms.CheckBox();
            this.dtTime_StartSCProgram = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBox_DeepConvSC = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBox_DelayPeriod = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmb_ManualSCPerc = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBox_StartSCPerc = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBox_StartSCSumm = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PnlBottom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.PnlTop.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDiscReason)).BeginInit();
            this.PnlFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPorogPoint)).BeginInit();
            this.PnlFillTop.SuspendLayout();
            this.PnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // PnlTop
            // 
            this.PnlTop.Controls.Add(this.lblScenariyName);
            this.PnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.PnlTop.Location = new System.Drawing.Point(0, 0);
            this.PnlTop.Name = "PnlTop";
            this.PnlTop.Size = new System.Drawing.Size(954, 27);
            this.PnlTop.TabIndex = 4;
            // 
            // lblScenariyName
            // 
            this.lblScenariyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblScenariyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblScenariyName.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblScenariyName.Location = new System.Drawing.Point(0, 0);
            this.lblScenariyName.Name = "lblScenariyName";
            this.lblScenariyName.Size = new System.Drawing.Size(954, 27);
            this.lblScenariyName.TabIndex = 1;
            this.lblScenariyName.Text = "Имя сценария";
            this.lblScenariyName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.dgDiscReason);
            this.pnlLeft.Controls.Add(this.label1);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 27);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(212, 450);
            this.pnlLeft.TabIndex = 5;
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
            this.dgDiscReason.Size = new System.Drawing.Size(212, 409);
            this.dgDiscReason.TabIndex = 2;
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
            this.label1.Text = "Типы чеков не учитываемые в программе.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // PnlFill
            // 
            this.PnlFill.Controls.Add(this.dgPorogPoint);
            this.PnlFill.Controls.Add(this.PnlFillBottom);
            this.PnlFill.Controls.Add(this.splitter1);
            this.PnlFill.Controls.Add(this.splitter2);
            this.PnlFill.Controls.Add(this.PnlFillTop);
            this.PnlFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlFill.Location = new System.Drawing.Point(212, 27);
            this.PnlFill.Name = "PnlFill";
            this.PnlFill.Size = new System.Drawing.Size(742, 416);
            this.PnlFill.TabIndex = 7;
            // 
            // dgPorogPoint
            // 
            this.dgPorogPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPorogPoint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Porog,
            this.Procent});
            this.dgPorogPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgPorogPoint.Location = new System.Drawing.Point(3, 133);
            this.dgPorogPoint.Name = "dgPorogPoint";
            this.dgPorogPoint.Size = new System.Drawing.Size(739, 239);
            this.dgPorogPoint.TabIndex = 8;
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
            // PnlFillBottom
            // 
            this.PnlFillBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlFillBottom.Location = new System.Drawing.Point(3, 372);
            this.PnlFillBottom.Name = "PnlFillBottom";
            this.PnlFillBottom.Size = new System.Drawing.Size(739, 41);
            this.PnlFillBottom.TabIndex = 4;
            this.PnlFillBottom.Visible = false;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter1.Location = new System.Drawing.Point(0, 133);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 280);
            this.splitter1.TabIndex = 10;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.SystemColors.Control;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 413);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(742, 3);
            this.splitter2.TabIndex = 9;
            this.splitter2.TabStop = false;
            // 
            // PnlFillTop
            // 
            this.PnlFillTop.Controls.Add(this.label2);
            this.PnlFillTop.Controls.Add(this.cmb_SC_Perc);
            this.PnlFillTop.Controls.Add(this.chkBox_StartSCProgram);
            this.PnlFillTop.Controls.Add(this.dtTime_StartSCProgram);
            this.PnlFillTop.Controls.Add(this.label8);
            this.PnlFillTop.Controls.Add(this.txtBox_DeepConvSC);
            this.PnlFillTop.Controls.Add(this.label7);
            this.PnlFillTop.Controls.Add(this.txtBox_DelayPeriod);
            this.PnlFillTop.Controls.Add(this.label6);
            this.PnlFillTop.Controls.Add(this.cmb_ManualSCPerc);
            this.PnlFillTop.Controls.Add(this.label5);
            this.PnlFillTop.Controls.Add(this.txtBox_StartSCPerc);
            this.PnlFillTop.Controls.Add(this.label4);
            this.PnlFillTop.Controls.Add(this.txtBox_StartSCSumm);
            this.PnlFillTop.Controls.Add(this.label3);
            this.PnlFillTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.PnlFillTop.Location = new System.Drawing.Point(0, 0);
            this.PnlFillTop.Name = "PnlFillTop";
            this.PnlFillTop.Size = new System.Drawing.Size(742, 133);
            this.PnlFillTop.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(227, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Поле в котором хранится текущий процент";
            // 
            // cmb_SC_Perc
            // 
            this.cmb_SC_Perc.FormattingEnabled = true;
            this.cmb_SC_Perc.Items.AddRange(new object[] {
            "ADDRESS1",
            "ADDRESS2",
            "ADDRESS3",
            "ADDRESS4",
            "ADDRESS5",
            "ADDRESS6"});
            this.cmb_SC_Perc.Location = new System.Drawing.Point(260, 74);
            this.cmb_SC_Perc.Name = "cmb_SC_Perc";
            this.cmb_SC_Perc.Size = new System.Drawing.Size(99, 21);
            this.cmb_SC_Perc.TabIndex = 17;
            this.cmb_SC_Perc.SelectedIndexChanged += new System.EventHandler(this.cmb_SC_Perc_SelectedIndexChanged);
            // 
            // chkBox_StartSCProgram
            // 
            this.chkBox_StartSCProgram.AutoSize = true;
            this.chkBox_StartSCProgram.Location = new System.Drawing.Point(275, 102);
            this.chkBox_StartSCProgram.Name = "chkBox_StartSCProgram";
            this.chkBox_StartSCProgram.Size = new System.Drawing.Size(339, 17);
            this.chkBox_StartSCProgram.TabIndex = 16;
            this.chkBox_StartSCProgram.Text = "Указать дату начиная с которой вести бонуснуюс программу";
            this.chkBox_StartSCProgram.UseVisualStyleBackColor = true;
            this.chkBox_StartSCProgram.CheckedChanged += new System.EventHandler(this.chkBox_StartSCProgram_CheckedChanged);
            // 
            // dtTime_StartSCProgram
            // 
            this.dtTime_StartSCProgram.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTime_StartSCProgram.Location = new System.Drawing.Point(623, 100);
            this.dtTime_StartSCProgram.Name = "dtTime_StartSCProgram";
            this.dtTime_StartSCProgram.Size = new System.Drawing.Size(100, 20);
            this.dtTime_StartSCProgram.TabIndex = 15;
            this.dtTime_StartSCProgram.Visible = false;
            this.dtTime_StartSCProgram.ValueChanged += new System.EventHandler(this.dtTime_StartSCProgram_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(414, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(166, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Глубина анализа чеков (в днях)";
            // 
            // txtBox_DeepConvSC
            // 
            this.txtBox_DeepConvSC.Location = new System.Drawing.Point(623, 25);
            this.txtBox_DeepConvSC.Name = "txtBox_DeepConvSC";
            this.txtBox_DeepConvSC.Size = new System.Drawing.Size(100, 20);
            this.txtBox_DeepConvSC.TabIndex = 13;
            this.txtBox_DeepConvSC.TextChanged += new System.EventHandler(this.txtBox_DeepConvSC_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(250, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Через сколько дней после пробития  считать %";
            // 
            // txtBox_DelayPeriod
            // 
            this.txtBox_DelayPeriod.Location = new System.Drawing.Point(259, 50);
            this.txtBox_DelayPeriod.Name = "txtBox_DelayPeriod";
            this.txtBox_DelayPeriod.Size = new System.Drawing.Size(100, 20);
            this.txtBox_DelayPeriod.TabIndex = 11;
            this.txtBox_DelayPeriod.TextChanged += new System.EventHandler(this.txtBox_DelayPeriod_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(229, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Поле в котором хранится процент для випа";
            // 
            // cmb_ManualSCPerc
            // 
            this.cmb_ManualSCPerc.FormattingEnabled = true;
            this.cmb_ManualSCPerc.Items.AddRange(new object[] {
            "UDF1",
            "UDF2",
            "UDF3",
            "UDF4",
            "UDF5",
            "UDF6",
            "UDF7",
            "UDF8",
            "UDF9",
            "UDF10",
            "UDF11",
            "UDF12",
            "UDF13",
            "UDF14",
            "UDF15",
            "UDF16",
            "ADDRESS1",
            "ADDRESS2",
            "ADDRESS3",
            "ADDRESS4",
            "ADDRESS5",
            "ADDRESS6"});
            this.cmb_ManualSCPerc.Location = new System.Drawing.Point(260, 25);
            this.cmb_ManualSCPerc.Name = "cmb_ManualSCPerc";
            this.cmb_ManualSCPerc.Size = new System.Drawing.Size(99, 21);
            this.cmb_ManualSCPerc.TabIndex = 9;
            this.cmb_ManualSCPerc.SelectedIndexChanged += new System.EventHandler(this.cmb_ManualSCPerc_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(417, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Стартовый процент";
            // 
            // txtBox_StartSCPerc
            // 
            this.txtBox_StartSCPerc.Location = new System.Drawing.Point(623, 50);
            this.txtBox_StartSCPerc.Name = "txtBox_StartSCPerc";
            this.txtBox_StartSCPerc.Size = new System.Drawing.Size(100, 20);
            this.txtBox_StartSCPerc.TabIndex = 7;
            this.txtBox_StartSCPerc.TextChanged += new System.EventHandler(this.txtBox_StartSCPerc_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(385, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(230, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Начальная сумма от которой считать бонус";
            // 
            // txtBox_StartSCSumm
            // 
            this.txtBox_StartSCSumm.Location = new System.Drawing.Point(624, 74);
            this.txtBox_StartSCSumm.Name = "txtBox_StartSCSumm";
            this.txtBox_StartSCSumm.Size = new System.Drawing.Size(100, 20);
            this.txtBox_StartSCSumm.TabIndex = 5;
            this.txtBox_StartSCSumm.TextChanged += new System.EventHandler(this.txtBox_StartSCSumm_TextChanged);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.DarkBlue;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(742, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Настройка порогов и других спецефичных параметров";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PnlBottom
            // 
            this.PnlBottom.Controls.Add(this.btnSave);
            this.PnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlBottom.Location = new System.Drawing.Point(212, 443);
            this.PnlBottom.Name = "PnlBottom";
            this.PnlBottom.Size = new System.Drawing.Size(742, 34);
            this.PnlBottom.TabIndex = 8;
            this.PnlBottom.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSave.ForeColor = System.Drawing.Color.DarkBlue;
            this.btnSave.Location = new System.Drawing.Point(525, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Сохранить изменения";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 477);
            this.Controls.Add(this.PnlFill);
            this.Controls.Add(this.PnlBottom);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.PnlTop);
            this.Name = "FConfig";
            this.Text = "Настройка сценария";
            this.PnlTop.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDiscReason)).EndInit();
            this.PnlFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPorogPoint)).EndInit();
            this.PnlFillTop.ResumeLayout(false);
            this.PnlFillTop.PerformLayout();
            this.PnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PnlTop;
        private System.Windows.Forms.Label lblScenariyName;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PnlFill;
        private System.Windows.Forms.Panel PnlFillBottom;
        private System.Windows.Forms.Panel PnlBottom;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel PnlFillTop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgPorogPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn Porog;
        private System.Windows.Forms.DataGridViewTextBoxColumn Procent;
        private System.Windows.Forms.DataGridView dgDiscReason;
        private System.Windows.Forms.DataGridViewTextBoxColumn DiscReasonId;
        private System.Windows.Forms.DataGridViewTextBoxColumn DiscReasonName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Check;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TextBox txtBox_StartSCPerc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBox_StartSCSumm;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmb_ManualSCPerc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBox_DelayPeriod;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBox_DeepConvSC;
        private System.Windows.Forms.DateTimePicker dtTime_StartSCProgram;
        private System.Windows.Forms.CheckBox chkBox_StartSCProgram;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_SC_Perc;
    }
}