using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace AlgoritmDM
{
    public partial class FPar : Form
    {
        // Конструктор
        public FPar()
        {
            InitializeComponent();
            for (int i = 0; i < this.cmbBox_Mode.Items.Count; i++)
            {
                if (this.cmbBox_Mode.Items[i].ToString() == Com.Config.Mode.ToString())
                {
                    this.cmbBox_Mode.SelectedIndex = i;
                    break;
                }
            }
            this.txtBox_CustomerCountryList.Text = Com.Config.CustomerCountryList;
            this.txtBox_CustomerPrefixPhoneList.Text = Com.Config.CustomerPrefixPhoneList;
            this.chkBox_Trace.Checked = Com.Config.Trace;
            this.txtBox_LogNotValidCustomer.Text = Com.Config.LogNotValidCustomer;
            this.lbl_DirAlgoritmSmtpOut.Text = Com.Config.CurAlgiritmSmtpDir;
            this.txtBox_AlgoritmSmtpText.Text = Com.Config.CurAlgiritmSmtpText;
            this.txtBox_AlgoritmSmtpQuery.Text = Com.Config.CurAlgiritmSmtpQuery;
            this.txtBox_AlgoritmSmtpPar.Text = Com.Config.CurAlgiritmSmtpPar;
            this.chkBox_VisibleCalculateCustomColumn.Checked = Com.Config.VisibleCalculateCustomColumn;
            this.txtBoxShopName.Text = Com.ConfigReg.ShopName;

            // Показываем инфу по интеграции с призмом
            if(Com.Lic.HashConnectPrizm)
            {
                this.pnltpIntegrationPrizmTop.Visible = false;
                this.pnltpIntegrationPrizmFill.Visible = true;
            }
            else
            {
                this.pnltpIntegrationPrizmTop.Visible = true;
                this.pnltpIntegrationPrizmFill.Visible = false;
            }
        }

        // Пользователь выбирает папку
        private void btn_DirAlgoritmSMTP_Click(object sender, EventArgs e)
        {
            try
            {
                // Если пользователь выбрал папку
                if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(string.Format(@"{0}\AlgoritmSMTP.exe", this.folderBrowserDialog.SelectedPath))) this.lbl_DirAlgoritmSmtpOut.Text = this.folderBrowserDialog.SelectedPath;
                    else throw new ApplicationException(string.Format(@"Мы не обнаружили в папке: ""{0}"" приложения: ""AlgoritmSMTP.exe""", this.folderBrowserDialog.SelectedPath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Пользователь сохраняет изменения
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.cmbBox_Mode.SelectedIndex > -1 && this.cmbBox_Mode.Items[this.cmbBox_Mode.SelectedIndex].ToString() != Com.Config.Mode.ToString())
                {
                   Com.Config.Mode=Lib.EventConverter.Convert(this.cmbBox_Mode.Items[this.cmbBox_Mode.SelectedIndex].ToString(), Lib.ModeEn.Normal);
                }
                if (Com.Config.Trace!=this.chkBox_Trace.Checked) Com.Config.Trace=this.chkBox_Trace.Checked;

                if(!string.IsNullOrWhiteSpace(this.txtBox_LogNotValidCustomer.Text)) Com.Config.LogNotValidCustomer = this.txtBox_LogNotValidCustomer.Text;

                if (Com.Config.CustomerCountryList != this.txtBox_CustomerCountryList.Text.Trim()) Com.Config.CustomerCountryList = this.txtBox_CustomerCountryList.Text.Trim();
                if (Com.Config.CustomerPrefixPhoneList != this.txtBox_CustomerPrefixPhoneList.Text.Trim()) Com.Config.CustomerPrefixPhoneList = this.txtBox_CustomerPrefixPhoneList.Text.Trim();

                // Проверка наличия файла в этой папке делает объект Com.Config в момент сохранения, если его нет то выдаст исключение.
                if (!string.IsNullOrWhiteSpace(this.lbl_DirAlgoritmSmtpOut.Text) && Com.Config.CurAlgiritmSmtpDir != this.lbl_DirAlgoritmSmtpOut.Text) Com.Config.CurAlgiritmSmtpDir = this.lbl_DirAlgoritmSmtpOut.Text;
                if (Com.Config.CurAlgiritmSmtpText != this.txtBox_AlgoritmSmtpText.Text) Com.Config.CurAlgiritmSmtpText = this.txtBox_AlgoritmSmtpText.Text;
                if (Com.Config.CurAlgiritmSmtpQuery != this.txtBox_AlgoritmSmtpQuery.Text) Com.Config.CurAlgiritmSmtpQuery = this.txtBox_AlgoritmSmtpQuery.Text;
                if (Com.Config.CurAlgiritmSmtpPar != this.txtBox_AlgoritmSmtpPar.Text) Com.Config.CurAlgiritmSmtpPar = this.txtBox_AlgoritmSmtpPar.Text;
                if (Com.Config.VisibleCalculateCustomColumn != this.chkBox_VisibleCalculateCustomColumn.Checked) Com.Config.VisibleCalculateCustomColumn = this.chkBox_VisibleCalculateCustomColumn.Checked;
                if(!string.IsNullOrWhiteSpace(this.txtBoxShopName.Text)) Com.ConfigReg.ShopName = this.txtBoxShopName.Text;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
