using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows.Forms;
using AlgoritmDM.Com.Data;
using AlgoritmDM.Com.Scenariy.Lib;
using AlgoritmDM.Lib;

using System.Xml;
using AlgoritmDM.Com.Scenariy.BonusDM;

namespace AlgoritmDM.Com.Scenariy
{
    /// <summary>
    /// Класс для реализации бонусной программы
    /// </summary>
    public sealed class BonusDMscn : ScenariyBase, ScenariyI
    {
        /// <summary>
        /// Какие типы продаж нам не нужно учитыват в итоговой сумме на которую купил клиент
        /// </summary>
        public List<int> NotDiscReasonId = new List<int>();

        /// <summary>
        /// Список порогов
        /// </summary>
        public List<BonusDM.PorogPoint> PrgpntList = new List<PorogPoint>();

        /// <summary>
        /// Элемент для хранения ссылки на узел содержащий список исключений по типам чеков
        /// </summary>
        private XmlElement xmlDiscReasId;

        /// <summary>
        /// Элемент для хранения ссылки на узел содержащий список порогов
        /// </summary>
        private XmlElement xmlPorogL;

        /// <summary>
        /// Для хранения флага о необходимости трасировать происходящее в лог All - по всем клиентам или CustSid по которому нам интересно всё посмотреть
        /// </summary>
        private string TraceCustSid;

        /// <summary>
        /// Стартовая сумма от которой считается бонусный процент
        /// </summary>
        private decimal _Start_SC_Summ = 0;

        /// <summary>
        /// Стартовая сумма от которой считается бонусный процент
        /// </summary>
        public decimal Start_SC_Summ
        {
            get { return this._Start_SC_Summ; }
            private set { }
        }

        /// <summary>
        /// Стартовый процент
        /// </summary>
        public decimal _Start_SC_Perc = 5;

        /// <summary>
        /// Стартовый процент
        /// </summary>
        public decimal Start_SC_Perc
        {
            get { return this._Start_SC_Perc; }
            private set { }
        }

        /// <summary>
        /// Поле, в котором указывается где хранится текущий процент
        /// </summary>
        public string _SC_Perc = "ADDRESS3";

        /// <summary>
        /// Поле, в котором указывается где хранится текущий процент
        /// </summary>
        public string SC_Perc
        {
            get { return this._SC_Perc; }
            private set { }
        }

        /// <summary>
        /// Поле, которое указывается процент пользователю тоесть випу
        /// </summary>
        public string _Manual_SC_Perc = "UDF6";

        /// <summary>
        /// Поле, которое указывается процент пользователю тоесть випу
        /// </summary>
        public string Manual_SC_Perc
        {
            get { return this._Manual_SC_Perc; }
            private set { }
        }

        /// <summary>
        /// Количество дней после пробития чека по истечении которого нужно начислять бонусный процент
        /// </summary>
        public int _Delay_Period = 14;

        /// <summary>
        /// Количество дней после пробития чека по истечении которого нужно начислять бонусный процент
        /// </summary>
        public int Delay_Period
        {
            get { return this._Delay_Period; }
            private set { }
        }

        /// <summary>
        /// Непонятная фигня для работы с возвратами
        /// </summary>
        //public string _Sale_Rcpt_N = "NOTE";

        /// <summary>
        /// Непонятная фигня для работы с возвратами
        /// </summary>
        //public string Sale_Rcpt_N
        //{
        //    get { return this._Sale_Rcpt_N; }
        //    private set { }
        //}

        /// <summary>
        /// Глубина за которую нужно анализировать чеки начиная за х дней от текущего дня
        /// </summary>
        public int _Deep_Conv_SC = 30;

        /// <summary>
        /// Глубина за которую нужно анализировать чеки начиная за х дней от текущего дня
        /// </summary>
        public int Deep_Conv_SC
        {
            get { return this._Deep_Conv_SC; }
            private set { }
        }

        /// <summary>
        /// Поле в таблице пока не понятно зачем оно нужно
        /// </summary>
        // public string _Call_Off_SC = "AUX6";

        /// <summary>
        /// Поле в таблице пока не понятно зачем оно нужно
        /// </summary>
        //public string Call_Off_SC
        // {
        //     get { return this._Call_Off_SC; }
        //     private set { }
        // }

        /// <summary>
        /// Начальная дата, начина я с которой мы вообще начинаем рассматривать чеки
        /// </summary>
        DateTime? _Start_SC_Program;

        /// <summary>
        /// Начальная дата, начина я с которой мы вообще начинаем рассматривать чеки
        /// </summary>
        public DateTime? Start_SC_Program
        {
            get { return this._Start_SC_Program; }
            private set { }
        }

        /// <summary>
        /// Контруктор
        /// </summary>
        /// <param name="ScenariyName">Имя сценария с которым мы потом будем работать</param>
        /// <param name="XmlNode">XML элемент из файла конфигурации</param>
        public BonusDMscn(string ScenariyName, XmlElement XmlNode)
        {
            try
            {
                //Генерим ячейку элемент меню для отображения информации по плагину
                using (ToolStripMenuItem InfoToolStripMenuItem = new ToolStripMenuItem(this.GetType().Name))
                {
                    InfoToolStripMenuItem.Text = "Бонусная программа";
                    InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                    //InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                    //InfoToolStripMenuItem.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                    InfoToolStripMenuItem.Click += new EventHandler(InfoToolStripMenuItem_Click);

                    //Генерим ячейку элемент меню для отображения информации по плагину
                    using (ToolStripMenuItem TSMItem = new ToolStripMenuItem(this.GetType().Name))
                    {
                        TSMItem.Text = string.Format("Настроить сценарий ({0})", ScenariyName);
                        TSMItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                        //InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        //InfoToolStripMenuItem.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                        TSMItem.Click += new EventHandler(ToolStripMenuItemConfig_Click);

                        // Инициируем базовый класс
                        base.SetupScenaryBase(this.GetType(), ScenariyName, InfoToolStripMenuItem, TSMItem, XmlNode);
                    }
                }

                // Устанавливаем значения нашему специфическому параметру по умолчанию только если это значение не установлено. Так как конструктор запускается много раз, а читается настроечный щайл, только если он до этого прочтён небыл
                if (string.IsNullOrWhiteSpace(Com.ConfigurationFarm.ParamsOfScenatiy["SC_Perc"]))
                {
                    Com.ConfigurationFarm.ParamsOfScenatiy.Add("SC_Perc", this._SC_Perc, false);
                }

                // Читаем объекты если их небыло то сгенерируется по умолчанию
                this.XmlLoad();
            }
            catch (Exception ex) { base.UScenariy.EventSave(ex.Message, "NakopDMscn", EventEn.Error); }
        }

        /// <summary>
        /// Сохранение изменений в файл
        /// </summary>
        public override void Save()
        {
            // Проверяет возможность геренации и сохранения элемента
            base.Save();

            // Запускаем сохранение изменений в файл
            base.AceessForDocXML.SaveDoc();
        }

