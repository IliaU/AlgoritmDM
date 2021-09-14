namespace AlgoritmDM.Com.Provider.RPRO8
{
    partial class FSetupConnectDB
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
            this.btnSaveRPro = new System.Windows.Forms.Button();
            this.btnTestRpro = new System.Windows.Forms.Button();
            this.lblPatchDB = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSaveRPro
            // 
            this.btnSaveRPro.Location = new System.Drawing.Point(239, 120);
            this.btnSaveRPro.Name = "btnSaveRPro";
            this.btnSaveRPro.Size = new System.Drawing.Size(75, 23);
            this.btnSaveRPro.TabIndex = 30;
            this.btnSaveRPro.Text = "Сохранить";
            this.btnSaveRPro.UseVisualStyleBackColor = true;
            this.btnSaveRPro.Click += new System.EventHandler(this.btnSaveRPro_Click);
            // 
            // btnTestRpro
            // 
            this.btnTestRpro.Location = new System.Drawing.Point(31, 120);
            this.btnTestRpro.Name = "btnTestRpro";
            this.btnTestRpro.Size = new System.Drawing.Size(179, 23);
            this.btnTestRpro.TabIndex = 29;
            this.btnTestRpro.Text = "Протестировать подключение";
            this.btnTestRpro.UseVisualStyleBackColor = true;
            this.btnTestRpro.Click += new System.EventHandler(this.btnTestRpro_Click);
            // 
            // lblPatchDB
            // 
            this.lblPatchDB.AutoEllipsis = true;
            this.lblPatchDB.Enabled = false;
            this.lblPatchDB.Location = new System.Drawing.Point(12, 39);
            this.lblPatchDB.Name = "lblPatchDB";
            this.lblPatchDB.Size = new System.Drawing.Size(332, 68);
            this.lblPatchDB.TabIndex = 31;
            this.lblPatchDB.Tag = "      ";
            this.lblPatchDB.Text = "      ";
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(249, 12);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(95, 23);
            this.btnSelectFolder.TabIndex = 32;
            this.btnSelectFolder.Text = "Выбор папки";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // FSetupConnectDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 154);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.lblPatchDB);
            this.Controls.Add(this.btnSaveRPro);
            this.Controls.Add(this.btnTestRpro);
            this.Name = "FSetupConnectDB";
            this.Text = "Настройка подключения RPro 8";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSaveRPro;
        private System.Windows.Forms.Button btnTestRpro;
        private System.Windows.Forms.Label lblPatchDB;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnSelectFolder;
    }
}