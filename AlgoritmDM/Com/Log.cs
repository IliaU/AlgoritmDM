using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;
using System.Windows.Forms;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com
{
    /// <summary>
    /// Класс для записи в лог файл
    /// </summary>
    public class Log
    {
        #region Private Param
        private static Log obj = null;

        /// <summary>
        /// Количество попыток записи в лог файл
        /// </summary>
        private static int IOCountPoput = 5;

        /// <summary>
        /// Колличество милесекунд между попытками записи
        /// </summary>
        private static int IOWhileInt = 500;
        #endregion
        #region Public Param
        /// <summary>
        /// Файл в который будем сохранять лог
        /// </summary>
        public static string File { get; private set; }

        /// <summary>
        /// Событие возникновения собятия в приложении
        /// </summary>
        public static event EventHandler<EventLog> onEventLog;

        #endregion
        #region Public metod
        /// <summary>
        /// Коонструктор
        /// </summary>
        /// <param name="FileLog">Имя файла лога программы</param>
        public Log(string FileLog)
        {
            // Если это первая загрузка класса то инициируем его
            if (obj == null)
            {
                if (FileLog == null) File = "Log.txt";
                else File = FileLog;

                obj = this;

                // Логируем запуск программы
                EventSave("Запуск программы.", GetType().Name, EventEn.Message);
            }
        }
        /// <summary>
        /// Коонструктор
        /// </summary>
        public Log()
            : this(null)
        { }

        /// <summary>
        /// Событие клиента
        /// </summary>
        /// <param name="Message">Сообщение события</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        public static void EventSave(string Message, string Source, EventEn evn)
        {
            EventSave(Message, Source, evn, true, false);
        }

        /// <summary>
        /// Событие клиента
        /// </summary>
        /// <param name="FileName">Сообщение события</param>
        /// <param name="Message">Сообщение события</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        public static void EventSave(string FileName, string Message, string Source, EventEn evn)
        {
            EventSave(FileName, Message, Source, evn, null, IOCountPoput);
        }

        /// <summary>
        /// Событие клиента
        /// </summary>
        /// <param name="Message">Сообщение события</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        /// <param name="PrefixLog">Префикс для специального лога отдельного</param>
        public static void EventSave(string Message, string Source, EventEn evn, string PrefixLog)
        {
            lock (obj)
            {
                EventSave(File, Message, Source, evn, PrefixLog, IOCountPoput);
            }
        }

        /// <summary>
        /// Событие клиента
        /// </summary>
        /// <param name="Message">Сообщение события</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        /// <param name="isLog">Писать в лог или нет</param>
        /// <param name="Show">Отобразить сообщение пользователю или нет</param>
        public static void EventSave(string Message, string Source, EventEn evn, bool isLog, bool Show)
        {
            if (obj == null) throw new ApplicationException("Класс Log ещё не инициирован. Сначала запустите констроуктор а потом используейте методы");
            lock (obj)
            {
                // Собственно обработка события
                EventLog myArg = new EventLog(Message, Source, evn, isLog, Show);
                if (onEventLog != null)
                {
                    onEventLog.Invoke(obj, myArg);
                }

                // Проверяем это дамп или нет и в настройках установлена запись дампов в лог
                if ((evn == EventEn.Dump && Config.Trace)
                    || (evn != EventEn.Dump && isLog))
                {   //Пишем в лог
                    EventSave(File, Message, Source, evn, null, IOCountPoput);
                }

                // Отображаем пользователю
                if (Show) MessageBox.Show(Message);
            }
        }
        #endregion
        #region Private metod

        delegate void delig_EventSave(string Message, string Source, EventEn evn, int IOCountPoput);
        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="FileName">Сообщение события</param>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        /// <param name="IOCountPoput">Количество попыток записи в лог</param>
        private static void EventSave(string FileName, string Message, string Source, EventEn evn, string PrefixLog, int IOCountPoput)
        {
            // Сохраняем задание
            try
            {
                lock (obj)
                {
                    if (Com.ConfigReg.ShopName != null)
                    {
                        if (!Directory.Exists(Environment.CurrentDirectory + @"\" + Com.ConfigReg.ShopName))
                        {
                            Directory.CreateDirectory(Environment.CurrentDirectory + @"\" + Com.ConfigReg.ShopName);
                        }
                    }

                    // Получаем имя файла с учётом префикса
                    string newFile = (string.IsNullOrWhiteSpace(FileName) ? File : FileName);
                    newFile = (string.IsNullOrWhiteSpace(PrefixLog) ? PrefixLog : PrefixLog) + newFile;

                    // Пишем в файл
                    using (StreamWriter SwFileLog = new StreamWriter(Environment.CurrentDirectory + @"\" + (Com.ConfigReg.ShopName == null ? newFile : Com.ConfigReg.ShopName + @"\" + newFile), true))   //,Encoding.Unicode
                    {
                        SwFileLog.WriteLine(DateTime.Now.ToString() + "\t" + (Com.UserFarm.CurrentUser == null ? "" : Com.UserFarm.CurrentUser.Logon + "\t") + evn.ToString() + "\t" + Source + "\t" + Message);
                    }
                }
            }
            catch (Exception)
            {
                if (IOCountPoput > 0)
                {
                    Thread.Sleep(IOWhileInt);
                    EventSave(FileName, Message, Source, evn, PrefixLog, IOCountPoput - 1);
                }
                else
                    throw;
            }
        }
        #endregion
    }
}
