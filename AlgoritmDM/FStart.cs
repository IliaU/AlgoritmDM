using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.IO;
using AlgoritmDM.Com.Data;

namespace AlgoritmDM
{
    public partial class FStart : Form
    {
        private Color DefBaskCoclortSSLabel;
        private object LockObj = new object();

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
        /// Поозиция левой панели с перечнем текущих конфигураций
        /// </summary>
        private int PnlLeftSizePosition = 0;

        /// <summary>
        /// Конфигурация которую сейчас смотрит пользователь
        /// </summary>
        private Lib.ConfigurationList CnfLSelected = null;

        /// <summary>
        /// Для отображения списка доступных конфигураций
        /// </summary>
        private DataTable dtSharedConfigurations;

        /// <summary>
        /// Таблица для отображения сценариев связанной конфигурации
        /// </summary>
        private DataTable dtConfigurations;

        /// <summary>
        /// Для отображения списка клиентов
        /// </summary>
        private DataTable dtCustomer;

        /// <summary>
        /// Представление отфильтрованных клиентов отоброжаемых пользователю
        /// </summary>
        private DataView dvCustomer;

        /// <summary>
        /// Показывает состояние процессинга получения списка клиентов из источника
        /// </summary>
        private bool HashPricessingCustomer = false;

        /// <summary>
        /// Процесс применения скидок
        /// </summary>
        private bool HashPricessingAployDMCalkMaxDiscPerc = false;

        /// <summary>
        /// Процесс закачки справочника причин скидки
        /// </summary>
        private bool HashProcessingDscReas = false;

        /// <summary>
        /// Процесс получения детелей по клиенту
        /// </summary>
        private bool HashProcessingDetailCust = false;

        /// <summary>
        /// Тестрование какойто конфигурации
        /// </summary>
        private FStartTasks tstTask;

        /// <summary>
        /// Конфигурация которую сейчас редактирует пользователь
        /// </summary>
        private Lib.ConfigurationList EditCngL;

