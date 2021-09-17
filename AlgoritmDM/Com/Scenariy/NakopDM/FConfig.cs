using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlgoritmDM.Com.Scenariy.NakopDM
{
    public partial class FConfig : Form
    {
        /// <summary>
        /// Типоризированный сценарий с которым мы работаем
        /// </summary>
        private NakopDMscn MainNDM;

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
        private bool HashSave=false;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="MainNDM">Ссылка на наш типоризированный объект который мы будем настраивать</param>
        public FConfig(NakopDMscn MainNDM)
        {
            try 
	        {
                MainNDM.XmlLoad(); // Читаем объекты если их небыло то сгенерируется по умолчанию
                this.MainNDM = MainNDM;
                InitializeComponent();
                this.Text = string.Format("{0} ({1})",this.MainNDM.ScenariyName,this.MainNDM.ScenariyInType.Name);
                this.lblScenariyName.Text = this.MainNDM.ScenariyName;

                // Создаём табличку с типами чеков которые нам нужно игнорировать
                if (this.dtDiscReason == null)
                {
                    this.dtDiscReason = new DataTable("DiscReason");
                    this.dtDiscReason.Columns.Add(new DataColumn("DiscReasonId", Type.GetType("System.String")));
                    this.dtDiscReason.Columns.Add(new DataColumn("DiscReasonName", Type.GetType("System.String")));
                    this.dtDiscReason.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
                    foreach (AlgoritmDM.Lib.DiscReason item in Com.DiscReasonFarm.List)
                    {
                        DataRow nrow= this.dtDiscReason.NewRow();
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
                    foreach (PorogPoint item in this.MainNDM.PrgpntList.OrderBy(t => t.Porog))
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
                this.MainNDM.EventSave(ex.Message, GetType().Name, AlgoritmDM.Lib.EventEn.Error, true, true);
            }
        }

        /// <summary>
        /// Проверка на существование флага поданному типу чека.
        /// </summary>
        /// <param name="DiscReasonId">Тип чека наличие которого нужно проверить.</param>
        /// <returns>Если существует в основном класса то возвращаем true</returns>
        private bool HashDiscReasonId(Int64 DiscReasonId)
        {
            bool rez = false;
            foreach (int item in this.MainNDM.NotDiscReasonId)
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
                        try{ DscReasId = int.Parse(this.dgDiscReason.Rows[e.RowIndex].Cells["DiscReasonId"].Value.ToString());}
                        catch (Exception){}

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
                this.MainNDM.EventSave(ex.Message, GetType().Name + ".dgDiscReason_CellClick", AlgoritmDM.Lib.EventEn.Error, true, true);
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
            DialogResult drez = MessageBox.Show("Вы действительно хотите удалить строку?","Подтверждение удаления.", MessageBoxButtons.YesNo);
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
                List<NakopDM.PorogPoint> newPrgpntList = new List<PorogPoint>();

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
                        decimal prg=0;
                        decimal prc = 0;
                        bool flag = false;
                        try
                        {
                            flag = true;
                            prg = decimal.Parse(item["Porog"].ToString());
                            prc = decimal.Parse(item["Procent"].ToString());
                        }
                        catch (Exception){}

                        // проверряем наличие результата
                        if (flag)
                        {
                            if(this.HashPorog(newPrgpntList, prg)) throw new ApplicationException("Порог с таким значением ({0}) уже существует.");
                            else newPrgpntList.Add(new PorogPoint(prg, prc));
                        }
                    }
                }

                // Устанавливаем новые значения параметров
                this.MainNDM.SetupNewPatForXML(newNotDiscReasonId, newPrgpntList);

                // Закрываем форму.
                this.Close();
            }
            catch (Exception ex)
            {
                this.MainNDM.EventSave(ex.Message, GetType().Name + ".btnSave_Click", AlgoritmDM.Lib.EventEn.Error, true, true);
            }
        }

        /// <summary>
        /// Проверка наличие существования такого прога в списке
        /// </summary>
        /// <param name="List">Список в котором нужно проверить значение</param>
        /// <param name="Prg">Порог наличие которога нужно проверить</param>
        /// <returns>Результат проверки true если такой порог уже существует</returns>
        private bool HashPorog(List<NakopDM.PorogPoint> List, decimal Prg)
        {
            bool rez = false;
            foreach (NakopDM.PorogPoint item in List)
            {
                if (item.Porog == Prg)
                {
                    rez = true;
                    break;
                }
            }
            return rez;
        }
    }
}
