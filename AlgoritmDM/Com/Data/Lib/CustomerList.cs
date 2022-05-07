using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Collections;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com.Data.Lib
{
    /// <summary>
    /// Список клиентов в системе, тут можно организовать обработку событий
    /// </summary>
    public sealed class CustomerList: CustomerBase.CustomerBaseList
    {
        private static CustomerList MyObj;

        /// <summary>
        /// Последнее возникновение события применения скидки
        /// </summary>
        private DateTime LastEventProcessedAployDMCalkMaxDiscPercProgressBar = new DateTime();

        /// <summary>
        /// Ссылка на асинхронный процесс   
        /// </summary>
        private Thread thr;

        /// <summary>
        /// Последнее возникновение события изменения статуса прогресс бара при процессинге просмотра чеков нашими сценариями
        /// </summary>
        private DateTime LastEventProcessedCalculateDMProgressBar = new DateTime();

        /// <summary>
        /// Событие возникновения начала получения списка клиентов
        /// </summary>
        public event EventHandler<EventCustomerList> onProcessingCustomerList;
        /// <summary>
        /// Событие окончания получения списка клиентов
        /// </summary>
        public event EventHandler<EventCustomerListAsicRez> onProcessedCustomerList;
        /// <summary>
        /// Событие возникновения процесса применения скидок
        /// </summary>
        public event EventHandler<EventCustomerList> onProcessingAployDMCalkMaxDiscPerc;
        /// <summary>
        /// Событие окончания процесса применения скидок
        /// </summary>
        public event EventHandler<EventCustomerListAsicRez> onProcessedAployDMCalkMaxDiscPerc;
        /// <summary>
        /// Событие работы прогресс бара при применении скидок
        /// </summary>
        public event EventHandler<EventCustomerListProcessingProgerssBar> onProcessedAployDMCalkMaxDiscPercProgressBar;

        /// <summary>
        /// Вытаскивает клиента по его индексу в эом контернере
        /// </summary>
        /// <param name="index">Index клиента</param>
        /// <returns>Клиент</returns>
        public Customer this[int index]
        {
            get 
            {
                Customer rez = null;
                lock (MyObj)
                {
                    rez=((Customer)base.getCustComponent(index));
                }
                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает клиента по его сиду в эом контернере
        /// </summary>
        /// <param name="sid">Index клиента</param>
        /// <returns>Клиент</returns>
        public Customer this[long? sid]
        {
            get
            {
                Customer rez = null;
                lock (MyObj)
                {
                    if (sid != null)
                    {
                        for (int i = 0; i < base.Count; i++)
                        {
                            if (base.getCustComponent(i).CustSid == ((long)sid))
                            {
                                rez = ((Customer)base.getCustComponent(i));
                                break;
                            }
                        }
                    }
                }
                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает клиента по его сиду в эом контернере
        /// </summary>
        /// <param name="sid">Index клиента</param>
        /// <returns>Клиент</returns>
        public Customer this[long sid]
        {
            get
            {
                Customer rez = null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (base.getCustComponent(i).CustSid == sid)
                        {
                            rez = ((Customer)base.getCustComponent(i));
                            break;
                        }
                    }
                }
                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает клиента по его карточку
        /// </summary>
        /// <param name="CustId">Номер карточки</param>
        /// <returns>Универсальный провайдер</returns>
        public Customer GetCustomer(string CustId)
        {
            try
            {
                Customer rez = null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (((Customer)base.getCustComponent(i)).CustId == CustId)
                        {
                            rez = ((Customer)base.getCustComponent(i));
                            break;
                        }
                    }
                }
                if (rez != null) return rez;
                throw new ApplicationException(string.Format("Клиента с карточкой {0} в системе не существует.", CustId));
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Вытаскивает клиента по его сиду
        /// </summary>
        /// <param name="CustSid">Сид клинета</param>
        /// <returns>Универсальный провайдер</returns>
        public Customer GetCustomer(long CustSid)
        {
            try
            {
                Customer rez = null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (((Customer)base.getCustComponent(i)).CustSid == CustSid)
                        {
                            rez = ((Customer)base.getCustComponent(i));
                            break;
                        }
                    }
                }
                if (rez != null) return rez;
                throw new ApplicationException(string.Format("Клиента с сидом {0} в системе не существует.", CustSid.ToString()));
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Вытаскивает клиента по его сиду
        /// </summary>
        /// <param name="CustSid">Сид клинета</param>
        /// <returns>Универсальный провайдер</returns>
        public Customer GetCustomer(long? CustSid)
        {
            try
            {
                Customer rez = null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (CustSid != null)
                        {
                            if (((Customer)base.getCustComponent(i)).CustSid == ((long)CustSid))
                            {
                                rez = ((Customer)base.getCustComponent(i));
                                break;
                            }
                        }
                    }
                }
                if (rez != null) return rez;
                throw new ApplicationException(string.Format("Клиента с сидом {0} в системе не существует.", CustSid.ToString()));
            }
            catch (Exception) { throw; }
        }
        /// <summary>
        /// Вытаскивает клиента на основе другого
        /// </summary>
        /// <param name="CustSid">Клиент которого пробуем найти</param>
        /// <returns>Универсальный провайдер</returns>
        public Customer GetCustomer(Customer Cust)
        {
            try
            {
                Customer rez = null;
                lock (MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (((Customer)base.getCustComponent(i)).CustSid == Cust.CustSid)
                        {
                            rez = ((Customer)base.getCustComponent(i));
                            break;
                        }
                    }
                }
                if (rez != null) return rez;
                throw new ApplicationException(string.Format("Клиента с сидом {0} в системе не существует.", Cust.CustSid));
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Получение экземпляра нашего синглетон списка клиентов
        /// </summary>
        /// <returns></returns>
        public static CustomerList GetInstatnce()
        {
            if (MyObj == null) MyObj = new CustomerList();
            return MyObj;
        }

        /// <summary>
        /// внутренний конструктор нашего листа
        /// </summary>
        private CustomerList()
        {

        }

        /// <summary>
        /// Запуск процесса получения данных о клиентах из базы
        /// </summary>
        public void ProcessingCustomers()
        {
            try
            {
                lock (MyObj)
                {
                    this.LastEventProcessedCalculateDMProgressBar = DateTime.Now;
                    Com.Log.EventSave(string.Format("Запуск процесса получения данных о клиентах из базы"), this.GetType().Name + ".ProcessingCustomers", EventEn.Message);

                    EventCustomerList myArg = new EventCustomerList(this, null);

                    // Обрабатываем события начала получения клиентов
                    if (onProcessingCustomerList != null) onProcessingCustomerList.Invoke(this, myArg);

                    // Если поьзователь не отменил то запускаем процесс асинхронно
                    if (myArg.Action)
                    {
                        // Асинхронный запуск процесса
                        this.thr = new Thread(ProcessingCustomersRun);
                        //this.thr = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                        this.thr.Name = "AE_Thr_ProcessingCustomers";
                        this.thr.IsBackground = true;
                        this.thr.Start();
                    }
                }

            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessingCustomers", EventEn.Error, true, true);
                throw;
            }
        }
        //
        /// <summary>
        /// Асинхронный запуск нашего процесса
        /// </summary>
        private void ProcessingCustomersRun()
        {
            ApplicationException RezEx = null;
            try
            {
                bool rez = false;
                // Чистим текущий список
                lock (MyObj)
                {
                    base.Clear(true);
                }

                // Обновление данных в списке пользователей
                if (Com.ProviderPrizmFarm.CurProviderPrizm != null) Com.ProviderPrizmFarm.CurProviderPrizm.UpdateCustomerDefaultCallOffSc();

                // Создаём объект для доступа к интерфейсу ProviderTransferI
                UProvider.Transfer TrfSource = new UProvider.Transfer(Com.ProviderFarm.CurrentPrv, this);
                Func<Customer, bool> FuncTarget = TransferCustomers;
                rez = TrfSource.PrvTI.getCustumers(FuncTarget);

                Com.Log.EventSave(string.Format("Завершён процесс получения данных о клиентах из базы с результатом: {0}", rez.ToString()), this.GetType().Name + ".ProcessingCustomersRun", EventEn.Message);


            }
            catch (Exception ex)
            {
                RezEx = new ApplicationException(ex.Message);
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessingCustomersRun", EventEn.Error);
                throw;
            }
            finally
            {
                EventCustomerListAsicRez myArg = new EventCustomerListAsicRez(this, null, RezEx);

                // Обрабатываем события окончания процесса просмотра чеков сценариями
                if (onProcessedCustomerList != null) onProcessedCustomerList.Invoke(this, myArg);
            }
        }
        //
        /// <summary>
        /// Получает результат от провайдера для передачи его дальше следующему по номеру сценарию
        /// </summary>
        /// <param name="newCust">Пользователь которого добавляем к списку</param>
        /// <param name="HashExistsNewCust">Проверяем на наличие дуюдей в нашем списке перед добавлением нового клиента</param>
        /// <returns>возвращаем успех если всё ОК</returns>
        private bool TransferCustomers(Customer newCust)
        {
            bool rez = false;  // По умолчанию не проверяем на дубли, это делается медленно
            bool HashExistsNewCust = false;
            try
            {
                lock (MyObj)
                {
                    Customer tmpCst = null;
                    if (HashExistsNewCust)
                    {
                        try { tmpCst = Com.CustomerFarm.List.GetCustomer(newCust); }
                        catch (Exception) { }
                    }

                    try
                    {
                        if (tmpCst == null) Com.CustomerFarm.List.Add(newCust, true);
                        else Com.CustomerFarm.List.Update(tmpCst.Index, newCust, true);

                        rez=true;
                    }
                    catch (Exception)  // Если упали, пробуем обновить этого клинета, Он при добавлении проверяется на существование и в случае чего выдаст исключение
                    {
                        rez = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".TransferCalculateDM", EventEn.Error, true, true);
            }
            return rez;
        }

        /// <summary>
        /// Запуск процесса сохранения скидок и бонусов по клиентам
        /// </summary>
        /// <param name="Cst">Клиент если null то по всем клиентам</param>
        public void ProcessAployDMCalkScn(CustomerBase Cst)
        {
            try
            {
                EventCustomerList myArg = new EventCustomerList(this, Cst);

                // Обрабатываем события начала процесса подсчёта конечной скидки относительно всех сценариев
                if (onProcessingAployDMCalkMaxDiscPerc != null) onProcessingAployDMCalkMaxDiscPerc.Invoke(this, myArg);

                // Проверяем необходимость запуска процесса
                if (myArg.Action)
                {
                    this.LastEventProcessedAployDMCalkMaxDiscPercProgressBar = DateTime.Now;
                    Com.Log.EventSave(string.Format("Запуск процесса применения скидок и бонусов."), this.GetType().Name + ".ProcessAployDMCalkScn", EventEn.Message);

                    // Асинхронный запуск процесса
                    this.thr = new Thread(new ParameterizedThreadStart(ProcessAployDMCalkScnRun));
                    this.thr.Name = "AE_Thr_ProcessAployDMCalkScn(";
                    this.thr.IsBackground = true;
                    this.thr.Start(Cst);
                }

                // Убиваем ссылки
                myArg = null;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessAployDMCalkScn(", EventEn.Error, true, true);
                throw;
            }
        }

        /// <summary>
        /// Запуск процесса сохранения скидок и бонусов по клиентам
        /// </summary>
        /// <param name="CstObj">Клиент если null то по всем клиентам</param>
        private void ProcessAployDMCalkScnRun(object CstObj)
        {
            // Приводим из object к типиризированному запросу
            CustomerBase Cst = (CustomerBase)CstObj;

            ApplicationException RezEx = null;
            try
            {
                // Пробегаем по списку клиентов
                foreach (Customer item in Com.CustomerFarm.List)
                {
                    // Если мы в тестовом режиме, то делаем задержку, чтобы увидеть красоту прогресс баров
                    if (Com.Config.Mode != ModeEn.Normal) Thread.Sleep(1500);

                    // Если событие было более 1 секунды назад и есть подписанные события, пробуем посчитать прогресс бар
                    if (this.LastEventProcessedAployDMCalkMaxDiscPercProgressBar.AddSeconds(1) < DateTime.Now && this.onProcessedAployDMCalkMaxDiscPercProgressBar != null)
                    {
                        // Для того чтобы прогресс бар не ронял никаких процессов
                        try
                        {
                            // Подсчитываем прогресс
                            EventCustomerListProcessingProgerssBar MaArg = new EventCustomerListProcessingProgerssBar(this, Com.CustomerFarm.List.Count, item.Index + 1);

                            // Если подписаны на событие то вызываем его
                            if (this.onProcessedAployDMCalkMaxDiscPercProgressBar != null) this.onProcessedAployDMCalkMaxDiscPercProgressBar.Invoke(this, MaArg);

                            // Убиваем ссылки
                            MaArg = null;

                            // Устанавливаем текущее время
                            this.LastEventProcessedAployDMCalkMaxDiscPercProgressBar = DateTime.Now;
                        }
                        catch (Exception) { }
                    }

                    // Применяем фильтр чтобы бежать не по всем клиентам
                    if (Cst == null || item.CustSid == Cst.CustSid)
                    {
                        if (item.CustSid == 2417549001015889916)
                        {
                        }

                        // Проверяем на то что подсчитаны данные по неподсчитанным нет смысла гулять
                        if (item.CalkMaxDiscPerc != null || item.CalkStoreCredit != null)
                        {
                            // Создаём объект для доступа к интерфейсу ProviderTransferI
                            UProvider.Transfer TrfSource = new UProvider.Transfer(Com.ProviderFarm.CurrentPrv);

                            // Запускаем процесс сохранения на провайдере
                            bool rezMaxDiscPerc = false;
                            bool rezStoreCredit = false;
                            bool rezScPerc = false;
                            if (item.CalkMaxDiscPerc != null) rezMaxDiscPerc = TrfSource.PrvTI.AployDMCalkMaxDiscPerc(item, (decimal)item.CalkMaxDiscPerc);
                            if (item.CalkStoreCredit != null && item.CalkScPerc != null) { rezStoreCredit = TrfSource.PrvTI.AployDMCalkStoreCredit(item, (decimal)item.CalkStoreCredit, (decimal)item.CalkScPerc, item.StoreCredit, item.ScPerc); rezScPerc = true; }

                            // Устанавливаем подсчитанные данные в интерфейсе
                            if (rezMaxDiscPerc || rezStoreCredit || rezScPerc)
                            {
                                if (rezMaxDiscPerc) base.SetupMaxDiscPerc(item, (decimal)item.CalkMaxDiscPerc);
                                if (rezStoreCredit && rezScPerc) base.SetupStoreCredit(item, (decimal)item.CalkStoreCredit, (decimal)item.CalkScPerc);
                            }
                            else throw new ApplicationException("Провайдер выдал false в резульитате своей работы");
                        }
                    }
                }

                Com.Log.EventSave(string.Format("Завершён процесс применения скидки и бонусов."), this.GetType().Name + ".ProcessAployDMCalkMaxDiscPercRun", EventEn.Message);
            }
            catch (Exception ex)
            {
                RezEx = new ApplicationException(ex.Message);
                Com.Log.EventSave(ex.Message, this.GetType().Name + ".ProcessAployDMCalkMaxDiscPercRun", EventEn.Error);
            }
            finally
            {
                EventCustomerListAsicRez myArg = new EventCustomerListAsicRez(this, Cst, RezEx);

                // Обрабатываем события окончания процесса просмотра чеков сценариями
                if (onProcessedAployDMCalkMaxDiscPerc != null) onProcessedAployDMCalkMaxDiscPerc.Invoke(this, myArg);

                // Убиваем ссылки
                myArg = null;
            }
        }

    }
}
