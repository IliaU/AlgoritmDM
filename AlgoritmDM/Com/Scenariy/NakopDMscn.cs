using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using AlgoritmDM.Com.Data;
using AlgoritmDM.Com.Scenariy.Lib;
using AlgoritmDM.Lib;

using System.Xml;
using AlgoritmDM.Com.Scenariy.NakopDM;

namespace AlgoritmDM.Com.Scenariy
{
    /// <summary>
    /// Класс для реализации накопиьельной скидки
    /// </summary>
    public sealed class NakopDMscn :ScenariyBase, ScenariyI
    {
        /// <summary>
        /// Какие типы продаж нам не нужно учитыват в итоговой сумме на которую купил клиент
        /// </summary>
        public List<int> NotDiscReasonId = new List<int>();

        /// <summary>
        /// Список порогов
        /// </summary>
        public List<NakopDM.PorogPoint> PrgpntList = new List<PorogPoint>();

        /// <summary>
        /// Элемент для хранения ссылки на узел содержащий список исключений по типам чеков
        /// </summary>
        private XmlElement xmlDiscReasId;

        /// <summary>
        /// Элемент для хранения ссылки на узел содержащий список порогов
        /// </summary>
        private XmlElement xmlPorogL;

        /// <summary>
        /// Контруктор
        /// </summary>
        /// <param name="ScenariyName">Имя сценария с которым мы потом будем работать</param>
        /// <param name="XmlNode">XML элемент из файла конфигурации</param>
        public NakopDMscn(string ScenariyName, XmlElement XmlNode)
        {
            try
            {

                //Генерим ячейку элемент меню для отображения информации по плагину
                using (ToolStripMenuItem InfoToolStripMenuItem = new ToolStripMenuItem(this.GetType().Name))
                {
                    InfoToolStripMenuItem.Text = "Накопительная скидка";
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

                // Тест
                // Передаём строку следующему сценарию если он указан можно реализовать свою логику на предмет передавать или нет
                // Например по части продуктов можно не передавать информацию следующим сценариям а в настройках указывать применение последнего если оно есть. Таким образом можно исключить например чатьс товаров. Например в первом сценарии указать товары c сейлом чтобы следующие сценарии такие товары не учитывали
                //if (FuncTarget != null) return FuncTarget(Chk, CnfL, NextScenary);

                // Если клиент не предъявил карту то будет null
                if (Chk.CustSid != null)
                {

                    // Пытаемся найти клиента в списке для того чтобы посмотреть что мы уже по нему расчитали а если ничего не расчитали то можно сделать элемент для того чтобы расчитать и затянуть в контекст клиента
                    Customer tmpCust=null;
                    try{tmpCust = Com.CustomerFarm.List.GetCustomer(Chk.CustSid);}
                    catch (Exception ex) { base.UScenariy.EventSave(ex.Message, "transfCheck", EventEn.Warning); return true; }

                    // Проверка существования такого клиента если его нет, то на выход
                    if (tmpCust == null) throw new ApplicationException(string.Format("Пытаемся обработать чек {0} по клиенту CustSid {1} которого не существует в системе.", Chk.InvcNo, Chk.CustSid));

                    // Получаем контекст нашего объектоа
                    //ScenariyDataBase tmpScnDb = base.AccessScnDataForCustomer.getScenariyDataBase(tmpCust);
                    NakopDMData tmpScnDb = (NakopDMData)base.AccessScnDataForCustomer.getScenariyDataBase(tmpCust);

                    // Если контекст не получен то можно его создать от базового или сделать свой отнаследовав от базового
                    //if (tmpScnDb == null) tmpScnDb = new ScenariyDataBase(this) { };
                    if (tmpScnDb == null) tmpScnDb = new NakopDMData(this) { };

                    // Подсчитывем скидку только если этот тип чека не стоит в нашем фильтре
                    if (!HashNotDiscReasonId(Chk))
                    {
                        tmpScnDb.TotalBuy = tmpScnDb.TotalBuy + (Chk.Qty * Chk.Price);

                        // Пробегаем по порогам чтобы понять каку скидку применить
                        foreach (NakopDM.PorogPoint item in this.PrgpntList.OrderByDescending(t => t.Porog))
                        {
                            if (tmpScnDb.TotalBuy >= item.Porog)
                            {
                                tmpScnDb.CalcMaxDiscPerc = item.Procent;
                                break;
                            }
                        }
                    }
                    //tmpScnDb.CalcMaxDiscPerc = (tmpScnDb.CalcMaxDiscPerc == null ? 0 : tmpScnDb.CalcMaxDiscPerc) + 10;

                    // Записываем наши данные обратно в контекст клиента
                    base.AccessScnDataForCustomer.setScenariyDataBase(tmpCust, tmpScnDb);

                    // Передаём добытый чек обработчику. Если обработчика нет, то ничего делать не нужно
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
                        xmlDiscReasId.InnerText = "7";
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
                                            NakopDM.PorogPoint nPorogPnt = new NakopDM.PorogPoint(Prg, Prc);
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
        /// <param name="newPrgpntList">Список порогов</param>
        public void SetupNewPatForXML(List<int> newNotDiscReasonId, List<NakopDM.PorogPoint> newPrgpntList)
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
                        else strNotDiscReasonId = string.Format("{0},{1}",strNotDiscReasonId,item.ToString());
                    }
                    this.xmlDiscReasId.InnerText = strNotDiscReasonId;

                    // Строим элементы с порогами
                    this.xmlPorogL.RemoveAll();
                    foreach (NakopDM.PorogPoint item in newPrgpntList.OrderBy(t => t.Porog))
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
            catch (Exception ex) { base.UScenariy.EventSave(ex.Message, GetType().Name+".SetupNewPatForXML", EventEn.Error); }
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
                if(Chk.DiscReasonId==item)
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
