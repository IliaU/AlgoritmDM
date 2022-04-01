using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.IO;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Класс для работы с конфиг файлом
    /// </summary>
    public partial class Config
    {
        #region Private Param
        private static Config obj = null;

        /// <summary>
        /// Версия XML файла
        /// </summary>
        private static int _Version = 2;

        /// <summary>
        /// Флаг трассировки
        /// </summary>
        private static bool _Trace = false;

        /// <summary>
        /// Лог файл в который мы пишем информацию по пользователям которых считаем навалижными
        /// </summary>
        private static string _LogNotValidCustomer = "CustomNotValid.txt";

        /// <summary>
        /// Режим работы
        /// </summary>
        private static ModeEn _Mode = ModeEn.Normal;

        /// <summary>
        /// Фильтр для коода региона, если он указан, то будут прогружаьбся только те клиенты которые относятся к этому коду региона
        /// </summary>
        private static string _CustomerCountryList;

        /// <summary>
        /// Фильтр по префиксу в телефонах, для того чтобы не всех клиентов грузить а только интересных нам
        /// </summary>
        private static string _CustomerPrefixPhoneList;

        /// <summary>
        /// Сохранение событий в таблицу для отправки их по SMTP
        /// </summary>
        private static bool _SaveSmtpEvent = false;

        /// <summary>
        /// Объект XML файла
        /// </summary>
        private static XmlDocument Document = new XmlDocument();

        /// <summary>
        /// Колренвой элемент нашего документа
        /// </summary>
        private static XmlElement xmlRoot;

        /// <summary>
        /// Корневой элемент лицензий
        /// </summary>
        private static XmlElement xmlLics;

        /// <summary>
        /// Корневой элемент пользователей
        /// </summary>
        private static XmlElement xmlUsers;

        /// <summary>
        /// Корневой элемент всех сценариев
        /// </summary>
        private static XmlElement xmlUScenariyList;

        /// <summary>
        /// Корневой элемент всех конфигураций
        /// </summary>
        private static XmlElement xmlSharedConfigurations;

        /// <summary>
        /// Текущий разделитель
        /// </summary>
        private static string _TekDelitel;

        /// <summary>
        /// Папка в которой лежит наше приложение AlgoritmSMTP
        /// </summary>
        private static string _CurAlgiritmSmtpDir = Environment.CurrentDirectory;

        /// <summary>
        /// Текст который отобразить пользователю при отображении менюшки связанной с отправкой смс
        /// </summary>
        private static string _CurAlgiritmSmtpText = @"Отправить сообщение пользователю {0} с информацией по его текущему бонусу и процентам.";

        /// <summary>
        /// Задание, которое нужно запустить
        /// </summary>
        private static string _CurAlgiritmSmtpQuery = "Q3";

        /// <summary>
        /// Параметры которые выставить
        /// </summary>
        private static string _CurAlgiritmSmtpPar = "@Par1:@CustSid|@Par2:1";

        /// <summary>
        /// Специфичный формат выгрузки данных по чекам
        /// </summary>
        private static string _SpecificProcessBonus;

        /// <summary>
        /// Видимость колонок в режиме просмотровщика, содержащих информацию по бонусам и скидкам
        /// </summary>
        private static bool _VisibleCalculateCustomColumn = false;
        #endregion
        #region Public Param
        /// <summary>
        /// Файл в который будем сохранять лог
        /// </summary>
        public static string FileXml { get; private set; }

        /// <summary>
        /// Версия XML файла
        /// </summary>
        public static int Version { get { return _Version; } private set { } }

        /// <summary>
        /// Флаг трассировки
        /// </summary>
        public static bool Trace 
        { 
            get 
            { 
                return _Trace; 
            } 
            set 
            {
                xmlRoot.SetAttribute("Trace", value.ToString());
                Save();
                _Trace = value;
            } 
        }

        /// <summary>
        /// Лог файл в который мы пишем информацию по пользователям которых считаем навалижными
        /// </summary>
        public static string LogNotValidCustomer
        {
            get 
            {
                return _LogNotValidCustomer; 
            } 
            set 
            {
                xmlRoot.SetAttribute("LogNotValidCustomer", _LogNotValidCustomer);
                Save();
                _LogNotValidCustomer = value;
            }
        }

        /// <summary>
        /// Режим работы
        /// </summary>
        public static ModeEn Mode
        {
            get
            {
                return (_Mode != ModeEn.Normal ? _Mode : ConfigReg.Mode);
            }
            set 
            {
                xmlRoot.SetAttribute("Mode", value.ToString());
                Save();
                _Mode = value;
            }
        }

        /// <summary>
        /// Фильтр для коода региона, если он указан, то будут прогружаьбся только те клиенты которые относятся к этому коду региона
        /// </summary>
        public static string CustomerCountryList 
        { 
            get 
            { 
                return _CustomerCountryList; 
            } 
            set 
            {
                xmlRoot.SetAttribute("CustomerCountryList", value.Trim());
                Save();
                _CustomerCountryList = value.Trim();
            } 
        }

        /// <summary>
        /// Фильтр по префиксу в телефонах, для того чтобы не всех клиентов грузить а только интересных нам
        /// </summary>
        public static string CustomerPrefixPhoneList 
        { 
            get 
            { 
                return _CustomerPrefixPhoneList; 
            } 
            set 
            {
                xmlRoot.SetAttribute("CustomerPrefixPhoneList", value.Trim());
                Save();
                _CustomerPrefixPhoneList = value.Trim();
            } 
        }

        /// <summary>
        /// Сохранение событий в таблицу для отправки их по SMTP
        /// </summary>
        public static bool SaveSmtpEvent { get { return _SaveSmtpEvent; } private set { } }

        /// <summary>
        /// Текущий разделитель
        /// </summary>
        public static string TekDelitel
        {
            get
            {
                if (_TekDelitel == null) _TekDelitel = (((decimal)1) / 2).ToString().Replace("0", "").Replace("5", "");
                return _TekDelitel;
            }
            private set { }
        }

        /// <summary>
        /// Папка в которой лежит наше приложение AlgoritmSMTP
        /// </summary>
        public static string CurAlgiritmSmtpDir
        {
            get
            {
                if (Directory.Exists(_CurAlgiritmSmtpDir))
                {
                    if (File.Exists(string.Format(@"{0}\AlgoritmSMTP.exe", _CurAlgiritmSmtpDir))) return _CurAlgiritmSmtpDir;
                }
                return null;
            }
            set 
            {
                if (File.Exists(string.Format(@"{0}\AlgoritmSMTP.exe", value)))
                {
                    xmlRoot.SetAttribute("CurAlgiritmSmtpDir", value.Trim());
                    Save();
                    _CurAlgiritmSmtpDir = value.Trim();
                }
                else throw new ApplicationException(string.Format(@"Мы не обнаружили в папке: ""{0}"" приложения: ""AlgoritmSMTP.exe""", value));
            }
        }

        /// <summary>
        /// Текст который отобразить пользователю при отображении менюшки связанной с отправкой смс
        /// </summary>
        public static string CurAlgiritmSmtpText
        {
            get
            {
                return _CurAlgiritmSmtpText;    
            }
            set
            {
                xmlRoot.SetAttribute("CurAlgiritmSmtpText", value.Trim());
                Save();
                _CurAlgiritmSmtpText = value.Trim();
            }
        }

        /// <summary>
        /// Задание, которое нужно запустить
        /// </summary>
        public static string CurAlgiritmSmtpQuery
        {
            get
            {
                return _CurAlgiritmSmtpQuery;
            }
            set
            {
                xmlRoot.SetAttribute("CurAlgiritmSmtpQuery", value.Trim());
                Save();
                _CurAlgiritmSmtpQuery = value.Trim();
            }
        }

        /// <summary>
        /// Параметры которые выставить
        /// </summary>
        public static string CurAlgiritmSmtpPar
        {
            get
            {
                return _CurAlgiritmSmtpPar;
            }
            set
            {
                xmlRoot.SetAttribute("CurAlgiritmSmtpPar", value.Trim());
                Save();
                _CurAlgiritmSmtpPar = value.Trim();
            }
        }


        /// <summary>
        /// Специфичный формат выгрузки данных по чекам
        /// </summary>
        public static string SpecificProcessBonus
        {
            get
            {
                return _SpecificProcessBonus;
            }
            set
            {
                xmlRoot.SetAttribute("SpecificProcessBonus", value.Trim());
                Save();
                _SpecificProcessBonus = value.Trim();
            }
        }

        /// <summary>
        /// Видимость колонок в режиме просмотровщика, содержащих информацию по бонусам и скидкам
        /// </summary>
        public static bool VisibleCalculateCustomColumn
        {
            get
            {
                return _VisibleCalculateCustomColumn;
            }
            set
            {
                xmlRoot.SetAttribute("VisibleCalculateCustomColumn", value.ToString());
                Save();
                _VisibleCalculateCustomColumn = value;
            }
        }
        #endregion
        #region Public metod
        /// <summary>
        /// Коонструктор
        /// </summary>
        /// <param name="FileConfig">Имя файла лога программы</param>
        public Config(string FileConfig)
        {
            try
            {
                // Если это первая загрузка класса то инициируем его
                if (obj == null)
                {
                    // Подгружаем данные из реестра
                    //this.ConfigReg();


                    if (FileConfig == null) FileXml = "Config.xml";
                    else FileXml = FileConfig;

                    obj = this;

                    // Логируем запуск программы
                    Log.EventSave("Загрузка чтения параметров.", GetType().Name, EventEn.Message);

                    // Читаем файл или создаём
                    if (File.Exists(Environment.CurrentDirectory + @"\" + FileXml)) { Load(); }
                    else { Create(); }

                    // Создаём костомизированный объект
                    GetDate();

                    // Подписываемся на события
                    Com.ProviderFarm.onEventSetup += new EventHandler<EventProviderFarm>(ProviderFarm_onEventSetup);
                    Com.ProviderPrizmFarm.onEventSetup += new EventHandler<EventProviderPrizmFarm>(ProviderPrizmFarm_onEventSetup);
                    Com.UserFarm.List.onUserListAddedUser += new EventHandler<User>(List_onUserListAddedUser);
                    Com.UserFarm.List.onUserListDeletedUser += new EventHandler<User>(List_onUserListDeletedUser);
                    Com.UserFarm.List.onUserListUpdatedUser += new EventHandler<User>(List_onUserListUpdatedUser);
                    Com.Lic.onCreatedLicKey += new EventHandler<LicLib.onLicEventKey>(Lic_onCreatedLicKey);
                    Com.Lic.onRegNewKey += new EventHandler<LicLib.onLicItem>(Lic_onRegNewKey);
                    Com.ScenariyFarm.List.onScenariyListAddedScenariy += new EventHandler<Scenariy.Lib.ScenariyBase>(List_onScenariyListAddedScenariy);
                    Com.ScenariyFarm.List.onScenariyListDeletedScenariy += new EventHandler<Scenariy.Lib.ScenariyBase>(List_onScenariyListDeletedScenariy);
                    Com.ConfigurationFarm.ShdConfigurations.onConfigurationsLstAddedConfigurationsLst += new EventHandler<EventConfigurationList>(ShdConfigurations_onConfigurationsLstAddedConfigurationsLst);
                    Com.ConfigurationFarm.ShdConfigurations.onConfigurationsLstListDeletedConfigurationsLst += new EventHandler<EventConfigurationList>(ShdConfigurations_onConfigurationsLstListDeletedConfigurationsLst);
                    Com.ConfigurationFarm.onСhengedCurrentCnfList += new EventHandler<EventConfigurationList>(ConfigurationFarm_onСhengedCurrentCnfList);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при загрузке конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Коонструктор
        /// </summary>
        public Config()
            : this(null)
        { }

        #endregion
        #region Private metod
        /// <summary>
        /// Читаем файл конфигурации
        /// </summary>
        private static void Load()
        {
            try
            {
                lock (obj)
                {
                    Document.Load(Environment.CurrentDirectory + @"\" + FileXml);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при чтении конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".Load()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Сохранение в файл
        /// </summary>
        private static void Save()
        {
            try
            {
                lock (obj)
                {
                    Document.Save(Environment.CurrentDirectory + @"\" + FileXml);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при сохранении конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".Save()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Создаём файл конфигурации
        /// </summary>
        private static void Create()
        {
            try
            {
                //Document = new XmlDocument();

                // Создаём строку инициализации
                XmlElement wbRoot = Document.DocumentElement;
                XmlDeclaration wbxmldecl = Document.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                Document.InsertBefore(wbxmldecl, wbRoot);

                // Создаём начальное тело в кторое будем потом всё вставлять
                XmlElement xmlMain = Document.CreateElement("AlgoritmDM");
                xmlMain.SetAttribute("Version", Version.ToString());
                xmlMain.SetAttribute("Mode", _Mode.ToString());
                xmlMain.SetAttribute("Trace", _Trace.ToString());
                xmlMain.SetAttribute("LogNotValidCustomer", _LogNotValidCustomer);
                xmlMain.SetAttribute("PrvFullName", null);
                xmlMain.SetAttribute("ConnectionString", "");
                xmlMain.SetAttribute("PrvPrizmFullName", null);
                xmlMain.SetAttribute("PrvPrizmConnectionString", "");
                xmlMain.SetAttribute("CurrentConfigurationList", "Стандартная конфигурация.");
                xmlMain.SetAttribute("CustomerCountryList", "1038,1225");
                xmlMain.SetAttribute("CustomerPrefixPhoneList", "+7");
                xmlMain.SetAttribute("SaveSmtpEvent", SaveSmtpEvent.ToString());
                xmlMain.SetAttribute("SpecificProcessBonus", "BonusDM");
                Document.AppendChild(xmlMain);

                XmlElement xmlLics = Document.CreateElement("Lics");
                xmlMain.AppendChild(xmlLics);

                XmlElement xmlUsers = Document.CreateElement("Users");
                xmlMain.AppendChild(xmlUsers);

                XmlElement xmlUScenariyList = Document.CreateElement("UScenariyList");
                xmlMain.AppendChild(xmlUScenariyList);
                XmlElement xmlUScenariy = Document.CreateElement("UScenariy");
                xmlUScenariy.SetAttribute("ScnFullName", "NakopDMscn");
                xmlUScenariy.SetAttribute("ScenariyName", "Накопительная скидка");
                xmlUScenariyList.AppendChild(xmlUScenariy);

                XmlElement xmlSharedConfigurations = Document.CreateElement("SharedConfigurations");
                xmlMain.AppendChild(xmlSharedConfigurations);
                XmlElement xmlConfigurationList = Document.CreateElement("ConfigurationList");
                xmlConfigurationList.SetAttribute("ConfigurationName", "Стандартная конфигурация.");
                xmlSharedConfigurations.AppendChild(xmlConfigurationList);
                XmlElement xmlConfiguration = Document.CreateElement("Configuration");
                xmlConfiguration.SetAttribute("ActionRows", (true).ToString());
                xmlConfiguration.SetAttribute("ScenariyName", "Накопительная скидка");
                xmlConfiguration.SetAttribute("SaleActionOut", CongigurationActionSaleEn.Last.ToString());
                xmlConfigurationList.AppendChild(xmlConfiguration);

                // Сохранение в файл
                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при создании конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".Create()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Чтение данных
        /// </summary>
        private static void GetDate()
        {
            ApplicationException appM = new ApplicationException("Неправильный настроечный файл, скорее всего не от этой программы.");
            ApplicationException appV = new ApplicationException("Неправильная версия настроечного файла, требуется " + Version.ToString() + " версия.");
            try
            {
                string tmpCurrentConfigurationList = null;
                xmlRoot = Document.DocumentElement;

                // Проверяем тип файла настройки по имени коренвого нода и версию
                if (xmlRoot.Name != "AlgoritmDM") throw appM;
                if (Version < int.Parse(xmlRoot.GetAttribute("Version"))) { throw appV; }
                if (Version > int.Parse(xmlRoot.GetAttribute("Version"))) UpdateVersionXml(xmlRoot, int.Parse(xmlRoot.GetAttribute("Version")));

                // Получаем значения из заголовка
                string PrvFullName = null;
                string ConnectionString = null;
                string PrvPrizmFullName = null;
                string PrvPrizmConnectionString = null;
                for (int i = 0; i < xmlRoot.Attributes.Count; i++)
                {
                    if (xmlRoot.Attributes[i].Name == "Trace") try { _Trace = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); }
                    catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "LogNotValidCustomer") LogNotValidCustomer = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "Mode") _Mode = EventConverter.Convert(xmlRoot.Attributes[i].Value.ToString(), _Mode);
                    if (xmlRoot.Attributes[i].Name == "PrvFullName") PrvFullName = xmlRoot.Attributes[i].Value.ToString();
                    try { if (xmlRoot.Attributes[i].Name == "ConnectionString") ConnectionString = Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                    catch (Exception) { }
                    if (xmlRoot.Attributes[i].Name == "PrvPrizmFullName") PrvPrizmFullName = xmlRoot.Attributes[i].Value.ToString();
                    try { if (xmlRoot.Attributes[i].Name == "PrvPrizmConnectionString") PrvPrizmConnectionString = Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                    catch (Exception) { }

                    if (xmlRoot.Attributes[i].Name == "CurrentConfigurationList") tmpCurrentConfigurationList = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "CustomerCountryList") _CustomerCountryList = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "CustomerPrefixPhoneList") _CustomerPrefixPhoneList = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "SaveSmtpEvent") _SaveSmtpEvent = bool.Parse(xmlRoot.Attributes[i].Value.ToString());
                    if (xmlRoot.Attributes[i].Name == "CurAlgiritmSmtpDir") _CurAlgiritmSmtpDir = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "CurAlgiritmSmtpText") _CurAlgiritmSmtpText = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "CurAlgiritmSmtpQuery") _CurAlgiritmSmtpQuery = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "CurAlgiritmSmtpPar") _CurAlgiritmSmtpPar = xmlRoot.Attributes[i].Value.ToString();
                    if (xmlRoot.Attributes[i].Name == "VisibleCalculateCustomColumn") _VisibleCalculateCustomColumn = bool.Parse(xmlRoot.Attributes[i].Value.ToString());
                    if (xmlRoot.Attributes[i].Name == "SpecificProcessBonus") _SpecificProcessBonus = xmlRoot.Attributes[i].Value.ToString();
                }

                // Подгружаем провайдер
                try
                {
                    Com.ProviderFarm.Setup(new UProvider(PrvFullName, ConnectionString), false);
                }
                catch (Exception) { }

                // Подгружаем провайдер Prizm
                try
                {
                    Com.ProviderPrizmFarm.SetupCurrentProvider(Com.ProviderPrizmFarm.CreateNewProviderPrizm(PrvPrizmFullName, PrvPrizmConnectionString));
                }
                catch (Exception) { }

                // Получаем список объектов
                foreach (XmlElement item in xmlRoot.ChildNodes)
                {
                    switch (item.Name)
                    {
                        case "Users":
                            xmlUsers = item;
                            foreach (XmlElement xuser in item.ChildNodes)
                            {
                                string Logon = xuser.Name;
                                string Password = null;
                                string Description = null;
                                Lib.RoleEn Role = RoleEn.None;
                                foreach (XmlAttribute auser in xuser.Attributes)
                                {
                                    if (auser.Name == "Password") Password = Com.Lic.DeCode(xuser.GetAttribute(auser.Name));
                                    if (auser.Name == "Description") Description = xuser.GetAttribute(auser.Name);
                                    if (auser.Name == "Role") Role = Lib.EventConverter.Convert(xuser.GetAttribute(auser.Name), Role);
                                }

                                // Если пароль не указан, то пользователя всё равно нужно добавить, просто при запуске он должен будет придумать пароль
                                if (!string.IsNullOrWhiteSpace(Logon) && Role != RoleEn.None)
                                {
                                    try
                                    {
                                        UserFarm.List.Add(new Lib.User(Logon, Password, Description, Role), true, false);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.EventSave(string.Format("Не смогли добавить пользователя с именем {0} при чтении конфигурационного файла: {1}", Logon, ex.Message), obj.GetType().Name + ".GetDate()", EventEn.Error);
                                    }

                                }
                            }
                            break;
                        case "Lics":
                            xmlLics = item;
                            foreach (XmlElement xkey in item.ChildNodes)
                            {
                                try
                                {
                                    string MachineName = null;
                                    string UserName = null;
                                    string ActivNumber = null;
                                    string LicKey = null;
                                    int ValidToYYYYMMDD = 0;
                                    string Info = null;
                                    bool HashUserOS = false;
                                    List<string> ScnFullNameList = new List<string>();

                                    //Получаем данные по параметру из файла
                                    for (int i = 0; i < xkey.Attributes.Count; i++)
                                    {
                                        if (xkey.Attributes[i].Name == "MachineName") { MachineName = xkey.Attributes[i].Value; }
                                        if (xkey.Attributes[i].Name == "UserName") { UserName = xkey.Attributes[i].Value; }
                                        if (xkey.Attributes[i].Name == "ActivNumber") { ActivNumber = xkey.Attributes[i].Value; }
                                        if (xkey.Attributes[i].Name == "LicKey") { LicKey = xkey.Attributes[i].Value; }
                                        if (xkey.Attributes[i].Name == "ValidToYYYYMMDD") { try { ValidToYYYYMMDD = int.Parse(xkey.Attributes[i].Value); } catch { } }
                                        if (xkey.Attributes[i].Name == "Info") { Info = xkey.Attributes[i].Value; }
                                        try { if (xkey.Attributes[i].Name == "HashUserOS") { HashUserOS = bool.Parse(xkey.Attributes[i].Value); } }
                                        catch (Exception) { }
                                    }
                                    if (!string.IsNullOrWhiteSpace(xkey.InnerText))
                                    {
                                        foreach (string sitem in xkey.InnerText.Split(','))
                                        {
                                            ScnFullNameList.Add(sitem);
                                        }
                                    }

                                    // Проверяем валидность подгруженного ключа
                                    if (!string.IsNullOrWhiteSpace(LicKey)) //&& Com.Lic.IsValidLicKey(LicKey)
                                    {
                                        Com.Lic.IsValidLicKey(LicKey);
                                        // Если ключь валидный то сохраняем его в списке ключей
                                        //Com.LicLib.onLicEventKey newKey = new Com.LicLib.onLicEventKey(MachineName, UserName, ActivNumber, LicKey, ValidToYYYYMMDD, Info, HashUserOS, ScnFullNameList);
                                        //Com.Lic.IsValidLicKey( .Add(newKey);
                                    }
                                }
                                catch { } // Если ключь прочитать не удалось или он не подходит, то исключения выдавать не нужно
                            }
                            break;
                        case "UScenariyList":
                            xmlUScenariyList = item;
                            foreach (XmlElement xUScenariy in item.ChildNodes)
                            {
                                if (xUScenariy.Name == "UScenariy")
                                {
                                    string ScnFullName = null;
                                    string ScenariyName = null;
                                    foreach (XmlAttribute aUScenariy in xUScenariy.Attributes)
                                    {
                                        if (aUScenariy.Name == "ScnFullName") ScnFullName = xUScenariy.GetAttribute(aUScenariy.Name);
                                        if (aUScenariy.Name == "ScenariyName") ScenariyName = xUScenariy.GetAttribute(aUScenariy.Name);
                                    }

                                    // Если пароль не указан, то пользователя всё равно нужно добавить, просто при запуске он должен будет придумать пароль
                                    if (!string.IsNullOrWhiteSpace(ScnFullName) && !string.IsNullOrWhiteSpace(ScenariyName))
                                    {
                                        try
                                        {
                                            ScenariyFarm.List.Add(new UScenariy(ScnFullName, ScenariyName, xUScenariy), true, false);
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.EventSave(string.Format("Не смогли добавить сценарий с именем {0} в общий список сценариев.\r\nПроизошла ошибка: {1}", ScenariyName, ex.Message), obj.GetType().Name + ".GetDate()", EventEn.Error);
                                        }

                                    }
                                }
                            }
                            break;
                        case "SharedConfigurations":
                            xmlSharedConfigurations = item;
                            break;
                        default:
                            break;
                    }
                }

                // Пробегаем по списку конфигураций после того как мы создали все сценарии в нашей системе (конечно если был найден тег для хранения списка конфигураций)
                if (xmlSharedConfigurations != null)
                {
                    foreach (XmlElement xitem in xmlSharedConfigurations.ChildNodes)
                    {
                        if (xitem.Name == "ConfigurationList")
                        {
                            string ConfigurationName = null;
                            foreach (XmlAttribute aitem in xitem.Attributes)
                            {
                                if (aitem.Name == "ConfigurationName") ConfigurationName = xitem.GetAttribute(aitem.Name);
                            }

                            // Если имя сценария есть то создаём пока пустой ценарий
                            if (!string.IsNullOrWhiteSpace(ConfigurationName))
                            {
                                ConfigurationList CnfL = new ConfigurationList(ConfigurationName);

                                // Пробегаем по элементам конфигурации
                                foreach (XmlElement xCnf in xitem.ChildNodes)
                                {   // Найден элемент конфигурации
                                    if (xCnf.Name == "Configuration")
                                    {
                                        bool ActionRows = true;
                                        string ScenariyName = null;
                                        CongigurationActionSaleEn SaleActionOut = CongigurationActionSaleEn.Last;
                                        foreach (XmlAttribute aCnf in xCnf.Attributes)
                                        {
                                            try { if (aCnf.Name == "ActionRows") ActionRows = bool.Parse(xCnf.GetAttribute(aCnf.Name)); }
                                            catch (Exception) { }
                                            if (aCnf.Name == "ScenariyName") ScenariyName = xCnf.GetAttribute(aCnf.Name);
                                            if (aCnf.Name == "SaleActionOut") SaleActionOut = Lib.EventConverter.Convert(xCnf.GetAttribute(aCnf.Name), SaleActionOut);
                                        }

                                        // Если сценарий обнаружен пробуем его получить из синглетон списка
                                        if (ScenariyName != null)
                                        {
                                            // Если удалось получить из синглетон списка то добавляем его в нашу конфигурацию
                                            UScenariy UScn = Com.ScenariyFarm.List[ScenariyName];
                                            if (UScn != null)
                                            {
                                                Configuration Cnf = new Configuration(ActionRows, UScn, SaleActionOut);
                                                CnfL.Add(Cnf, true, false);
                                            }
                                        }
                                    }
                                }

                                // Подписываемся на события этого листа
                                CnfL.onConfigurationListAddedConfiguration += new EventHandler<Configuration>(CnfL_onConfigurationListAddedConfiguration);
                                CnfL.onConfigurationListDeletedConfiguration += new EventHandler<Configuration>(CnfL_onConfigurationListDeletedConfiguration);

                                // Добавляем конфигурацию в список доступных конфигураций
                                Com.ConfigurationFarm.ShdConfigurations.Add(CnfL, true, false);
                            }
                        }
                    }

                    // Списки конфигураций подгружены теперь устанавливаем текущий список
                    Com.ConfigurationFarm.SetupCurrentSharedConfigurations(Com.ConfigurationFarm.ShdConfigurations[tmpCurrentConfigurationList], false);
                }

            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при парсинге файла конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".GetDate()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Oбновляем до текущей версии
        /// </summary>
        /// <param name="root">Корневой элемент</param>
        /// <param name="oldVersion">Текущая версия элемента</param>
        private static void UpdateVersionXml(XmlElement root, int oldVersion)
        {
            try
            {
                if (oldVersion <= 2)
                {
                    string _SpecificProcessBonus = null;
                    for (int i = 0; i < root.Attributes.Count; i++)
                    {
                        if (root.Attributes[i].Name == "SpecificProcessBonus") _SpecificProcessBonus = root.Attributes[i].Value.ToString();
                    }

                    root.SetAttribute("SpecificProcessBonus", string.Empty);
/*
                    // Устанавливаем строку подключения в объекте провайдера
                    if (OraTNS != null && OraTNS.Trim() != string.Empty && OraUser != null && OraUser.Trim() != string.Empty && OraPassword != null && OraPassword.Trim() != string.Empty)
                    {
                        Com_Provider_Ora conOra = new Com_Provider_Ora(this._MyCom);
                        try
                        {
                            if (!conOra.SaveConnectStr(OraTNS, OraUser, OraPassword) || conOra.ConnectString() == null || conOra.ConnectString().Trim() == string.Empty) new ApplicationException("Не можем обновить конфиг файл так как подключение к ораклу невалидно.");
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("Не можем обновить конфиг файл так как подключение к ораклу невалидно. (" + ex.Message + ")");
                        }

                        root.SetAttribute("ProviderTyp", Enum.GetName(typeof(Lib.Provider_En), Lib.Provider_En.Oracle));
                        root.SetAttribute("ConnectionString", conOra.ConnectString());
                        root.RemoveAttribute("OraTNS");
                        root.RemoveAttribute("OraUser");
                        root.RemoveAttribute("OraPassword");
                    }
                    */
                }

                root.SetAttribute("Version", _Version.ToString());
                Save();
            }
            catch (Exception ex)
            {
                Log.EventSave(ex.Source + @": " + ex.Message, "UpdateVersionXml", EventEn.Error);
                throw ex;
            }

        }

        // Событие изменения текщего провайдера
        private void ProviderFarm_onEventSetup(object sender, EventProviderFarm e)
        {
            try
            {
                XmlElement root = Document.DocumentElement;

                root.SetAttribute("PrvFullName", e.Uprv.PrvInType);
                try { root.SetAttribute("ConnectionString", Com.Lic.InCode(e.Uprv.ConnectionString)); }
                catch (Exception) { }



                // Получаем список объектов
                //foreach (XmlElement item in root.ChildNodes)
                //{
                //}

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при изменении файла конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".ProviderFarm_onEventSetup()", EventEn.Error);
                throw ae;
            }
        }

        // Событие изменения текщего провайдера Prizm
        private void ProviderPrizmFarm_onEventSetup(object sender, EventProviderPrizmFarm e)
        {
            try
            {
                XmlElement root = Document.DocumentElement;

                root.SetAttribute("PrvPrizmFullName", e.PrvPrizm.PlugInType);
                try { root.SetAttribute("PrvPrizmConnectionString", Com.Lic.InCode(e.PrvPrizm.ConnectionString)); }
                catch (Exception) { }



                // Получаем список объектов
                //foreach (XmlElement item in root.ChildNodes)
                //{
                //}

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при изменении файла конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".ProviderPrizmFarm_onEventSetup()", EventEn.Error);
                throw ae;
            }
        }

        // Событие добавления пользователя
        private void List_onUserListAddedUser(object sender, User e)
        {
            try
            {
                if (e.Logon == "Console" && e.Password == "123456" && e.Description == "Console" && e.Role == RoleEn.Admin)
                {
                    // Это системная запись её нельзя использовать
                }
                else
                {
                    XmlElement u = Document.CreateElement(e.Logon);
                    if (e.Password != null) u.SetAttribute("Password", Com.Lic.InCode(e.Password));
                    if (e.Description != null) u.SetAttribute("Description", e.Description);
                    if (e.Role != RoleEn.None) u.SetAttribute("Role", e.Role.ToString());
                    xmlUsers.AppendChild(u);

                    Save();
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при добавлении пользователя {0} в файл xml: {1}", e.Logon, ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".List_onUserListAddedUser()", EventEn.Error);
                throw ae;
            }
        }

        // Событие удаления пользователя
        private void List_onUserListDeletedUser(object sender, User e)
        {
            try
            {
                // Получаем список объектов
                foreach (XmlElement item in xmlUsers.ChildNodes)
                {
                    if (item.Name == e.Logon)
                    {
                        xmlUsers.RemoveChild(item);
                    }
                }

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при удалении пользователя {0} из файла xml: {1}", e.Logon, ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".List_onUserListDeletedUser()", EventEn.Error);
                throw ae;
            }
        }

        // Событие изменения данных пользователя
        private void List_onUserListUpdatedUser(object sender, User e)
        {
            try
            {
                if (e.Logon == "Console" && e.Password == "123456" && e.Description == "Console" && e.Role == RoleEn.Admin)
                {
                    // Это системная запись её нельзя использовать
                }
                else
                {

                    // Получаем список объектов
                    foreach (XmlElement item in xmlUsers.ChildNodes)
                    {
                        if (item.Name == e.Logon)
                        {
                            if (e.Password != null) item.SetAttribute("Password", Com.Lic.InCode(e.Password));
                            if (e.Description != null) item.SetAttribute("Description", e.Description);
                            if (e.Role != RoleEn.None) item.SetAttribute("Role", e.Role.ToString());
                        }
                    }

                    Save();
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при изменении данных о пользователе {0} в файл xml: {1}", e.Logon, ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".List_onUserListUpdatedUser()", EventEn.Error);
                throw ae;
            }
        }

        // Событие регистрации нового ключа
        void Lic_onRegNewKey(object sender, LicLib.onLicItem e)
        {
            try
            {
                if (xmlLics == null)
                {
                    xmlLics = Document.CreateElement("Lics");
                    xmlRoot.AppendChild(xmlLics);
                }

                XmlElement k = Document.CreateElement("Key");
                if (e._LicEventKey.MachineName != null) k.SetAttribute("MachineName", e._LicEventKey.MachineName);
                if (e._LicEventKey.UserName != null) k.SetAttribute("UserName", e._LicEventKey.UserName);
                if (e._LicEventKey.ActivNumber != null) k.SetAttribute("ActivNumber", e._LicEventKey.ActivNumber);
                if (e._LicEventKey.LicKey != null) k.SetAttribute("LicKey", e._LicEventKey.LicKey);
                if (e._LicEventKey.ValidToYYYYMMDD != 0) k.SetAttribute("ValidToYYYYMMDD", e._LicEventKey.ValidToYYYYMMDD.ToString());
                if (e._LicEventKey.Info != null) k.SetAttribute("Info", e._LicEventKey.Info);
                k.SetAttribute("HashUserOS", e._LicEventKey.HashUserOS.ToString());
                k.SetAttribute("HashConnectPrizm", e._LicEventKey.HashConnectPrizm.ToString());
                k.InnerText = string.Join(",", e._LicEventKey.ScnFullNameList.ToArray());
                xmlLics.AppendChild(k);

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении во время создания нового ключа в файл xml: {0}", ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".Lic_onCreatedLicKey()", EventEn.Error);
                throw ae;
            }
        }
        //
        // Событие создания нового ключа
        void Lic_onCreatedLicKey(object sender, LicLib.onLicEventKey e)
        {
            try
            {
                if (xmlLics == null)
                {
                    xmlLics = Document.CreateElement("Lics");
                    xmlRoot.AppendChild(xmlLics);
                }

                XmlElement k = Document.CreateElement("Key");
                if (e.MachineName != null) k.SetAttribute("MachineName", e.MachineName);
                if (e.UserName != null) k.SetAttribute("UserName", e.UserName);
                if (e.ActivNumber != null) k.SetAttribute("ActivNumber", e.ActivNumber);
                if (e.LicKey != null) k.SetAttribute("LicKey", e.LicKey);
                if (e.ValidToYYYYMMDD != 0) k.SetAttribute("ValidToYYYYMMDD", e.ValidToYYYYMMDD.ToString());
                if (e.Info != null) k.SetAttribute("Info", e.Info);
                k.SetAttribute("HashUserOS", e.HashUserOS.ToString());
                k.InnerText = string.Join(",", e.ScnFullNameList.ToArray());
                xmlLics.AppendChild(k);

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении во время создания нового ключа в файл xml: {0}", ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".Lic_onCreatedLicKey()", EventEn.Error);
                throw ae;
            }
        }


        // Cобытие добавления нового сценария в систему
        void List_onScenariyListAddedScenariy(object sender, Scenariy.Lib.ScenariyBase e)
        {
            // получаем объект через который файл конфигурации при добавлении сценария подменит ссылку xml ноды на ту которрая была создана
            Com.Scenariy.Lib.ScenariyBase.AccessForBaseScenary AccFoBase = new Scenariy.Lib.ScenariyBase.AccessForBaseScenary(e);

            XmlElement xmlUScenariy = Document.CreateElement("UScenariy");
            xmlUScenariy.SetAttribute("ScnFullName", e.ScenariyInType.Name);
            xmlUScenariy.SetAttribute("ScenariyName", e.ScenariyName);
            xmlUScenariyList.AppendChild(xmlUScenariy);

            // Устанавливаем ссылку на элемент сценария в базовый класс сценария
            AccFoBase.setXmlNodeForBaseScenary(xmlUScenariy);

            // Запускать сохранение в файл нет смысла предыдущий метод при установке ссылки запускает в сценарии процесс сохранения в внутри сценария который запускает сохранение в файл
        }

        // Cобытие удаления сценария из системы
        void List_onScenariyListDeletedScenariy(object sender, Scenariy.Lib.ScenariyBase e)
        {
            try
            {
                // Получаем список объектов
                foreach (XmlElement item in xmlUScenariyList.ChildNodes)
                {
                    if (item.Name == "UScenariy" && item.GetAttribute("ScnFullName") == e.ScenariyInType.Name && item.GetAttribute("ScenariyName") == e.ScenariyName)
                    {
                        xmlUScenariyList.RemoveChild(item);
                    }
                }

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при удалении сценария {0} ({1}) из файла xml: {2}", e.ScenariyName, e.ScenariyInType.Name, ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".List_onScenariyListDeletedScenariy()", EventEn.Error);
                throw ae;
            }
        }

        // Событие добавления новой конфигурации в систему
        void ShdConfigurations_onConfigurationsLstAddedConfigurationsLst(object sender, EventConfigurationList e)
        {
            XmlElement xmlConfigurationList = Document.CreateElement("ConfigurationList");
            xmlConfigurationList.SetAttribute("ConfigurationName", e.CfgL.ConfigurationName);
            xmlSharedConfigurations.AppendChild(xmlConfigurationList);

            Save();

            // Подписываемся на события этого листа
            e.CfgL.onConfigurationListAddedConfiguration += new EventHandler<Configuration>(CnfL_onConfigurationListAddedConfiguration);
            e.CfgL.onConfigurationListDeletedConfiguration += new EventHandler<Configuration>(CnfL_onConfigurationListDeletedConfiguration);
        }

        // Событие уделения конфигурации из системы
        void ShdConfigurations_onConfigurationsLstListDeletedConfigurationsLst(object sender, EventConfigurationList e)
        {
            try
            {
                // Получаем список объектов
                foreach (XmlElement item in xmlSharedConfigurations.ChildNodes)
                {
                    if (item.Name == "ConfigurationList" && item.GetAttribute("ConfigurationName").Trim() == e.CfgL.ConfigurationName.Trim())
                    {
                        xmlSharedConfigurations.RemoveChild(item);
                    }
                }

                Save();

                // Отписываемся от событий по удалённым конфигурациям
                e.CfgL.onConfigurationListAddedConfiguration -= new EventHandler<Configuration>(CnfL_onConfigurationListAddedConfiguration);
                e.CfgL.onConfigurationListDeletedConfiguration -= new EventHandler<Configuration>(CnfL_onConfigurationListDeletedConfiguration);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при удалении конфигурации {0} из файла xml: {1}", e.CfgL.ConfigurationName.Trim(), ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".ShdConfigurations_onConfigurationsLstListDeletedConfigurationsLst()", EventEn.Error);
                throw ae;
            }
        }

        // Событие добавления нового элемента в конфигурацию
        static void CnfL_onConfigurationListAddedConfiguration(object sender, Configuration e)
        {
            Lib.ConfigurationList CnfL = (Lib.ConfigurationList)sender;

            // Получаем список объектов
            foreach (XmlElement item in xmlSharedConfigurations.ChildNodes)
            {
                if (item.Name == "ConfigurationList" && item.GetAttribute("ConfigurationName").Trim() == CnfL.ConfigurationName.Trim())
                {
                    XmlElement xmlConfiguration = Document.CreateElement("Configuration");
                    xmlConfiguration.SetAttribute("ActionRows", e.ActionRows.ToString());
                    xmlConfiguration.SetAttribute("ScenariyName", e.UScn.ScenariyName);
                    xmlConfiguration.SetAttribute("SaleActionOut", e.SaleActionOut.ToString());
                    item.AppendChild(xmlConfiguration);

                    Save();
                }
            }
        }

        // Событие удаления элемента конфигурации
        static void CnfL_onConfigurationListDeletedConfiguration(object sender, Configuration e)
        {
            Lib.ConfigurationList CnfL = (Lib.ConfigurationList)sender;

            // Получаем список объектов
            foreach (XmlElement item in xmlSharedConfigurations.ChildNodes)
            {
                if (item.Name == "ConfigurationList" && item.GetAttribute("ConfigurationName").Trim() == CnfL.ConfigurationName.Trim())
                {
                    int index = -1;
                    foreach (XmlElement iCnf in item.ChildNodes)
                    {
                        if (iCnf.Name == "Configuration")
                        {
                            index++;
                            if (e.Index == index)
                            {
                                item.RemoveChild(iCnf);
                                Save();
                                break;
                            }
                        }
                    }

                    break;
                }
            }
        }

        // Событие изменения текущей конфигурации
        void ConfigurationFarm_onСhengedCurrentCnfList(object sender, EventConfigurationList e)
        {
            xmlRoot.SetAttribute("CurrentConfigurationList", e.CfgL.ConfigurationName);
            Save();
        }

        #endregion

        #region Вложенные классы

        /// <summary>
        /// Вложенный класс для доступа к элементам внутренним нашего конфигурационного класса
        /// </summary>
        public class AceessForDoc
        {
            /// <summary>
            /// Плучить новый экземпляр элемента
            /// </summary>
            /// <param name="ElementName">Задаём имя элемента который мы хотим создать</param>
            /// <returns></returns>
            public XmlElement getNewXmlElement(string ElementName)
            {
                return Com.Config.Document.CreateElement(ElementName);
            }

            /// <summary>
            /// Сохранение изменений в файл
            /// </summary>
            public void SaveDoc()
            {
                Com.Config.Save();
            }
        }
        #endregion
    }
}
