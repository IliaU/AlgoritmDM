using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com.PlgProviderPrizm.Lib
{
    /// <summary>
    /// Базовый класс для провайдера типа Prizm
    /// </summary>
    public abstract class ProviderPrizmBase
    {
        /// <summary>
        /// Получаем элемент меню для получения информации по провайдеру
        /// </summary>
        //public ToolStripMenuItem InfoToolStripMenuItem { get; protected set; }

        /// <summary>
        /// Тип палгина
        /// </summary>
        public string PlugInType { get; private set; }

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Возвращает версию базы данных в виде строки
        /// </summary>
        public string VersionDB { get; private set; }

        /// <summary>
        /// Возвращаем версию драйвера
        /// </summary>
        public string Driver { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="PlugInType">Тип плагина</param>
        /// <param name="ConnectionString">Строка подключения</param>
        public ProviderPrizmBase(string PlugInType, string ConnectionString)
        {
            try
            {
                this.PlugInType = PlugInType;
                this.ConnectionString = ConnectionString;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации конструктора с ошибкой: ({0})", ex.Message));
                Com.Log.EventSave(ae.Message, GetType().Name, EventEn.Error, true, true);
                throw ae;
            }

        }

        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        public void EventSave(string Message, string Source, EventEn evn)
        {
            try
            {
                Log.EventSave(Message, Source + " (" + this.PlugInType + ")", evn);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации конструктора с ошибкой: ({0})", ex.Message));
                Com.Log.EventSave(ae.Message, string.Format("{0}.EventSave", GetType().Name), EventEn.Error, true, true);
                throw ae;
            }
        }

        /// <summary>
        /// Установка строки подключения
        /// </summary>
        /// <param name="ConnectionString">Строка подключения</param>
        /// <param name="VersionDB">Возвращает версию базы данных в виде строки</param>
        /// <param name="Driver">Возвращаем версию драйвера</param>
        protected void SetupConnectionStringAndVersionDB(string ConnectionString, string VersionDB, string Driver)
        {
            this.ConnectionString = ConnectionString;
            this.VersionDB = VersionDB;
            this.Driver = Driver;
        }
    }
}
