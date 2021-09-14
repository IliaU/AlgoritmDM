using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlgoritmDM.Com.Provider.RPRO8
{
    public partial class FSetupConnectDB : Form
    {
        private RPRO8prv Prv;
        public string ConnectionString;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Prv">Класс который представляет из себя данный тип провайдера</param>
        public FSetupConnectDB(RPRO8prv Prv)
        {
            this.Prv = Prv;
            InitializeComponent();
            if (Prv != null && !string.IsNullOrWhiteSpace(Prv.ConnectionString))
            {
                this.ConnectionString = Prv.ConnectionString;
                this.lblPatchDB.Text = this.ConnectionString;
            }
        }

        // Пользователь решил протестировать соединение
        private void btnSaveRPro_Click(object sender, EventArgs e)
        {
            this.ConnectionString = this.Prv.InstalProvider(this.lblPatchDB.Text, true, false, false);
            if (this.ConnectionString != null && this.ConnectionString.Trim() != string.Empty) MessageBox.Show("Тестирование подключения завершилось успешно");
        }

        // Пользователь сохраняет подключение
        private void btnTestRpro_Click(object sender, EventArgs e)
        {
            this.ConnectionString = this.Prv.InstalProvider(this.lblPatchDB.Text, true, true, true);
            if (this.ConnectionString != null && this.ConnectionString.Trim() != string.Empty) this.DialogResult = DialogResult.Yes;
            else this.DialogResult = DialogResult.No;

            this.Close();
        }

        // Пользователь выбирает папку
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            DialogResult rez = this.folderBrowserDialog.ShowDialog();
            if (rez == DialogResult.OK)
            {
                this.ConnectionString = this.folderBrowserDialog.SelectedPath;
                this.lblPatchDB.Text = this.ConnectionString;
            }
        }
    }
}
