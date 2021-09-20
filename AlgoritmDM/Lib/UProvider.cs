using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using AlgoritmDM.Com.Provider.Lib;
using AlgoritmDM.Com.Data.Lib;
using AlgoritmDM.Com.Data;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Универсальный провайдер
    /// </summary>
    public class UProvider : ProviderBase.UProviderBase, ProviderI
    {
        /// <summary>
        /// Базовый провайдер
        /// </summary>
        private ProviderBase PrvB;

        /// <summary>
        /// Интерфейс провайдера
        /// </summary>
        private ProviderI PrvI;

        /// <summary>
        /// Тип провайдера
        /// </summary>
        public string PrvInType
        {
            get { return (this.PrvB == null ? null : this.PrvB.PlugInType.Name);}
            private set { }
        }

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString 
        {
            get { return this.PrvB.ConnectionString; }
            private set { }
        }

        /// <summary>
        /// Версия источника данных
        /// </summary>
        /// <returns>Возвращет значение версии источника данных в случае возможности получения подключения</returns>
        public string VersionDB
        {
            get { return this.PrvB.VersionDB; }
            private set { }
        }

        /// <summary>
        /// Возвращаем версию драйвера
        /// </summary>
        /// <returns></returns>
        public string Driver
        {
            get { return this.PrvB.Driver; }
            private set { }
        }

        /// <summary>
        /// Доступно ли подключение или нет
        /// </summary>
        /// <returns>true Если смогли подключиться к базе данных</returns>
        public bool HashConnect
        {
            get { return this.PrvB.HashConnect(); }
            private set { }
        }

        /// <summary>
        /// Печать строки подключения с маскировкой секретных данных
        /// </summary>
        /// <returns>Строка подклюения с замасированной секретной информацией</returns>
        public string PrintConnectionString()
        {
            try
            {
                return this.PrvI.PrintConnectionString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }   
        }

        /// <summary>
        /// Получаем элемент меню для получения информации по плагину
        /// </summary>
        public ToolStripMenuItem InfoToolStripMenuItem()
        {
            return (this.PrvB == null ? null : this.PrvB.InfoToolStripMenuItem); 
        }

        /// <summary>
        /// Конструктор по созданию универсального плагина
        /// </summary>
        /// <param name="PrvFullName">Имя плагина с которым хотим работать</param>
        /// <param name="ConnectionString">Строка подключения</param>
        public UProvider(string PrvFullName, string ConnectionString)
        {
            if (PrvFullName == null || PrvFullName.Trim() == string.Empty) throw new ApplicationException(string.Format("Не можем создать провайдер указанного типа: ({0})", PrvFullName == null ? "" : PrvFullName.Trim()));

            // Получаем инфу о класса 1 параметр полный путь например "EducationAnyProvider.Provider.MSSQL.MsSqlProvider", 2 параметр пропускать или не пропускать ошибки сейчас пропускаем, а третий учитывать или нет регистр из первого параметра
            //, если первый параметр нужно взять из другой зборки то сначала её загружаем Assembly asm = Assembly.LoadFrom("MyApp.exe"); а потом тоже самое только первый параметр кажется будет так "Reminder.Common.PLUGIN.MonitoringSetNedost, РЕШЕНИЕ" 
            Type myType = Type.GetType("AlgoritmDM.Com.Provider." + PrvFullName.Trim(), false, true);

            // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
            bool flagI = false;
            bool flagTI = false;
            foreach (Type i in myType.GetInterfaces())
            {
                if (i.FullName == "AlgoritmDM.Com.Provider.Lib.ProviderI") flagI = true;
                if (i.FullName == "AlgoritmDM.Com.Provider.Lib.ProviderTransferI") flagTI = true;
            }
            if (!flagI) throw new ApplicationException("Класс который вы грузите не реализовывает интерфейс (ProviderI)");
            if (!flagTI) throw new ApplicationException("Класс который вы грузите не реализовывает интерфейс (ProviderTransferI)");

            // Проверяем что наш клас наследует PlugInBase 
            bool flagB = false;
            foreach (MemberInfo mi in myType.GetMembers())
            {
                if (mi.DeclaringType.FullName == "AlgoritmDM.Com.Provider.Lib.ProviderBase") flagB = true;
            }
            if (!flagB) throw new ApplicationException("Класс который вы грузите не наследует от класса ProviderBase");


            // Проверяем конструктор нашего класса  
            bool flag = false;
            string nameConstructor;
            foreach (ConstructorInfo ctor in myType.GetConstructors())
            {
                nameConstructor = myType.Name;

                // получаем параметры конструктора  
                ParameterInfo[] parameters = ctor.GetParameters();

                // если в этом конструктаре 1 параметр то проверяем тип и имя параметра  
                if (parameters.Length == 1)
                {

                    if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ConnectionString") flag = true;

                }
            }
            if (!flag) throw new ApplicationException("Класс который вы грузите не имеет конструктора (string ConnectionString)");
            
            // Создаём экземпляр объекта  
            object[] targ = { ConnectionString };
            object obj = Activator.CreateInstance(myType, targ);
            this.PrvB = (ProviderBase)obj;
            this.PrvI = (ProviderI)obj;

            base.UPoviderSetupForProviderBase(this.PrvB, this);
        }
        public UProvider(string PrvFullName)
            : this(PrvFullName, null)
        { }


        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        public void EventSave(string Message, string Source, EventEn evn)
        {
            this.PrvB.EventSave(Message, Source, evn);
        }

        /// <summary>
        /// Получаем список доступных провайдеров
        /// </summary>
        /// <returns>Список имён доступных провайдеров</returns>
        public static List<string> ListProviderName()
        {
            List<string> ProviderName = new List<string>();

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "AlgoritmDM.Com.Provider").ToArray();


            foreach (Type item in typelist)
            {
                // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
                bool flagI = false;
                bool flagTI = false;
                foreach (Type i in item.GetInterfaces())
                {
                    if (i.FullName == "AlgoritmDM.Com.Provider.Lib.ProviderI") flagI = true;
                    if (i.FullName == "AlgoritmDM.Com.Provider.Lib.ProviderTransferI") flagTI = true;
                }
                if (!flagI) continue;
                if (!flagTI) continue;

                // Проверяем что наш клас наследует PlugInBase 
                bool flagB = false;
                foreach (MemberInfo mi in item.GetMembers())
                {
                    if (mi.DeclaringType.FullName == "AlgoritmDM.Com.Provider.Lib.ProviderBase") flagB = true;
                }
                if (!flagB) continue;


                // Проверяем конструктор нашего класса  
                bool flag = false;
                string nameConstructor;
                foreach (ConstructorInfo ctor in item.GetConstructors())
                {
                    nameConstructor = item.Name;

                    // получаем параметры конструктора  
                    ParameterInfo[] parameters = ctor.GetParameters();

                    // если в этом конструктаре 1 параметр то проверяем тип и имя параметра  
                    if (parameters.Length == 1)
                    {

                        if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ConnectionString") flag = true;

                    }
                }
                if (!flag) continue;

                ProviderName.Add(item.Name);
            }

            return ProviderName;
        }

        /// <summary>
        /// Процедура вызывающая настройку подключения
        /// </summary>
        /// <param name="Uprv">Ссылка на универсальный провайдер</param>
        /// <returns>Возвращает значение требуется ли сохранить подключение как основное или нет</returns>
        public bool SetupConnectDB()
        {
            return this.PrvI.SetupConnectDB();
        }

        /// <summary>
        /// Пользователь хочет посмотреть детали по какому-то клиенту
        /// </summary>
        /// <param name="CstB">Клиент по которому мы хотим получить данные</param>
        /// <returns>Возвращает объект, через который мы сможем потом получить данные по клиенту</returns>
        public CustDetailCheck GetDetailCheckForCustumer(CustomerBase CstB)
        {
            if (CstB == null) throw new ApplicationException("Нельзя получать данные по всем клинетам таким стпособом, так как будут выкачены все чеки из базы в ОЗУ.");
            return new CustDetailCheck(this, CstB);
        }


        /// <summary>
        /// Получение любых данных из источника например чтобы плагины могли что-то дополнительно читать
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        /// <returns>Результата В виде таблицы</returns>
        public DataTable getData(string SQL)
        {
            return this.PrvB.getData(SQL);
        }

        /// <summary>
        /// Выполнение любых запросов на источнике
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        public void setData(string SQL)
        {
            this.PrvB.setData(SQL);
        }

        /// <summary>
        /// Объединение клиентов
        /// </summary>
        /// <param name="MergeClientMain">Основной клиент</param>
        /// <param name="MergeClientDonors">Клинеты доноры</param>
        public void MergeClient(Customer MergeClientMain, List<Customer> MergeClientDonors)
        {
            this.PrvI.MergeClient(MergeClientMain, MergeClientDonors);
        }

        /// <summary>
        /// Класс для выкачивания или закачивания денных из провайдера
        /// </summary>
        public sealed class Transfer
        {
            private UProvider Uprv;
            private ConfigurationList CnfL;
            private int NextScenary;
            private CustomerList CustL;
            private DiscReasonList DscReasL;

            /// <summary>
            /// Получаем интерфейс для работы с данными
            /// </summary>
            public ProviderTransferI PrvTI { get { return (ProviderTransferI)this.Uprv.PrvB; } private set { } }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="Uprv">Универсальный провайдер с которым мы потом будем работать</param>
            /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
            /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
            public Transfer(UProvider Uprv, ConfigurationList CnfL, int NextScenary)
            {
                this.Uprv = Uprv;
                this.CnfL = CnfL;
                this.NextScenary = NextScenary;
            }
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="Uprv">Универсальный провайдер с которым мы потом будем работать</param>
            /// <param name="CustL">Ссылка на список клиентов</param>
            public Transfer(UProvider Uprv, CustomerList CustL)
                : this(Uprv, null, -1)
            {
                this.CustL = CustL;
            }
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="Uprv">Универсальный провайдер с которым мы потом будем работать</param>
            /// <param name="DscReasL">Список, причин скидок</param>
            public Transfer(UProvider Uprv, DiscReasonList DscReasL)
                : this(Uprv, null, -1)
            {
                this.DscReasL = DscReasL;
            }
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="Uprv">Универсальный провайдер с которым мы потом будем работать</param>
            /// <param name="CustL">Ссылка на список причин скидок</param>
            public Transfer(UProvider Uprv)
                : this(Uprv, null, -1)
            {
                this.DscReasL = null;
            }
        }

        /// <summary>
        /// Класс для выкачивания детелей по чекам по конкретному клиенту
        /// </summary>
        public sealed class CustDetailCheck : IDisposable
        {
            /// <summary>
            /// Ссылка на провайдер которым будем качать данные
            /// </summary>
            private UProvider UPrv;

            /// <summary>
            /// Ссылка на клиента по которому нам интересна информация
            /// </summary>
            public CustomerBase CstB { get; private set; }

            /// <summary>
            /// Таблица с результатом запроса
            /// </summary>
            public DataTable dtRez;

            /// <summary>
            /// Последнее время изменения статуса в прогресс баре
            /// </summary>
            private DateTime LastEventProcessedProgressBar = new DateTime();

            /// <summary>
            /// Ссылка на асинхронный процесс   
            /// </summary>
            private Thread thr;

            /// <summary>
            /// Событие возникновения начала просмотра деталей чеков по выбранному клиенту
            /// </summary>
            public event EventHandler<EventCustDetailCheck> onProcessing;
            /// <summary>
            /// Событие окончания просмотра деталей чеков по выбранному клиенту
            /// </summary>
            public event EventHandler<EventCustDetailCheckAsicRez> onProcessed;
            /// <summary>
            /// Событие работы прогресс бара при просмотре деталей чеков по выбранному клиенту
            /// </summary>
            public event EventHandler<EventCustDetailCheckProgerssBar> onProgressBar;

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="UPrv">Ссылка на провайдер которым будем качать данные</param>
            /// <param name="CstB">Ссылка на клиента по которому нам интересна информация</param>
            public CustDetailCheck(UProvider UPrv, CustomerBase CstB)
            {
                this.UPrv = UPrv;
                this.CstB = CstB;
                // Создаём таблицу для отрисовки чеков
                if (this.dtRez == null)
                {
                    this.dtRez = new DataTable("dtRez");
                    this.dtRez.Columns.Add(new DataColumn("InvcType", Type.GetType("System.Int32")));
                    this.dtRez.Columns.Add(new DataColumn("InvcNo", Type.GetType("System.Int32")));
                    this.dtRez.Columns.Add(new DataColumn("CreatedDate", Type.GetType("System.DateTime")));
                    this.dtRez.Columns.Add(new DataColumn("Alu", Type.GetType("System.String")));
                    this.dtRez.Columns.Add(new DataColumn("Description1", Type.GetType("System.String")));
                    this.dtRez.Columns.Add(new DataColumn("Description2", Type.GetType("System.String")));
                    this.dtRez.Columns.Add(new DataColumn("Siz", Type.GetType("System.String")));
                    this.dtRez.Columns.Add(new DataColumn("Qty", Type.GetType("System.Decimal")));
                    this.dtRez.Columns.Add(new DataColumn("CustSid", Type.GetType("System.Int64")));
                    this.dtRez.Columns.Add(new DataColumn("StoreNo", Type.GetType("System.Int32")));
                    this.dtRez.Columns.Add(new DataColumn("DiscReasonId", Type.GetType("System.Int32")));
                    this.dtRez.Columns.Add(new DataColumn("ItemSid", Type.GetType("System.Int64")));
                    this.dtRez.Columns.Add(new DataColumn("OrigPrice", Type.GetType("System.Decimal")));
                    this.dtRez.Columns.Add(new DataColumn("Price", Type.GetType("System.Decimal")));
                    this.dtRez.Columns.Add(new DataColumn("UsrDiscPerc", Type.GetType("System.Decimal")));
                }
            }

            /// <summary>
            /// Запуск процесса выкачивания данных по завершении процесса не забудьте отписаться от событий, чтобы небыло ссылок и сборщик уничтожил объект
            /// </summary>
            public void Run()
            {
                try
                {
                    this.LastEventProcessedProgressBar = DateTime.Now;
                    Com.Log.EventSave(string.Format("Запуск процесса просмотра деталей чеков по клиенту: '{0}'", this.CstB.FirstName), this.GetType().Name + ".Run", EventEn.Message);

                    EventCustDetailCheck myArg = new EventCustDetailCheck(this);

                    // Обрабатываем события начала процесса просмотра чеков сценариями
                    if (onProcessing != null) onProcessing.Invoke(this, myArg);

                    // Асинхронный запуск процесса
                    this.thr = new Thread(AsRun);
                    //this.thr = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                    this.thr.Name = "AE_Thr_ProcessingCustDetailCheck";
                    this.thr.IsBackground = true;
                    this.thr.Start();

                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(ex.Message, this.GetType().Name + ".Run", EventEn.Error, true, true);
                    throw;
                }
            }

            /// <summary>
            /// Запуск процесса выкачивания деталей по клиенту
            /// </summary>
            private void AsRun()
            {
                ApplicationException RezEx = null;
                try
                {
                    bool rez = false;

                    // Создаём объект для доступа к интерфейсу ProviderTransferI
                    Transfer TrfSource = new Transfer(UPrv, null, -1);
                    Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget = TransferDetailCheckForCustumer;
                    rez = TrfSource.PrvTI.getCheck(FuncTarget, null, -1, null, CstB.CustSid);

                    // Убиваем ссылки
                    TrfSource = null;
                    FuncTarget = null;

                    Com.Log.EventSave(string.Format("Завершён процесс просмотра чеков по клиенту: '{0}'", CstB.FirstName), this.GetType().Name + ".AsRun", EventEn.Message);
                }
                catch (Exception ex)
                {
                    RezEx = new ApplicationException(ex.Message);
                    Com.Log.EventSave(ex.Message, this.GetType().Name + ".AsRun", EventEn.Error);
                }
                finally
                {
                    EventCustDetailCheckAsicRez myArg = new EventCustDetailCheckAsicRez(this, RezEx);

                    // Обрабатываем события окончания процесса просмотра чеков сценариями
                    if (onProcessed != null) onProcessed.Invoke(this, myArg);
                }
            }

            /// <summary>
            /// Получает результат от провайдера для передачи его дальше следующему по номеру сценарию
            /// </summary>
            /// <param name="ChkRow">Строка которую получили от источника</param>
            /// <param name="CnfL">Лист со сценариями</param>
            /// <param name="NextScenary">Следующий элемент который должен обработать эту стрроку полученную от провайдера</param>
            /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
            /// <returns>возвращаем успех если всё ОК</returns>
            private bool TransferDetailCheckForCustumer(Com.Data.Check ChkRow, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate)
            {
                try
                {
                    bool rez = true;

                    // По логике сюжа всегда будет приходить null
                    if (CnfL==null)
                    {
                        // Получаем первую дату чека для нашего прогресс бара
                        if (FirstDate == null) FirstDate = ChkRow.CreatedDate;

                        // Если строка пришла от провайдера и последнее событие было более 1 секунды назад и есть подписанные события, пробуем посчитать прогресс бар
                        if (this.LastEventProcessedProgressBar.AddSeconds(1) < DateTime.Now && onProgressBar != null)
                        {
                            // Для того чтобы прогресс бар не ронял никаких процессов
                            try
                            {
                                // Подсчитываем прогресс
                                int AllStep = ((TimeSpan)(DateTime.Now - FirstDate)).Days;
                                int CurStep = AllStep - ((TimeSpan)(DateTime.Now - ChkRow.CreatedDate)).Days;
                                EventCustDetailCheckProgerssBar MaArg = new EventCustDetailCheckProgerssBar(this, AllStep, CurStep);

                                // Если подписаны на событие то вызываем его
                                if (onProgressBar != null) onProgressBar.Invoke(this, MaArg);

                                // Устанавливаем текущее время
                                this.LastEventProcessedProgressBar = DateTime.Now;
                            }
                            catch (Exception) { }
                        }

                        // Тут заливаем строки в таблицу которую потом будет смотреть пользователь
                        DataRow nrow = this.dtRez.NewRow();
                        nrow["InvcType"] = ChkRow.InvcType;
                        nrow["InvcNo"] = ChkRow.InvcNo;
                        nrow["CreatedDate"] = ChkRow.CreatedDate;
                        nrow["Alu"] = ChkRow.Alu;
                        nrow["Description1"] = ChkRow.Description1;
                        nrow["Description2"] = ChkRow.Description2;
                        nrow["Siz"] = ChkRow.Siz;
                        nrow["Qty"] = ChkRow.Qty;
                        if(ChkRow.CustSid != null) nrow["CustSid"] = ChkRow.CustSid;
                        nrow["StoreNo"] = ChkRow.StoreNo;
                        nrow["DiscReasonId"] = ChkRow.DiscReasonId;
                        nrow["ItemSid"] = ChkRow.ItemSid;
                        nrow["OrigPrice"] = ChkRow.OrigPrice;
                        nrow["Price"] = ChkRow.Price;
                        nrow["UsrDiscPerc"] = ChkRow.UsrDiscPerc;
                        this.dtRez.Rows.Add(nrow);

                        
                    }
                    return rez;
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(ex.Message, this.GetType().Name + ".TransferDetailCheckForCustumer", EventEn.Error, true, true);
                    return false;
                }
            }


            /// <summary>
            /// Освобождение ресурсов. Вызывайте когда объект будет более не нужен
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            ~CustDetailCheck() 
            {
                this.Dispose(false);
            }
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (dtRez != null)
                    {
                        this.dtRez.Dispose();
                        this.dtRez = null;
                    }
                }
            }
        }
    }
}
