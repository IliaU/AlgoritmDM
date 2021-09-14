using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using AlgoritmDM.Com.Data;
using AlgoritmDM.Com.Data.Lib;
using AlgoritmDM.Com.Scenariy.Lib;
using System.Windows.Forms;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Класс представляющий из себя конщигурацию
    /// </summary>
    public sealed class ConfigurationList :ConfigurationBase.ConfigurationBaseList
    {
        private object MyObj= new object();

        /// <summary>
        /// Ссылка на асинхронный процесс   
        /// </summary>
        private Thread thr; 
        
        /// <summary>
        /// Последнее возникновение события изменения статуса прогресс бара при процессинге просмотра чеков нашими сценариями
        /// </summary>
        private DateTime LastEventProcessedCalculateDMProgressBar = new DateTime();

        /// <summary>
        /// Последнее возникновение события изменения статуса прогресс бара при процессе подсчёта итоговой скидки
        /// </summary>
        private DateTime LastEventProcessedCalculateTotalProgressBar = new DateTime();

        /// <summary>
        /// Событие возникновения добавления элемента конфигурации
        /// </summary>
        public event EventHandler<EventConfiguration> onConfigurationListAddingConfiguration;
        /// <summary>
        /// Событие добавления элемента конфигурации
        /// </summary>
        public event EventHandler<Configuration> onConfigurationListAddedConfiguration;
        /// <summary>
        /// Событие возникновения удаления элемента конфигурации
        /// </summary>
        public event EventHandler<EventConfiguration> onConfigurationListDeletingConfiguration;
        /// <summary>
        /// Событие удаления элемента конфигурации
        /// </summary>
        public event EventHandler<Configuration> onConfigurationListDeletedConfiguration;
        /// <summary>
        /// Событие возникновения изменения данных элемента конфигурации
        /// </summary>
        public event EventHandler<EventConfiguration> onConfigurationListUpdatingConfiguration;
        /// <summary>
        /// Событие изменения данных элемента конфигурации
        /// </summary>
        public event EventHandler<Configuration> onConfigurationListUpdatedConfiguration;
        /// <summary>
        /// Событие возникновения начала просмотра чеков сценариями
        /// </summary>
        public event EventHandler<EventConfigurationList> onProcessingCalculateDM;
        /// <summary>
        /// Событие окончания просмотра чеков всеми конфигурациями
        /// </summary>
        public event EventHandler<EventConfigurationListAsicRez> onProcessedCalculateDM;
        /// <summary>
        /// Событие работы прогресс бара при просмотре чеков сценариями
        /// </summary>
        public event EventHandler<EventConfigurationListProcessingProgerssBar> onProcessedCalculateDMProgressBar;
        /// <summary>
        /// Событие возникновения подсчёта итоговой скидки относительно всех сценариев данной конфигурации
        /// </summary>
        public event EventHandler<EventConfigurationList> onProcessingCalculateTotal;
        /// <summary>
        /// Событие окончания подсчёта итоговой скидки относительно всех сценариев данной конфигурации
        /// </summary>
        public event EventHandler<EventConfigurationListAsicRez> onProcessedCalculateTotal;
        /// <summary>
        /// Событие работы прогресс бара при просчёте итоговой скидки относительно всех сценариев данной конфигурации
        /// </summary>
        public event EventHandler<EventConfigurationListProcessingProgerssBar> onProcessedCalculateTotalProgressBar;


        /// <summary>
        /// Вытаскивает элемент конфигурации по его индексу в этом контернере
        /// </summary>
        /// <param name="i">Индекс</param>
        /// <returns>Елемент конфигурации</returns>
        public Configuration this[int i]
        {
            get 
            { 
                Configuration rez = null;
                lock (this.MyObj)
                {
                    rez = ((Configuration)base.getConfigurationComponent(i));
                }

                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает элемент конфигурации по его имени сценария в этом контернере
        /// </summary>
        /// <param name="ScenariyName">Имя сценария</param>
        /// <returns>Елемент конфигурации</returns>
        public Configuration this[string ScenariyName]
        {
            get
            {
                Configuration rez = null;
                lock (this.MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (base.getConfigurationComponent(i).UScn.ScenariyName == ScenariyName) rez = (Configuration)base.getConfigurationComponent(i);
                    }
                }

                return rez;
            }
            private set { }
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ConfigurationName">Имя конфигурации. Например день гранённого стакана.</param>
        public ConfigurationList(string ConfigurationName)
        {
            base.SetupConfigurationBaseList(ConfigurationName);
        }

        /// <summary>
        /// Запуск процесса подсчёта скидок
        /// </summary>
        public void ProcessingCalculateDM()
        {
            try
            {
                this.LastEventProcessedCalculateDMProgressBar = DateTime.Now;
                Com.Log.EventSave(string.Format("Запуск процесса просмотра чеков по всем сценариям конфигурации: '{0}'", this.ConfigurationName), this.GetType().Name + ".ProcessingCalculateDM", EventEn.Message);

                EventConfigurationList myArg = new EventConfigurationList(this, null);

                // Обрабатываем события начала процесса просмотра чеков сценариями
                if (onProcessingCalculateDM != null) onProcessingCalculateDM.Invoke(this, myArg);

                // Асинхронный запуск процесса
                this.thr = new Thread(ProcessingCalculateDMRun);
                //this.thr = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                this.thr.Name = "AE_Thr_ProcessingCalculateDM" + this.ConfigurationName;
                this.thr.IsBackground = true;
                this.thr.Start(); 
                                
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessingCalculateDM", EventEn.Error, true, true);
                throw;
            }
        }
        //
        /// <summary>
        /// Асинхронный запуск нашего процесса
        /// </summary>
        private void ProcessingCalculateDMRun()
        {
            ApplicationException RezEx = null; 
            try
            {
                bool rez = false;
                
                // Сначала чистим от предыдущих данных если они есть
                this.ProcessClearCalcMaxDiscPerc();

                // Создаём объект для доступа к интерфейсу ProviderTransferI
                UProvider.Transfer TrfSource = new UProvider.Transfer(Com.ProviderFarm.CurrentPrv, this, 0);
                Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget = TransferCalculateDM;
                rez = TrfSource.PrvTI.getCheck(FuncTarget, this, 0, null, null);

                // Убиваем ссылки
                TrfSource = null;
                FuncTarget = null;

                Com.Log.EventSave(string.Format("Завершён процесса просмотра чеков по всем сценариям конфигурации: '{0}'", this.ConfigurationName), this.GetType().Name + ".ProcessingCalculateDMRun", EventEn.Message);
            }
            catch (Exception ex)
            {
                RezEx = new ApplicationException(ex.Message);
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessingCalculateDMRun", EventEn.Error);
            }
            finally
            {
                EventConfigurationListAsicRez myArg = new EventConfigurationListAsicRez(this, RezEx, null);

                // Обрабатываем события окончания процесса просмотра чеков сценариями
                if (onProcessedCalculateDM != null) onProcessedCalculateDM.Invoke(this, myArg);
            }
        }
        //
        /// <summary>
        /// Получает результат от провайдера для передачи его дальше следующему по номеру сценарию
        /// </summary>
        /// <param name="ChkRow">Строка которую получили от источника</param>
        /// <param name="CnfL">Лист со сценариями</param>
        /// <param name="NextScenary">Следующий элемент который должен обработать эту стрроку полученную от провайдера</param>
        /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
        /// <returns>возвращаем успех если всё ОК</returns>
        private bool TransferCalculateDM(Com.Data.Check ChkRow, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate)
        {
            try
            {
                bool rez = true;

                // Если существует сценарий которому мы хотим передать строку
                if (CnfL.Count > NextScenary)
                {
                    // Получаем первую дату чека для нашего прогресс бара
                    if (FirstDate == null) FirstDate = ChkRow.CreatedDate;

                    // Если строка пришла от провайдера и последнее событие было более 1 секунды назад и есть подписанные события, пробуем посчитать прогресс бар
                    if (NextScenary == 0 && CnfL.LastEventProcessedCalculateDMProgressBar.AddSeconds(1) < DateTime.Now && onProcessedCalculateDMProgressBar != null)
                    {
                        // Для того чтобы прогресс бар не ронял никаких процессов
                        try
                        {
                            // Подсчитываем прогресс
                            int AllStep = ((TimeSpan)(DateTime.Now - FirstDate)).Days;
                            int CurStep = AllStep - ((TimeSpan)(DateTime.Now - ChkRow.CreatedDate)).Days;
                            // Для того чтобы небыло глюков в интерефейе мы делаем чтобы текущий шаг никогда небыл больше общего количества шагов
                            if (CurStep > AllStep) CurStep = AllStep;
                            EventConfigurationListProcessingProgerssBar MaArg = new EventConfigurationListProcessingProgerssBar(this, AllStep, CurStep);

                            // Если подписаны на событие то вызываем его
                            if (onProcessedCalculateDMProgressBar != null) onProcessedCalculateDMProgressBar.Invoke(this, MaArg);

                            // Устанавливаем текущее время
                            CnfL.LastEventProcessedCalculateDMProgressBar = DateTime.Now;
                        }
                        catch (Exception) { }

                    }

                    // Строка пришла от провайдера (NextScenary==0) или в сценарии указано, что нужно передавать строки чеков следующему сценарию.
                    if (NextScenary==0 ||CnfL[NextScenary - 1].ActionRows)
                    {
                        // Создаём объект для доступа к интерфейсу ScenariyTransferI
                        UScenariy.Transfer TrfSource = new UScenariy.Transfer(CnfL[NextScenary].UScn, CnfL, NextScenary + 1, FirstDate);
                        // Создаём функцию чтобы передать сценарию информацию о методе таргета который обработает сценарий
                        Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget = TransferCalculateDM; // делаем ссылку на этот же метод
                        // Собственно используя интрефайс ScenariyTransferI запускаем в плагине сценария подсчёт исходя из чека который мы передаём ему. И передаём также новый номер сценария которому тот сценарий должен будет передать строку чека
                        // Если мы обрабатываем уже последний элемент, то вместо ссылки на следующий элемент передаём null и вместо индекса -1. По этим признакам. Сценарий может определить что он последний в списке сценариев (правдв пока не знаю зачем это нужно, но вдруг пригодится)
                        if (NextScenary + 1 == CnfL.Count) rez = TrfSource.PcnTI.transfCheck(null, ChkRow, CnfL, -1, FirstDate);
                        else rez = TrfSource.PcnTI.transfCheck(FuncTarget, ChkRow, CnfL, NextScenary + 1, FirstDate);

                        // Убиваем ссылки
                        TrfSource = null;
                        FuncTarget = null;

                        // Если есть ошибка, то работать делее не нужно
                        if (!rez) return rez;
                    }
                }
                return rez;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".TransferCalculateDM", EventEn.Error, true, true);
                return false;
            }
        }

        /// <summary>
        /// Процесс отчистки от промежуточных результатов наших сценариев перед прогрузкой чеков в контексте клиентов
        /// </summary>
        private void ProcessClearCalcMaxDiscPerc()
        {
            try
            { 
                // Получаем объект через который будем иметь доступ к базовым классам клиентов
                CustomerBase.AccessForConfigurationList aCfl = new CustomerBase.AccessForConfigurationList(this);

                // Пробегаем посписку клиентов
                foreach (Customer item in Com.CustomerFarm.List)
                {
                    aCfl.ClearScnDataList(item);
                }

                // Убиваем ссылки
                aCfl = null;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessClearCalcMaxDiscPerc", EventEn.Error, true, true);
                throw;
            }
        }

        /// <summary>
        /// Запуск процесса подсчёта итоговой скидки относительно правил применения сценариев
        /// </summary>
        public void ProcessCalcTotalMaxDiscPerc()
        {
            try
            {
                this.LastEventProcessedCalculateTotalProgressBar = DateTime.Now;
                Com.Log.EventSave(string.Format("Запуск процесса подсчёта итоговой скидки относительно правил применения сценариев по всем сценариям конфигурации: '{0}'", this.ConfigurationName), this.GetType().Name + ".ProcessCalcTotalMaxDiscPerc", EventEn.Message);

                EventConfigurationList myArg = new EventConfigurationList(this, null);

                // Обрабатываем события начала процесса подсчёта конечной скидки относительно всех сценариев
                if (onProcessingCalculateTotal != null) onProcessingCalculateTotal.Invoke(this, myArg);

                // Асинхронный запуск процесса
                this.thr = new Thread(ProcessCalcTotalMaxDiscPercRun);
                //this.thr = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                this.thr.Name = "AE_Thr_ProcessingCalculateTotal" + this.ConfigurationName;
                this.thr.IsBackground = true;
                this.thr.Start();

                // Убиваем ссылки
                myArg = null;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessCalcTotalMaxDiscPerc", EventEn.Error, true, true);
                throw;
            }
        }
        //
        /// <summary>
        /// Запуск процесса подсчёта итоговой скидки относительно правил применения сценариев
        /// </summary>
        private void ProcessCalcTotalMaxDiscPercRun()
        {
            ApplicationException RezEx = null; 
            try
            {
                // Выводим предупреждения по нелицензированным типам сценариев
                int tekdtint = int.Parse(((DateTime.Now.Year * 10000) + (DateTime.Now.Month * 100) + DateTime.Now.Day).ToString());
                List<string> tmpNotLicScenary = new List<string>();
                for (int i = 0; i < base.Count; i++)
                {
                    // Получаем элемент сценаария
                    ConfigurationBase Cnf = base.getConfigurationComponent(i);
                    // если какой-то модуль не лицензирован, надо предупредить пользователя
                    if (Cnf.UScn.ValidLicData() < tekdtint)
                    {
                        tmpNotLicScenary.Add(Cnf.UScn.ScenariyName);
                    }
                }
                if (tmpNotLicScenary.Count > 0)
                {
                    string MessageLicInfo = string.Join("\r\n", tmpNotLicScenary);
                    MessageLicInfo=string.Format("У части сценариев в конфигурации '{0}' нет лицензии на использование, по этой причине они не будут применяться в итоговой скидке:\r\n'{1}'", this.ConfigurationName, MessageLicInfo);
                    Com.Log.EventSave(MessageLicInfo, this.GetType().Name + ".ProcessCalcTotalMaxDiscPercRun", EventEn.Warning, true, false);

                    if (Com.UserFarm.CurrentUser != null && Com.UserFarm.CurrentUser.Logon != "Console")
                    {
                        MessageBox.Show(MessageLicInfo);
                    }
                }

                // Объект для доступа к закрытым элементам клиента
                Customer.AccessForConfigurationList AccCuct = new CustomerBase.AccessForConfigurationList(this);

                // Пробегаем по списку клиентов
                foreach (Customer item in Com.CustomerFarm.List)
                {
                    // Если мы в тестовом режиме, то делаем задержку, чтобы увидеть красоту прогресс баров
                    if (Com.Config.Mode != ModeEn.Normal) Thread.Sleep(1500);

                    // Если строка пришла от провайдера и последнее событие было более 1 секунды назад и есть подписанные события, пробуем посчитать прогресс бар
                    if (this.LastEventProcessedCalculateTotalProgressBar.AddSeconds(1) < DateTime.Now && this.onProcessedCalculateTotalProgressBar != null)
                    {
                        // Для того чтобы прогресс бар не ронял никаких процессов
                        try
                        {
                            // Подсчитываем прогресс
                            EventConfigurationListProcessingProgerssBar MaArg = new EventConfigurationListProcessingProgerssBar(this, Com.CustomerFarm.List.Count, item.Index+1);

                            // Если подписаны на событие то вызываем его
                            if (this.onProcessedCalculateTotalProgressBar != null) this.onProcessedCalculateTotalProgressBar.Invoke(this, MaArg);

                            // Убиваем ссылки
                            MaArg = null;

                            // Устанавливаем текущее время
                            this.LastEventProcessedCalculateTotalProgressBar = DateTime.Now;
                        }
                        catch (Exception) { }
                    }

                    decimal? OldCalcMaxDiscPerc = null;
                    decimal? NewCalcMaxDiscPerc = null;
                    decimal? RezCalcMaxDiscPerc = null;
                    decimal? OldCalcStoreCredit = null;
                    decimal? NewCalcStoreCredit = null;
                    decimal? RezCalcStoreCredit = null;
                    decimal? OldCalcScPerc = null;
                    decimal? NewCalcScPerc = null;
                    decimal? RezCalcScPerc = null;

                    if (item.CustSid == 2417549001015889916)
                    {
                    }

                    // Пробегаем по сценария в нашем списке
                    for (int i = 0; i < base.Count; i++)
                    {
                        // Получаем элемент сценаария
                        ConfigurationBase Cnf = base.getConfigurationComponent(i);

                        // Если в контексте сценария что-то есть
                        if (item.ScnDataList != null && Cnf.UScn.ValidLicData() >= tekdtint)
                        {
                            // Получаем доступ к контексту сценария внутри нашего клиента
                            ScenariyDataBase ScnDB = item.ScnDataList[Cnf.UScn.ScenariyName];

                            if (ScnDB != null)
                            {

                                // Получаем рассчитанную скидку сценарием
                                NewCalcMaxDiscPerc = ScnDB.CalcMaxDiscPerc;

                                // Получаем рассчитанный бонус
                                NewCalcStoreCredit = ScnDB.CalcStoreCredit;

                                // Получаем процент по которому расситывался бонус
                                NewCalcScPerc = ScnDB.CalcScPerc;

                                // Если это первый сценарий в списке то нужно его применить без разбора по строкам что делать
                                if (i == 0)
                                {
                                    RezCalcMaxDiscPerc = NewCalcMaxDiscPerc;
                                    OldCalcMaxDiscPerc = NewCalcMaxDiscPerc;
                                    RezCalcStoreCredit = NewCalcStoreCredit;
                                    OldCalcStoreCredit = NewCalcStoreCredit;
                                    RezCalcScPerc = NewCalcScPerc;
                                    OldCalcScPerc = NewCalcScPerc;
                                }
                                else
                                {
                                    // Обработка по теме скидок
                                    // Если есть какое-то значение у пердыдущего сцерария и у текущего
                                    if (OldCalcMaxDiscPerc != null && NewCalcMaxDiscPerc != null)
                                    {
                                        switch (base.getConfigurationComponent(i).SaleActionOut)
                                        {
                                            // При выборе последнего, просто применяем его без разговоров
                                            case CongigurationActionSaleEn.Last:
                                                RezCalcMaxDiscPerc = NewCalcMaxDiscPerc;
                                                OldCalcMaxDiscPerc = NewCalcMaxDiscPerc;
                                                break;
                                            case CongigurationActionSaleEn.Max:
                                                if (OldCalcMaxDiscPerc > NewCalcMaxDiscPerc) RezCalcMaxDiscPerc = OldCalcMaxDiscPerc;
                                                else RezCalcMaxDiscPerc = NewCalcMaxDiscPerc;
                                                OldCalcMaxDiscPerc = RezCalcMaxDiscPerc;
                                                break;
                                            case CongigurationActionSaleEn.Min:
                                                if (OldCalcMaxDiscPerc > NewCalcMaxDiscPerc) RezCalcMaxDiscPerc = NewCalcMaxDiscPerc;
                                                else RezCalcMaxDiscPerc = OldCalcMaxDiscPerc;
                                                OldCalcMaxDiscPerc = RezCalcMaxDiscPerc;
                                                break;
                                            case CongigurationActionSaleEn.Avg:
                                                RezCalcMaxDiscPerc = (OldCalcMaxDiscPerc + NewCalcMaxDiscPerc) / 2;
                                                OldCalcMaxDiscPerc = RezCalcMaxDiscPerc;
                                                break;
                                            // Если выбрано применять первое, то ничего не делаем просто пропускаем, работает первы сценарий
                                            case CongigurationActionSaleEn.First:
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        // Если предыдущий сценарий (по логике OldCalcMaxDiscPerc и RezCalcMaxDiscPerc должны быть одинаковы) вернул null то просто применяем новую скидку
                                        if (RezCalcMaxDiscPerc == null && NewCalcMaxDiscPerc!=null)
                                        {
                                            RezCalcMaxDiscPerc = NewCalcMaxDiscPerc;
                                            OldCalcMaxDiscPerc = NewCalcMaxDiscPerc;
                                        }
                                        // Если текущий результат = null то по логике делать ничего не нужно.
                                    }

                                    // Обработка по теме бонусоы
                                    // Если есть какое-то значение у пердыдущего сцерария и у текущего
                                    if (OldCalcStoreCredit != null && NewCalcStoreCredit != null && NewCalcScPerc != null)
                                    {
                                        switch (base.getConfigurationComponent(i).SaleActionOut)
                                        {
                                            // При выборе последнего, просто применяем его без разговоров
                                            case CongigurationActionSaleEn.Last:
                                                RezCalcStoreCredit = NewCalcStoreCredit;
                                                OldCalcStoreCredit = NewCalcStoreCredit;
                                                RezCalcScPerc = NewCalcScPerc;
                                                OldCalcScPerc = NewCalcScPerc;
                                                break;
                                            case CongigurationActionSaleEn.Max:
                                                if (OldCalcStoreCredit > NewCalcStoreCredit) RezCalcStoreCredit = OldCalcStoreCredit;
                                                else RezCalcStoreCredit = NewCalcStoreCredit;
                                                OldCalcStoreCredit = RezCalcStoreCredit;

                                                if (OldCalcScPerc > NewCalcScPerc) RezCalcScPerc = OldCalcScPerc;
                                                else RezCalcScPerc = NewCalcScPerc;
                                                OldCalcScPerc = RezCalcScPerc;
                                                break;
                                            case CongigurationActionSaleEn.Min:
                                                if (OldCalcStoreCredit > NewCalcStoreCredit) RezCalcStoreCredit = NewCalcStoreCredit;
                                                else RezCalcStoreCredit = OldCalcStoreCredit;
                                                OldCalcStoreCredit = RezCalcStoreCredit;

                                                if (OldCalcScPerc > NewCalcScPerc) RezCalcScPerc = NewCalcScPerc;
                                                else RezCalcScPerc = OldCalcScPerc;
                                                OldCalcScPerc = RezCalcScPerc;
                                                break;
                                            case CongigurationActionSaleEn.Avg:
                                                RezCalcStoreCredit = (OldCalcStoreCredit + NewCalcStoreCredit) / 2;
                                                OldCalcStoreCredit = RezCalcStoreCredit;

                                                RezCalcScPerc = (OldCalcScPerc + NewCalcScPerc) / 2;
                                                OldCalcScPerc = RezCalcScPerc;
                                                break;
                                            // Если выбрано применять первое, то ничего не делаем просто пропускаем, работает первы сценарий
                                            case CongigurationActionSaleEn.First:
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        // Если предыдущий сценарий (по логике OldCalcMaxDiscPerc и RezCalcMaxDiscPerc должны быть одинаковы) вернул null то просто применяем новую скидку
                                        if (RezCalcStoreCredit == null && NewCalcStoreCredit != null)
                                        {
                                            RezCalcStoreCredit = NewCalcStoreCredit;
                                            OldCalcStoreCredit = NewCalcStoreCredit;
                                        }
                                        // Если предыдущий сценарий (по логике OldCalcScPerc и RezCalcScPerc должны быть одинаковы) вернул null то просто применяем новую скидку
                                        if (RezCalcScPerc == null && NewCalcScPerc != null)
                                        {
                                            RezCalcScPerc = NewCalcScPerc;
                                            OldCalcScPerc = NewCalcScPerc;
                                        }
                                        // Если текущий результат = null то по логике делать ничего не нужно.
                                    }

                                }
                            }
                        }
                    }
                    // Всё мы пробежалист по всем сценария можно установить скидку для клинета
                    AccCuct.SetupCalkMaxDiscPerc(item, RezCalcMaxDiscPerc);

                    // Если изменений в бонусной программе нет, то нужно сохранить старые данные
                    if (RezCalcStoreCredit == null && RezCalcScPerc == null) AccCuct.SetupCalkStoreCredit(item, item.StoreCredit, item.ScPerc);
                    else AccCuct.SetupCalkStoreCredit(item, RezCalcStoreCredit, RezCalcScPerc);
                }
                // Убиваем ссылки
                AccCuct = null;

                Com.Log.EventSave(string.Format("Завершён подсчёта итоговой скидки относительно правил применения сценариев по всем сценариям конфигурации: '{0}'", this.ConfigurationName), this.GetType().Name + ".ProcessCalcTotalMaxDiscPercRun", EventEn.Message);
            }
            catch (Exception ex)
            {
                RezEx = new ApplicationException(ex.Message);
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessCalcTotalMaxDiscPerc", EventEn.Error);
            }
            finally
            {
                EventConfigurationListAsicRez myArg = new EventConfigurationListAsicRez(this, RezEx, null);

                // Обрабатываем события окончания процесса просмотра чеков сценариями
                if (onProcessedCalculateTotal != null) onProcessedCalculateTotal.Invoke(this, myArg);

                // Убиваем ссылки
                myArg = null;
            }
        }

        /// <summary>
        /// Добавление нового элемента конфигурации
        /// </summary>
        /// <param name="newCnf">Элемент конфигурации которого нужно добавить в список</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="HashExeption">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(Configuration newCnf, bool HashExeption, bool WriteLog)
        {
            EventConfiguration MyArg = new EventConfiguration(newCnf);
            if (onConfigurationListAddingConfiguration != null) onConfigurationListAddingConfiguration.Invoke(this, MyArg);

            // Если мы одобрили добавление
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (this.MyObj)
                    {
                        rez = base.Add(newCnf, HashExeption);
                    }

                    // Если добавление пользователя прошло успешно.
                    if (rez)
                    {
                        if (onConfigurationListAddedConfiguration != null) onConfigurationListAddedConfiguration.Invoke(this, MyArg.Cfg);
                        if (WriteLog) Com.Log.EventSave(string.Format("Добавился новый элемент конфигурации: {0} ({1})", newCnf.UScn.ScenariyName, base.ConfigurationName), GetType().Name + ".Add", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при добавлении нового элемента конфигурации: {0} ({1})", newCnf.UScn.ScenariyName, base.ConfigurationName), GetType().Name + ".Add", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при добавлении нового элемента конфигурации: {0} ({1})\r\n({2})", newCnf.UScn.ScenariyName, base.ConfigurationName, ex.Message), GetType().Name + ".Add", EventEn.Error);
                    throw;
                }
                MyArg = null;
                return rez;
            }
            return false;
        }
        /// <summary>
        /// Добавление нового элемента конфигурации
        /// </summary>
        /// <param name="newCnf">Элемент конфигурации которого нужно добавить в список</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(Configuration newCnf, bool HashExeption)
        {
            return Add(newCnf, true, true);
        }
        /// <summary>
        /// Добавление нового элемента конфигурации
        /// </summary>
        /// <param name="newCnf">Элемент конфигурации которого нужно добавить в список</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Add(Configuration newCnf)
        {
            return Add(newCnf, true);
        }

        /// <summary>
        /// Удаление элемента конфигурации
        /// </summary>
        /// <param name="delCnf">Элемент конфигурации который нужно удалить из списка</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="WriteLog">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(Configuration delCnf, bool HashExeption, bool WriteLog)
        {
            EventConfiguration MyArg = new EventConfiguration(delCnf);
            if (onConfigurationListDeletingConfiguration != null) onConfigurationListDeletingConfiguration.Invoke(this, MyArg);

            // Если мы одобрили удаление
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (this.MyObj)
                    {
                        rez = base.Remove(delCnf, HashExeption);
                    }

                    // Если удаление пользователя прошло успешно.
                    if (rez)
                    {
                        if (onConfigurationListDeletedConfiguration != null) onConfigurationListDeletedConfiguration.Invoke(this, MyArg.Cfg);
                        if (WriteLog) Com.Log.EventSave(string.Format("Удалили элемент конфигурации: {0} ({1})", delCnf.UScn.ScenariyName, base.ConfigurationName), GetType().Name + ".Remove", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при удалении элемента конфигурации: {0} ({1})", delCnf.UScn.ScenariyName, base.ConfigurationName), GetType().Name + ".Remove", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при удалении элемента конфигурации: {0} ({1})\r\n({2})", delCnf.UScn.ScenariyName, base.ConfigurationName, ex.Message), GetType().Name + ".Remove", EventEn.Error);
                    throw;
                }
                // Убиваем ссылки
                MyArg = null;

                return rez;
            }
            return false;
        }
        /// <summary>
        /// Удаление элемента конфигурации
        /// </summary>
        /// <param name="delCnf">Элемент конфигурации который нужно удалить из списка</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(Configuration delCnf, bool HashExeption)
        {
            return Remove(delCnf, true, true);
        }
        /// <summary>
        /// Удаление элемента конфигурации
        /// </summary>
        /// <param name="delCnf">Элемент конфигурации который нужно удалить из списка</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Remove(Configuration delCnf)
        {
            return Remove(delCnf, true);
        }

        /// <summary>
        /// Обновление элемента конфигурации
        /// </summary>
        /// <param name="IndexId">Индекс элемента который нужно обновить</param>
        /// <param name="updCnf">Новый элемент конфигурации который нужно встроить на место указанное в индексе</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <param name="WriteLog">Записывать сообщения в лог или нет</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(int IndexId, Configuration updCnf, bool HashExeption, bool WriteLog)
        {
            EventConfiguration MyArg = new EventConfiguration(updCnf);
            if (onConfigurationListUpdatingConfiguration != null) onConfigurationListUpdatingConfiguration.Invoke(this, MyArg);

            // Если мы одобрили обновлени
            if (MyArg.Action)
            {
                bool rez = false;
                try
                {
                    lock (this.MyObj)
                    {
                        rez = base.Update(IndexId, updCnf, HashExeption);
                    }

                    // Если обновление данных пользователя прошло успешно.
                    if (rez)
                    {
                        if (onConfigurationListUpdatedConfiguration != null) onConfigurationListUpdatedConfiguration.Invoke(this, MyArg.Cfg);
                        if (WriteLog) Com.Log.EventSave(string.Format("Обновление элемента конфигурации: {0} по индексу {1} ({2})", updCnf.UScn.ScenariyName, IndexId.ToString(), base.ConfigurationName), GetType().Name + ".Update", EventEn.Message);
                    }
                    else if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при обновлении элемента конфигурации: {0} по индексу {1} ({2})", updCnf.UScn.ScenariyName, IndexId.ToString(), base.ConfigurationName), GetType().Name + ".Update", EventEn.Error);
                }
                catch (Exception ex)
                {
                    if (WriteLog) Com.Log.EventSave(string.Format("Произошла ошибка при обновлении элемента конфигурации: {0} по индексу {1} ({2})\r\n({3})", updCnf.UScn.ScenariyName, IndexId.ToString(), base.ConfigurationName, ex.Message), GetType().Name + ".Update", EventEn.Error);
                    throw;
                }
                // Убиваем ссылки
                MyArg = null;

                return rez;
            }
            return false;
        }
        /// <summary>
        /// Обновление элемента конфигурации
        /// </summary>
        /// <param name="IndexId">Индекс элемента который нужно обновить</param>
        /// <param name="updCnf">Новый элемент конфигурации который нужно встроить на место указанное в индексе</param>
        /// <param name="HashExeption">C отображением исключений</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(int IndexId, Configuration updCnf, bool HashExeption)
        {
            return Update(IndexId, updCnf, true, true);
        }
        /// <summary>
        /// Обновление элемента конфигурации
        /// </summary>
        /// <param name="IndexId">Индекс элемента который нужно обновить</param>
        /// <param name="updCnf">Новый элемент конфигурации который нужно встроить на место указанное в индексе</param>
        /// <returns>Результат операции (Успех или нет)</returns>
        public bool Update(int IndexId, Configuration updCnf)
        {
            return Update(IndexId, updCnf, true);
        }
    }
}