        // Конструктор
        public FStart()
        {
            InitializeComponent();
            this.DefBaskCoclortSSLabel = this.tSSLabel.BackColor;

            // Если работаем в режиме без базы данных, то скриываем менюшку настройки подключения
            if (Com.UserFarm.CurrentUser.Role == Lib.RoleEn.Admin)
            {
                this.TSMItemAboutPrv.Visible = true;
                this.TSMItemConfigPar.Visible = true;
                this.TSMItemConfigUsers.Visible = true;
                this.TlSpMenuItemCustDetail.Visible = true;
                //
                if (Com.Config.Mode == Lib.ModeEn.NotDB) this.TSMItemConfigDB.Visible = false;
                else this.TSMItemConfigDB.Visible = true;
                //
                if (Com.Lic.HashConnectPrizm) TSMItemConfigDbPrizm.Visible = true;
                else TSMItemConfigDbPrizm.Visible = false;
            }
            //
            // Открываем объекты котороые можно смотреть данному пользователю
            if (Com.UserFarm.CurrentUser.Role != Lib.RoleEn.Viewer)
            {
                this.TSMItemProcCustomer.Visible = true;
                this.TSMItemConfigScenary.Visible = true;
            }
            else
            {
                this.TSMItemAction.Visible = false;
                this.CalkMaxDiscPerc.Visible = false;
                this.CalkStoreCredit.Visible = false;
                this.CalkScPerc.Visible = false;
                this.MaxDiscPerc.Visible = Com.Config.VisibleCalculateCustomColumn;
                this.StoreCredit.Visible = Com.Config.VisibleCalculateCustomColumn;
                this.ScPerc.Visible = Com.Config.VisibleCalculateCustomColumn;
            }

            // Меняем текст в менюшке
            if (Com.ConfigurationFarm.CurrentCnfList != null) this.TSMItemProcCustomer.Text = ((string)this.TSMItemProcCustomer.Tag).Replace("@CurCnfl", Com.ConfigurationFarm.CurrentCnfList.ConfigurationName);

            // Заполняем варианты действий между применениями сценариев в выбранной конфигурации
            foreach (string item in Lib.EventConverter.GetListCongigurationActionSaleEn())
	        {
                this.SaleActionOut.Items.AddRange(item);
                this.cmbBoxSaleActionOut.Items.Add(item);
	        }   
            
            // Добавляем список сценариев которые потом можно выбирать
            foreach (Lib.UScenariy item in Com.ScenariyFarm.List)
	        {
                this.ScenariyName.Items.AddRange(item.ScenariyName);
                this.cmbBoxScenariyName.Items.Add(item.ScenariyName);
	        }         

            // Получаем текущий статус программы
            Log_onEventLog(null, null);

            // Подключаем список доступных провайдеров для подключения ToolStripMenuItem
            // TSMItemAboutPrv
            foreach (string item in Com.ProviderFarm.ListProviderName())
            {
                this.TSMItemAboutPrv.DropDownItems.Add((new Lib.UProvider(item)).InfoToolStripMenuItem());
            }

            // Подключаем список описанием сценариев
            this.TSMItemAboutScn_Load();

            // Показываем ту конфигурацию которая выбрана пользователем
            this.lbl_CurConfigList.Text = string.Format(this.lbl_CurConfigList.Tag.ToString(), (Com.ConfigurationFarm.CurrentCnfList != null?Com.ConfigurationFarm.CurrentCnfList.ConfigurationName:"не выбрана."));

            // Заполняем список доступных конфигураций
            if (this.dtSharedConfigurations == null)
            {
                this.dtSharedConfigurations = new DataTable("SharedConfigurations");
                this.dtSharedConfigurations.Columns.Add(new DataColumn("ConfigurationName", Type.GetType("System.String")));
                foreach (Lib.ConfigurationList item in Com.ConfigurationFarm.ShdConfigurations)
                {
                    DataRow nrow = this.dtSharedConfigurations.NewRow();
                    nrow["ConfigurationName"] = item.ConfigurationName;
                    this.dtSharedConfigurations.Rows.Add(nrow);

                    if (item.Count > -1 && Com.UserFarm.CurrentUser.Role != Lib.RoleEn.Viewer) 
                    {
                        // Строим менюшку чтобы пользователь мог пересчитать скидку в любой конфигурации
                        ToolStripMenuItem nitmConfList = new ToolStripMenuItem();
                        nitmConfList.Text = string.Format("Пересчитать всё в конфигурации: {0}.", item.ConfigurationName);
                        nitmConfList.Font = new System.Drawing.Font("Segoe UI", 9F);
                        nitmConfList.Tag = item;
                        //nitmConfList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        //nitmConfList.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                        nitmConfList.Click += new EventHandler(nitmConfList_Click);
                        this.TSMItemAction.DropDownItems.Add(nitmConfList);
                    }
   
                }
                this.dgSharedConfigurations.DataSource = this.dtSharedConfigurations;   
            }

            // Создаём таблицу для отрисовки списка сценариев внутри конфигурации
            if (this.dtConfigurations == null)
            {
                this.dtConfigurations = new DataTable("Configurations");
                this.dtConfigurations.Columns.Add(new DataColumn("ScenariyName", Type.GetType("System.String")));
                this.dtConfigurations.Columns.Add(new DataColumn("SaleActionOut", Type.GetType("System.String")));
                this.dtConfigurations.Columns.Add(new DataColumn("ActionRows", Type.GetType("System.Boolean")));
                this.ConfigurationsLoad(Com.ConfigurationFarm.CurrentCnfList);
                this.dgConfigurations.DataSource = this.dtConfigurations; 
            }

            // Заполняем список клиентов
            if (this.dtCustomer == null)
            {
                this.dtCustomer = new DataTable("Customer");

                this.dtCustomer.Columns.Add(new DataColumn("CustSid", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("FirstName", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("LastName", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("CustId", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("MaxDiscPerc", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("CalkMaxDiscPerc", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("StoreCredit", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("CalkStoreCredit", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("ScPerc", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("CalkScPerc", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("Phone1", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("Address1", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("FstSaleDate", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("LstSaleDate", Type.GetType("System.String")));
                this.dtCustomer.Columns.Add(new DataColumn("EmailAddr", Type.GetType("System.String")));
                this.CustomerLoad();
                this.dvCustomer = new DataView(this.dtCustomer);
                this.dgCustomer.DataSource = this.dvCustomer;
            }

            // Подписываемся на события 
            Com.Log.onEventLog += new EventHandler<Lib.EventLog>(Log_onEventLog);       //Common.Log.EventSave("Тест", this.GetType().Name, Librory.EventEn.Error);
            //
            Com.Lic.onRegNewKeyAfte += new EventHandler<Com.LicLib.onLicEventKey>(Lic_onRegNewKey);
            //
            Com.ScenariyFarm.List.onScenariyListAddedScenariy += new EventHandler<Com.Scenariy.Lib.ScenariyBase>(List_onScenariyListAddedScenariy);
            Com.ScenariyFarm.List.onScenariyListDeletedScenariy += new EventHandler<Com.Scenariy.Lib.ScenariyBase>(List_onScenariyListDeletedScenariy);
            //
            Com.ConfigurationFarm.ShdConfigurations.onConfigurationsLstAddedConfigurationsLst += new EventHandler<Lib.EventConfigurationList>(ShdConfigurations_onConfigurationsLstAddedConfigurationsLst);
            Com.ConfigurationFarm.ShdConfigurations.onConfigurationsLstListDeletedConfigurationsLst += new EventHandler<Lib.EventConfigurationList>(ShdConfigurations_onConfigurationsLstListDeletedConfigurationsLst);
            //
            Com.DiscReasonFarm.List.onProcessingDiscReasonList +=new EventHandler<Lib.EventDiscReasonList>(List_onProcessingDiscReasonList);
            Com.DiscReasonFarm.List.onProcessedDiscReasonList += new EventHandler<Lib.EventDiscReasonListAsicRez>(List_onProcessedDiscReasonList);
            //
            Com.CustomerFarm.List.onProcessingCustomerList += new EventHandler<Lib.EventCustomerList>(List_onProcessingCustomerList);
            Com.CustomerFarm.List.onProcessedCustomerList += new EventHandler<Lib.EventCustomerListAsicRez>(List_onProcessedCustomerList);
            Com.CustomerFarm.List.onProcessingAployDMCalkMaxDiscPerc += new EventHandler<Lib.EventCustomerList>(List_onProcessingAployDMCalkMaxDiscPerc);
            Com.CustomerFarm.List.onProcessedAployDMCalkMaxDiscPerc += new EventHandler<Lib.EventCustomerListAsicRez>(List_onProcessedAployDMCalkMaxDiscPerc);
            Com.CustomerFarm.List.onProcessedAployDMCalkMaxDiscPercProgressBar += new EventHandler<Lib.EventCustomerListProcessingProgerssBar>(List_onProcessedAployDMCalkMaxDiscPercProgressBar);
            //
            Com.ConfigurationFarm.onСhengedCurrentCnfList += new EventHandler<Lib.EventConfigurationList>(ConfigurationFarm_onСhengedCurrentCnfList);
            if (Com.ConfigurationFarm.CurrentCnfList != null)
            {
                Com.ConfigurationFarm.CurrentCnfList.onProcessingCalculateDM += new EventHandler<Lib.EventConfigurationList>(CurrentCnfList_onProcessingCalculateDM);
                Com.ConfigurationFarm.CurrentCnfList.onProcessedCalculateDM += new EventHandler<Lib.EventConfigurationListAsicRez>(CurrentCnfList_onProcessedCalculateDM);
                Com.ConfigurationFarm.CurrentCnfList.onProcessedCalculateDMProgressBar += new EventHandler<Lib.EventConfigurationListProcessingProgerssBar>(CurrentCnfList_onProcessedCalculateDMProgressBar);
                Com.ConfigurationFarm.CurrentCnfList.onProcessingCalculateTotal += new EventHandler<Lib.EventConfigurationList>(CurrentCnfList_onProcessingCalculateTotal);
                Com.ConfigurationFarm.CurrentCnfList.onProcessedCalculateTotal += new EventHandler<Lib.EventConfigurationListAsicRez>(CurrentCnfList_onProcessedCalculateTotal);
                Com.ConfigurationFarm.CurrentCnfList.onProcessedCalculateTotalProgressBar += new EventHandler<Lib.EventConfigurationListProcessingProgerssBar>(CurrentCnfList_onProcessedCalculateTotalProgressBar);
            }
            else
            {
                Com.Log.EventSave("Не определена текущая конфигурация, Мы не сможем расчитывать скидки.", this.GetType().Name, Lib.EventEn.Warning, true, true);
            }
        }

        // Чтение формы
        private void FStart_Load(object sender, EventArgs e)
        {
            try
            {
                // Если есть подключение или мы в режиме без базы данных то нужно проверить необходимость первичной загрузки данных
                if (Com.ProviderFarm.HashConnect() || Com.Config.Mode == Lib.ModeEn.NotDB)
                {
                    if (Com.Config.Mode == Lib.ModeEn.NotData && (Com.ProviderFarm.CurrentPrv == null || !Com.ProviderFarm.CurrentPrv.HashConnect)) throw new ApplicationException("Не установлено подключение с базой данных.");
                    else
                    {
                        if (Com.DiscReasonFarm.List.Count == 0) Com.DiscReasonFarm.List.ProcessingDiscReason();
                        if (Com.CustomerFarm.List.Count == 0) Com.CustomerFarm.List.ProcessingCustomers();
                    }
                }

                // Отрисовываем значки чтобы выдвинуть левую панель
                this.LoadVisiblePnlLeft();
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, GetType().Name + ".FStart_Load", Lib.EventEn.Error, true, true);
            }
        }

        /// <summary>
        /// Чтение элементов меню с описанием сценариев
        /// </summary>
        private void TSMItemAboutScn_Load()
        {
            try
            {
                this.TSMItemAboutScn.DropDownItems.Clear();

                foreach (string item in Com.ScenariyFarm.ListScenariyName())
                {
                    Lib.UScenariy Uscn = new Lib.UScenariy(item, "Info", null);
                    ToolStripMenuItem TSMIInfo = Uscn.getScenariyPlugIn().InfoToolStripMenuItem;
                    TSMIInfo.Text = TSMIInfo.Text + string.Format(" ({0})", Uscn.ScenariyInType.Name);
                    if (Uscn.ValidLicData() == 0)
                    {
                        TSMIInfo.BackColor = Color.Tomato;
                        TSMIInfo.Text = TSMIInfo.Text + string.Format(" [Этот тип сценария не лицензирован.])");
                    }
                    else
                    {
                        TSMIInfo.Text = TSMIInfo.Text + string.Format(" [{0}])", Uscn.ValidLicData());
                    }
                    this.TSMItemAboutScn.DropDownItems.Add(TSMIInfo);
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Не смогли перестроить список доступных типов сценариев. Произошла ошибка: {0}", ex.Message), GetType().Name + ".TSMItemAboutScn_Load", Lib.EventEn.Error, true, true);
            }

        }

        /// <summary>
        /// Перестраиваем элемент для отображения задачи установки новой текущей конфигурации
        /// </summary>
        private void TSMItemSetupNewCnfL_load(ApplicationException ae)
        {
            if (Com.UserFarm.CurrentUser.Role != Lib.RoleEn.Viewer) 
            {
                // Проверяем если текущая конфигурация отличается от той что тестируется, то нужно отобразить возможность назначения новой конфигурации
                if (ae == null && Com.ConfigurationFarm.CurrentCnfList.Count > 0 &&
                        (Com.ConfigurationFarm.CurrentCnfList == null ||
                            (this.tstTask != null && this.tstTask.CnfList.ConfigurationName != Com.ConfigurationFarm.CurrentCnfList.ConfigurationName)))
                {
                    this.TSMItemSetupNewCnfL.Visible = true;
                    if (this.tstTask == null) this.TSMItemSetupNewCnfL.Text = this.TSMItemSetupNewCnfL.Tag.ToString().Replace("@NewCnfL", Com.ConfigurationFarm.CurrentCnfList.ConfigurationName);
                    else this.TSMItemSetupNewCnfL.Text = this.TSMItemSetupNewCnfL.Tag.ToString().Replace("@NewCnfL", this.tstTask.CnfList.ConfigurationName);
                }
                else
                {
                    this.TSMItemSetupNewCnfL.Visible = false;
                }
            }
        }


        #region Обработка событий связанных с детализацией асинхронных запросов

        // Начало выкачивания данных из источника
        delegate void delig_List_onProcessingDiscReasonList(object sender, Lib.EventDiscReasonList e);
        void List_onProcessingDiscReasonList(object sender, Lib.EventDiscReasonList e)
        {
            if (this.InvokeRequired)
            {
                lock(this.LockObj)
                {
                    delig_List_onProcessingDiscReasonList dl = new delig_List_onProcessingDiscReasonList(List_onProcessingDiscReasonList);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.HashProcessingDscReas = true;
                    this.pnlDscReas.Visible = true;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии начала выкачивания данных из источника. ({0})", ex.Message), GetType().Name + ".List_onProcessingDiscReasonList", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Окончание выкачивания данных из источника
        delegate void delig_List_onProcessedDiscReasonList(object sender, Lib.EventDiscReasonListAsicRez e);
        void List_onProcessedDiscReasonList(object sender, Lib.EventDiscReasonListAsicRez e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_List_onProcessedDiscReasonList dl = new delig_List_onProcessedDiscReasonList(List_onProcessedDiscReasonList);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.HashProcessingDscReas = false;
                    this.pnlDscReas.Visible = false;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();

                    if (e.ex != null) MessageBox.Show(string.Format("При получении списка причин скидок произошла ошибка: {0}", e.ex.Message));
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии окончания выкачивания данных из источника. ({0})", ex.Message), GetType().Name + ".List_onProcessedDiscReasonList", Lib.EventEn.Error, true, true);
                }
            }
        }

        // Событие начала получения данных о Клиентах
        delegate void delig_List_onProcessingCustomerList(object sender, Lib.EventCustomerList e);
        private void List_onProcessingCustomerList(object sender, Lib.EventCustomerList e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_List_onProcessingCustomerList dl = new delig_List_onProcessingCustomerList(List_onProcessingCustomerList);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.HashPricessingCustomer = true;
                    this.pnlCustomers.Visible = true;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии начала получения данных о Клиентах. ({0})", ex.Message), GetType().Name + ".List_onProcessingCustomerList", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие окончания получения списка пользователей
        delegate void delig_List_onProcessedCustomerList(object sender, Lib.EventCustomerListAsicRez e);
        private void List_onProcessedCustomerList(object sender, Lib.EventCustomerListAsicRez e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_List_onProcessedCustomerList dl = new delig_List_onProcessedCustomerList(List_onProcessedCustomerList);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    // Обновляем список клиентов
                    this.CustomerLoad();

                    this.HashPricessingCustomer = false;
                    this.pnlCustomers.Visible = false;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();

                    // Проверяем результат работы асинхронного потока
                    if (e.ex != null) MessageBox.Show(string.Format("При получении списка пользователей произошла ошибка: {0}", e.ex.Message));
                    else
                    {
                        if (Com.UserFarm.CurrentUser.Role != Lib.RoleEn.Viewer)
                        {
                            // запускаем выкачивание чеков
                            if (this.tstTask == null) this.tstTask = new FStartTasks(Com.ConfigurationFarm.CurrentCnfList);
                            this.tstTask.RunProcess();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии окончания получения списка пользователей. ({0})", ex.Message), GetType().Name + ".List_onProcessedCustomerList", Lib.EventEn.Error, true, true);
                }
            }
        }

        // Событие начала применения скидок
        delegate void delig_List_onProcessingAployDMCalkMaxDiscPerc(object sender, Lib.EventCustomerList e);
        void List_onProcessingAployDMCalkMaxDiscPerc(object sender, Lib.EventCustomerList e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_List_onProcessingAployDMCalkMaxDiscPerc dl = new delig_List_onProcessingAployDMCalkMaxDiscPerc(List_onProcessingAployDMCalkMaxDiscPerc);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.HashPricessingAployDMCalkMaxDiscPerc = true;
                    this.pnlApplayDM.Visible = true;
                    this.progressBarApplayDM.Minimum = 0; this.progressBarApplayDM.Maximum = 100; this.progressBarApplayDM.Value = 0;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии начала применения скидок. ({0})", ex.Message), GetType().Name + ".List_onProcessingAployDMCalkMaxDiscPerc", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие окончания применения скидок
        delegate void delig_List_onProcessedAployDMCalkMaxDiscPerc(object sender, Lib.EventCustomerListAsicRez e);
        void List_onProcessedAployDMCalkMaxDiscPerc(object sender, Lib.EventCustomerListAsicRez e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_List_onProcessedAployDMCalkMaxDiscPerc dl = new delig_List_onProcessedAployDMCalkMaxDiscPerc(List_onProcessedAployDMCalkMaxDiscPerc);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.progressBarApplayDM.Minimum = 0; this.progressBarApplayDM.Maximum = 100; this.progressBarApplayDM.Value = 0;
                    this.HashPricessingAployDMCalkMaxDiscPerc = false;
                    this.pnlApplayDM.Visible = false;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();

                    for (int i = 0; i < this.dtCustomer.Rows.Count; i++)
			        {
                        long tmpCstSid = long.Parse(this.dtCustomer.Rows[i]["CustSid"].ToString());
                        Com.Data.Customer Cst = Com.CustomerFarm.List[tmpCstSid];
                        if (e.Cst == null || Cst.CustSid == e.Cst.CustSid)
                        {
                            this.dtCustomer.Rows[i]["MaxDiscPerc"] = Cst.MaxDiscPerc;
                            this.dtCustomer.Rows[i]["StoreCredit"] = Cst.StoreCredit;
                            this.dtCustomer.Rows[i]["ScPerc"] = Cst.ScPerc;
                        }
			        }

                    // Проверяем результат работы асинхронного потока
                    if (e.ex != null) MessageBox.Show(string.Format("При применении скидок произошла ошибка: {0}", e.ex.Message));
                    else this.btnApplay.Visible = false;
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии окончания применения скидок. ({0})", ex.Message), GetType().Name + ".List_onProcessedAployDMCalkMaxDiscPerc", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие изменения значения прогресс бара во время применения скидок
        delegate void delig_List_onProcessedAployDMCalkMaxDiscPercProgressBar(object sender, Lib.EventCustomerListProcessingProgerssBar e);
        void List_onProcessedAployDMCalkMaxDiscPercProgressBar(object sender, Lib.EventCustomerListProcessingProgerssBar e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_List_onProcessedAployDMCalkMaxDiscPercProgressBar dl = new delig_List_onProcessedAployDMCalkMaxDiscPercProgressBar(List_onProcessedAployDMCalkMaxDiscPercProgressBar);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    // Если панель доступна
                    if (this.pnlApplayDM.Visible)
                    {
                        this.progressBarApplayDM.Minimum = 0; this.progressBarApplayDM.Maximum = e.AllStep; this.progressBarApplayDM.Value = e.CurrentStep;
                    }

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии изменения значения прогресс бара во время применения скидок. ({0})", ex.Message), GetType().Name + ".List_onProcessedAployDMCalkMaxDiscPercProgressBar", Lib.EventEn.Error, true, true);
                }
            }
        }

        // Событие начала просмотра чеков и расчёт скидки внутри сценариев
        delegate void delig_CurrentCnfList_onProcessingCalculateDM(object sender, Lib.EventConfigurationList e);
        private void CurrentCnfList_onProcessingCalculateDM(object sender, Lib.EventConfigurationList e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CurrentCnfList_onProcessingCalculateDM dl = new delig_CurrentCnfList_onProcessingCalculateDM(CurrentCnfList_onProcessingCalculateDM);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    if (this.tstTask != null) this.tstTask.ProcessCalcDM = true;
                    this.pnlCaltDM.Visible = true;
                    this.progressBarCaltDM.Minimum = 0; this.progressBarCaltDM.Maximum = 100; this.progressBarCaltDM.Value = 0;

                    // Прорисовка состояния пользователю
                    this.btnApplay.Visible = false;             // Уситанавливаем видимость кнопки применения скидок
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии начала просмотра чеков и расчёт скидки внутри сценариев. ({0})", ex.Message), GetType().Name + ".CurrentCnfList_onProcessingCalculateDM", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие окончания просмотра чеков и расчёт скидки внутри сценариев
        delegate void delig_CurrentCnfList_onProcessedCalculateDM(object sender, Lib.EventConfigurationListAsicRez e);
        private void CurrentCnfList_onProcessedCalculateDM(object sender, Lib.EventConfigurationListAsicRez e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CurrentCnfList_onProcessedCalculateDM dl = new delig_CurrentCnfList_onProcessedCalculateDM(CurrentCnfList_onProcessedCalculateDM);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.progressBarCaltDM.Minimum = 0; this.progressBarCaltDM.Maximum = 100; this.progressBarCaltDM.Value = 0;
                    if (this.tstTask != null) this.tstTask.ProcessCalcDM = false;
                    this.pnlCaltDM.Visible = false;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();

                    // Проверяем результат работы асинхронного потока
                    if (e.ex != null) MessageBox.Show(string.Format("При окончания просмотра чеков и расчёт скидки внутри сценариев: {0}", e.ex.Message));
                    else
                    {
                        // Запускаем подсчёт итоговой скидки
                        if (tstTask != null)
                        {
                            this.tstTask.RunProseccTotal();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии окончания построения итоговой скидки. ({0})", ex.Message), GetType().Name + ".CurrentCnfList_onProcessedCalculateDM", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие изменения значения прогресс бара во время просмотра чеков во время расчёта скидок
        delegate void delig_CurrentCnfList_onProcessedCalculateDMProgressBar(object sender, Lib.EventConfigurationListProcessingProgerssBar e);
        private void CurrentCnfList_onProcessedCalculateDMProgressBar(object sender, Lib.EventConfigurationListProcessingProgerssBar e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CurrentCnfList_onProcessedCalculateDMProgressBar dl = new delig_CurrentCnfList_onProcessedCalculateDMProgressBar(CurrentCnfList_onProcessedCalculateDMProgressBar);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    // Если панель доступна
                    if (this.pnlCaltDM.Visible)
                    {
                        this.progressBarCaltDM.Minimum = 0; this.progressBarCaltDM.Maximum = e.AllStep; this.progressBarCaltDM.Value = e.CurrentStep;
                    }

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception /*ex*/) // Был момент когда из за прогресс бара падала обработка, для того чтобы этого небыло закоментил этот блок, потом вроде починил, но обратно раскоменчивать не стал, так как у Алгоритма должна быть презентация софта
                {
//                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии изменения значения прогресс бара во время просмотра чеков во время расчёта скидок. ({0})", ex.Message), GetType().Name + ".CurrentCnfList_onProcessedCalculateDMProgressBar", Lib.EventEn.Error, true, true);
                }
            }
        }

        // Событие начало построения итоговой скидки
        delegate void delig_CurrentCnfList_onProcessingCalculateTotal(object sender, Lib.EventConfigurationList e);
        void CurrentCnfList_onProcessingCalculateTotal(object sender, Lib.EventConfigurationList e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CurrentCnfList_onProcessingCalculateTotal dl = new delig_CurrentCnfList_onProcessingCalculateTotal(CurrentCnfList_onProcessingCalculateTotal);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.tstTask.ProcessTotalCalcDM = true;
                    this.pnlCaltDMTotal.Visible = true;
                    this.progressBarCaltDMTotal.Minimum = 0; this.progressBarCaltDMTotal.Maximum = 100; this.progressBarCaltDMTotal.Value = 0;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии начало построения итоговой скидки. ({0})", ex.Message), GetType().Name + ".CurrentCnfList_onProcessingCalculateTotal", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие окончания построения итоговой скидки
        delegate void delig_CurrentCnfList_onProcessedCalculateTotal(object sender, Lib.EventConfigurationListAsicRez e);
        void CurrentCnfList_onProcessedCalculateTotal(object sender, Lib.EventConfigurationListAsicRez e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CurrentCnfList_onProcessedCalculateTotal dl = new delig_CurrentCnfList_onProcessedCalculateTotal(CurrentCnfList_onProcessedCalculateTotal);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    // Обновляем список клиентов
                    this.CustomerLoad();

                    this.progressBarCaltDMTotal.Minimum = 0; this.progressBarCaltDMTotal.Maximum = 100; this.progressBarCaltDMTotal.Value = 0;
                    if (this.tstTask != null) this.tstTask.ProcessTotalCalcDM = false;
                    this.pnlCaltDMTotal.Visible = false;               

                    // Прорисовка состояния пользователю
                    if (Com.UserFarm.CurrentUser.Role != Lib.RoleEn.Viewer)
                    {
                        if (e.ex == null) this.btnApplay.Visible = true;    // Уситанавливаем видимость кнопки применения скидок
                        this.RenderHashWaitProcessing();
                    }

                    // Проверяем результат работы асинхронного потока
                    if (e.ex != null) MessageBox.Show(string.Format("При построении окончательной скидки произошла ошибка: {0}", e.ex.Message));
                
                    // Отрисовываем в меню возможность р=применения этой конфигурации
                    TSMItemSetupNewCnfL_load(e.ex);
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии окончания построения итоговой скидки. ({0})", ex.Message), GetType().Name + ".CurrentCnfList_onProcessedCalculateTotal", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие изменения значения прогресс бара во время построения итоговой скидки
        delegate void delig_CurrentCnfList_onProcessedCalculateTotalProgressBar(object sender, Lib.EventConfigurationListProcessingProgerssBar e);
        void CurrentCnfList_onProcessedCalculateTotalProgressBar(object sender, Lib.EventConfigurationListProcessingProgerssBar e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CurrentCnfList_onProcessedCalculateTotalProgressBar dl = new delig_CurrentCnfList_onProcessedCalculateTotalProgressBar(CurrentCnfList_onProcessedCalculateTotalProgressBar);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    // Если панель доступна
                    if (this.pnlCaltDMTotal.Visible)
                    {
                        this.progressBarCaltDMTotal.Minimum = 0; this.progressBarCaltDMTotal.Maximum = e.AllStep; this.progressBarCaltDMTotal.Value = e.CurrentStep;
                    }

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при событии изменения значения прогресс бара во время построения итоговой скидки. ({0})", ex.Message), GetType().Name + ".CurrentCnfList_onProcessedCalculateTotalProgressBar", Lib.EventEn.Error, true, true);
                }
            }
        }

        /// <summary>
        /// Прорисовка состояния процесса. Сообщает пользователю о необходимости ожидания долгих операций
        /// </summary>
        private void RenderHashWaitProcessing()
        {
            try
            {
                this.pnlWaitDetails.Visible = true;

                if (this.HashPricessingCustomer || this.HashProcessingDscReas || this.HashPricessingAployDMCalkMaxDiscPerc || (this.tstTask == null ? false : this.tstTask.ProcessCalcDM) || (this.tstTask == null ? false : this.tstTask.ProcessTotalCalcDM) || this.HashProcessingDetailCust)
                {
                    this.pnlTopRight.Visible = true;
                }
                else
                {
                    this.pnlTopRight.Visible = false;
                }

                // Прорисовка панели с деталями асинхронных работ
                int Size = (this.pnlDscReas.Visible ? this.pnlDscReas.Size.Height : 0)
                        + (this.pnlCustomers.Visible ? this.pnlCustomers.Size.Height : 0)
                        + (this.pnlApplayDM.Visible ? this.pnlApplayDM.Size.Height : 0)
                        + (this.pnlCaltDM.Visible ? this.pnlCaltDM.Size.Height : 0)
                        + (this.pnlCaltDMTotal.Visible ? this.pnlCaltDMTotal.Size.Height : 0)
                        + (this.pnlDetailCust.Visible ? this.pnlDetailCust.Size.Height : 0);
                //
                this.pnlWaitDetails.Size = new Size(this.pnlWaitDetails.Size.Width, Size);
                this.FStart_SizeChanged(null, null);

                // Меняем место положения кнопки пирменения скидок
                if (this.btnApplay.Visible) this.btnApplay.Location = new Point(this.pnlFill.Size.Width - this.btnApplay.Size.Width - 5, this.btnApplay.Location.Y);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Упали с ошибкой при прорисовки состояния процесса. ({0})", ex.Message), GetType().Name + ".RenderHashWaitProcessing", Lib.EventEn.Error, true, true);
            }
        }

        // Пользовател меняет размеры
        private void FStart_SizeChanged(object sender, EventArgs e)
        {
            this.pnlWaitDetails.Location = new Point(this.pnlFill.Size.Width - this.pnlWaitDetails.Size.Width, this.pnlFill.Size.Height - this.pnlWaitDetails.Size.Height);
        }

        // Событие начала получения деталей по клиенту
        delegate void delig_CustDetailCheck_onProcessing(object sender, Lib.EventCustDetailCheck e);
        void CustDetailCheck_onProcessing(object sender, Lib.EventCustDetailCheck e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CustDetailCheck_onProcessing dl = new delig_CustDetailCheck_onProcessing(CustDetailCheck_onProcessing);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.HashProcessingDetailCust = true;
                    this.pnlDetailCust.Visible = true;
                    this.progressBarDetailCust.Minimum = 0; this.progressBarDetailCust.Maximum = 100; this.progressBarDetailCust.Value = 0;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии начала получения деталей по клиенту. ({0})", ex.Message), GetType().Name + ".CustDetailCheck_onProcessing", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие окончания получения деталей по клиенту
        delegate void delig_CustDetailCheck_onProcessed(object sender, Lib.EventCustDetailCheckAsicRez e);
        void CustDetailCheck_onProcessed(object sender, Lib.EventCustDetailCheckAsicRez e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CustDetailCheck_onProcessed dl = new delig_CustDetailCheck_onProcessed(CustDetailCheck_onProcessed);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    this.progressBarDetailCust.Minimum = 0; this.progressBarDetailCust.Maximum = 100; this.progressBarDetailCust.Value = 0;
                    this.HashProcessingDetailCust = false;
                    this.pnlDetailCust.Visible = false;

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();

                    // Запускаем форму с детализацией
                    using (FCustDetail Frm = new FCustDetail(e.CstDetailChk))
                    {
                        Frm.ShowDialog();
                    }

                    // Проверяем результат работы асинхронного потока
                    if (e.ex != null) MessageBox.Show(string.Format("При получения деталей по клиенту произошла ошибка: {0}", e.ex.Message));
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при cобытии окончания получения деталей по клиенту. ({0})", ex.Message), GetType().Name + ".CustDetailCheck_onProcessed", Lib.EventEn.Error, true, true);
                }
            }
        }
        //
        // Событие изменения значения прогресс бара во время получения деталей по клиенту
        delegate void delig_CustDetailCheck_onProgressBar(object sender, Lib.EventCustDetailCheckProgerssBar e);
        void CustDetailCheck_onProgressBar(object sender, Lib.EventCustDetailCheckProgerssBar e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_CustDetailCheck_onProgressBar dl = new delig_CustDetailCheck_onProgressBar(CustDetailCheck_onProgressBar);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                try
                {
                    // Если панель доступна
                    if (this.pnlDetailCust.Visible)
                    {
                        this.progressBarDetailCust.Minimum = 0; this.progressBarDetailCust.Maximum = e.AllStep; this.progressBarDetailCust.Value = e.CurrentStep;
                    }

                    // Прорисовка состояния пользователю
                    this.RenderHashWaitProcessing();
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Упали с ошибкой при событии изменения значения прогресс бара во время получения деталей по клиенту. ({0})", ex.Message), GetType().Name + ".CustDetailCheck_onProgressBar", Lib.EventEn.Error, true, true);
                }
            }
        }


        #endregion

        #region Другие асинхронные события
        // Произошло событие системное правим текущий статус
        delegate void delig_Log_onEventLog(object sender, Lib.EventLog e);
        void Log_onEventLog(object sender, Lib.EventLog e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_Log_onEventLog dl = new delig_Log_onEventLog(Log_onEventLog);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                bool HashConnect = Com.ProviderFarm.HashConnect();
                if (e == null)
                {
                    if (!HashConnect)
                    {
                        if (Com.Config.Mode == Lib.ModeEn.NotDB)
                        {
                            this.tSSLabel.BackColor = this.DefBaskCoclortSSLabel;
                            this.tSSLabel.Text = "Работаем в режиме NotDB.";
                        }
                        else
                        {
                            this.tSSLabel.BackColor = Color.Khaki;
                            this.tSSLabel.Text = "Подключение с базой данных не установлено.";
                        }
                    }
                    else
                    {
                        this.tSSLabel.Text = string.Format("Подключение с базой данных версии {0} ({1}) установлено.", Com.ProviderFarm.CurrentPrv.VersionDB, Com.ProviderFarm.CurrentPrv.PrvInType);
                    }
                }

                if (e != null)
                {
                    switch (e.Evn)
                    {
                        case Lib.EventEn.Empty:
                        case Lib.EventEn.Dump:
                            break;
                        case Lib.EventEn.Warning:
                            this.tSSLabel.BackColor = Color.Khaki;
                            this.tSSLabel.Text = e.Message;
                            break;
                        case Lib.EventEn.Error:
                        case Lib.EventEn.FatalError:
                            this.tSSLabel.BackColor = Color.Tomato;
                            this.tSSLabel.Text = e.Message;
                            break;
                        default:
                            this.tSSLabel.BackColor = this.DefBaskCoclortSSLabel;
                            this.tSSLabel.Text = e.Message;
                            break;
                    }
                }
            }
        }

        // Произошло добавление нового сценария нужно отобразить это в списке вариантов чтобы пользователь мог выбрать этот сценарий
        delegate void delig_List_onScenariyListAddedScenariy(object sender, Com.Scenariy.Lib.ScenariyBase e);
        void List_onScenariyListAddedScenariy(object sender, Com.Scenariy.Lib.ScenariyBase e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_List_onScenariyListAddedScenariy dl = new delig_List_onScenariyListAddedScenariy(List_onScenariyListAddedScenariy);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                // Добавляем список сценариев которые потом можно выбирать
                this.cmbBoxScenariyName.Items.Add(e.ScenariyName);

                this.ScenariyName.Items.AddRange(e.ScenariyName);

                // обновляем столбец удалив его из таблицы и обратно жобавив, интересно то что порядок в гриде не изменился
                this.dgConfigurations.Columns.Remove(this.ScenariyName.Name);
                this.dgConfigurations.Columns.Add(this.ScenariyName);
            }
        }
        //
        // Пользователь удалил какой-то сценарий
        delegate void delig_List_onScenariyListDeletedScenariy(object sender, Com.Scenariy.Lib.ScenariyBase e);
        void List_onScenariyListDeletedScenariy(object sender, Com.Scenariy.Lib.ScenariyBase e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_List_onScenariyListDeletedScenariy dl = new delig_List_onScenariyListDeletedScenariy(List_onScenariyListDeletedScenariy);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                // Перестраиваем список с доступными сценариями
                this.cmbBoxScenariyName.Items.Clear();
                foreach (Lib.UScenariy item in Com.ScenariyFarm.List)
                {
                    this.cmbBoxScenariyName.Items.Add(item.ScenariyName);
                } 

                for (int i = 0; i < this.ScenariyName.Items.Count; i++)
			    {
                    string ScnName = (string)this.ScenariyName.Items[i];

                    if (ScnName==e.ScenariyName)
                    {
                        this.ScenariyName.Items.RemoveAt(i);
                    }
			    }

                // обновляем столбец удалив его из таблицы и обратно жобавив, интересно то что порядок в гриде не изменился
                this.dgConfigurations.Columns.Remove(this.ScenariyName.Name);
                this.dgConfigurations.Columns.Add(this.ScenariyName);   
            }
        }

        // Пользователь зарегистрировал новый ключь
        delegate void delig_Lic_onRegNewKey(object sender, Com.LicLib.onLicEventKey e);
        void Lic_onRegNewKey(object sender, Com.LicLib.onLicEventKey e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_Lic_onRegNewKey dl = new delig_Lic_onRegNewKey(Lic_onRegNewKey);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                TSMItemAboutScn_Load();
            }
        }

        // Поменялась текущая конфигурация
        delegate void delig_ConfigurationFarm_onСhengedCurrentCnfList(object sender, Lib.EventConfigurationList e);
        void ConfigurationFarm_onСhengedCurrentCnfList(object sender, Lib.EventConfigurationList e)
        {
            if (this.InvokeRequired)
            {
                lock (this.LockObj)
                {
                    delig_ConfigurationFarm_onСhengedCurrentCnfList dl = new delig_ConfigurationFarm_onСhengedCurrentCnfList(ConfigurationFarm_onСhengedCurrentCnfList);
                    this.Invoke(dl, new object[] { sender, e });
                }
            }
            else
            {
                this.lbl_CurConfigList.Text = string.Format(this.lbl_CurConfigList.Tag.ToString(), (Com.ConfigurationFarm.CurrentCnfList != null ? Com.ConfigurationFarm.CurrentCnfList.ConfigurationName : "не выбрана."));
            }
        }

        #endregion

        // Пользователь вызвал правку параметров системы
        private void TSMItemConfigPar_Click(object sender, EventArgs e)
        {
            using (FPar Frm = new FPar())
            {
                Frm.ShowDialog();
            }
        }

        // Пользователь решил поправить подключение
        private void TSMItemConfigDB_Click(object sender, EventArgs e)
        {
            using (FConnetSetup Frm = new FConnetSetup())
            {
                Frm.ShowDialog();
            }
        }

        // Пользователь решил поправить подключение к второй базе типа призм
        private void TSMItemConfigDbPrizm_Click(object sender, EventArgs e)
        {
            try
            {
                using (FConnetSetupPrizm Frm = new FConnetSetupPrizm())
                {
                    Frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, string.Format("{0}.TSMItemConfigDbPrizm_Click", GetType().FullName), Lib.EventEn.Error);
            }
        }

        // Пользователь решил настроить список пользователей
        private void TSMItemConfigUsers_Click(object sender, EventArgs e)
        {
            using (FUsers Frm = new FUsers())
            {
                Frm.ShowDialog();
            }
        }

        // Пользователь решил поправить список доступных сценариев
        private void TSMItemConfigScenary_Click(object sender, EventArgs e)
        {
            using (FScenary Frm = new FScenary())
            {
                Frm.ShowDialog();
            }
        }

        // Пользователь запускает процесс обновления списка клиентов
        private void TSMItemProcCustomer_Click(object sender, EventArgs e)
        {
            // Если есть подключение или мы в режиме без базы данных то нужно проверить необходимость первичной загрузки данных
            if (Com.ProviderFarm.HashConnect() || Com.Config.Mode == Lib.ModeEn.NotDB)
            {
                if(sender != null) this.tstTask = null;
                Com.CustomerFarm.List.ProcessingCustomers();
            }
        }

        /// <summary>
        /// Обновление списка клиентов
        /// </summary>
        private void CustomerLoad()
        {
            this.dtCustomer.Rows.Clear();

            foreach (Com.Data.Customer item in Com.CustomerFarm.List)
            {
                DataRow nrow = this.dtCustomer.NewRow();
                nrow["CustSid"] = item.CustSid.ToString();
                nrow["FirstName"] = item.FirstName;
                nrow["LastName"] = item.LastName;
                nrow["CustId"] = item.CustId;
                nrow["MaxDiscPerc"] = item.MaxDiscPerc.ToString();
                nrow["CalkMaxDiscPerc"] = item.CalkMaxDiscPerc.ToString();
                nrow["StoreCredit"] = item.StoreCredit.ToString();
                nrow["CalkStoreCredit"] = item.CalkStoreCredit.ToString();
                nrow["ScPerc"] = item.ScPerc.ToString();
                nrow["CalkScPerc"] = item.CalkScPerc.ToString();
                nrow["Phone1"] = item.Phone1;
                nrow["Address1"] = item.Address1;
                if (item.FstSaleDate!=null) nrow["FstSaleDate"] = ((DateTime)item.FstSaleDate).ToString();
                if (item.LstSaleDate != null) nrow["LstSaleDate"] = ((DateTime)item.LstSaleDate).ToString();
                nrow["EmailAddr"] = item.EmailAddr;
                this.dtCustomer.Rows.Add(nrow);
            }
        }

        /// <summary>
        /// Настройка видимости палели левой с перечнем текущих конфигураций
        /// </summary>
        private void LoadVisiblePnlLeft()
        {
            if (Com.UserFarm.CurrentUser.Role != Lib.RoleEn.Viewer)
            {
                switch (this.PnlLeftSizePosition)
                {
                    case 1:
                        this.pnlLeft.Visible = true;
                        this.pnlLeft.Size = new Size(251, this.pnlLeft.Size.Height);
                        this.pctBoxRR.Visible = true;
                        this.pctBoxLL.Visible = true;
                        break;
                    case 2:
                        this.pnlLeft.Visible = true;
                        this.pnlLeft.Size = new Size(363, this.pnlLeft.Size.Height);
                        this.pctBoxRR.Visible = false;
                        this.pctBoxLL.Visible = true;
                        break;
                    default:
                        this.pnlLeft.Visible = false;
                        this.pctBoxRR.Visible = true;
                        this.pctBoxLL.Visible = false;
                        this.PnlLeftSizePosition = 0;
                        break;
                }

                // Прорисовка состояния пользователю
                this.RenderHashWaitProcessing();
            }
        }
        //
        // Пользователь ткнул на картинке сдвига вправо
        private void pctBoxRR_Click(object sender, EventArgs e)
        {
            if(this.PnlLeftSizePosition<2) this.PnlLeftSizePosition++;
            else this.PnlLeftSizePosition = 0;

            // Отрисовываем значки чтобы выдвинуть левую панель
            this.LoadVisiblePnlLeft();
        }
        //
        // Пользователь ткнул на картинке сдвига влево
        private void pctBoxLL_Click(object sender, EventArgs e)
        {
            if (this.PnlLeftSizePosition >0) this.PnlLeftSizePosition--;
            else this.PnlLeftSizePosition = 0;

            // Отрисовываем значки чтобы выдвинуть левую панель
            this.LoadVisiblePnlLeft();
        }

        // Пользователь вызывает пересчёт всего в рамках выбранной конфигурации из меню списка
        void nitmConfList_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем конфигурацию в которой хотим посчитать
                Lib.ConfigurationList tmpCnfL = (Lib.ConfigurationList)((ToolStripMenuItem)sender).Tag;

                if (Com.Config.Mode == Lib.ModeEn.NotData && (Com.ProviderFarm.CurrentPrv == null || !Com.ProviderFarm.CurrentPrv.HashConnect)) throw new ApplicationException("Не установлено подключение с базой данных.");               

                if (this.pnlTopRight.Visible) { MessageBox.Show("Попробуйте позже.\r\nпрограмма занята работой с базой данных."); }
                else
                {
                    // Создаём ссылку на конфигурацию по которой будет идти расчёт скидок
                    this.tstTask = new FStartTasks(tmpCnfL);
                    //
                    this.tstTask.CnfList.onProcessingCalculateDM += new EventHandler<Lib.EventConfigurationList>(CurrentCnfList_onProcessingCalculateDM);
                    this.tstTask.CnfList.onProcessedCalculateDM += new EventHandler<Lib.EventConfigurationListAsicRez>(CurrentCnfList_onProcessedCalculateDM);
                    this.tstTask.CnfList.onProcessedCalculateDMProgressBar += new EventHandler<Lib.EventConfigurationListProcessingProgerssBar>(CurrentCnfList_onProcessedCalculateDMProgressBar);
                    this.tstTask.CnfList.onProcessingCalculateTotal += new EventHandler<Lib.EventConfigurationList>(CurrentCnfList_onProcessingCalculateTotal);
                    this.tstTask.CnfList.onProcessedCalculateTotal += new EventHandler<Lib.EventConfigurationListAsicRez>(CurrentCnfList_onProcessedCalculateTotal);
                    this.tstTask.CnfList.onProcessedCalculateTotalProgressBar += new EventHandler<Lib.EventConfigurationListProcessingProgerssBar>(CurrentCnfList_onProcessedCalculateTotalProgressBar);


                    // Перестраиваем элемент для отображения задачи установки новой текущей конфигурации
                    //TSMItemSetupNewCnfL_load(null);       //Если мы хотим чтобы отображалась сразу не дожыдаясть просчёта, то раскоментируй строку

                    // Запускаем перестроение списка клиентов по окончании построения списка система сама начнёт тянуть чеки расчитывать скидку
                    this.TSMItemProcCustomer_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Произошлла ошибка при вызове подсчёта по выбранной пользователем конфигурации. {0}", ex.Message), GetType().Name + ".nitmConfList_Click", Lib.EventEn.Error, true, true);
            }
        }

        // Пользователь меняет значение фильтра
        private void txtBoxFilter_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.txtBoxFilter.Text)) this.dvCustomer.RowFilter = null;
                else this.dvCustomer.RowFilter = @"[FirstName] like '%" + this.txtBoxFilter.Text.Trim() + "%' or [LastName] like '%" + this.txtBoxFilter.Text.Trim() +  "%' or [CustId] like '%" + this.txtBoxFilter.Text.Trim() +"%' or [Phone1] like '%" + this.txtBoxFilter.Text.Trim() + "%' or [Address1] like '%" + this.txtBoxFilter.Text.Trim() + "%' or [EmailAddr] like '%" + this.txtBoxFilter.Text.Trim() + "%'";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Пользователь ткнул на ячейке конкретной конфигурации
        private void dgSharedConfigurations_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                // Проверяем индекс элемента который выбрали пользователи
                if (e.ColumnIndex > -1 && e.RowIndex > -1)
                {
                    string strCnfL = this.dgSharedConfigurations.Rows[e.RowIndex].Cells["ConfigurationName"].Value.ToString();
                    this.EditCngL = Com.ConfigurationFarm.ShdConfigurations[strCnfL];
                    this.ConfigurationsLoad(this.EditCngL);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        // Пользователь собирается удалить конфигурацию
        private void dgSharedConfigurations_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                DialogResult drez = MessageBox.Show(string.Format("Вы действительно хотите удалить конфигурацию: {0}", e.Row.Cells["ConfigurationName"].Value.ToString()), "Удаление конфигурации", MessageBoxButtons.YesNo);

                if (drez != System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    string strCnfL = e.Row.Cells["ConfigurationName"].Value.ToString();
                    Lib.ConfigurationList UCnfL = Com.ConfigurationFarm.ShdConfigurations[strCnfL];
                    Com.ConfigurationFarm.ShdConfigurations.Remove(UCnfL);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        //
        /// <summary>
        /// Отрисовываем данные по сценариям в выбранной конфигурации
        /// </summary>
        /// <param name="CnfL">Конфигурация у которой мы хотим увидеть список сценариев</param>
        private void ConfigurationsLoad(Lib.ConfigurationList CnfL)
        {
            try
            {
                if (this.EditCngL == null || this.EditCngL != CnfL) this.EditCngL = CnfL;

                if (CnfL != null)
                {
                    this.pnlLeftFillBottom.Visible = true;

                    this.CnfLSelected = CnfL;

                    this.dtConfigurations.Rows.Clear();

                    foreach (Lib.Configuration item in CnfL)
                    {
                        DataRow nrow = this.dtConfigurations.NewRow();
                        nrow["ScenariyName"] = item.UScn.ScenariyName;
                        nrow["SaleActionOut"] = item.SaleActionOut.ToString();
                        nrow["ActionRows"] = item.ActionRows;
                        this.dtConfigurations.Rows.Add(nrow);
                    }
                }
                else
                {
                    this.CnfLSelected = null;
                    this.dtConfigurations.Rows.Clear();
                    this.pnlLeftFillBottom.Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Пользователь попал мышкой над ячейкой конфигурации, строим контекстное меню
        private void dgSharedConfigurations_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Чистим контекстное меню
                this.cMStripSharedConf.Items.Clear();

                // Обрабатываем только если мы на актихных ячейках
                if (e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    Lib.ConfigurationList CnfL = Com.ConfigurationFarm.ShdConfigurations[this.dgSharedConfigurations.Rows[e.RowIndex].Cells["ConfigurationName"].Value.ToString()];

                    //Генерим ячейку для удаления сценария
                    using(ConfFToolStripMenuItem TSMItem = new ConfFToolStripMenuItem())
                    {
                        //
                        TSMItem.TSMItem.Text = string.Format(string.Format("Удалить конфигурацию: {0}",CnfL.ConfigurationName));
                        TSMItem.TSMItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                        TSMItem.TSMItem.Tag = CnfL;
                        //InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        //InfoToolStripMenuItem.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                        TSMItem.TSMItem.Click += new EventHandler(TSMItem_Click);
                        //
                        this.cMStripSharedConf.Items.Add(TSMItem.TSMItem);
                    }

                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Упали с ошибкой при наведении мышки на ячёйку. ({0})", ex.Message), GetType().Name + ".dgSharedConfigurations_CellMouseEnter", Lib.EventEn.Error, true, true);
            }
        }
        //
        // Пользователь ткнул на контекстном меню ячейке удаления конфигурации
        void TSMItem_Click(object sender, EventArgs e)
        {
            try
            {
                Lib.ConfigurationList CnfL = (Lib.ConfigurationList)((ToolStripMenuItem)sender).Tag;

                DialogResult drez = MessageBox.Show(string.Format("Вы действительно хотите удалить конфигурацию: {0}", CnfL.ConfigurationName), "Удаление конфигурации", MessageBoxButtons.YesNo);

                if (drez == System.Windows.Forms.DialogResult.Yes)
                {
                    Com.ConfigurationFarm.ShdConfigurations.Remove(CnfL);
                    for (int i = 0; i < this.dtSharedConfigurations.Rows.Count; i++)
                    {
                        if (this.dtSharedConfigurations.Rows[i]["ConfigurationName"].ToString() == CnfL.ConfigurationName) this.dtSharedConfigurations.Rows[i].Delete();
                    }
                }

                this.ConfigurationsLoad(null);

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Пользователь попал мышкой над ячейкой элемента конфигурации, строим контекстное меню
        private void dgConfigurations_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Чистим контекстное меню
                this.cMStripCnf.Items.Clear();

                // Обрабатываем только если мы на актихных ячейках
                if (e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    Lib.Configuration Cnf = this.CnfLSelected[this.dgConfigurations.Rows[e.RowIndex].Cells["ScenariyName"].Value.ToString()];

                    //Генерим ячейку для удаления сценария
                    using (ConfFToolStripMenuItem TSMItem = new ConfFToolStripMenuItem())
                    {
                        //
                        TSMItem.TSMItem.Text = string.Format(string.Format("Удалить элемент: {0}", Cnf.UScn.ScenariyName));
                        TSMItem.TSMItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                        TSMItem.TSMItem.Tag = Cnf;
                        //InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        //InfoToolStripMenuItem.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                        TSMItem.TSMItem.Click += new EventHandler(TSMItemCnf_Click);
                        //
                        this.cMStripCnf.Items.Add(TSMItem.TSMItem);
                    }

                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Упали с ошибкой при наведении мышки на ячёйку. ({0})", ex.Message), GetType().Name + ".dgSharedConfigurations_CellMouseEnter", Lib.EventEn.Error, true, true);
            }
        }
        //
        // Пользователь ткнул на контекстном меню ячейке удаления конфигурации
        void TSMItemCnf_Click(object sender, EventArgs e)
        {
            try
            {
                Lib.Configuration CnfL = (Lib.Configuration)((ToolStripMenuItem)sender).Tag;

                Lib.Configuration delCnf = this.EditCngL[CnfL.UScn.ScenariyName];

                DialogResult drez = MessageBox.Show(string.Format("Вы действительно хотите удалить элемент конфигурации: {0} ({1})", delCnf.UScn.ScenariyName, delCnf.SaleActionOut.ToString()), "Удаление элемента конфигурации", MessageBoxButtons.YesNo);

                if (drez == System.Windows.Forms.DialogResult.Yes)
                {
                    string tmpScenariyName = delCnf.UScn.ScenariyName;
                    this.EditCngL.Remove(delCnf, true, true);
                    for (int i = 0; i < this.dtConfigurations.Rows.Count; i++)
                    {
                        if (this.dtConfigurations.Rows[i]["ScenariyName"].ToString() == tmpScenariyName) this.dtConfigurations.Rows[i].Delete();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Пользователь создал новую конфигурацию
        private void btnAddCnfL_Click(object sender, EventArgs e)
        {
            try 
	        {
                // Проверяем на пустоту
                if (string.IsNullOrWhiteSpace(this.txtBoxAddCnfName.Text)) new ApplicationException("Имя не может быть пустым");

                // Проверяем на существование данной конфигурации
                Lib.ConfigurationList CnfL = Com.ConfigurationFarm.ShdConfigurations[this.txtBoxAddCnfName.Text.Trim()];
                if (CnfL != null) new ApplicationException(string.Format("Конфигурация с таким именем ({0}) уже существует. Переименуйте и попробуйте снова.", this.txtBoxAddCnfName.Text.Trim()));

                // Добавляем новую конфигурацию в список
                CnfL = new Lib.ConfigurationList(this.txtBoxAddCnfName.Text.Trim());
                Com.ConfigurationFarm.ShdConfigurations.Add(CnfL);

                // Сброс имени, чтобы пользователь мог добавить ещё
                this.txtBoxAddCnfName.Text = null;
	        }
	        catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
        }

        // Пользователь добавляет сценарий в выбранную конфигурацию
        private void btnAddCnf_Click(object sender, EventArgs e)
        {
            try 
	        {
                if (this.EditCngL == null) throw new ApplicationException("Вы не выбрали конфигурацию которую вы хотите редактировать."); 
                if (this.cmbBoxScenariyName.SelectedIndex == -1) throw new ApplicationException("Не выбран сценарий, который вы хотите добавить.");
                if (this.cmbBoxSaleActionOut.SelectedIndex == -1) throw new ApplicationException("Не выбрано правило применения скидки.");

                Lib.UScenariy UScn = Com.ScenariyFarm.List[this.cmbBoxScenariyName.Items[this.cmbBoxScenariyName.SelectedIndex].ToString()];
                if (UScn==null) throw new ApplicationException("Не найден такой сценарий.");
                bool FlagExists = false;
                foreach (DataRow item in this.dtConfigurations.Rows)
                {
                    if (item["ScenariyName"].ToString() == UScn.ScenariyName) FlagExists = true;
                }
                if (FlagExists) throw new ApplicationException("Такой сценарий уже используется в данной конфигурации.");

                string SActionOut = this.cmbBoxSaleActionOut.Items[this.cmbBoxSaleActionOut.SelectedIndex].ToString();
                if (string.IsNullOrWhiteSpace(SActionOut)) throw new ApplicationException("Не найдено такого правила применения.");

                // Спрашиваем и если нужно применяем
                DialogResult drez = MessageBox.Show(string.Format("Вы действительно хотите добавить сценарий: {0} ({1})\r\n в конфигурацию: {2}", UScn.ScenariyName, SActionOut, this.EditCngL.ConfigurationName), string.Format("Добавление сценария в конфигурацию: {0}", this.EditCngL.ConfigurationName), MessageBoxButtons.YesNo);
                if (drez == System.Windows.Forms.DialogResult.Yes)
                {
                    this.EditCngL.Add(new Lib.Configuration(true, UScn, Lib.EventConverter.Convert(SActionOut, Lib.CongigurationActionSaleEn.Last)), true, true);

                    // Перерисовываем
                    this.ConfigurationsLoad(this.EditCngL);
                }
            }
	        catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }
        //
        // Пользователь собирается удалить элемент в конфигурации  dgConfigurations
        private void dgConfigurations_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                DialogResult drez = MessageBox.Show(string.Format("Вы действительно хотите удалить элемент конфигурации: {0} ({1})", e.Row.Cells["ScenariyName"].Value.ToString(), e.Row.Cells["SaleActionOut"].Value.ToString()), "Удаление элемента конфигурации", MessageBoxButtons.YesNo);

                if (drez != System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    string strCnf = e.Row.Cells["ScenariyName"].Value.ToString();
                    Lib.Configuration delCnf = this.EditCngL[e.Row.Index];
                    this.EditCngL.Remove(delCnf, true, true);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // В системе появилась новая конфигурация нужно её добавить чтобы пользователь увидел её в гриде
        void ShdConfigurations_onConfigurationsLstAddedConfigurationsLst(object sender, Lib.EventConfigurationList e)
        {
            try
            {
                DataRow nrow = this.dtSharedConfigurations.NewRow();
                nrow["ConfigurationName"] = e.CfgL.ConfigurationName;
                this.dtSharedConfigurations.Rows.Add(nrow);

                // Строим менюшку чтобы пользователь мог пересчитать скидку в любой конфигурации
                /*ToolStripMenuItem nitmConfList = new ToolStripMenuItem();
                nitmConfList.Text = string.Format("Пересчитать всё в конфигурации: {0}.", e.CfgL.ConfigurationName);
                nitmConfList.Font = new System.Drawing.Font("Segoe UI", 9F);
                nitmConfList.Tag = e.CfgL;
                //nitmConfList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                //nitmConfList.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                nitmConfList.Click += new EventHandler(nitmConfList_Click);
                this.TSMItemAction.DropDownItems.Add(nitmConfList);*/

                using (ConfFToolStripMenuItem itm = new ConfFToolStripMenuItem())
                {
                    itm.TSMItem.Text = string.Format("Пересчитать всё в конфигурации: {0}.", e.CfgL.ConfigurationName);
                    itm.TSMItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                    itm.TSMItem.Tag = e.CfgL;
                    itm.TSMItem.Click += new EventHandler(nitmConfList_Click);
                    this.TSMItemAction.DropDownItems.Add(itm.TSMItem);
                }
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Произошлла ошибка при вызове подсчёта по выбранной пользователем конфигурации. {0}", ex.Message), GetType().Name + ".ShdConfigurations_onConfigurationsLstAddedConfigurationsLst", Lib.EventEn.Error, true, true);
            }
        }

        // Произошло событие удаления конфигурации
        void ShdConfigurations_onConfigurationsLstListDeletedConfigurationsLst(object sender, Lib.EventConfigurationList e)
        {
            bool flag = false;
            if (this.TSMItemAction.HasDropDownItems)
            {
                for (int i = 0; i < this.TSMItemAction.DropDownItems.Count; i++)
	            {
                    try
                    {
                        // Если обнаружили эту конфигурацию в спаиске, то грохаем её
                        if (((Lib.ConfigurationList)this.TSMItemAction.DropDownItems[i].Tag).ConfigurationName == e.CfgL.ConfigurationName)
                        {
                            this.TSMItemAction.DropDownItems.RemoveAt(i);
                            flag = true;
                        }
                    }
                    catch (Exception){}
	            }  
            }

            if (flag)
            {
                this.tstTask = null;
                TSMItemSetupNewCnfL_load(null);
            }

        }

        // Пользователь применяет текущие рассчёты
        private void btnApplay_Click(object sender, EventArgs e)
        {
            Com.CustomerFarm.List.ProcessAployDMCalkScn(null);
        }

        // Пользователь хочет посмотреть наличие лицензии или её продлить
        private void TSMItemLic_Click(object sender, EventArgs e)
        {
            using (FLic Frm = new FLic())
            {
                Frm.ShowDialog();
            }
        }

        // Пользователь устанавливает новую текущую конфигурацию
        private void TSMItemSetupNewCnfL_Click(object sender, EventArgs e)
        {
            try
            {
                Com.ConfigurationFarm.SetupCurrentSharedConfigurations(this.tstTask.CnfList);

                // Перестраиваем элемент для отображения задачи установки новой текущей конфигурации    Пересчитать в текущей конфигурации (@CurCnfl)
                TSMItemSetupNewCnfL_load(null); 

                // VМеняем текст в менюшке
                if (Com.ConfigurationFarm.CurrentCnfList!=null) this.TSMItemProcCustomer.Text=((string)this.TSMItemProcCustomer.Tag).Replace("@CurCnfl",Com.ConfigurationFarm.CurrentCnfList.ConfigurationName);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format("Произошлла ошибка при установке новой текущей конфигурации. {0}", ex.Message), GetType().Name + ".TSMItemSetupNewCnfL_Click", Lib.EventEn.Error, true, true);
            }
        }

        // Пользователь попадает в какую-то ячейкой своей мышкой
        private void dgCustomer_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try 
	        {
                if (e.ColumnIndex > -1 && e.RowIndex > -1 && Com.UserFarm.CurrentUser.Role == Lib.RoleEn.Admin)
                {
                    Com.Data.Lib.CustomerBase CstB = Com.CustomerFarm.List[long.Parse(this.dgCustomer.Rows[e.RowIndex].Cells["CustSid"].Value.ToString())];

                    this.TlSpMenuItemCustDetail.Visible = true;
                    this.TlSpMenuItemCustDetail.Tag = CstB;
                    this.TlSpMenuItemCustDetail.Text = string.Format("Посмотреть детали по клиенту: {0}", CstB.FirstName);

                    this.TSMenuItemMainClient.Tag = CstB;
                    this.TSMenuItemMainClient.Text = string.Format("Выбрать как основного клиента для объединения карточек: {0}", CstB.FirstName);
                    //
                    //if (this.TSMenuItemMainClient.Visible)
                    //{
                        this.TSMItemDonorClient.Tag = CstB;
                        this.TSMItemDonorClient.Text = string.Format("Выбрать как клиента для объединения с основным клиентом: {0}", CstB.FirstName);
                    //}
                }
                else this.TlSpMenuItemCustDetail.Visible = false;

                if (Com.Config.CurAlgiritmSmtpDir != null)
                {
                    Com.Data.Lib.CustomerBase CstB = Com.CustomerFarm.List[long.Parse(this.dgCustomer.Rows[e.RowIndex].Cells["CustSid"].Value.ToString())];

                    this.TlSpMenuItemCustAlgoritmSMTP.Visible = true;
                    this.TlSpMenuItemCustAlgoritmSMTP.Tag = CstB;
                    this.TlSpMenuItemCustAlgoritmSMTP.Text = string.Format(Com.Config.CurAlgiritmSmtpText, CstB.FirstName);
                }
                else this.TlSpMenuItemCustAlgoritmSMTP.Visible = false;
	        }
	        catch (Exception) { }
        }

        // Пользователь отправляет сообщение через AlgoritmSMTP
        private void TlSpMenuItemCustAlgoritmSMTP_Click(object sender, EventArgs e)
        {
            try
            {
                // Получили наш объект
                Com.Data.Lib.CustomerBase CstB = (Com.Data.Lib.CustomerBase)((ToolStripMenuItem)sender).Tag;

                // Строим параметры, которые передадим внешней программе
                string parOut = Com.Config.CurAlgiritmSmtpPar;
                parOut = parOut.Replace("@CustId", CstB.CustId.ToString());
                parOut = parOut.Replace("@CustSid", CstB.CustSid.ToString());

                // Создаём процесс с нашими настройками и ключами
                ProcessStartInfo pi = new ProcessStartInfo(string.Format(@"{0}\AlgoritmSMTP.exe", Com.Config.CurAlgiritmSmtpDir), string.Format(@"-s {0} -c ""{1}""", Com.Config.CurAlgiritmSmtpQuery, parOut));
                pi.WorkingDirectory = Com.Config.CurAlgiritmSmtpDir;
                pi.UseShellExecute = false;
                pi.RedirectStandardOutput = true;
                pi.CreateNoWindow = false;
                pi.WindowStyle = ProcessWindowStyle.Hidden;

                // Собственно запускаем процесс
                Process p = Process.Start(pi);

                // Получаем строку вывода
                StreamReader sr = p.StandardOutput;

                // Собственно сам результат
                string line;
                string rez = null;
                while ((line = sr.ReadLine()) != null)
                {
                   rez=line;
                }
                p.WaitForExit();

                if (!string.IsNullOrWhiteSpace(rez)) MessageBox.Show(rez);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Пользователь захотел увидеть детали по клиенту
        private void TlSpMenuItemCustDetail_Click(object sender, EventArgs e)
        {
            try 
	        {
                if (this.HashProcessingDetailCust) MessageBox.Show("Уже идёт получение деталей по клиенту. Дождитесь его окончания и попробуйте снова.");
                else
                {
                    Com.Data.Lib.CustomerBase CstB = (Com.Data.Lib.CustomerBase)((ToolStripMenuItem)sender).Tag;
                    Lib.UProvider.CustDetailCheck CustDetailCheck = Com.ProviderFarm.CurrentPrv.GetDetailCheckForCustumer(CstB);
                    CustDetailCheck.onProcessing += new EventHandler<Lib.EventCustDetailCheck>(CustDetailCheck_onProcessing);
                    CustDetailCheck.onProcessed += new EventHandler<Lib.EventCustDetailCheckAsicRez>(CustDetailCheck_onProcessed);
                    CustDetailCheck.onProgressBar += new EventHandler<Lib.EventCustDetailCheckProgerssBar>(CustDetailCheck_onProgressBar);
                    CustDetailCheck.Run();
                }
	        }
	        catch (Exception) { }
        }

        // Пользователь выбрал основного клиента
        private void TSMenuItemMainClient_Click(object sender, EventArgs e)
        {
            try
            {
                this.MergeClientMain = (Com.Data.Customer)((ToolStripMenuItem)sender).Tag;
                this.MergeClientDonors.Clear();

                this.TSMenuItemMainClient.Visible = false;
                this.TSMItemDonorClient.Visible = true;

                // Отрисовка кнопки
                this.RenderbtnMerge();
            }
            catch (Exception) { }
        }

        // Пользователь выбрал донора
        private void TSMItemDonorClient_Click(object sender, EventArgs e)
        {
            try
            {
                this.MergeClientDonors.Add((Com.Data.Customer)((ToolStripMenuItem)sender).Tag);

                //this.TSMenuItemMainClient.Visible = false;
                this.TSMItemDonorClient.Visible = true;

                // Отрисовка кнопки
                this.RenderbtnMerge();
            }
            catch (Exception) { }
        }

        // Отрисовка кнопки
        private void RenderbtnMerge()
        {
            try
            {
                if (this.MergeClientMain != null)
                {
                    this.btnMerge.Visible = true;
                    this.btnMerge.Text = string.Format("Объединить с {0} клиентами.", this.MergeClientDonors.Count);
                }
                else
                {
                    this.btnMerge.Visible = false;
                }

            }
            catch (Exception){}
        }

        // Пользователь ажал накнопку объединения клиентов
        private void btnMerge_Click(object sender, EventArgs e)
        {
            try
            {
                using (FMerge Frm = new FMerge(this.MergeClientMain, this.MergeClientDonors))
                {
                    Frm.ShowDialog();

                    DialogResult Drez = Frm.DialogResult;
                    if (Drez == System.Windows.Forms.DialogResult.OK || Drez == System.Windows.Forms.DialogResult.No)
                    {
                        this.MergeClientMain = null;
                        this.MergeClientDonors.Clear();

                        this.TSMenuItemMainClient.Visible = true;
                        this.TSMItemDonorClient.Visible = false;

                        // Отрисовка кнопки
                        this.RenderbtnMerge();
                    }
                    else
                    {
                        
                    }
                }
            }
            catch (Exception) { }
        }
    }

    /// <summary>
    ///  Класс для обработки заданий 
    /// </summary>
    public class FStartTasks
    {
        /// <summary>
        /// Конфигурация которую тестируем
        /// </summary>
        public Lib.ConfigurationList CnfList { get; private set; }

        /// <summary>
        ///  Флаг процесса подсчёта по чекам
        /// </summary>
        public bool ProcessCalcDM = false;

        /// <summary>
        /// Процесс получения итоговой скидки
        /// </summary>
        public bool ProcessTotalCalcDM = false;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CnfList">Ссылка на конфигурацию которую считаем</param>
        public FStartTasks(Lib.ConfigurationList CnfList)
        {
            this.CnfList = CnfList;
        }

        /// <summary>
        /// Запуск процесса выкачивания данных из чеков
        /// </summary>
        public void RunProcess()
        {
            // Чистим от предыдущих данных и выкачиваем чеки
            //CnfList.ProcessClearCalcMaxDiscPerc();
            this.CnfList.ProcessingCalculateDM();
        }

        /// <summary>
        /// Запуск процесса подсчёта итоговой скидки
        /// </summary>
        public void RunProseccTotal()
        {
            this.CnfList.ProcessCalcTotalMaxDiscPerc(); 
        }
    }

    /// <summary>
    ///  Обёртка для элемента меню, чтобы не ругался анализатор
    /// </summary>
    public sealed class ConfFToolStripMenuItem  :IDisposable
    {
        public ToolStripMenuItem TSMItem;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ConfFToolStripMenuItem()
        {
            this.TSMItem = new ToolStripMenuItem();
        }

        public void Dispose() 
        { 
            Dispose(false); 
        }
        ~ConfFToolStripMenuItem() 
        { 
            Dispose(false); 
        }
        public void Dispose(bool disposing) 
        { 
            if (disposing) 
            { 
                // free managed resources 
                if (this.TSMItem != null) 
                {
                    this.TSMItem.Dispose();
                    this.TSMItem = null; 
                }

                GC.SuppressFinalize(this); 
            } 
        }
    }
}
