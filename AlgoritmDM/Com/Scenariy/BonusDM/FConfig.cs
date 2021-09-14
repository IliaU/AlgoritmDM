using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlgoritmDM.Com.Scenariy.BonusDM
{
    public partial class FConfig : Form
    {
        /// <summary>
        /// Типоризированный сценарий с которым мы работаем
        /// </summary>
        private BonusDMscn MainBDM;

        /// <summary>
        /// Таблица для хранения типов чека
        /// </summary>
        private DataTable dtDiscReason;

        /// <summary>
        /// Таблица для хранения текущих порогов
        /// </summary>
        private DataTable dtPorogPoint;

        /// <summary>
        /// Флаг для отображения кнопки сохранить
        /// </summary>
        private bool HashSave = false;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="MainBDM">Ссылка на наш типоризированный объект который мы будем настраивать</param>
        public FConfig(BonusDMscn MainBDM)
        {
            try 
	        {
                MainBDM.XmlLoad(); // Читаем объекты если их небыло то сгенерируется по умолчанию

                this.MainBDM = MainBDM;
                InitializeComponent();
                this.Text = string.Format("{0} ({1})", this.MainBDM.ScenariyName, this.MainBDM.ScenariyInType.Name);
                this.lblScenariyName.Text = this.MainBDM.ScenariyName;

                // Заполняем параметрами
                //this.cmb_SCPerc.SelectedIndexChanged -= new EventHandler(cmb_SCPerc_SelectedIndexChanged);
                this.cmb_ManualSCPerc.SelectedIndexChanged -= new EventHandler(cmb_ManualSCPerc_SelectedIndexChanged);
                this.cmb_SC_Perc.SelectedIndexChanged -=new EventHandler(cmb_SC_Perc_SelectedIndexChanged);
                this.txtBox_StartSCSumm.TextChanged -= new EventHandler(txtBox_StartSCSumm_TextChanged);
                this.txtBox_StartSCPerc.TextChanged -= new EventHandler(txtBox_StartSCPerc_TextChanged);
                this.txtBox_DelayPeriod.TextChanged -= new EventHandler(txtBox_DelayPeriod_TextChanged);
                this.txtBox_DeepConvSC.TextChanged -= new EventHandler(txtBox_DeepConvSC_TextChanged);
                this.chkBox_StartSCProgram.CheckedChanged -=new EventHandler(chkBox_StartSCProgram_CheckedChanged);
                this.dtTime_StartSCProgram.ValueChanged -=new EventHandler(dtTime_StartSCProgram_ValueChanged);
                //
                this.txtBox_StartSCSumm.Text = MainBDM.Start_SC_Summ.ToString();
                this.txtBox_StartSCPerc.Text = MainBDM.Start_SC_Perc.ToString();

                for (int i = 0; i < this.cmb_SC_Perc.Items.Count; i++)
                {
                    if (this.cmb_SC_Perc.Items[i].ToString() == MainBDM.SC_Perc) this.cmb_SC_Perc.SelectedIndex = i;
                }
                for (int i = 0; i < this.cmb_ManualSCPerc.Items.Count; i++)
                {
                    if (this.cmb_ManualSCPerc.Items[i].ToString() == MainBDM.Manual_SC_Perc) this.cmb_ManualSCPerc.SelectedIndex = i;
                }
                this.txtBox_DelayPeriod.Text = MainBDM.Delay_Period.ToString();
                //for (int i = 0; i < this.сmb_SaleRcptN.Items.Count; i++)
                //{
                //    if (this.сmb_SaleRcptN.Items[i].ToString() == MainBDM.Sale_Rcpt_N) this.сmb_SaleRcptN.SelectedIndex = i;
                //}
                if(MainBDM.Start_SC_Program!=null)  
                {
                    this.chkBox_StartSCProgram.Checked=true;
                    this.dtTime_StartSCProgram.Value = (DateTime)MainBDM.Start_SC_Program;
                }
                //
                this.txtBox_DeepConvSC.Text = MainBDM.Deep_Conv_SC.ToString();
                //for (int i = 0; i < this.cmb_CallOffSC.Items.Count; i++)
                //{
                //    if (this.cmb_CallOffSC.Items[i].ToString() == MainBDM.Call_Off_SC) this.cmb_CallOffSC.SelectedIndex = i;
                //}
                //
                this.cmb_ManualSCPerc.SelectedIndexChanged += new EventHandler(cmb_ManualSCPerc_SelectedIndexChanged);
                this.cmb_SC_Perc.SelectedIndexChanged += new EventHandler(cmb_SC_Perc_SelectedIndexChanged);
                this.txtBox_StartSCSumm.TextChanged += new EventHandler(txtBox_StartSCSumm_TextChanged);
                this.txtBox_StartSCPerc.TextChanged += new EventHandler(txtBox_StartSCPerc_TextChanged);
                this.txtBox_DelayPeriod.TextChanged += new EventHandler(txtBox_DelayPeriod_TextChanged);
                this.txtBox_DeepConvSC.TextChanged += new EventHandler(txtBox_DeepConvSC_TextChanged);
                this.chkBox_StartSCProgram.CheckedChanged += new EventHandler(chkBox_StartSCProgram_CheckedChanged);
                this.dtTime_StartSCProgram.ValueChanged += new EventHandler(dtTime_StartSCProgram_ValueChanged);
                

                // Создаём табличку с типами чеков которые нам нужно игнорировать
                if (this.dtDiscReason == null)
                {
                    this.dtDiscReason = new DataTable("DiscReason");
                    this.dtDiscReason.Columns.Add(new DataColumn("DiscReasonId", Type.GetType("System.String")));
                    this.dtDiscReason.Columns.Add(new DataColumn("DiscReasonName", Type.GetType("System.String")));
                    this.dtDiscReason.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
                    foreach (AlgoritmDM.Lib.DiscReason item in Com.DiscReasonFarm.List)
                    {
                        DataRow nrow = this.dtDiscReason.NewRow();
                        nrow["DiscReasonId"] = item.DiscReasonId.ToString();
                        nrow["DiscReasonName"] = item.DiscReasonName;
                        nrow["Check"] = HashDiscReasonId(item.DiscReasonId);
                        this.dtDiscReason.Rows.Add(nrow);
                    }

                    // Привя зываем грид к таблице
                    this.dgDiscReason.DataSource = this.dtDiscReason;
                }

                // Создаём таблицу с порогами
                if (this.dtPorogPoint == null)
                {
                    this.dtPorogPoint = new DataTable("PorogPoint");
                    this.dtPorogPoint.Columns.Add(new DataColumn("Porog", Type.GetType("System.Decimal")));
                    this.dtPorogPoint.Columns.Add(new DataColumn("Procent", Type.GetType("System.Decimal")));

                    // Предварительно отсортировав данные
                    foreach (PorogPoint item in this.MainBDM.PrgpntList.OrderBy(t => t.Porog))
                    {
                        DataRow nrow = this.dtPorogPoint.NewRow();
                        nrow["Porog"] = item.Porog;
                        nrow["Procent"] = item.Procent;
                        this.dtPorogPoint.Rows.Add(nrow);
                    }

                    // Привя зываем грид к таблице
                    this.dgPorogPoint.DataSource = this.dtPorogPoint;
                }
                this.dgPorogPoint.CellValueChanged += new DataGridViewCellEventHandler(dgPorogPoint_CellValueChanged);

            }
            catch (Exception ex)
            {
                this.MainBDM.EventSave(ex.Message, GetType().Name, AlgoritmDM.Lib.EventEn.Error, true, true);
            }
        }

        /// <summary>
        /// Проверка на существование флага поданному типу чека.
        /// </summary>
        /// <param name="DiscReasonId">Тип чека наличие которого нужно проверить.</param>
        /// <returns>Если существует в основном класса то возвращаем true</returns>
        private bool HashDiscReasonId(int DiscReasonId)
        {
            bool rez = false;
            foreach (int item in this.MainBDM.NotDiscReasonId)
            {
                if (item == DiscReasonId)
                {
                    rez = true;
                    break;
                }
            }
            return rez;
        }

        // Пользователь стукнул на любой части ячейки
        private void dgDiscReason_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {   // если мы в пправильном диапазоне по которой стукнул пользователь
                if (e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    // Работаем только если пользователь стукнул по колонке с флагом
                    if (this.dgDiscReason.Columns[e.ColumnIndex].Name == "Check")
                    {
                        int DscReasId = -1;
                        try { DscReasId = int.Parse(this.dgDiscReason.Rows[e.RowIndex].Cells["DiscReasonId"].Value.ToString()); }
                        catch (Exception) { }

                        // если всё гуд то запускаем установку нового значения у этого поля
                        if (DscReasId > -1)
                        {
                            this.SetupNewCheck(DscReasId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.MainBDM.EventSave(ex.Message, GetType().Name + ".dgDiscReason_CellClick", AlgoritmDM.Lib.EventEn.Error, true, true);
            }
        }

        /// <summary>
        /// Смена значения флага у типа чеков
        /// </summary>
        /// <param name="DscReasId">тип чека у которого нужно сменить занчение флага</param>
        private void SetupNewCheck(int DscReasId)
        {
            this.HashSave = true;
            foreach (DataRow item in dtDiscReason.Rows)
            {
                // Если встали на нужной ячейке
                if ((string)item["DiscReasonId"] == DscReasId.ToString())
                {
                    if ((bool)item["Check"]) item["Check"] = false;
                    else item["Check"] = true;

                    // Принимаем решение о отображении панели с кнопкой сохранения
                    VisiblePnlBottom();
                }
            }
        }

        /// <summary>
        /// Меняем видимость панели с кнопкой сохранения
        /// </summary>
        private void VisiblePnlBottom()
        {
            this.PnlBottom.Visible = this.HashSave;
        }

        // Произошло изменение порога
        private void dgPorogPoint_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.HashSave = true;
            this.VisiblePnlBottom();
        }

        // Произошло при добавлении строки пользователем
        private void dgPorogPoint_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            this.HashSave = true;
            this.VisiblePnlBottom();
        }

        // Пользователь хочет удалить строку
        private void dgPorogPoint_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult drez = MessageBox.Show("Вы действительно хотите удалить строку?", "Подтверждение удаления.", MessageBoxButtons.YesNo);
            if (drez == System.Windows.Forms.DialogResult.Yes)
            {
                this.HashSave = true;
                this.VisiblePnlBottom();
            }
            else e.Cancel = true;
        }

        // Пользователь ткнул на сохранение изменений
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> newNotDiscReasonId = new List<int>();
                List<BonusDM.PorogPoint> newPrgpntList = new List<PorogPoint>();

                // Строим список типов чеков
                foreach (DataRow item in this.dtDiscReason.Rows)
                {
                    if ((Boolean)item["Check"])
                    {
                        try { newNotDiscReasonId.Add(int.Parse(item["DiscReasonId"].ToString())); }
                        catch (Exception) { }
                    }
                }

                // Строим список с порогами
                foreach (DataRow item in this.dtPorogPoint.Rows)
                {

                    if (!string.IsNullOrWhiteSpace(item["Porog"].ToString()) && !string.IsNullOrWhiteSpace(item["Procent"].ToString()))
                    {
                        decimal prg = 0;
                        decimal prc = 0;
                        bool flag = false;
                        try
                        {
                            flag = true;
                            prg = decimal.Parse(item["Porog"].ToString());
                            prc = decimal.Parse(item["Procent"].ToString());
                        }
                        catch (Exception) { }

                        // проверряем наличие результата
                        if (flag)
                        {
                            if (this.HashPorog(newPrgpntList, prg)) throw new ApplicationException("Порог с таким значением ({0}) уже существует.");
                            else newPrgpntList.Add(new PorogPoint(prg, prc));
                        }
                    }
                }

                decimal tmpStartSCSumm=-1;
                try{tmpStartSCSumm = Decimal.Parse(this.txtBox_StartSCSumm.Text);}
                catch (Exception ex) { throw ex; }
                decimal tmpStartSCPerc = -1;
                try { tmpStartSCPerc = Decimal.Parse(this.txtBox_StartSCPerc.Text); }
                catch (Exception ex) { throw ex; }
                int tmpDelayPeriod = -1;
                try { tmpDelayPeriod = Int32.Parse(this.txtBox_DelayPeriod.Text); }
                catch (Exception ex) { throw ex; }
                int tmpDeepConvSC=-1;
                try { tmpDeepConvSC = Int32.Parse(this.txtBox_DeepConvSC.Text); }
                catch (Exception ex) { throw ex; }
                DateTime? tmpStartSCProgram = null;
                try
                {
                    if (dtTime_StartSCProgram.Visible) tmpStartSCProgram=dtTime_StartSCProgram.Value;
                }
                catch (Exception ex) { throw ex; }

                // Устанавливаем новые значения параметров
                this.MainBDM.SetupNewPatForXML(newNotDiscReasonId, newPrgpntList, tmpStartSCSumm, tmpStartSCPerc, this.cmb_ManualSCPerc.Items[this.cmb_ManualSCPerc.SelectedIndex].ToString(), tmpDelayPeriod, tmpDeepConvSC, tmpStartSCProgram, this.cmb_SC_Perc.Items[this.cmb_SC_Perc.SelectedIndex].ToString());

                // Закрываем форму.
                this.Close();
            }
            catch (Exception ex)
            {
                this.MainBDM.EventSave(ex.Message, GetType().Name + ".btnSave_Click", AlgoritmDM.Lib.EventEn.Error, true, true);
            }
        }

        /// <summary>
        /// Проверка наличие существования такого прога в списке
        /// </summary>
        /// <param name="List">Список в котором нужно проверить значение</param>
        /// <param name="Prg">Порог наличие которога нужно проверить</param>
        /// <returns>Результат проверки true если такой порог уже существует</returns>
        private bool HashPorog(List<BonusDM.PorogPoint> List, decimal Prg)
        {
            bool rez = false;
            foreach (BonusDM.PorogPoint item in List)
            {
                if (item.Porog == Prg)
                {
                    rez = true;
                    break;
                }
            }
            return rez;
        }



        private void cmb_ManualSCPerc_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.HashSave = true;
            VisiblePnlBottom();
        }

        private void txtBox_StartSCSumm_TextChanged(object sender, EventArgs e)
        {
            this.HashSave = true;
            VisiblePnlBottom();
        }

        private void txtBox_StartSCPerc_TextChanged(object sender, EventArgs e)
        {
            this.HashSave = true;
            VisiblePnlBottom();
        }

        private void txtBox_DelayPeriod_TextChanged(object sender, EventArgs e)
        {
            this.HashSave = true;
            VisiblePnlBottom();
        }

        private void txtBox_DeepConvSC_TextChanged(object sender, EventArgs e)
        {
            this.HashSave = true;
            VisiblePnlBottom();
        }

        // Пользователь указывает дату начиная с которой начинает работать бонусная прогроамма
        private void chkBox_StartSCProgram_CheckedChanged(object sender, EventArgs e)
        {
            this.dtTime_StartSCProgram.Visible = this.chkBox_StartSCProgram.Checked;
            this.HashSave = true;
            VisiblePnlBottom();
        }

        private void dtTime_StartSCProgram_ValueChanged(object sender, EventArgs e)
        {
            this.HashSave = true;
            VisiblePnlBottom();
        }

        private void cmb_SC_Perc_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.HashSave = true;
            VisiblePnlBottom();
        }

        //private void cmb_SCPerc_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    this.HashSave = true;
         //   VisiblePnlBottom();
        //}

        



    }
}
