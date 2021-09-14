using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AlgoritmDM.Com.Data;

namespace AlgoritmDM
{
    public partial class FMerge : Form
    {
        /// <summary>
        /// Основной клиент
        /// </summary>
        private Customer MergeClientMain;
        //
        /// <summary>
        /// Клинеты доноры
        /// </summary>
        private List<Customer> MergeClientDonors = new List<Customer>();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="MergeClientMain">Основной клиент</param>
        /// <param name="MergeClientDonors">Клинеты доноры</param>
        public FMerge(Customer MergeClientMain, List<Customer> MergeClientDonors)
        {
            try
            {
                this.MergeClientMain = MergeClientMain;
                this.MergeClientDonors = MergeClientDonors;
                InitializeComponent();

                this.lblMain.Text = string.Format("{0} {1}",this.MergeClientMain.FirstName, this.MergeClientMain.LastName);

                for (int i = 0; i < this.MergeClientDonors.Count; i++)
                {
                    if (i==0) this.lblDonor.Text=string.Format("{0} {1}",this.MergeClientDonors[i].FirstName, this.MergeClientDonors[i].LastName);
                    else this.lblDonor.Text += string.Format("\r\n{0} {1}", this.MergeClientDonors[i].FirstName, this.MergeClientDonors[i].LastName);
                }
            }
            catch (Exception){}
        }

        // Пользователь применяет
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Com.ProviderFarm.CurrentPrv.MergeClient(this.MergeClientMain, this.MergeClientDonors);


                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                MessageBox.Show("Изменения вступят в силу после пересчёта.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }

        // Пользователь отчищает
        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

    }
}
