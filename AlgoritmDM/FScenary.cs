using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlgoritmDM
{
    public partial class FScenary : Form
    {
        /// <summary>
        /// Таблица для хранения списка сценариев
        /// </summary>
        private DataTable dtScenary;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FScenary()
        {
            InitializeComponent();

            // Заполняем список возможных сценариев
            this.cmbBoxTypScn.Items.Clear();
            foreach (string item in Lib.UScenariy.ListScenariyName())
	        {
                this.cmbBoxTypScn.Items.Add(item);
	        } 

            // Заполняем список доступных конфигураций
            if (this.dtScenary == null)
            {
                this.dtScenary = new DataTable("Scenariy");
                this.dtScenary.Columns.Add(new DataColumn("ScnFullName", Type.GetType("System.String")));
                this.dtScenary.Columns.Add(new DataColumn("ScenariyName", Type.GetType("System.String")));
                this.LoadDtScenary();
                this.dgScenary.DataSource = this.dtScenary;
            }

            // Подписываемся на события, чтобы при изменении в списке сценариев обновить грид
            Com.ScenariyFarm.List.onScenariyListAddedScenariy += new EventHandler<Com.Scenariy.Lib.ScenariyBase>(List_onScenariyListAddedScenariy);
            Com.ScenariyFarm.List.onScenariyListDeletingScenariy += new EventHandler<Lib.EventScenariy>(List_onScenariyListDeletingScenariy);  
            Com.ScenariyFarm.List.onScenariyListDeletedScenariy += new EventHandler<Com.Scenariy.Lib.ScenariyBase>(List_onScenariyListAddedScenariy);
        }

        // Произошло событие добавления нового сценария
        void List_onScenariyListAddedScenariy(object sender, Com.Scenariy.Lib.ScenariyBase e)
        {
            this.LoadDtScenary();
        }

        // Чтение данных по существующим сценариям
        private void LoadDtScenary()
        {
            this.dtScenary.Rows.Clear();
            foreach (Lib.UScenariy item in Com.ScenariyFarm.List)
            {
                DataRow nrow = this.dtScenary.NewRow();
                nrow["ScnFullName"] = item.ScenariyInType.Name;
                nrow["ScenariyName"] = item.ScenariyName;
                this.dtScenary.Rows.Add(nrow);
            }
        }

        // Пользователь навёл мышку на ячейку 
        private void dgScenary_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Чистим контекстное меню
                this.СntMStripScenary.Items.Clear();

                // Обрабатываем только если мы на актихных ячейках
                if (e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    Lib.UScenariy UScn = Com.ScenariyFarm.List[this.dgScenary.Rows[e.RowIndex].Cells["ScenariyName"].Value.ToString()];
                    this.СntMStripScenary.Items.Add(UScn.getScenariyPlugIn().TSMItemGonfig);

                    //Генерим ячейку для удаления сценария
                    ToolStripMenuItem TSMItem = new ToolStripMenuItem(this.GetType().Name);
                    //
                    TSMItem.Text = string.Format("Удалить сценарий ({0})", UScn.ScenariyName);
                    TSMItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                    TSMItem.Tag = UScn;
                    //InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                    //InfoToolStripMenuItem.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                    TSMItem.Click += new EventHandler(ToolStripMenuItemDeletedScn_Click);
                    //
                    this.СntMStripScenary.Items.Add(TSMItem);

                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Упали с ошибкой при наведении мышки на ячёйку. ({0})", ex.Message), GetType().Name + ".dgScenary_CellMouseEnter", Lib.EventEn.Error, true, true);
            }
        }

        //
        // Пользователь хочет удалить какойто сценарий, нужно проверить ссылки на него в наших конфигурациях
        void List_onScenariyListDeletingScenariy(object sender, Lib.EventScenariy e)
        {
            try
            {
                bool hashLinkForConfigwrationList = false;
                Lib.ConfigurationList CnfL=null;

                // Пробегаем по всем конфигурациям
                foreach (Lib.ConfigurationList iCnfL in Com.ConfigurationFarm.ShdConfigurations)
                {
                    // Пробегаем по элементам конфигураций
                    foreach (Lib.Configuration iCnf in iCnfL)
                    {
                        if (iCnf.UScn.ScenariyName == e.UScn.ScenariyName)
                        {
                            hashLinkForConfigwrationList = true;
                            CnfL = iCnfL;
                            break;
                        }
                    }
                    if (hashLinkForConfigwrationList) break;
                }

                if (hashLinkForConfigwrationList)
                {
                    e.Action = false;
                    MessageBox.Show(string.Format("Нельзя удалить этот сценарий так как на него ссылается конфигурация с именем: {0}", (CnfL==null?"null":CnfL.ConfigurationName)));
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Упали при попытки удалить сценария из системы. ({0})", ex.Message), GetType().Name + ".List_onScenariyListDeletingScenariy", Lib.EventEn.Error, true, true);
            }
        }

        // Пользователь вызвал меню настройки этого типа сценария
        private void ToolStripMenuItemDeletedScn_Click(object sender, EventArgs e)
        {
            try
            {
                Lib.UScenariy UScn = (Lib.UScenariy)((ToolStripMenuItem)sender).Tag;
                Com.ScenariyFarm.List.Remove(UScn);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Упали при удалении сценария из системы. ({0})", ex.Message), GetType().Name + ".ToolStripMenuItemDeletedScn_Click", Lib.EventEn.Error, true, true);
            }
        }

        // Пользователь добавляет новый тип сценария
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.cmbBoxTypScn.Text)) throw new ApplicationException(string.Format("Не выбран тип сценария. ({0})", this.txtBoxNameScn.Text.Trim()));
                if (string.IsNullOrWhiteSpace(this.txtBoxNameScn.Text)) throw new ApplicationException(string.Format("Не выбрано имя сценария."));
                if (!Lib.UScenariy.HashScnFullName(this.cmbBoxTypScn.Text.Trim())) throw new ApplicationException(string.Format("Такого типа сценария ({0}) в системе не существует.", this.cmbBoxTypScn.Text.Trim()));

                // Пробуем найти такой сценарий в списке
                Lib.UScenariy UScn = null;
                try{ UScn = Com.ScenariyFarm.List[this.txtBoxNameScn.Text.Trim()];}
                catch (Exception){}
                if (UScn != null) throw new ApplicationException(string.Format("Сценарий с именем '{0} уже существует.'", this.txtBoxNameScn.Text.Trim()));

                // Если мы дошли до сюда, то можно создать этот сценарий
                UScn = new Lib.UScenariy(this.cmbBoxTypScn.Text.Trim(), this.txtBoxNameScn.Text.Trim(), null);
                Com.ScenariyFarm.List.Add(UScn);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Упали с ошибкой при Добпалениии нового сценария. ({0})", ex.Message), GetType().Name + ".btnAdd_Click", Lib.EventEn.Error, true, true);
            }
        }
    }
}
