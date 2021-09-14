namespace AlgoritmDM
{
    partial class FPar
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
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lbl_DirAlgoritmSMTP = new System.Windows.Forms.Label();
            this.btn_DirAlgoritmSMTP = new System.Windows.Forms.Button();
            this.lbl_DirAlgoritmSmtpOut = new System.Windows.Forms.Label();
            this.lbl_AlgoritmSmtpText = new System.Windows.Forms.Label();
            this.txtBox_AlgoritmSmtpText = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtBoxShopName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_AlgoritmSmtpQuery = new System.Windows.Forms.Label();
            this.lbl_AlgoritmSmtpPar = new System.Windows.Forms.Label();
            this.txtBox_AlgoritmSmtpQuery = new System.Windows.Forms.TextBox();
            this.txtBox_AlgoritmSmtpPar = new System.Windows.Forms.TextBox();
            this.chkBox_VisibleCalculateCustomColumn = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtBox_CustomerPrefixPhoneList = new System.Windows.Forms.TextBox();
            this.lbl_CustomerPrefixPhoneList = new System.Windows.Forms.Label();
            this.txtBox_CustomerCountryList = new System.Windows.Forms.TextBox();
            this.lbl_CustomerCountryList = new System.Windows.Forms.Label();
            this.chkBox_Trace = new System.Windows.Forms.CheckBox();
            this.lbl_Mode = new System.Windows.Forms.Label();
            this.cmbBox_Mode = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.pnl_Buttom = new System.Windows.Forms.Panel();
            this.pnl_Fill = new System.Windows.Forms.Panel();
            this.txtBox_LogNotValidCustomer = new System.Windows.Forms.TextBox();
            this.lbl_LogNotValidCustomer = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.pnl_Buttom.SuspendLayout();
            this.pnl_Fill.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_DirAlgoritmSMTP
            // 
            this.lbl_DirAlgoritmSMTP.AutoSize = true;
            this.lbl_DirAlgoritmSMTP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_DirAlgoritmSMTP.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_DirAlgoritmSMTP.Location = new System.Drawing.Point(19, 21);
            this.lbl_DirAlgoritmSMTP.Name = "lbl_DirAlgoritmSMTP";
            this.lbl_DirAlgoritmSMTP.Size = new System.Drawing.Size(355, 13);
            this.lbl_DirAlgoritmSMTP.TabIndex = 0;
            this.lbl_DirAlgoritmSMTP.Text = "Путь к папке в которой лежит приложение  AlgoritmSMTP:";
            // 
            // btn_DirAlgoritmSMTP
            // 
            this.btn_DirAlgoritmSMTP.Location = new System.Drawing.Point(468, 16);
            this.btn_DirAlgoritmSMTP.Name = "btn_DirAlgoritmSMTP";
            this.btn_DirAlgoritmSMTP.Size = new System.Drawing.Size(102, 23);
            this.btn_DirAlgoritmSMTP.TabIndex = 1;
            this.btn_DirAlgoritmSMTP.Text = "Выбрать папку";
            this.btn_DirAlgoritmSMTP.UseVisualStyleBackColor = true;
            this.btn_DirAlgoritmSMTP.Click += new System.EventHandler(this.btn_DirAlgoritmSMTP_Click);
            // 
            // lbl_DirAlgoritmSmtpOut
            // 
            this.lbl_DirAlgoritmSmtpOut.AutoSize = true;
            this.lbl_DirAlgoritmSmtpOut.Location = new System.Drawing.Point(36, 40);
            this.lbl_DirAlgoritmSmtpOut.Name = "lbl_DirAlgoritmSmtpOut";
            this.lbl_DirAlgoritmSmtpOut.Size = new System.Drawing.Size(13, 13);
            this.lbl_DirAlgoritmSmtpOut.TabIndex = 2;
            this.lbl_DirAlgoritmSmtpOut.Text = "_";
            // 
            // lbl_AlgoritmSmtpText
            // 
            this.lbl_AlgoritmSmtpText.AutoSize = true;
            this.lbl_AlgoritmSmtpText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_AlgoritmSmtpText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_AlgoritmSmtpText.Location = new System.Drawing.Point(20, 59);
            this.lbl_AlgoritmSmtpText.Name = "lbl_AlgoritmSmtpText";
            this.lbl_AlgoritmSmtpText.Size = new System.Drawing.Size(473, 13);
            this.lbl_AlgoritmSmtpText.TabIndex = 3;
            this.lbl_AlgoritmSmtpText.Text = "Текст в контекстром меню прпри отправке через приложение  AlgoritmSMTP:";
            // 
            // txtBox_AlgoritmSmtpText
            // 
            this.txtBox_AlgoritmSmtpText.Location = new System.Drawing.Point(39, 75);
            this.txtBox_AlgoritmSmtpText.Name = "txtBox_AlgoritmSmtpText";
            this.txtBox_AlgoritmSmtpText.Size = new System.Drawing.Size(531, 20);
            this.txtBox_AlgoritmSmtpText.TabIndex = 4;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(538, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtBoxShopName
            // 
            this.txtBoxShopName.Location = new System.Drawing.Point(245, 11);
            this.txtBoxShopName.Name = "txtBoxShopName";
            this.txtBoxShopName.Size = new System.Drawing.Size(199, 20);
            this.txtBoxShopName.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Navy;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Папка в которую писать логи:";
            // 
            // lbl_AlgoritmSmtpQuery
            // 
            this.lbl_AlgoritmSmtpQuery.AutoSize = true;
            this.lbl_AlgoritmSmtpQuery.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_AlgoritmSmtpQuery.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_AlgoritmSmtpQuery.Location = new System.Drawing.Point(20, 101);
            this.lbl_AlgoritmSmtpQuery.Name = "lbl_AlgoritmSmtpQuery";
            this.lbl_AlgoritmSmtpQuery.Size = new System.Drawing.Size(217, 13);
            this.lbl_AlgoritmSmtpQuery.TabIndex = 8;
            this.lbl_AlgoritmSmtpQuery.Text = "Задание которое нужно запустить:";
            // 
            // lbl_AlgoritmSmtpPar
            // 
            this.lbl_AlgoritmSmtpPar.AutoSize = true;
            this.lbl_AlgoritmSmtpPar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_AlgoritmSmtpPar.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_AlgoritmSmtpPar.Location = new System.Drawing.Point(20, 125);
            this.lbl_AlgoritmSmtpPar.Name = "lbl_AlgoritmSmtpPar";
            this.lbl_AlgoritmSmtpPar.Size = new System.Drawing.Size(285, 13);
            this.lbl_AlgoritmSmtpPar.TabIndex = 9;
            this.lbl_AlgoritmSmtpPar.Text = "Параметры которое нужно передать заданию:";
            // 
            // txtBox_AlgoritmSmtpQuery
            // 
            this.txtBox_AlgoritmSmtpQuery.Location = new System.Drawing.Point(314, 98);
            this.txtBox_AlgoritmSmtpQuery.Name = "txtBox_AlgoritmSmtpQuery";
            this.txtBox_AlgoritmSmtpQuery.Size = new System.Drawing.Size(199, 20);
            this.txtBox_AlgoritmSmtpQuery.TabIndex = 10;
            // 
            // txtBox_AlgoritmSmtpPar
            // 
            this.txtBox_AlgoritmSmtpPar.Location = new System.Drawing.Point(314, 122);
            this.txtBox_AlgoritmSmtpPar.Name = "txtBox_AlgoritmSmtpPar";
            this.txtBox_AlgoritmSmtpPar.Size = new System.Drawing.Size(199, 20);
            this.txtBox_AlgoritmSmtpPar.TabIndex = 11;
            // 
            // chkBox_VisibleCalculateCustomColumn
            // 
            this.chkBox_VisibleCalculateCustomColumn.AutoSize = true;
            this.chkBox_VisibleCalculateCustomColumn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkBox_VisibleCalculateCustomColumn.ForeColor = System.Drawing.Color.MidnightBlue;
            this.chkBox_VisibleCalculateCustomColumn.Location = new System.Drawing.Point(21, 91);
            this.chkBox_VisibleCalculateCustomColumn.Name = "chkBox_VisibleCalculateCustomColumn";
            this.chkBox_VisibleCalculateCustomColumn.Size = new System.Drawing.Size(501, 17);
            this.chkBox_VisibleCalculateCustomColumn.TabIndex = 12;
            this.chkBox_VisibleCalculateCustomColumn.Text = "Видимость полей с текущими данными по скидкам в режиме просматровщика";
            this.chkBox_VisibleCalculateCustomColumn.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(630, 318);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lbl_LogNotValidCustomer);
            this.tabPage1.Controls.Add(this.txtBox_LogNotValidCustomer);
            this.tabPage1.Controls.Add(this.txtBox_CustomerPrefixPhoneList);
            this.tabPage1.Controls.Add(this.lbl_CustomerPrefixPhoneList);
            this.tabPage1.Controls.Add(this.txtBox_CustomerCountryList);
            this.tabPage1.Controls.Add(this.lbl_CustomerCountryList);
            this.tabPage1.Controls.Add(this.chkBox_Trace);
            this.tabPage1.Controls.Add(this.lbl_Mode);
            this.tabPage1.Controls.Add(this.cmbBox_Mode);
            this.tabPage1.Controls.Add(this.chkBox_VisibleCalculateCustomColumn);
            this.tabPage1.Location = new System.Drawing.Point(23, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(603, 310);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Глобальные";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtBox_CustomerPrefixPhoneList
            // 
            this.txtBox_CustomerPrefixPhoneList.Location = new System.Drawing.Point(118, 198);
            this.txtBox_CustomerPrefixPhoneList.Name = "txtBox_CustomerPrefixPhoneList";
            this.txtBox_CustomerPrefixPhoneList.Size = new System.Drawing.Size(404, 20);
            this.txtBox_CustomerPrefixPhoneList.TabIndex = 19;
            // 
            // lbl_CustomerPrefixPhoneList
            // 
            this.lbl_CustomerPrefixPhoneList.AutoSize = true;
            this.lbl_CustomerPrefixPhoneList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_CustomerPrefixPhoneList.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_CustomerPrefixPhoneList.Location = new System.Drawing.Point(18, 182);
            this.lbl_CustomerPrefixPhoneList.Name = "lbl_CustomerPrefixPhoneList";
            this.lbl_CustomerPrefixPhoneList.Size = new System.Drawing.Size(458, 13);
            this.lbl_CustomerPrefixPhoneList.TabIndex = 18;
            this.lbl_CustomerPrefixPhoneList.Text = "Фильтрвать клиентов по указанному списку через запятую кодов региона";
            // 
            // txtBox_CustomerCountryList
            // 
            this.txtBox_CustomerCountryList.Location = new System.Drawing.Point(118, 159);
            this.txtBox_CustomerCountryList.Name = "txtBox_CustomerCountryList";
            this.txtBox_CustomerCountryList.Size = new System.Drawing.Size(404, 20);
            this.txtBox_CustomerCountryList.TabIndex = 17;
            // 
            // lbl_CustomerCountryList
            // 
            this.lbl_CustomerCountryList.AutoSize = true;
            this.lbl_CustomerCountryList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_CustomerCountryList.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_CustomerCountryList.Location = new System.Drawing.Point(18, 143);
            this.lbl_CustomerCountryList.Name = "lbl_CustomerCountryList";
            this.lbl_CustomerCountryList.Size = new System.Drawing.Size(458, 13);
            this.lbl_CustomerCountryList.TabIndex = 16;
            this.lbl_CustomerCountryList.Text = "Фильтрвать клиентов по указанному списку через запятую кодов региона";
            // 
            // chkBox_Trace
            // 
            this.chkBox_Trace.AutoSize = true;
            this.chkBox_Trace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkBox_Trace.ForeColor = System.Drawing.Color.MidnightBlue;
            this.chkBox_Trace.Location = new System.Drawing.Point(21, 114);
            this.chkBox_Trace.Name = "chkBox_Trace";
            this.chkBox_Trace.Size = new System.Drawing.Size(186, 17);
            this.chkBox_Trace.TabIndex = 15;
            this.chkBox_Trace.Text = "Расширенное логирование";
            this.chkBox_Trace.UseVisualStyleBackColor = true;
            // 
            // lbl_Mode
            // 
            this.lbl_Mode.AutoSize = true;
            this.lbl_Mode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_Mode.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_Mode.Location = new System.Drawing.Point(18, 20);
            this.lbl_Mode.Name = "lbl_Mode";
            this.lbl_Mode.Size = new System.Drawing.Size(94, 13);
            this.lbl_Mode.TabIndex = 14;
            this.lbl_Mode.Text = "Режим работы";
            // 
            // cmbBox_Mode
            // 
            this.cmbBox_Mode.FormattingEnabled = true;
            this.cmbBox_Mode.Items.AddRange(new object[] {
            "Normal",
            "NotData",
            "NotDB"});
            this.cmbBox_Mode.Location = new System.Drawing.Point(118, 17);
            this.cmbBox_Mode.Name = "cmbBox_Mode";
            this.cmbBox_Mode.Size = new System.Drawing.Size(121, 21);
            this.cmbBox_Mode.TabIndex = 13;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.txtBoxShopName);
            this.tabPage2.Location = new System.Drawing.Point(23, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(603, 310);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Локальные";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lbl_DirAlgoritmSMTP);
            this.tabPage3.Controls.Add(this.btn_DirAlgoritmSMTP);
            this.tabPage3.Controls.Add(this.txtBox_AlgoritmSmtpPar);
            this.tabPage3.Controls.Add(this.lbl_DirAlgoritmSmtpOut);
            this.tabPage3.Controls.Add(this.txtBox_AlgoritmSmtpQuery);
            this.tabPage3.Controls.Add(this.lbl_AlgoritmSmtpText);
            this.tabPage3.Controls.Add(this.lbl_AlgoritmSmtpPar);
            this.tabPage3.Controls.Add(this.txtBox_AlgoritmSmtpText);
            this.tabPage3.Controls.Add(this.lbl_AlgoritmSmtpQuery);
            this.tabPage3.Location = new System.Drawing.Point(23, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(603, 310);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "AlgoritmSMTP";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // pnl_Buttom
            // 
            this.pnl_Buttom.Controls.Add(this.btnSave);
            this.pnl_Buttom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl_Buttom.Location = new System.Drawing.Point(0, 318);
            this.pnl_Buttom.Name = "pnl_Buttom";
            this.pnl_Buttom.Size = new System.Drawing.Size(630, 31);
            this.pnl_Buttom.TabIndex = 14;
            // 
            // pnl_Fill
            // 
            this.pnl_Fill.Controls.Add(this.tabControl1);
            this.pnl_Fill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Fill.Location = new System.Drawing.Point(0, 0);
            this.pnl_Fill.Name = "pnl_Fill";
            this.pnl_Fill.Size = new System.Drawing.Size(630, 318);
            this.pnl_Fill.TabIndex = 15;
            // 
            // txtBox_LogNotValidCustomer
            // 
            this.txtBox_LogNotValidCustomer.Location = new System.Drawing.Point(346, 48);
            this.txtBox_LogNotValidCustomer.Name = "txtBox_LogNotValidCustomer";
            this.txtBox_LogNotValidCustomer.Size = new System.Drawing.Size(176, 20);
            this.txtBox_LogNotValidCustomer.TabIndex = 20;
            // 
            // lbl_LogNotValidCustomer
            // 
            this.lbl_LogNotValidCustomer.AutoSize = true;
            this.lbl_LogNotValidCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_LogNotValidCustomer.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbl_LogNotValidCustomer.Location = new System.Drawing.Point(18, 51);
            this.lbl_LogNotValidCustomer.Name = "lbl_LogNotValidCustomer";
            this.lbl_LogNotValidCustomer.Size = new System.Drawing.Size(328, 13);
            this.lbl_LogNotValidCustomer.TabIndex = 21;
            this.lbl_LogNotValidCustomer.Text = "Имя лог файла для клиентов не прошедших проверку";
            // 
            // FPar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 349);
            this.Controls.Add(this.pnl_Fill);
            this.Controls.Add(this.pnl_Buttom);
            this.Name = "FPar";
            this.Text = "Правка параметров приложения";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.pnl_Buttom.ResumeLayout(false);
            this.pnl_Fill.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label lbl_DirAlgoritmSMTP;
        private System.Windows.Forms.Button btn_DirAlgoritmSMTP;
        private System.Windows.Forms.Label lbl_DirAlgoritmSmtpOut;
        private System.Windows.Forms.Label lbl_AlgoritmSmtpText;
        private System.Windows.Forms.TextBox txtBox_AlgoritmSmtpText;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtBoxShopName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_AlgoritmSmtpQuery;
        private System.Windows.Forms.Label lbl_AlgoritmSmtpPar;
        private System.Windows.Forms.TextBox txtBox_AlgoritmSmtpQuery;
        private System.Windows.Forms.TextBox txtBox_AlgoritmSmtpPar;
        private System.Windows.Forms.CheckBox chkBox_VisibleCalculateCustomColumn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel pnl_Buttom;
        private System.Windows.Forms.Panel pnl_Fill;
        private System.Windows.Forms.Label lbl_Mode;
        private System.Windows.Forms.ComboBox cmbBox_Mode;
        private System.Windows.Forms.CheckBox chkBox_Trace;
        private System.Windows.Forms.TextBox txtBox_CustomerPrefixPhoneList;
        private System.Windows.Forms.Label lbl_CustomerPrefixPhoneList;
        private System.Windows.Forms.TextBox txtBox_CustomerCountryList;
        private System.Windows.Forms.Label lbl_CustomerCountryList;
        private System.Windows.Forms.TextBox txtBox_LogNotValidCustomer;
        private System.Windows.Forms.Label lbl_LogNotValidCustomer;
    }
}