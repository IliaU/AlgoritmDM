namespace AlgoritmDM
{
    partial class FScenary
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
            this.components = new System.ComponentModel.Container();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.PnlBottom = new System.Windows.Forms.Panel();
            this.PnlFill = new System.Windows.Forms.Panel();
            this.dgScenary = new System.Windows.Forms.DataGridView();
            this.ScnFullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScenariyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.СntMStripScenary = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBoxTypScn = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBoxNameScn = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.PnlBottom.SuspendLayout();
            this.PnlFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgScenary)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(612, 32);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Visible = false;
            // 
            // PnlBottom
            // 
            this.PnlBottom.Controls.Add(this.btnAdd);
            this.PnlBottom.Controls.Add(this.txtBoxNameScn);
            this.PnlBottom.Controls.Add(this.label3);
            this.PnlBottom.Controls.Add(this.cmbBoxTypScn);
            this.PnlBottom.Controls.Add(this.label2);
            this.PnlBottom.Controls.Add(this.label1);
            this.PnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlBottom.Location = new System.Drawing.Point(0, 277);
            this.PnlBottom.Name = "PnlBottom";
            this.PnlBottom.Size = new System.Drawing.Size(612, 85);
            this.PnlBottom.TabIndex = 1;
            // 
            // PnlFill
            // 
            this.PnlFill.Controls.Add(this.dgScenary);
            this.PnlFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlFill.Location = new System.Drawing.Point(0, 32);
            this.PnlFill.Name = "PnlFill";
            this.PnlFill.Size = new System.Drawing.Size(612, 245);
            this.PnlFill.TabIndex = 2;
            // 
            // dgScenary
            // 
            this.dgScenary.AllowUserToAddRows = false;
            this.dgScenary.AllowUserToDeleteRows = false;
            this.dgScenary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgScenary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ScnFullName,
            this.ScenariyName});
            this.dgScenary.ContextMenuStrip = this.СntMStripScenary;
            this.dgScenary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgScenary.Location = new System.Drawing.Point(0, 0);
            this.dgScenary.Name = "dgScenary";
            this.dgScenary.ReadOnly = true;
            this.dgScenary.Size = new System.Drawing.Size(612, 245);
            this.dgScenary.TabIndex = 0;
            this.dgScenary.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgScenary_CellMouseEnter);
            // 
            // ScnFullName
            // 
            this.ScnFullName.DataPropertyName = "ScnFullName";
            this.ScnFullName.HeaderText = "Тип сценария";
            this.ScnFullName.Name = "ScnFullName";
            this.ScnFullName.ReadOnly = true;
            this.ScnFullName.Width = 150;
            // 
            // ScenariyName
            // 
            this.ScenariyName.DataPropertyName = "ScenariyName";
            this.ScenariyName.HeaderText = "Имя сценария";
            this.ScenariyName.Name = "ScenariyName";
            this.ScenariyName.ReadOnly = true;
            this.ScenariyName.Width = 400;
            // 
            // СntMStripScenary
            // 
            this.СntMStripScenary.Name = "СntMStripScenary";
            this.СntMStripScenary.Size = new System.Drawing.Size(61, 4);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.DarkBlue;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(612, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Добавить новый сценарий";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.DarkBlue;
            this.label2.Location = new System.Drawing.Point(3, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Тип сценария";
            // 
            // cmbBoxTypScn
            // 
            this.cmbBoxTypScn.FormattingEnabled = true;
            this.cmbBoxTypScn.Location = new System.Drawing.Point(88, 26);
            this.cmbBoxTypScn.Name = "cmbBoxTypScn";
            this.cmbBoxTypScn.Size = new System.Drawing.Size(146, 21);
            this.cmbBoxTypScn.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.DarkBlue;
            this.label3.Location = new System.Drawing.Point(240, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Имя сценария";
            // 
            // txtBoxNameScn
            // 
            this.txtBoxNameScn.Location = new System.Drawing.Point(320, 26);
            this.txtBoxNameScn.Name = "txtBoxNameScn";
            this.txtBoxNameScn.Size = new System.Drawing.Size(280, 20);
            this.txtBoxNameScn.TabIndex = 4;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(525, 52);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // FScenary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 362);
            this.Controls.Add(this.PnlFill);
            this.Controls.Add(this.PnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "FScenary";
            this.Text = "Редактирование списка доступных сценариев для пользователя";
            this.PnlBottom.ResumeLayout(false);
            this.PnlBottom.PerformLayout();
            this.PnlFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgScenary)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel PnlBottom;
        private System.Windows.Forms.Panel PnlFill;
        private System.Windows.Forms.DataGridView dgScenary;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScnFullName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScenariyName;
        private System.Windows.Forms.ContextMenuStrip СntMStripScenary;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtBoxNameScn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbBoxTypScn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}