        /// <summary>
        /// Прокачивание чека по цепочке сценариев
        /// </summary>
        /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
        /// <param name="Chk">Чек который был на входе</param>
        /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
        /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
        /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
        /// <returns>Успех обработки функции</returns>
        public override bool transfCheck(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, Check Chk, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate)
        {
            try
            {
                bool rez = false;
                string GeteroSQL = null;

                // Так можно получить любые данные из источника
                //System.Data.DataTable dt = Com.ProviderFarm.CurrentPrv.getData("Select ' GGG' A from dual");
                /*// А так можно записать что-то на источник или что-то там выполнить
                Com.ProviderFarm.CurrentPrv.setData(@"declare a integer;
begin
    a:=1;
end;");*/

                // Тест
                // Передаём строку следующему сценарию если он указан можно реализовать свою логику на предмет передавать или нет
                // Например по части продуктов можно не передавать информацию следующим сценариям а в настройках указывать применение последнего если оно есть. Таким образом можно исключить например чатьс товаров. Например в первом сценарии указать товары c сейлом чтобы следующие сценарии такие товары не учитывали
                // if (FuncTarget != null) return FuncTarget(Chk, CnfL, NextScenary);






                if (Chk.CustSid == 3572385410575699964 /*&& Chk.InvcNo == 2785*/)
                {
                }


                // Пытаемся найти клиента в списке для того чтобы посмотреть что мы уже по нему расчитали а если ничего не расчитали то можно сделать элемент для того чтобы расчитать и затянуть в контекст клиента
                Customer tmpCust = null;
                try { tmpCust = Com.CustomerFarm.List.GetCustomer(Chk.CustSid); }
                catch (Exception ex) { base.UScenariy.EventSave(ex.Message, "transfCheck", EventEn.Warning); return true; }

                // Проверка существования такого клиента если его нет, то на выход
                if (tmpCust == null) throw new ApplicationException(string.Format("Пытаемся обработать чек {0} по клиенту CustSid {1} которого не существует в системе.", Chk.InvcNo, Chk.CustSid));

                // Логируем чек который мы получили если у нас включён режим Trace
                if (Com.Config.Trace && !String.IsNullOrWhiteSpace(this.TraceCustSid) && (this.TraceCustSid == Chk.CustSid.ToString() || this.TraceCustSid == "All")) base.UScenariy.EventSave(String.Format("\r\nОбрабатывается чек {0} по клиенту {1} детали:\r\n{2}", Chk.InvcNo, Chk.CustSid, Chk.Print()), GetType().Name + ".transfCheck", EventEn.Dump);

                // Если клиент не предъявил карту то будет null
                if (Chk.CustSid != null)
                {
                    // Если дата с которой работает бонусная программа в настройках больше чем дата чека, то этот чек обрабатывать не нужно.
                    if (this.Start_SC_Program != null && Chk.PostDate < (DateTime)this.Start_SC_Program) return true;

                    // Получаем контекст нашего объекта
                    //ScenariyDataBase tmpScnDb = base.AccessScnDataForCustomer.getScenariyDataBase(tmpCust);
                    BonusDMData tmpScnDb = (BonusDMData)base.AccessScnDataForCustomer.getScenariyDataBase(tmpCust);

                    // Если контекст не получен то можно его создать от базового или сделать свой отнаследовав от базового
                    //if (tmpScnDb == null) tmpScnDb = new ScenariyDataBase(this) { };
                    if (tmpScnDb == null) tmpScnDb = new BonusDMData(this) { };

                    // Подсчитывем скидку только если этот тип чека не стоит в нашем фильтре
                    if (!HashNotDiscReasonId(Chk))
                    {
                        //tmpScnDb.TotalBuy = tmpScnDb.TotalBuy + (Chk.Qty * Chk.Price);
                        ////////////////////                     
                        // Фильтруем только чеки с максимальной глубиной обработки
                        if (Chk.PostDate.AddDays(this.Deep_Conv_SC) < DateTime.Now && Chk.InvcType == 0)
                        {
                            if (Com.Config.Trace && !String.IsNullOrWhiteSpace(this.TraceCustSid) && (this.TraceCustSid == Chk.CustSid.ToString() || this.TraceCustSid == "All")) base.UScenariy.EventSave(String.Format("\r\nВ параметре Deep_Conv_SC ({0}) указано что обрабатывать только только за {0} дней от текущей даты.", this.Deep_Conv_SC), GetType().Name + ".transfCheck", EventEn.Dump);
                            return true;
                        }
                        else if (Com.Config.Trace && !String.IsNullOrWhiteSpace(this.TraceCustSid) && (this.TraceCustSid == Chk.CustSid.ToString() || this.TraceCustSid == "All")) base.UScenariy.EventSave(String.Format("\r\n\tПроверка в соответствии с параметром Deep_Conv_SC ({0}) прошла  успешно.", this.Deep_Conv_SC), GetType().Name + ".transfCheck", EventEn.Dump);

                        // Проверяем тип чека указанного по условию в ТЗ пункт V.2.i
                        if (Chk.InvcType != 0 && Chk.InvcType != 2)
                        {
                            if (Com.Config.Trace && !String.IsNullOrWhiteSpace(this.TraceCustSid) && (this.TraceCustSid == Chk.CustSid.ToString() || this.TraceCustSid == "All")) base.UScenariy.EventSave(String.Format("\r\nЭтот чек не удовлетворяем пункту ТЗ (V.2.i) у него тип чека {0}.", Chk.InvcType), GetType().Name + ".transfCheck", EventEn.Dump);
                            return true;
                        }
                        else if (Com.Config.Trace && !String.IsNullOrWhiteSpace(this.TraceCustSid) && (this.TraceCustSid == Chk.CustSid.ToString() || this.TraceCustSid == "All")) base.UScenariy.EventSave(String.Format("\r\n\tПункту ТЗ (V.2.i) прошли успешно у него тип чека {0}.", Chk.InvcType), GetType().Name + ".transfCheck", EventEn.Dump);

                        // Проверяем метку ТЗ пункт V.2.iii Если в чеке не указан покупатель 
                        if (Chk.CustSid == null || Chk.CustSid == 0) return true;

                        // Так как мы работаем с таблицей тендор, то нам не нужно учитыватьпозиции там сумма по всему чеку в разбивке по типу
                        if (Chk.ItemPos > 1) return true;

                        if (Chk.CustSid == 5586289060213100540 && Chk.InvcNo == 27757)
                        {
                        }

                        // Проверяем метку ТЗ пункт V.2.ii Если у чека есть метка 
                        GeteroSQL = "";
                        switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                        {
                            case "ODBCprv":
                                if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                {
                                    switch (Com.ProviderFarm.CurrentPrv.Driver)
                                    {
                                        case "SQORA32.DLL":
                                        case "SQORA64.DLL":
                                            GeteroSQL = String.Format(@"Select CUST_SID, SC_PERC, VIP, To_Char(CALL_OFF_SC) As CALL_OFF_SC, LAST_POST_DATE From AKS.CUST_SC_PARAM Where CUST_SID={0}", Chk.CustSid);
                                            break;
                                        case "myodbc8a.dll":
                                        case "myodbc8w.dll":
                                            GeteroSQL = String.Format(@"Select CUST_SID, SC_PERC, VIP, Char(CALL_OFF_SC) As CALL_OFF_SC, LAST_POST_DATE From `aks`.`cust_sc_param` Where CUST_SID='{0}'", Chk.CustSid);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        DataTable tmpCustScParam = Com.ProviderFarm.CurrentPrv.getData(GeteroSQL);
                        //
                        GeteroSQL = "";
                        switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                        {
                            case "ODBCprv":
                                if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                {
                                    switch (Com.ProviderFarm.CurrentPrv.Driver)
                                    {
                                        case "SQORA32.DLL":
                                        case "SQORA64.DLL":
                                            GeteroSQL = String.Format("Select INVC_SID, INVC_NO, ITEM_POS, POST_DATE, CUST_SID, To_Char(TOTAL_SUM) As TOTAL_SUM, To_Char(SC_PERC) As SC_PERC, To_Char(STORE_CREDIT) As STORE_CREDIT From AKS.INVC_SC_DOWN Where INVC_SID={0} and INVC_NO={1} and ITEM_POS={2} and CUST_SID={3}", Chk.InvcSid, Chk.InvcNo, Chk.ItemPos, Chk.CustSid);
                                            break;
                                        case "myodbc8a.dll":
                                        case "myodbc8w.dll":
                                            GeteroSQL = String.Format("Select INVC_SID, INVC_NO, ITEM_POS, POST_DATE, CUST_SID, TOTAL_SUM, SC_PERC, STORE_CREDIT From `aks`.`invc_sc_down` Where INVC_SID={0} and INVC_NO={1} and ITEM_POS={2} and CUST_SID={3}", Chk.InvcSid, Chk.InvcNo, Chk.ItemPos, Chk.CustSid);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        DataTable tmpInvcScDown = Com.ProviderFarm.CurrentPrv.getData(GeteroSQL);
                        if (tmpInvcScDown != null && tmpInvcScDown.Rows.Count > 0)
                        {
                            try
                            {
                                tmpScnDb.TotalBuy = Decimal.Parse(tmpInvcScDown.Rows[0]["TOTAL_SUM"].ToString());
                                tmpScnDb.TotalPrc = Decimal.Parse(tmpInvcScDown.Rows[0]["SC_PERC"].ToString());
                                tmpScnDb.TotalStoreCredit = Decimal.Parse(tmpInvcScDown.Rows[0]["STORE_CREDIT"].ToString());
                                tmpScnDb.CalcScPerc = tmpScnDb.TotalPrc;
                                tmpScnDb.CalcStoreCredit = tmpScnDb.TotalStoreCredit;
                                tmpScnDb.CalcStoreCredit = tmpScnDb.TotalStoreCredit;

                                // Записываем наши данные обратно в контекст клиента
                                base.AccessScnDataForCustomer.setScenariyDataBase(tmpCust, tmpScnDb);

                                // Выполняем просьбу логировать факт смены покукпателя в чеке
                                long tmpItemSid = long.Parse(tmpInvcScDown.Rows[0]["CUST_SID"].ToString());
                                if (Chk.CustSid != tmpItemSid) Com.Log.EventSave(string.Format("Обнаружили что у чека: {0} (от {1} № {2}) изменился покупатель: раньше был {3} а теперь {4}", Chk.InvcSid, Chk.PostDate.ToString(), Chk.InvcNo, tmpItemSid, Chk.CustSid), "BonusDMscn.transfCheck", EventEn.Message, "Alert");

                                return true;
                            }
                            catch (Exception) { }
                        }
                        else  // Если этого чека ещё не обрабатывали то и последующие чеки нужно пересчитать, а для этого нужно удалить все последующие чеки
                        {
                            GeteroSQL = "";
                            switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                            {
                                case "ODBCprv":
                                    if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                    {
                                        switch (Com.ProviderFarm.CurrentPrv.Driver)
                                        {
                                            case "SQORA32.DLL":
                                            case "SQORA64.DLL":
                                                GeteroSQL = String.Format(@"Delete From AKS.INVC_SC_DOWN 
Where (CUST_SID={0} and POST_DATE>To_Date('{1}','DD.MM.YYYY HH24:MI:SS'))
    or (CUST_SID={0} and POST_DATE=To_Date('{1}','DD.MM.YYYY HH24:MI:SS') and invc_no>{2})
    or (CUST_SID={0} and POST_DATE=To_Date('{1}','DD.MM.YYYY HH24:MI:SS') and invc_no={2} and Item_Pos>{3})", Chk.CustSid, Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString() + " " + Chk.PostDate.Hour.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Minute.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Second.ToString().PadLeft(2, '0'), Chk.InvcNo, Chk.ItemPos);
                                                break;
                                            case "myodbc8a.dll":
                                            case "myodbc8w.dll":
                                                GeteroSQL = String.Format(@"Delete From `aks`.`invc_sc_down` 
Where (CUST_SID={0} and POST_DATE>STR_TO_DATE('{1}','%d.%m.%Y %H:%i:%s'))
    or (CUST_SID={0} and POST_DATE=STR_TO_DATE('{1}','%d.%m.%Y %H:%i:%s') and invc_no>{2})
    or (CUST_SID={0} and POST_DATE=STR_TO_DATE('{1}','%d.%m.%Y %H:%i:%s') and invc_no={2} and Item_Pos>{3})", Chk.CustSid, Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString() + " " + Chk.PostDate.Hour.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Minute.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Second.ToString().PadLeft(2, '0'), Chk.InvcNo, Chk.ItemPos);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            Com.ProviderFarm.CurrentPrv.setData(GeteroSQL);
                        }

                        if (Chk.CustSid == 2834063479242428412 && Chk.InvcNo == 3070)
                        {
                        }

                        // Проверяем метку ТЗ пункт V.2.iv Проверяем тип чека 
                        string ComandManualSCPerc = null;
                        //
                        if (this.Manual_SC_Perc.IndexOf("ADDRERSS") > -1)
                        {
                            switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                            {
                                case "ODBCprv":
                                    if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                    {
                                        switch (Com.ProviderFarm.CurrentPrv.Driver)
                                        {
                                            case "SQORA32.DLL":
                                            case "SQORA64.DLL":
                                                ComandManualSCPerc = String.Format(@"Select C.CUST_SID, {1} AS UDF_VALUE
From CMS.CUSTOMER C
    left join CMS.cust_address a on c.cust_sid=a.cust_sid
Where C.CUST_SID={0}", Chk.CustSid, this.Manual_SC_Perc);
                                                break;
                                            case "myodbc8a.dll":
                                            case "myodbc8w.dll":
                                                ComandManualSCPerc = String.Format(@"Select C.CUST_SID, {1} AS UDF_VALUE
From CMS.CUSTOMER C
    left join CMS.cust_address a on c.cust_sid=a.cust_sid
Where C.CUST_SID={0}", Chk.CustSid, this.Manual_SC_Perc);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                            {
                                case "ODBCprv":
                                    if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                    {
                                        switch (Com.ProviderFarm.CurrentPrv.Driver)
                                        {
                                            case "SQORA32.DLL":
                                            case "SQORA64.DLL":
                                                ComandManualSCPerc = String.Format(@"Select C.CUST_SID, S.UDF_ID, U.UDF_NO, S.UDF_VAL_ID, V.UDF_VALUE
From CMS.CUSTOMER C
    Inner Join CMS.UDF U On C.SBS_NO=U.SBS_NO and U.UDF_TYPE=1 and U.UDF_NO={1}
    Inner Join CMS.CUST_SUPPL S On C.CUST_SID=S.CUST_SID and U.UDF_ID=S.UDF_ID
    Inner Join CMS.UDF_VAL V On U.UDF_ID=V.UDF_ID and S.UDF_VAL_ID=V.UDF_VAL_ID
Where C.CUST_SID={0}
", Chk.CustSid, this.Manual_SC_Perc.Replace("UDF", ""));
                                                break;
                                            case "myodbc8a.dll":
                                            case "myodbc8w.dll":
                                                string namecol = this.Manual_SC_Perc;
                                                switch ((this.Manual_SC_Perc).ToLower())
                                                {
                                                    case "udf1_large_string":
                                                    case "udf2_large_string":
                                                        namecol = (this.Manual_SC_Perc + "_large_string").ToLower();
                                                        break;
                                                    default:

                                                        namecol = (this.Manual_SC_Perc + "_string").ToLower();
                                                        break;
                                                }


                                                ComandManualSCPerc = String.Format(@"Select C.sid As CUST_SID, V.{1} as UDF_VALUE
From `rpsods`.`customer` C
  Inner Join `rpsods`.`customer_extend` V On C.sid=V.Cust_sid
Where C.sid={0}
", Chk.CustSid, namecol);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        DataTable tmpUdfValVIP = Com.ProviderFarm.CurrentPrv.getData(ComandManualSCPerc);
                        if (tmpCustScParam != null && tmpCustScParam.Rows.Count > 0 && tmpCustScParam.Rows[0]["CALL_OFF_SC"] != null)
                        {
                            tmpScnDb.TotalPrc = (decimal)tmpCustScParam.Rows[0]["sc_perc"];

                        }

                        // Проверяем пользователь вип или нет
                        bool HashVip = false;
                        try
                        {
                            if (tmpUdfValVIP != null && tmpUdfValVIP.Rows.Count > 0 && !String.IsNullOrWhiteSpace(tmpUdfValVIP.Rows[0]["UDF_VALUE"].ToString()) && decimal.Parse(tmpUdfValVIP.Rows[0]["UDF_VALUE"].ToString()) != 0)
                            {
                                HashVip = true;
                                tmpScnDb.TotalPrc = decimal.Parse(tmpUdfValVIP.Rows[0]["UDF_VALUE"].ToString());
                            }
                        }
                        catch (Exception ex) { string str = ex.Message; }
                        if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nПроверяем пользователь вип или нет. ({0})", HashVip), GetType().Name + ".transfCheck", EventEn.Dump);

                        if (Chk.CustSid == 2834063479242428412 && Chk.InvcNo == 3070)
                        {
                        }

                        // Подсчёт общей суммы на которую купил клиент
                        // Если сейчас там 0, то нам нужно получить инфу из нашей таблицы сумму с предыдущего чека от этого клиента
                        if (tmpScnDb.TotalBuy == 0)
                        {
                            GeteroSQL = "";
                            switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                            {
                                case "ODBCprv":
                                    if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                    {
                                        switch (Com.ProviderFarm.CurrentPrv.Driver)
                                        {
                                            case "SQORA32.DLL":
                                            case "SQORA64.DLL":
                                                GeteroSQL = String.Format(@"With  T As (Select CUST_SID,Max(POST_DATE) As POST_DATE
            From AKS.INVC_SC_DOWN
            Where (CUST_SID={0} and POST_DATE<To_Date('{1}','DD.MM.YYYY HH24:MI:SS'))
                or (CUST_SID={0} and POST_DATE=To_Date('{1}','DD.MM.YYYY HH24:MI:SS') and invc_no<{2})
                or (CUST_SID={0} and POST_DATE=To_Date('{1}','DD.MM.YYYY HH24:MI:SS') and invc_no={2} and Item_Pos<{3})
            Group By CUST_SID), 
    R As (Select C.INVC_SID, C.INVC_NO, C.ITEM_POS, C.POST_DATE, C.CUST_SID, To_Char(C.TOTAL_SUM) as TOTAL_SUM, 
            To_Char(C.SC_PERC) As SC_PERC, To_Char(C.STORE_CREDIT) As STORE_CREDIT, 
            To_Char(C.NEXT_STORE_CREDIT) As NEXT_STORE_CREDIT, C.APPLAY_NEXT_STORE_CREDIT, 
            row_number() Over(Order by INVC_SID desc, INVC_NO desc) As RN
        From AKS.INVC_SC_DOWN C
            inner Join T On C.CUST_SID=T.CUST_SID and C.POST_DATE=T.POST_DATE)
Select *
From R
Where RN=1", Chk.CustSid, Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString() +
           " " + Chk.PostDate.Hour.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Minute.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Second.ToString().PadLeft(2, '0'), Chk.InvcNo, Chk.ItemPos);
                                                break;
                                            case "myodbc8a.dll":
                                            case "myodbc8w.dll":
                                                GeteroSQL = String.Format(@"With T As (Select CUST_SID,Max(POST_DATE) As POST_DATE
            From `aks`.`invc_sc_down`
            Where (CUST_SID={0} and POST_DATE<STR_TO_DATE('{1}','%d.%m.%Y %H:%i:%s'))
                or (CUST_SID={0} and POST_DATE=STR_TO_DATE('{1}','%d.%m.%Y %H:%i:%s') and invc_no<{2})
                or (CUST_SID={0} and POST_DATE=STR_TO_DATE('{1}','%d.%m.%Y %H:%i:%s') and invc_no={2} and Item_Pos<{3})
            Group By CUST_SID),
     R1 As (Select T.CUST_SID, T.POST_DATE, Max(C.INVC_SID) As INVC_SID
            From `aks`.`invc_sc_down` C
              inner Join T On C.CUST_SID=T.CUST_SID and C.POST_DATE=T.POST_DATE
            Group By T.CUST_SID, T.POST_DATE),
     R2 As (Select R1.CUST_SID, R1.POST_DATE, R1.INVC_SID, Max(C.INVC_NO) As INVC_NO
            From `aks`.`invc_sc_down` C
              inner Join R1 On C.CUST_SID=R1.CUST_SID and C.POST_DATE=R1.POST_DATE and C.INVC_SID=R1.INVC_SID
            Group By R1.CUST_SID, R1.POST_DATE, R1.INVC_SID)
Select  C.INVC_SID, C.INVC_NO, C.ITEM_POS, C.POST_DATE, C.CUST_SID, C.TOTAL_SUM, 
            C.SC_PERC, C.STORE_CREDIT, C.NEXT_STORE_CREDIT, C.APPLAY_NEXT_STORE_CREDIT
From `aks`.`invc_sc_down` C
   inner Join R2 On C.CUST_SID=R2.CUST_SID and C.POST_DATE=R2.POST_DATE and C.INVC_SID=R2.INVC_SID and C.INVC_NO=R2.INVC_NO", Chk.CustSid, Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString() +
           " " + Chk.PostDate.Hour.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Minute.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Second.ToString().PadLeft(2, '0'), Chk.InvcNo, Chk.ItemPos);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            DataTable tmpLactCheck = Com.ProviderFarm.CurrentPrv.getData(GeteroSQL);
                            if (tmpLactCheck != null && tmpLactCheck.Rows.Count > 0)
                            {
                                try
                                {
                                    // Тупой ODBC клиент не может прочитать значение 0,01 парает вместо это го пытается читать ,01
                                    string tmpTotalSum = tmpLactCheck.Rows[0]["TOTAL_SUM"].ToString();
                                    if (tmpTotalSum.IndexOf(',') == 0) tmpTotalSum = "0" + tmpTotalSum.Replace(",", Config.TekDelitel);
                                    if (tmpTotalSum.IndexOf('.') == 0) tmpTotalSum = "0" + tmpTotalSum.Replace(".", Config.TekDelitel);
                                    tmpScnDb.TotalBuy = decimal.Parse(tmpTotalSum);
                                    //
                                    string tmpScPerc = tmpLactCheck.Rows[0]["SC_PERC"].ToString();
                                    if (tmpScPerc.IndexOf(',') == 0) tmpScPerc = "0" + tmpScPerc.Replace(",", Config.TekDelitel);
                                    if (tmpScPerc.IndexOf('.') == 0) tmpScPerc = "0" + tmpScPerc.Replace(".", Config.TekDelitel);
                                    tmpScnDb.TotalPrc = decimal.Parse(tmpScPerc);
                                    //
                                    string tmpStoreCredit = tmpLactCheck.Rows[0]["STORE_CREDIT"].ToString();
                                    if (tmpStoreCredit.IndexOf(',') == 0) tmpStoreCredit = "0" + tmpStoreCredit.Replace(",", Config.TekDelitel);
                                    if (tmpStoreCredit.IndexOf('.') == 0) tmpStoreCredit = "0" + tmpStoreCredit.Replace(".", Config.TekDelitel);
                                    tmpScnDb.TotalStoreCredit = decimal.Parse(tmpStoreCredit);
                                }
                                catch (Exception) { }
                            }
                            if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nПодсчёт общей суммы на которую купил клиент. (TotalBuy={0}, TotalPrc={1}, TotalStoreCredit={2})", tmpScnDb.TotalBuy, tmpScnDb.TotalPrc, tmpScnDb.TotalStoreCredit), GetType().Name + ".transfCheck", EventEn.Dump);
                        }



                        if (Chk.CustSid == 2834063479242428412 && Chk.InvcNo == 3070)
                        {
                        }

                        // Проверяем метку ТЗ пункт V.2.iv.b Если это чек "Продажа" и и количество дней прошло после пробития чека 
                        //                        if (Chk.InvcType == 0 && Chk.PostDate.AddDays(this.Delay_Period) > DateTime.Now) return true;
                        if (Chk.InvcType == 0 && Chk.PostDate.AddDays(this.Delay_Period) > DateTime.Now)
                        { }
                        if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nПроверяем метку ТЗ пункт V.2.iv.b Если это чек 'Продажа' и и количество дней прошло после пробития чека )"), GetType().Name + ".transfCheck", EventEn.Dump);

                        // Получаем инфу по чеку из таблицы cms.invc_tender AMT
                        GeteroSQL = "";
                        switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                        {
                            case "ODBCprv":
                                if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                {
                                    switch (Com.ProviderFarm.CurrentPrv.Driver)
                                    {
                                        case "SQORA32.DLL":
                                        case "SQORA64.DLL":
                                            GeteroSQL = String.Format("Select TENDER_TYPE, To_Char(AMT) AMT from cms.invc_tender Where invc_sid={0}", Chk.InvcSid);
                                            break;
                                        case "myodbc8a.dll":
                                        case "myodbc8w.dll":
                                            GeteroSQL = String.Format("Select TENDER_TYPE, amount As AMT from `rpsods`.`tender` Where doc_sid={0}", Chk.InvcSid);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        DataTable tmpTender = Com.ProviderFarm.CurrentPrv.getData(GeteroSQL);
                        if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nПолучаем инфу по чеку из таблицы cms.invc_tender AMT"), GetType().Name + ".transfCheck", EventEn.Dump);


                        //////

                        // К текущей сумме прибавляем сумму чека оплаченного не бонусами, а от бонусного счёта отнимаем сумму оплаченную бонусами
                        tmpScnDb.SumCurentChek = 0;
                        if (tmpTender != null && tmpTender.Rows.Count > 0)
                        {
                            foreach (DataRow item in tmpTender.Rows)
                            {
                                // Тупой ODBC клиент не может прочитать значение 0,01 парает вместо это го пытается читать ,01
                                string tmpAmt = item["AMT"].ToString();
                                if (tmpAmt.IndexOf(',') == 0) tmpAmt = "0" + tmpAmt.Replace(",", Config.TekDelitel);
                                if (tmpAmt.IndexOf('.') == 0) tmpAmt = "0" + tmpAmt.Replace(".", Config.TekDelitel);


                                switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                                {
                                    case "ODBCprv":
                                        if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                        {
                                            switch (Com.ProviderFarm.CurrentPrv.Driver)
                                            {
                                                case "SQORA32.DLL":
                                                case "SQORA64.DLL":

                                                    // Если это продажа
                                                    if (Chk.InvcType == 0)
                                                    {
                                                        if (((decimal)item["TENDER_TYPE"]) == 5)
                                                        {
                                                            tmpScnDb.TotalStoreCredit -= decimal.Parse(tmpAmt);
                                                        }
                                                        else
                                                        {
                                                            tmpScnDb.SumCurentChek += decimal.Parse(tmpAmt);
                                                            tmpScnDb.TotalBuy += decimal.Parse(tmpAmt);
                                                        }
                                                    }
                                                    else // Это возврат
                                                    {
                                                        if (((decimal)item["TENDER_TYPE"]) == 5)
                                                        {
                                                            tmpScnDb.TotalStoreCredit += decimal.Parse(tmpAmt);
                                                        }
                                                        else
                                                        {
                                                            tmpScnDb.SumCurentChek += decimal.Parse(tmpAmt);
                                                            tmpScnDb.TotalBuy -= decimal.Parse(tmpAmt);
                                                        }
                                                    }


                                                    break;
                                                case "myodbc8a.dll":
                                                case "myodbc8w.dll":

                                                    // Если это продажа
                                                    if (Chk.InvcType == 0)
                                                    {
                                                        if (((int)item["TENDER_TYPE"]) == 5)
                                                        {
                                                            tmpScnDb.TotalStoreCredit -= decimal.Parse(tmpAmt);
                                                        }
                                                        else
                                                        {
                                                            tmpScnDb.SumCurentChek += decimal.Parse(tmpAmt);
                                                            tmpScnDb.TotalBuy += decimal.Parse(tmpAmt);
                                                        }
                                                    }
                                                    else // Это возврат
                                                    {
                                                        if (((int)item["TENDER_TYPE"]) == 5)
                                                        {
                                                            tmpScnDb.TotalStoreCredit += decimal.Parse(tmpAmt);
                                                        }
                                                        else
                                                        {
                                                            tmpScnDb.SumCurentChek += decimal.Parse(tmpAmt);
                                                            tmpScnDb.TotalBuy -= decimal.Parse(tmpAmt);
                                                        }
                                                    }

                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nК текущей сумме прибавляем сумму чека оплаченного не бонусами, а от бонусного счёта отнимаем сумму оплаченную бонусами (TotalStoreCredit={0}, SumCurentChek={1}, TotalBuy={2})", tmpScnDb.TotalStoreCredit, tmpScnDb.SumCurentChek, tmpScnDb.TotalBuy), GetType().Name + ".transfCheck", EventEn.Dump);


                        if (Chk.CustSid == 2834063479242428412 && Chk.InvcNo == 3070)
                        {
                        }
                        // Старый процент и подытог который был у клиента до наших манипуляций
                        decimal OldTotalPrc = 0;
                        decimal OldActivePorog = 0;
                        if (tmpScnDb.TotalPrc < this.Start_SC_Perc) OldTotalPrc = this.Start_SC_Perc;
                        else OldTotalPrc = tmpScnDb.TotalPrc;
                        if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nСтарый процент и подытог который был у клиента до наших манипуляций (OldTotalPrc={0}, OldActivePorog={1})", OldTotalPrc, OldActivePorog), GetType().Name + ".transfCheck", EventEn.Dump);

                        // Если это не вип то нужно проверить порог чтобы понять какой процент должен быть назначен
                        if (!HashVip)
                        {
                            if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nЕсли это не вип то нужно проверить порог чтобы понять какой процент должен быть назначен"), GetType().Name + ".transfCheck", EventEn.Dump);
                            // Если накопительная сумма больше или равна стартовому параметру в настройке, то рассчитываем процент бонусной скидки
                            tmpScnDb.ActivePorog = 0;
                            if (tmpScnDb.TotalPrc >= this.Start_SC_Summ)
                            {
                                // Начальный порог, через который прошёл пользователь
                                tmpScnDb.ActivePorog = this.Start_SC_Summ;

                                // Пробегаем по порогам чтобы понять каку скидку применить
                                tmpScnDb.TotalPrc = this.Start_SC_Perc;
                                foreach (BonusDM.PorogPoint item in this.PrgpntList.OrderByDescending(t => t.Porog))
                                {
                                    if (Chk.InvcType == 0 && tmpScnDb.TotalBuy >= item.Porog)
                                    {
                                        tmpScnDb.TotalPrc = item.Procent;
                                        tmpScnDb.ActivePorog = item.Porog;
                                        break;
                                    }
                                    if (Chk.InvcType != 0 && tmpScnDb.TotalBuy >= item.Porog)
                                    {
                                        tmpScnDb.TotalPrc = item.Procent;
                                        tmpScnDb.ActivePorog = item.Porog;
                                    }
                                    if (Chk.InvcType != 0 && tmpScnDb.TotalBuy + tmpScnDb.SumCurentChek >= item.Porog)
                                    {
                                        OldActivePorog = item.Porog;
                                    }
                                    if (Chk.InvcType == 0 && tmpScnDb.SumCurentChek < 0 && tmpScnDb.TotalBuy + (tmpScnDb.SumCurentChek * -1) >= item.Porog)
                                    {
                                        tmpScnDb.ActivePorog = item.Porog;
                                    }
                                }
                            }
                            else tmpScnDb.TotalPrc = 0;
                        }

                        if (Chk.CustSid == 2834063479242428412 && Chk.InvcNo == 3070)
                        {
                        }

                        // Переменная для того чтобы сохранить то значение скидки которое нужно будет применить после того как пройдёт заданный период
                        decimal NextStoreCredid = 0;
                        // Признак того обработано отложенное начисление бонуса илинет
                        int ApplayNextStoreCredit = 0;

                        // Если процент по нашим критериям нулевой, то нет смысла делать эти операции так как это вызовет деление на 0
                        if ((Chk.InvcType == 0 && tmpScnDb.TotalPrc != 0)
                            || (Chk.InvcType != 0 && OldTotalPrc != 0))
                        {
                            if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nЕсли процент по нашим критериям нулевой, то нет смысла делать эти операции так как это вызовет деление на 0"), GetType().Name + ".transfCheck", EventEn.Dump);

                            // Если процент не изменился
                            if (OldTotalPrc == tmpScnDb.TotalPrc)
                            {
                                if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nЕсли процент не изменился"), GetType().Name + ".transfCheck", EventEn.Dump);

                                // Если это продажа то нужно сохранить инфу промежуточную в таблицы
                                if (Chk.InvcType == 0)
                                {
                                    // Проверяем дату если она меньше глубиной чем 14 дней то нужно запомнить эти проценты и применить их потом когда пройдёт заданный период
                                    if (Chk.InvcType == 0 && Chk.PostDate.AddDays(this.Delay_Period) > DateTime.Now && tmpScnDb.SumCurentChek > 0)
                                    {
                                        if (Com.Config.Trace && !String.IsNullOrWhiteSpace(this.TraceCustSid) && (this.TraceCustSid == Chk.CustSid.ToString() || this.TraceCustSid == "All")) base.UScenariy.EventSave(String.Format("\r\nВ параметре Deep_Conv_SC ({0}) указано что обрабатывать только только за {0} дней от текущей даты. Поэтому запоминаем текущую скидку, чтобы потом применить через заданный период.", this.Deep_Conv_SC), GetType().Name + ".transfCheck", EventEn.Dump);

                                        NextStoreCredid += tmpScnDb.SumCurentChek / 100 * tmpScnDb.TotalPrc;
                                    }
                                    else
                                    {
                                        ApplayNextStoreCredit = 1;
                                        tmpScnDb.TotalStoreCredit += tmpScnDb.SumCurentChek / 100 * tmpScnDb.TotalPrc;
                                    }
                                }
                                else //Если это возврат, то бонусы начисленные по процентам нужно вернуть
                                {
                                    tmpScnDb.TotalStoreCredit -= tmpScnDb.SumCurentChek / 100 * tmpScnDb.TotalPrc;
                                }
                            }
                            else //Процент изменился значит произошёл переход на новый уровень, нужно разбить чек и по каждой части начислить бонус отдельно
                            {
                                if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nПроцент изменился значит произошёл переход на новый уровень, нужно разбить чек и по каждой части начислить бонус отдельно"), GetType().Name + ".transfCheck", EventEn.Dump);

                                // Если это продажа то нужно сохранить инфу промежуточную в таблицы
                                if (Chk.InvcType == 0)
                                {
                                    // Нарастающий итог который было до наших манипуляций
                                    decimal OldTotalBuy = tmpScnDb.TotalBuy - tmpScnDb.SumCurentChek;
                                    // Получаем ту часть чека которая была со старым процентом
                                    decimal SumCurentChekBefor = tmpScnDb.ActivePorog - OldTotalBuy;
                                    // Получаем часть чека по которой нужно будет считать уже по новому проценту
                                    decimal SumCurentChekAfte = tmpScnDb.SumCurentChek - SumCurentChekBefor;


                                    // Проверяем дату если она меньше глубиной чем 14 дней то нужно запомнить эти проценты и применить их потом когда пройдёт заданный период
                                    if (Chk.InvcType == 0 && Chk.PostDate.AddDays(this.Delay_Period) > DateTime.Now && tmpScnDb.SumCurentChek > 0)
                                    {
                                        if (Com.Config.Trace && !String.IsNullOrWhiteSpace(this.TraceCustSid) && (this.TraceCustSid == Chk.CustSid.ToString() || this.TraceCustSid == "All")) base.UScenariy.EventSave(String.Format("\r\nВ параметре Deep_Conv_SC ({0}) указано что обрабатывать только только за {0} дней от текущей даты. Поэтому запоминаем текущую скидку, чтобы потом применить через заданный период.", this.Deep_Conv_SC), GetType().Name + ".transfCheck", EventEn.Dump);

                                        // Добавляем бонус по части которая со старым процентом
                                        if (OldTotalPrc != 0) NextStoreCredid += SumCurentChekBefor / 100 * OldTotalPrc;

                                        // Добавляем по той части суммы которая уже с новым процентом
                                        if (tmpScnDb.TotalPrc != 0) NextStoreCredid += SumCurentChekAfte / 100 * tmpScnDb.TotalPrc;
                                    }
                                    else
                                    {
                                        ApplayNextStoreCredit = 1;

                                        // Добавляем бонус по части которая со старым процентом
                                        if (OldTotalPrc != 0) tmpScnDb.TotalStoreCredit += SumCurentChekBefor / 100 * OldTotalPrc;

                                        // Добавляем по той части суммы которая уже с новым процентом
                                        if (tmpScnDb.TotalPrc != 0) tmpScnDb.TotalStoreCredit += SumCurentChekAfte / 100 * tmpScnDb.TotalPrc;
                                    }
                                }
                                else //Если это возврат, то бонусы начисленные по процентам нужно вернуть
                                {
                                    // Нарастающий итог который было до наших манипуляций
                                    decimal OldTotalBuy = tmpScnDb.TotalBuy + tmpScnDb.SumCurentChek;
                                    // Получаем ту часть чека которая была со старым процентом
                                    decimal SumCurentChekBefor = OldTotalBuy - OldActivePorog;
                                    // Получаем часть чека по которой нужно будет считать уже по новому проценту
                                    decimal SumCurentChekAfte = tmpScnDb.SumCurentChek - SumCurentChekBefor;

                                    // Добавляем бонус по части которая со старым процентом
                                    if (OldTotalPrc != 0) tmpScnDb.TotalStoreCredit -= SumCurentChekBefor / 100 * OldTotalPrc;

                                    // Добавляем по той части суммы которая уже с новым процентом
                                    if (tmpScnDb.TotalPrc != 0) tmpScnDb.TotalStoreCredit -= SumCurentChekAfte / 100 * tmpScnDb.TotalPrc;
                                }
                            }
                            if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nNextStoreCredid={0}, ApplayNextStoreCredit={1}", NextStoreCredid, ApplayNextStoreCredit), GetType().Name + ".transfCheck", EventEn.Dump);
                        }

                        // Сохраняем значения для применения в чеке
                        tmpScnDb.CalcScPerc = tmpScnDb.TotalPrc;
                        tmpScnDb.CalcStoreCredit = tmpScnDb.TotalStoreCredit;
                        if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nСохраняем значения для применения в чеке (CalcScPerc={0}, CalcStoreCredit={1}", tmpScnDb.CalcScPerc, tmpScnDb.CalcStoreCredit), GetType().Name + ".transfCheck", EventEn.Dump);

                        // Проверяем наличие промежуточной записи по клиенту
                        if (tmpCustScParam != null && tmpCustScParam.Rows.Count > 0)
                        {
                            if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nПромежуточная записи по клиенту существует"), GetType().Name + ".transfCheck", EventEn.Dump);

                            // Если информация по этому клиенту уже есть, то нужно проверить дату
                            if (tmpCustScParam.Rows[0]["LAST_POST_DATE"] != null && (DateTime)tmpCustScParam.Rows[0]["LAST_POST_DATE"] <= Chk.PostDate)
                            {
                                GeteroSQL = "";
                                switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                                {
                                    case "ODBCprv":
                                        if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                        {
                                            switch (Com.ProviderFarm.CurrentPrv.Driver)
                                            {
                                                case "SQORA32.DLL":
                                                case "SQORA64.DLL":
                                                    GeteroSQL = String.Format("Update AKS.CUST_SC_PARAM Set SC_PERC={1}, VIP={2}, CALL_OFF_SC={3}, LAST_POST_DATE=To_Date('{4}','DD.MM.YYYY') Where CUST_SID={0}", Chk.CustSid, tmpScnDb.TotalPrc.ToString().Replace(",", "."), (HashVip ? 1 : 0), tmpScnDb.TotalStoreCredit.ToString().Replace(",", "."), Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString());
                                                    break;
                                                case "myodbc8a.dll":
                                                case "myodbc8w.dll":
                                                    GeteroSQL = String.Format("Update `aks`.`cust_sc_param` Set SC_PERC={1}, VIP={2}, CALL_OFF_SC={3}, LAST_POST_DATE=STR_TO_DATE('{4}','%d.%m.%Y') Where CUST_SID={0}", Chk.CustSid, tmpScnDb.TotalPrc.ToString().Replace(",", "."), (HashVip ? 1 : 0), tmpScnDb.TotalStoreCredit.ToString().Replace(",", "."), Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString());
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                Com.ProviderFarm.CurrentPrv.setData(GeteroSQL);
                            }
                        }
                        else
                        {
                            if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nПромежуточная записи по клиенту не существует"), GetType().Name + ".transfCheck", EventEn.Dump);
                            GeteroSQL = "";
                            switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                            {
                                case "ODBCprv":
                                    if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                    {
                                        switch (Com.ProviderFarm.CurrentPrv.Driver)
                                        {
                                            case "SQORA32.DLL":
                                            case "SQORA64.DLL":
                                                GeteroSQL = String.Format("Insert into AKS.CUST_SC_PARAM(CUST_SID, SC_PERC, VIP, CALL_OFF_SC, LAST_POST_DATE) Values({0},{1},{2},{3}, To_Date('{4}','DD.MM.YYYY'))", Chk.CustSid, tmpScnDb.TotalPrc.ToString().Replace(",", "."), (HashVip ? 1 : 0), tmpScnDb.TotalStoreCredit.ToString().Replace(",", "."), Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString());
                                                break;
                                            case "myodbc8a.dll":
                                            case "myodbc8w.dll":
                                                GeteroSQL = String.Format("Insert into `aks`.`cust_sc_param`(CUST_SID, SC_PERC, VIP, CALL_OFF_SC, LAST_POST_DATE) Values({0},{1},{2},{3}, STR_TO_DATE('{4}','%d.%m.%Y'))", Chk.CustSid, tmpScnDb.TotalPrc.ToString().Replace(",", "."), (HashVip ? 1 : 0), tmpScnDb.TotalStoreCredit.ToString().Replace(",", "."), Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString());
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            Com.ProviderFarm.CurrentPrv.setData(GeteroSQL);
                        }

                        // Сохраним инфу по чеку в нашу табличку
                        if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nСохраним инфу по чеку в нашу табличку"), GetType().Name + ".transfCheck", EventEn.Dump);
                        if (tmpInvcScDown != null && tmpInvcScDown.Rows.Count > 0)
                        {
                            GeteroSQL = "";
                            switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                            {
                                case "ODBCprv":
                                    if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                    {
                                        switch (Com.ProviderFarm.CurrentPrv.Driver)
                                        {
                                            case "SQORA32.DLL":
                                            case "SQORA64.DLL":
                                                GeteroSQL = String.Format("Update AKS.INVC_SC_DOWN Set POST_DATE=To_Date('{3}','DD.MM.YYYY'), CUST_SID={4}, TOTAL_SUM={5}, SC_PERC={6}, STORE_CREDIT={7} Where INVC_SID={0} and INVC_NO={1} and ITEM_POS={2}", Chk.InvcSid, Chk.InvcNo, Chk.ItemPos, Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString(), Chk.CustSid, tmpScnDb.TotalBuy.ToString().Replace(",", "."), tmpScnDb.TotalPrc.ToString().Replace(",", "."), tmpScnDb.TotalStoreCredit.ToString().Replace(",", "."));
                                                break;
                                            case "myodbc8a.dll":
                                            case "myodbc8w.dll":
                                                GeteroSQL = String.Format("Update `aks`.`invc_sc_down` Set POST_DATE=STR_TO_DATE('{3}','%d.%m.%Y'), CUST_SID={4}, TOTAL_SUM={5}, SC_PERC={6}, STORE_CREDIT={7} Where INVC_SID={0} and INVC_NO={1} and ITEM_POS={2}", Chk.InvcSid, Chk.InvcNo, Chk.ItemPos, Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString(), Chk.CustSid, tmpScnDb.TotalBuy.ToString().Replace(",", "."), tmpScnDb.TotalPrc.ToString().Replace(",", "."), tmpScnDb.TotalStoreCredit.ToString().Replace(",", "."));
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            Com.ProviderFarm.CurrentPrv.setData(GeteroSQL);
                        }
                        else
                        {
                            GeteroSQL = "";
                            switch (Com.ProviderFarm.CurrentPrv.PrvInType)
                            {
                                case "ODBCprv":
                                    if (!string.IsNullOrWhiteSpace(Com.ProviderFarm.CurrentPrv.Driver))
                                    {
                                        switch (Com.ProviderFarm.CurrentPrv.Driver)
                                        {
                                            case "SQORA32.DLL":
                                            case "SQORA64.DLL":
                                                GeteroSQL = String.Format("Insert into AKS.INVC_SC_DOWN(INVC_SID, INVC_NO, ITEM_POS, POST_DATE, CUST_SID, TOTAL_SUM, SC_PERC, STORE_CREDIT, NEXT_STORE_CREDIT, APPLAY_NEXT_STORE_CREDIT) Values({0},{1}, {2}, To_Date('{3}','DD.MM.YYYY HH24:MI:SS'), {4}, {5}, {6}, {7}, {8}, {9})", Chk.InvcSid, Chk.InvcNo, Chk.ItemPos, Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString() +
" " + Chk.PostDate.Hour.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Minute.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Second.ToString().PadLeft(2, '0'), Chk.CustSid, tmpScnDb.TotalBuy.ToString().Replace(",", "."), tmpScnDb.TotalPrc.ToString().Replace(",", "."), tmpScnDb.TotalStoreCredit.ToString().Replace(",", "."), NextStoreCredid.ToString().Replace(",", "."), ApplayNextStoreCredit.ToString().Replace(",", "."));
                                                break;
                                            case "myodbc8a.dll":
                                            case "myodbc8w.dll":
                                                GeteroSQL = String.Format("Insert into `aks`.`invc_sc_down`(INVC_SID, INVC_NO, ITEM_POS, POST_DATE, CUST_SID, TOTAL_SUM, SC_PERC, STORE_CREDIT, NEXT_STORE_CREDIT, APPLAY_NEXT_STORE_CREDIT) Values({0},{1}, {2}, STR_TO_DATE('{3}','%d.%m.%Y %H:%i:%s'), {4}, {5}, {6}, {7}, {8}, {9})", Chk.InvcSid, Chk.InvcNo, Chk.ItemPos, Chk.PostDate.Day.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Month.ToString().PadLeft(2, '0') + "." + Chk.PostDate.Year.ToString() +
" " + Chk.PostDate.Hour.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Minute.ToString().PadLeft(2, '0') + ":" + Chk.PostDate.Second.ToString().PadLeft(2, '0'), Chk.CustSid, tmpScnDb.TotalBuy.ToString().Replace(",", "."), tmpScnDb.TotalPrc.ToString().Replace(",", "."), tmpScnDb.TotalStoreCredit.ToString().Replace(",", "."), NextStoreCredid.ToString().Replace(",", "."), ApplayNextStoreCredit.ToString().Replace(",", "."));
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            Com.ProviderFarm.CurrentPrv.setData(GeteroSQL);
                        }
                    }

                    // Записываем наши данные обратно в контекст клиента
                    if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nЗаписываем наши данные обратно в контекст клиента"), GetType().Name + ".transfCheck", EventEn.Dump);
                    base.AccessScnDataForCustomer.setScenariyDataBase(tmpCust, tmpScnDb);

                    // Передаём добытый чек обработчику. Если обработчика нет, то ничего делать не нужно
                    if (Com.Config.Trace) base.UScenariy.EventSave(String.Format("\r\nПередаём добытый чек обработчику. Если обработчика нет, то ничего делать не нужно"), GetType().Name + ".transfCheck", EventEn.Dump);
                    if (FuncTarget != null) rez = FuncTarget(Chk, CnfL, NextScenary, FirstDate);
                    else rez = true;
                    // 
                    if (!rez) new ApplicationException("Возникла проблема при выполнении следующего сценария во время просмотра чеков. Нет смысла продолжать.");

                }
                else
                {
                    // В данном случае это не ошибка, просто пробивали чек без предьявления карточки
                    //throw new ApplicationException(string.Format("Пытаемся обработать чек у которого не указан клиент.", Chk.InvcNo, Chk.CustSid));
                    rez = true;
                }

                return rez;
            }
            catch (Exception ae)
            {
                base.UScenariy.EventSave(ae.Message, "transfCheck", EventEn.Error);
                return false;
            }
        }

        /// <summary>
        /// Метод чтения из XML файл
        /// </summary>
        public void XmlLoad()
        {
            try
            {
                // Можно протестировать доступ к ветке нашего сценария и есть возможность через класс base.AceessForDocXML делать с ветками что угодно
                if (base.XmlNode != null)
                {
                    // Чистим массивы;
                    this.NotDiscReasonId.Clear();
                    this.PrgpntList.Clear();

                    if (base.XmlNode.ChildNodes.Count == 0)
                    {
                        // Какие типы строк не учитывать
                        XmlElement xmlDiscReasId = base.AceessForDocXML.getNewXmlElement("NotDiscReasonId");
                        xmlDiscReasId.InnerText = "";
                        base.XmlNode.AppendChild(xmlDiscReasId);

                        // Коренвой элемент порогов
                        XmlElement xmlPorogL = base.AceessForDocXML.getNewXmlElement("PorogPointList");
                        base.XmlNode.AppendChild(xmlPorogL);

                        // Порог по умолчанию
                        XmlElement xmlPorog = base.AceessForDocXML.getNewXmlElement("PorogPoint");
                        xmlPorog.SetAttribute("Porog", "10000");
                        xmlPorog.SetAttribute("Procent", "0");
                        xmlPorogL.AppendChild(xmlPorog);

                        // Сохранить документ
                        base.AceessForDocXML.SaveDoc();
                    }


                    // Выгрузка атрибутов коренвого узла 
                    for (int i = 0; i < base.XmlNode.Attributes.Count; i++)
                    {
                        if (base.XmlNode.Attributes[i].Name == "TraceCustSid") this.TraceCustSid = base.XmlNode.Attributes[i].Value.ToString();
                        if (base.XmlNode.Attributes[i].Name == "Start_SC_Summ")
                        {
                            try { this._Start_SC_Summ = Decimal.Parse(base.XmlNode.Attributes[i].Value.ToString()); }
                            catch (Exception) { }
                        }
                        if (base.XmlNode.Attributes[i].Name == "Start_SC_Perc")
                        {
                            try { this._Start_SC_Perc = Decimal.Parse(base.XmlNode.Attributes[i].Value.ToString()); }
                            catch (Exception) { }
                        }
                        if (base.XmlNode.Attributes[i].Name == "SC_Perc")
                        {
                            this._SC_Perc = base.XmlNode.Attributes[i].Value.ToString();
                            Com.ConfigurationFarm.ParamsOfScenatiy.Add("SC_Perc", this._SC_Perc, false);
                        }
                        if (base.XmlNode.Attributes[i].Name == "Manual_SC_Perc") this._Manual_SC_Perc = base.XmlNode.Attributes[i].Value.ToString();
                        if (base.XmlNode.Attributes[i].Name == "Delay_Period")
                        {
                            try { this._Delay_Period = Int32.Parse(base.XmlNode.Attributes[i].Value.ToString()); }
                            catch (Exception) { }
                        }
                        //if (base.XmlNode.Attributes[i].Name == "Sale_Rcpt_N") this._Sale_Rcpt_N = base.XmlNode.Attributes[i].Value.ToString();
                        if (base.XmlNode.Attributes[i].Name == "Deep_Conv_SC")
                        {
                            try { this._Deep_Conv_SC = Int32.Parse(base.XmlNode.Attributes[i].Value.ToString()); }
                            catch (Exception) { }
                        }
                        if (base.XmlNode.Attributes[i].Name == "Start_SC_Program")
                        {
                            try
                            { this._Start_SC_Program = DateTime.Parse(base.XmlNode.Attributes[i].Value.ToString()); }
                            catch (Exception) { }
                        }
                        //if (base.XmlNode.Attributes[i].Name == "Call_Off_SC") this._Call_Off_SC = base.XmlNode.Attributes[i].Value.ToString();
                    }

                    // Пробегаем по элементам
                    foreach (XmlElement item in base.XmlNode.ChildNodes)
                    {
                        switch (item.Name)
                        {
                            case "NotDiscReasonId":
                                this.xmlDiscReasId = item;
                                foreach (string ird in item.InnerText.Trim().Split(','))
                                {
                                    try
                                    {
                                        // Тип продаж Sale мы не учитываем
                                        this.NotDiscReasonId.Add(int.Parse(ird));
                                    }
                                    catch (Exception) { }
                                }
                                break;
                            case "PorogPointList":
                                this.xmlPorogL = item;
                                foreach (XmlElement xPorogPnt in item.ChildNodes)
                                {
                                    if (xPorogPnt.Name == "PorogPoint")
                                    {
                                        decimal Prg = 0;
                                        decimal Prc = 0;

                                        for (int i = 0; i < xPorogPnt.Attributes.Count; i++)
                                        {
                                            if (xPorogPnt.Attributes[i].Name == "Porog") try { Prg = decimal.Parse(xPorogPnt.Attributes[i].Value.ToString()); }
                                                catch (Exception) { }
                                            if (xPorogPnt.Attributes[i].Name == "Procent") try { Prc = decimal.Parse(xPorogPnt.Attributes[i].Value.ToString()); }
                                                catch (Exception) { }
                                        }
                                        if (Prg != 0 || Prc != 0)
                                        {
                                            BonusDM.PorogPoint nPorogPnt = new BonusDM.PorogPoint(Prg, Prc);
                                            this.PrgpntList.Add(nPorogPnt);
                                        }
                                    }
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) { base.UScenariy.EventSave(ex.Message, GetType().Name + ".XmlLoad", EventEn.Error); }
        }

        /// <summary>
        /// Сохранение новых значений наших параметров в XML файл
        /// </summary>
        /// <param name="newNotDiscReasonId">Какие типы продаж нам не нужно учитыват в итоговой сумме на которую купил клиент</param>
        /// <param name="newPrgpntList">Новый список порогов</param>
        /// <param name="newStart_SC_Summ">Новая стартовая сумма от которой считается бонусный процент</param>
        /// <param name="newStart_SC_Perc">Новый стартовый процент</param>
        /// <param name="newSC_Perc">Новое поле, в котором будет храниться текущий процент</param>
        /// <param name="newManual_SC_Perc">Новое поле, которое указывается процент пользователю тоесть випу</param>
        /// <param name="newDelay_Period">Новое кол-во дней после пробития чека по истечении которого нужно начислять бонусный процент</param>
        /// <param name="newSale_Rcpt_N">Новая непонятная фигня для работы с возвратами</param>
        /// <param name="newDeep_Conv_SC">Новая глубина за которую нужно анализировать чеки начиная за х дней от текущего дня</param>
        /// <param name="newCall_Off_SC">Новое поле в таблице пока не понятно зачем оно нужно</param>
        /// <param name="newStart_SC_Program">Начальная дата, начина я с которой мы вообще начинаем рассматривать чеки</param>
        /// <param name="newSC_Perc">Поле в котором будем хранить текущий процент</param>
        public void SetupNewPatForXML(List<int> newNotDiscReasonId, List<BonusDM.PorogPoint> newPrgpntList, decimal newStart_SC_Summ, decimal newStart_SC_Perc, string newManual_SC_Perc, int newDelay_Period, int newDeep_Conv_SC, DateTime? newStart_SC_Program, string newSC_Perc)
        {
            try
            {
                // Нужно проверить наличие ссылки на XML узел
                if (base.XmlNode != null && this.xmlDiscReasId != null && this.xmlPorogL != null)
                {
                    // Какие типы строк не учитывать
                    string strNotDiscReasonId = null;
                    foreach (int item in newNotDiscReasonId.OrderBy(t => t))
                    {
                        if (strNotDiscReasonId == null) strNotDiscReasonId = item.ToString();
                        else strNotDiscReasonId = string.Format("{0},{1}", strNotDiscReasonId, item.ToString());
                    }
                    this.xmlDiscReasId.InnerText = strNotDiscReasonId;

                    // Записываем глобальные значения плагина
                    base.XmlNode.SetAttribute("TraceCustSid", this.TraceCustSid);
                    if (newStart_SC_Summ >= 0)
                    {
                        this._Start_SC_Summ = newStart_SC_Summ;
                        base.XmlNode.SetAttribute("Start_SC_Summ", this._Start_SC_Summ.ToString());
                    }
                    if (newStart_SC_Perc >= 0)
                    {
                        this._Start_SC_Perc = newStart_SC_Perc;
                        base.XmlNode.SetAttribute("Start_SC_Perc", this._Start_SC_Perc.ToString());
                    }
                    if (!String.IsNullOrWhiteSpace(newSC_Perc))
                    {
                        this._SC_Perc = newSC_Perc;
                        base.XmlNode.SetAttribute("SC_Perc", this._SC_Perc);
                        Com.ConfigurationFarm.ParamsOfScenatiy.Add("SC_Perc", this._SC_Perc, false);
                    }
                    if (!String.IsNullOrWhiteSpace(newManual_SC_Perc))
                    {
                        this._Manual_SC_Perc = newManual_SC_Perc;
                        base.XmlNode.SetAttribute("Manual_SC_Perc", this._Manual_SC_Perc);
                    }
                    if (newDelay_Period >= 0)
                    {
                        this._Delay_Period = newDelay_Period;
                        base.XmlNode.SetAttribute("Delay_Period", this._Delay_Period.ToString());
                    }
                    //if (!String.IsNullOrWhiteSpace(newSale_Rcpt_N))
                    //{
                    //    this._Sale_Rcpt_N = newSale_Rcpt_N;
                    //    base.XmlNode.SetAttribute("Sale_Rcpt_N", this._Sale_Rcpt_N);
                    //}
                    if (newDeep_Conv_SC >= 0)
                    {
                        this._Deep_Conv_SC = newDeep_Conv_SC;
                        base.XmlNode.SetAttribute("Deep_Conv_SC", this._Deep_Conv_SC.ToString());
                    }
                    //if (!String.IsNullOrWhiteSpace(newCall_Off_SC))
                    //{
                    //    this._Call_Off_SC = newCall_Off_SC;
                    //   base.XmlNode.SetAttribute("Call_Off_SC", this._Call_Off_SC);
                    // }
                    if (newStart_SC_Program != null)
                    {
                        this._Start_SC_Program = DateTime.Parse(((DateTime)newStart_SC_Program).ToShortDateString());
                        base.XmlNode.SetAttribute("Start_SC_Program", this._Start_SC_Program.ToString().Replace("0:00:00", "").Trim());
                    }
                    else
                    {
                        this._Start_SC_Program = null;
                        base.XmlNode.SetAttribute("Start_SC_Program", string.Empty);
                    }

                    // Строим элементы с порогами
                    this.xmlPorogL.RemoveAll();
                    foreach (BonusDM.PorogPoint item in newPrgpntList.OrderBy(t => t.Porog))
                    {
                        // Порог по умолчанию
                        XmlElement nxmlPorog = base.AceessForDocXML.getNewXmlElement("PorogPoint");
                        nxmlPorog.SetAttribute("Porog", item.Porog.ToString());
                        nxmlPorog.SetAttribute("Procent", item.Procent.ToString());
                        this.xmlPorogL.AppendChild(nxmlPorog);
                    }

                    // Сохранить документ
                    base.AceessForDocXML.SaveDoc();

                    // Сохранить изменения
                    this.PrgpntList = newPrgpntList;
                    this.NotDiscReasonId = newNotDiscReasonId;
                }
            }
            catch (Exception ex) { base.UScenariy.EventSave(ex.Message, GetType().Name + ".SetupNewPatForXML", EventEn.Error); }
        }

        /// <summary>
        /// Проверка чека на тип продажи если содержится ли такой тип в нашем справочнике
        /// </summary>
        /// <param name="Chk">Строка чека</param>
        /// <returns>true если содержится иначе false</returns>
        private bool HashNotDiscReasonId(Check Chk)
        {
            bool rez = false;
            foreach (int item in this.NotDiscReasonId)
            {
                if (Chk.DiscReasonId == item)
                {
                    rez = true;
                    break;
                }
            }
            return rez;
        }

        // Пользователь вызвал меню настройки этого типа сценария
        private void ToolStripMenuItemConfig_Click(object sender, EventArgs e)
        {
            try
            {
                using (FConfig Frm = new FConfig(this))
                {
                    Frm.ShowDialog();
                }
            }
            catch (Exception ex) { base.UScenariy.EventSave(ex.Message, GetType().Name + ".ToolStripMenuItemConfig_Clic", EventEn.Error); }
        }

        // Пользователь вызывает информацию по этому типу сценариев
        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (FInfo Frm = new FInfo(this))
                {
                    Frm.ShowDialog();
                }
            }
            catch (Exception ex) { base.UScenariy.EventSave(ex.Message, GetType().Name + ".InfoToolStripMenuItem_Click", EventEn.Error); }
        }

    }
}
